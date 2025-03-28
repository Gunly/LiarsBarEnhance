﻿# 中文说明文档

***English README is in [README_EN.md](README_en.md)***

## 开源协议

***使用[LGPLv2.1](LICENSE.txt)协议开源***

***请遵守开源协议***

***请遵守开源协议***

## 功能

### 被动技

- 移除视角转动限制
- 修复中文玩家名显示`□`
- 移除玩家名字长度限制（HUD与聊天）
- 移除只能发送小写限制
- 移除敏感词限制（房主需要）
- 修复滚动列表使用鼠标滚轮速度过慢
- 重置设置只在设置页面生效(F7)
- 过滤大厅列表

### 主动技

- 疯狂转头
- 张嘴
- 头部移动
- 身体移动
- 身体转动
- 自动旋转
- 跳跃
- 等级修改
- 传送
- 游戏名修改
- 视野大小调整
- 死亡后头部转动
- 死亡后自由视角
- 玩家缩放(仅自己可见)
- 显示自身头顶信息
- 主动使用动画
- 回到主菜单(解决卡死)

#### 按键列表
大多数按键可以更改，建议使用[ConfigurationManager](https://github.com/Gunly/LiarsBarEnhance/releases/download/1.2.1/ConfigurationManager.dll)  
传送坐标和快捷键可编辑，默认4个，最高9个  
动画快捷键默认未设置，可编辑`BepInEx\config\com.github.gunly.LiarsBarEnhance.cfg`后启动游戏，或使用`ConfigurationManager`在游戏内修改快捷键  

| 按键                | 功能         |
| :-----------------: | :----------: |
| `Tab`               | 启用提示     |
| `C`                 | 疯狂转头     |
| `B`                 | 张嘴         |
| `R`                 | 重置身体     |
| `T`                 | 重置视角     |
| `↑` `↓` `←` `→` | 身体移动     |
| `I` `J` `K` `L`     | 头部移动     |
| `U` `O`             | 头部上下移动 |
| `[` `]`             | 头部偏转     |
| `鼠标按键` + `转动` | 身体转动     |
| `小键盘0`           | 跳跃         |
| `P`                 | 自动旋转     |
| `1` `2` `3` `4`     | 传送         |
| `鼠标滚轮`          | 调整视野大小 |
| `F9`                | 回到主菜单   |

### 做不到

- 看别人的牌或骰子
- 修改手中的牌或骰子
- 不死

## 安装

### 直接安装(2选1)

1. 下载[LiarsBarEnhance.zip](https://github.com/gunly/LiarsBarEnhance/releases/download/1.2.1/LiarsBarEnhance.zip)解压至游戏根目录
2. 仅包含被动技: 下载[LiarsBarEnhanceOnlyFix.zip](https://github.com/gunly/LiarsBarEnhance/releases/download/1.2.1/LiarsBarEnhanceOnlyFix.zip)解压至游戏根目录

### 手动安装

1. 下载[BepInEx](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.2)
2. 将`BepInEx`解压至游戏根目录（[官方安装教程](https://docs.bepinex.dev/articles/user_guide/installation/index.html)）
3. 3和4二选一: 从[Release](https://github.com/gunly/LiarsBarEnhance/releases)下载[最新Dll本体](https://github.com/gunly/LiarsBarEnhance/releases/download/1.2.1/com.github.gunly.LiarsBarEnhanceIl2cpp.dll)以及[BepInEx.KeyboardShortcut](https://github.com/gunly/LiarsBarEnhance/releases/download/1.2.1/BepInEx.KeyboardShortcut.dll)
4. 3和4二选一: 仅包含被动技, 下载[LiarsBarEnhanceOnlyFix](https://github.com/gunly/LiarsBarEnhance/releases/download/1.2.1/com.github.gunly.LiarsBarEnhanceOnlyFixIl2cpp.dll)
5. 将插件放置在插件文件夹（即`<游戏根目录>/BepInEx/plugins`）（没有`plugins`文件夹请手动创建）
6. 可选: 下载[ConfigurationManager](https://github.com/Gunly/LiarsBarEnhance/releases/download/1.2.1/ConfigurationManager.dll)放置在插件文件夹

## 自行构建插件

***Release 会发布构建完成的插件，非必要不建议自行构建***

1. 确保已经安装了[.NET SDK](https://dotnet.microsoft.com/zh-cn/download)（兼容netstandard2.1的SDK如6.0或以上）  
2. `cmd` 或者 `powershell` 等终端输入 `git clone https://github.com/dogdie233/LiarsBarEnhance.git`克隆本仓库到本地(前提安装`Git`网上教程一堆)或点击绿色`code`按钮后点击`Download Zip`下载解压  
3. 设置环境变量`LiarsBarManaged`为`<游戏根目录>/Liar's Bar_Data/Managed/`或者从`<游戏根目录>/Liar's Bar_Data/Managed/`中复制所有dll文件复制到`lib`文件夹  
4. 在项目根目录执行`dotnet build -c Release`  
5. 插件本体`com.github.dogdie233.LiarsBarEnhance.dll`将会生成在`Output`目录下  

***按照[安装](#安装)继续进行下一步操作***  

## 其他

欢迎欢愉的功能贡献（影响游戏平衡除外），可以提起功能请求等待有兴趣的开发者实现  
Xiaoye97大佬的BepInEx模组插件基础教程:  
[BepinEx5安装教程【Unity游戏Mod/插件制作教程02 - BepInEx的安装和使用-哔哩哔哩】](https://www.bilibili.com/read/cv8997496/)
