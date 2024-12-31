# English README

## Open-source License

***Use [LGPLv2.1](LICENSE.txt) LICENSE***

***Please follow open-source license.***

***Please follow open-source license.***

## Functionalities

### Passive

- Remove camera angle restrictions  
- Fix display of Chinese player names as `□` when you use Chinese as game language  
- Remove player name length restrictions (HUD and chat)  
- Remove lowercase-only sending restriction  
- Remove sensitive word restrictions (required by owner of the room)  
- Fix scrollview with slow mouse wheel speed
- Reset settings only take effect on the settings page
- Filter Lobby List

### You can

- Spin Crazy  
- Open Mouth  
- Head Movement  
- Body Movement  
- Body Rotation  
- Auto Rotate
- Jump
- Change Level
- Teleport
- Change Name
- Adjust Field Of View
- Rotate Head In Death
- Free Camera In Death
- Player Scale(only visible to self)
- Show Self Top Information
- Use Animation
- Return to main menu (resolve stuck)

#### Key Bindings
Key Bindings is configurable. Suggest using [ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager)
If ConfigurationManager window cannot be opened, you can try editing `BepInEx/config/BepInEx.cfg` to change `HideManagerGameObject = false` to `true`

| Key                     | Function            |
| :---------------------: | :-----------------: |
| `Tab`                   | Enable Hint         |
| `C`                     | Spin Crazy          |
| `B`                     | Open Mouth          |
| `R`                     | Reset Body          |
| `T`                     | Reset Head          |
| `↑` `↓` `←` `→`     | Body Movement       |
| `I` `J` `K` `L`         | Head Movement       |
| `Mouse Button` + `Turn` | Body Rotation       |
| `Numpad 0`              | Jump                |
| `P`                     | Auto Rotate         |
| `1` `2` `3` `4`         | Teleport            |
| `Mouse Wheel`           | Fiwld Of View       |
| `F9`                    | Return to main menu |

### You cannot

- Look at other people's cards  
- Modify the hand of cards  
- Be Immortal  

## Installation

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip)  
2. Extract `BepInEx` to the game root directory ([official installation tutorial](https://docs.bepinex.dev/articles/user_guide/installation/index.html))  
3. Download the latest DLL from [Release](https://github.com/gunly/LiarsBarEnhance/releases)[latest DLL](https://github.com/gunly/LiarsBarEnhance/releases/download/1.1.0/com.github.gunly.LiarsBarEnhance.dll) and [BepInEx.KeyboardShortcut](https://github.com/gunly/LiarsBarEnhance/releases/download/1.1.0/BepInEx.KeyboardShortcut.dll)  
4. If only need Passive(Chinese Fix, Lobby Filter, etc. Not contains Remove camera angle restrictions), Download[com.github.gunly.LiarsBarEnhanceOnlyFix.dll](https://github.com/gunly/LiarsBarEnhance/releases/download/1.1.0/com.github.gunly.LiarsBarEnhanceOnlyFix.dll), No need for `BepInEx.KeyboardShortcut`
5. Place the plugin DLL (`com.github.gunly.LiarsBarEnhance.dll` or `com.github.gunly.LiarsBarEnhanceOnlyFix.dll`) and `BepInEx.KeyboardShortcut.dll`(if need) in the plugin folder (i.e., `<game root directory>/BepInEx/plugins`), create the `plugins` folder if it doesn't exist  

## Building the Plugin Yourself

***The Release will publish the built plugin, it is not necessary to build it yourself unless you know what you are doing.***

1. Ensure that you have installed the [.NET SDK](https://dotnet.microsoft.com/zh-cn/download) (compatible with netstandard2.1 SDK such as 6.0 or above)  
2. In `cmd` or `powershell` terminal, input `git clone https://github.com/dogdie233/LiarsBarEnhance.git` to clone the repository to your local machine or click the green `code` button and then click `Download Zip` to download and extract it  
3. Set the environment variable `LiarsBarManaged` to `<game root directory>/Liar's Bar_Data/Managed/` or copy all dll files from `<game root directory>/Liar's Bar_Data/Managed/` to the `lib` folder.  
4. Execute `dotnet build -c Release` in the root directory of the project  
5. The plugin DLL (`com.github.dogdie233.LiarsBarEnhance.dll`) will be generated in the `Output` directory  

***Continue with the next steps according to [Installation](#installation)***
