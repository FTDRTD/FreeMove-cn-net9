// FreeMove -- Move directories without breaking shortcuts or installations 
//    Copyright(C) 2020  Luca De Martini

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace FreeMove
{
    public partial class Updater : Form
    {
        readonly string CurrentVersion;
        string NewVersion = "";
        readonly bool Silent;

        public Updater(bool Silent)
        {
            this.Silent = Silent;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(assemblyLocation))
            {
                // 如果路径为空，使用当前目录作为默认路径
                assemblyLocation = System.IO.Path.Combine(System.AppContext.BaseDirectory, "FreeMove.exe");
            }
            this.CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            InitializeComponent();
        }

        private void Updater_Shown(object sender, EventArgs e)
        {
            if (Silent)
                ShowUpdate();
            else
                Check();
        }

        private async void Check()
        {
            try
            {
                if (await CheckGitHub())
                {
                    ShowUpdate();
                }
                else
                {
                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        label1.Text = "There are no updates available";
                    }
                    progressBar1.Dispose();
                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { Dispose(); };
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == Properties.Resources.GitHubErrorMessage || ex is WebException)
                {
                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        label1.Text = Properties.Resources.GitHubErrorMessage;
                    }
                    progressBar1.Dispose();
                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { Dispose(); };
                }
                else throw;
            }
        }

        private void ShowUpdate()
        {
            progressBar1.Dispose();
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                label1.Font = new Font("Lucida Console", label1.Font.Size);
            }
            label1.Text = String.Format($"New version available\n\nCurrent Version: {CurrentVersion}\nLatest Version:  {NewVersion}\n\nOpen the download page?");
            button_Cancel.Enabled = true;
            button_Cancel.Click += delegate { Dispose(); };

            button_Ok.Enabled = true;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                button_Ok.Click += delegate { System.Diagnostics.Process.Start("https://github.com/ImDema/FreeMove/releases/latest"); Dispose(); };
            }
        }

        public async Task<bool> CheckGitHub()
        {
            // ServicePointManager is obsolete, using HttpClient instead
            Stream ResponseStream = await GetGitHubStreamAsync();

            TextReader Reader = new StreamReader(ResponseStream);
            const string pattern = "\"tag_name\":\"([0-9.]{5,9})\"";
            NewVersion = Regex.Match(Reader.ReadToEnd(), pattern, RegexOptions.Multiline).Groups[1].Value;

            if (NewVersion == "") throw new Exception(Properties.Resources.GitHubErrorMessage);
            return CurrentVersion != NewVersion;
        }

        private async Task<Stream> GetGitHubStreamAsync()
        {
            using var Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(Properties.Resources.UserAgent, CurrentVersion));
            var Resp = await Client.GetStreamAsync("https://api.github.com/repos/ImDema/FreeMove/releases/latest");
            return Resp;
        }

        public static async Task<Updater> SilentCheck()
        {
            Updater updater = new Updater(true);
            try
            {
                if (await updater.CheckGitHub())
                {
                    return updater;
                }
                else return null;
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
