# FreeMove
[![license](https://img.shields.io/github/license/ImDema/FreeMove.svg)](https://github.com/imDema/FreeMove/blob/master/LICENSE.txt)

此项目修改自[https://github.com/imDema/FreeMove](https://github.com/imDema/FreeMove)，并使用Net9框架构建。

## 项目描述
FreeMove 是一个用于自由移动目录的工具，不会破坏安装或快捷方式。您可以使用此工具将默认安装在C盘的程序移动到另一个驱动器，以节省主驱动器的空间。

### 工作原理
1. 文件被移动到新位置。
2. 在旧位置创建一个[符号链接](https://www.wikiwand.com/en/NTFS_junction_point)，指向新位置。任何尝试访问旧位置的程序都会自动重定向到新位置。

## 下载
[![Github All Releases](https://img.shields.io/github/downloads/imDema/FreeMove/total.svg)](https://github.com/imDema/FreeMove/releases/latest)

#### 从GitHub下载
[下载最新版本](https://github.com/FTDRTD/FreeMove-cn-net9/releases/tag/exe)

### 使用方法
运行可执行文件并使用图形用户界面。

> 注意：此程序的核心功能需要管理员权限。

## 推荐
您不应移动重要的系统目录，因为这可能会破坏核心功能，如Windows更新和Windows应用商店应用。

`C:\Users` - `C:\Documents and Settings` - `C:\Program Files` - `C:\Program Files (x86)` 不应移动。如果您希望这样做，请自行承担风险。要移动回目录，请参阅README的最后部分。

也就是说，移动包含在上述目录中的目录应该不会引起任何问题。因此，您可以自由移动 `C:\Program Files\HugeProgramIDontWantOnMySSD` 而不会出现问题。

## 截图
![截图](image.png)

## 卸载移动的程序
像平常一样卸载程序，不要删除符号链接。卸载程序将正常工作，在您移动程序的位置留下一个空目录，以及原始位置的目录链接，您可以手动删除这两个内容。

## 移动回程序
删除旧位置的符号链接（这不会删除内容），并将目录移回其原始位置。

