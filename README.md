# 中文说明文档

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

#### 按键列表
按键可配置，建议使用[ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager)  
如果无法打开ConfigurationManager窗口，可以尝试编辑`BepInEx/config/BepInEx.cfg`，将`HideManagerGameObject = false`更改为`true`

| 按键                | 功能         |
| :-----------------: | :----------: |
| `Tab`               | 启用提示     |
| `C`                 | 疯狂转头     |
| `B`                 | 张嘴         |
| `R`                 | 重置身体     |
| `T`                 | 重置视角     |
| `↑` `↓` `←` `→` | 身体移动     |
| `I` `J` `K` `L`     | 头部移动     |
| `鼠标按键` + `转动` | 身体转动     |
| `小键盘0`           | 跳跃         |
| `P`                 | 自动旋转     |
| `1` `2` `3` `4`     | 传送         |
| `鼠标滚轮`          | 调整视野大小 |

### 做不到

- 看别人的牌
- 修改手中的牌型
- 不死

## 安装

### 仅安装LiarsBarEnhance

1. 下载[BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip)
2. 将`BepInEx`解压至游戏根目录（[官方安装教程](https://docs.bepinex.dev/articles/user_guide/installation/index.html)）
3. 从[Release](https://github.com/gunly/LiarsBarEnhance/releases)下载[最新Dll本体](https://github.com/gunly/LiarsBarEnhance/releases/download/1.0.5/com.github.gunly.LiarsBarEnhance.dll)
4. 将插件本体(`com.github.gunly.LiarsBarEnhance.dll`)放置在插件文件夹（即`<游戏根目录>/BepInEx/plugins`）（没有`plugins`文件夹请手动创建）

### 安装可配置的LiarsBarEnhance

1. 下载[BepInEx_ConfigurationManager_LiarsBarEnhance.zip](https://github.com/Gunly/LiarsBarEnhance/releases/download/1.0.5/BepInEx_ConfigurationManager_LiarsBarEnhance.zip)
2. 将`BepInEx_ConfigurationManager_LiarsBarEnhance.zip`解压至游戏根目录
3. 进入游戏后按F1打开`ConfigurationManager`窗口，可以在游戏内修改配置和自定义快捷键

## 自行构建插件

***Release 会发布构建完成的插件，非必要不建议自行构建***

1. 确保已经安装了[.NET SDK](https://dotnet.microsoft.com/zh-cn/download)（兼容netstandard2.1的SDK如6.0或以上）  
2. `cmd` 或者 `powershell` 等终端输入 `git clone https://github.com/dogdie233/LiarsBarEnhance.git`克隆本仓库到本地(前提安装`Git`网上教程一堆)或点击绿色`code`按钮后点击`Download Zip`下载解压  
3. 设置环境变量`LiarsBarManaged`为`<游戏根目录>/Liar's Bar_Data/Managed/`或者从`<游戏根目录>/Liar's Bar_Data/Managed/`中复制所有dll文件复制到`lib`文件夹  
4. 在项目根目录执行`dotnet build -c Release`  
5. 插件本体`com.github.gunly.LiarsBarEnhance.dll`将会生成在`Output`目录下  

***按照[安装](#安装)继续进行下一步操作***  

## 其他

欢迎欢愉的功能贡献（影响游戏平衡除外），可以提起功能请求等待有兴趣的开发者实现  
Xiaoye97大佬的BepInEx模组插件基础教程:  
[BepinEx5安装教程【Unity游戏Mod/插件制作教程02 - BepInEx的安装和使用-哔哩哔哩】](https://www.bilibili.com/read/cv8997496/)
