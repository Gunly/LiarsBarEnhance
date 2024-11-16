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

### You can

- Spin Crazy  
- Open Mouth  
- Head Movement  
- Body Movement  
- Body Rotation  
- Auto Rotate
- Jump
- Change Level
- Change Name
- Adjust Field Of View
- Free Camera In Death
- Player Scale(only visible to self)

#### Key Bindings
Key Bindings is configurable. Suggest using [ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager)
If ConfigurationManager window cannot be opened, you can try editing `BepInEx/config/BepInEx.cfg` to change `HideManagerGameObject = false` to `true`

| Key                     | Function           |
| :---------------------: | :----------------: |
| `I`                     | Spin Crazy         |
| `O`                     | Open Mouth         |
| `↑` `↓` `←` `→`     | Body Movement      |
| `Numpad 0`              | Jump               |
| `Numpad 1-9`            | Head Movement      |
| `Mouse Wheel`           | Fiwld Of View      |
| `W` `S` `A` `D`         | Body Movement      |
| `Mouse Button` + `Turn` | Body Rotation      |
| `Tab`                   | Enable Hint        |
| `R`                     | Reset Body         |
| `U`                     | Change Free Camrea |

### You cannot

- Look at other people's cards  
- Modify the hand of cards  
- Be Immortal  

## Installation

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip)  
2. Extract `BepInEx` to the game root directory ([official installation tutorial](https://docs.bepinex.dev/articles/user_guide/installation/index.html))  
3. Download the latest DLL from [Release](https://github.com/gunly/LiarsBarEnhance/releases)[latest DLL](https://github.com/gunly/LiarsBarEnhance/releases/download/1.0.3/com.github.gunly.LiarsBarEnhance.dll)  
4. Place the plugin DLL (`com.github.gunly.LiarsBarEnhance.dll`) in the plugin folder (i.e., `<game root directory>/BepInEx/plugins`), create the `plugins` folder if it doesn't exist  

## Building the Plugin Yourself

***The Release will publish the built plugin, it is not necessary to build it yourself unless you know what you are doing.***

1. Ensure that you have installed the [.NET SDK](https://dotnet.microsoft.com/zh-cn/download) (compatible with netstandard2.1 SDK such as 6.0 or above)  
2. In `cmd` or `powershell` terminal, input `git clone https://github.com/dogdie233/LiarsBarEnhance.git` to clone the repository to your local machine or click the green `code` button and then click `Download Zip` to download and extract it  
3. Set the environment variable `LiarsBarManaged` to `<game root directory>/Liar's Bar_Data/Managed/` or copy all dll files from `<game root directory>/Liar's Bar_Data/Managed/` to the `lib` folder.  
4. Execute `dotnet build -c Release` in the root directory of the project  
5. The plugin DLL (`com.github.dogdie233.LiarsBarEnhance.dll`) will be generated in the `Output` directory  

***Continue with the next steps according to [Installation](#installation)***
