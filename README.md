# Mage Arena ModTemplate

This is a template to get you started creating mods for Mage Arena.
It comes with some scripts to help you setup the template as well as build it into a Plugin.

Any questions or concerns can be directed to `Walthzer` in the Mage Arena modding discord.

### Prerequisites
   - `Mage Arena` installed.
   - `.NET SDK` Installed
   - `C#` and `.NET` compatible editor, such as the free [Visual Studio Community](https://visualstudio.microsoft.com/free-developer-offers/)

### Installing `.NET SDK`
The .NET SDK is required to build your plugin files into a .DLL.<br>
This is an excerpt from the [BepInEx WIKI](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html)

1. Head to [.NET downloads page](https://dotnet.microsoft.com/download)
2. Select the latest recommended .NET SDK for your OS:

![](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/images/dotnet_download.png)

3. **Download and run the installer according to your OS's instructions** <br>
	You may need to restart your PC for the install to finalise.

4. **Once you have installed .NET SDK, verify that it works by opening a command line prompt and running**

```
dotnet --list-sdks
```


## Acquire the MageArenaModTemplate

If you have git installed use it to get the template.
```
git clone https://github.com/Walthzer/MageArenaModTemplate/tree/master
```
For those that don't want to use git, download the template as a zip <br>
[https://github.com/Walthzer/MageArenaModTemplate/archive/refs/heads/master.zip]()


## Setup and Run in Mage Arena
This step-by-step will help you get your mod setup, build and run inside of Mage Arena. So that you can start developing custom features afterwards!

1. Extract the ZIP into a new folder, e.g. `MyCoolMageArenaMod` <br>
it should looks similar to this:
   ```
   MyCoolMageArenaMod/
   |── packageFiles/
   |── src/
   |── tools/
   |── .gitignore
   |── README.md
   └── settings.bat

   ```

2. Edit the values inside `settings.bat` to be relevant to you and your mod.<br>
`MOD_NAME` and `MOD_AUTHOR` should only contain alphanumeric characters and underscores `_`
    ```
    set MOD_NAME=CHANGE_TO_YOU_MOD_NAME
    set MOD_AUTHOR=CHANGE_TO_YOUR_NAME
    set MOD_VERSION=1.0.0
    ```

3. run the `setup.bat` script from the `MyCoolMageArenaMod` folder, using `CMD` or the terminal in your Editor. <br>
This script renames files and variables inside `manifest.json` and the `.csproj` file so they match your chosen names. 
    ```
    .\tools\setup.bat
    ```

3. Copy the required .DLL files from your Mage Arena installation.
	1. In your steam library: `right-click` Mage Arena, then click on `Properties`.
	2. In the pop-up click `Installed Files` then  `Browse...`. File Explorer will pop-up inside of `Mage Arena`
	2. Navigate to `MageArena_Data\Managed`. This folder is full of `.DLL` files.
	3. Copy the `FishNet.Runtime.dll` to `MyCoolMageArenaMod\src\References\`
	4. Copy the `Path` of this folder.
	5. Find the lines with `Assembly-CSharp.dll` in `MyCoolMageArenaMod\src\MOD_NAME.csproj` and paste the path onto `<PASTE_MAGE_ARENA_PATH_HERE>`

4. run the `build.bat` script from the `MyCoolMageArenaMod` folder, using `CMD` or the terminal in your Editor. <br>
This build script will create a folder `build` and a zipfile of that folder named `MOD_NAME-MOD_VERSION.zip`
    ```
    .\tools\build.bat
    ```
  	**If the zip file is not created, then the build failed. Look in the terminal for any errors.**
    
5. Open the `Thunderstore`, `r2modman` or your prefered modmanager. Just make sure they allow you to import a local mod.
	1. Create a new profile and pick a good name to indicate this is for development. e.g. `DEVELOPMENT`
	2. Open the `DEVELOPMENT` profile and go to `settings > Profile > Import local mod`
	3. In the pop-up, click `Select File`
	3. In the window that opens you need to select the zipfile that was created by the build script: `MOD_NAME-MOD_VERSION.zip`<br> where `MOD_NAME` and `MOD_VERSION` are equal to the values you set in settings.bat!
	4. After selecting the zip file, in the window `Import mod from file`, you can see your the name of your mod and author.<br> These should match what you set in the `settings.bat` 
6. With your local mod imported, simply install the `BepInEx` and `modsync` mods from Thunderstore.
7. Run the game via `Modded` in your modmanager.
8. Go into the tutorial and select/unselelect your Spell Book. Everytime you do, you should cast `Magic Missile`
<br>
Congrats, you just build a custom mod and ran it in Mage Arena. Refer to `Start Development` to see what you should do now.

### Let `built` update your local mod
Every time you edit your mod and rebuilt it, you have to reimport the local mod into Thunderstore. The `built.bat` file can help you with this and automatically update the files for your mod. So that the only steps you need to test features are:
 1. Build
 2. Launch from Thunderstore

To do this you need to add the path to your installed local mod into the `settings.bat` file. 
1. Open your modmanager: Thunderstore, r2modman, etc.
2. Select the `DEVELOPMENT` profile you created earlier.
3. Go to `Settings > Locations > Browse profile folder`. This will open file explorer.
4. The newly opened file explorer window should be in a folder such as:
    ```
    C:\Users\<user>Roaming\Thunderstore Mod Manager\DataFolder\MageArena\profiles\<DEV PROFILE>
    ```
5. Copy this path and put it into `settings.bat`
    ```
	set THUNDERSTORE_LOCAL_MOD_PATH="<PUT PATH HERE>"
    ```
6. Running `build.bat` will now update your mod files in this profile.

## Start Development

The Mod Template contains two important subfolders:
```
   MyCoolMageArenaMod/
   |── packageFiles/ <-- Contains the files Thunderstore needs.
   └── src/ <-- Source files to build the .DLL file for your mod.
```

### packageFiles

The files in this folder are only used by Thunderstore to figure out the details of your mod. <br>
More details: [https://wiki.thunderstore.io/mods/packaging-your-mods]()
- `icon.png` is a `256x256 pixels` image that is the icon of your mod inside of Thunderstore.
- `manifest.json` contains the details of your mod. Name, Author, Version etc. the `setup.bat` script has already set these values for you. LEAVE VERSION AS MOD_VERSION, this allows the `build.bat` script to automatically change the version of your mod upon building it.
- `README.md` tells thunderstore what to show on your modpage. 

### src
 This is the most important folder for your mod. The files in here are what decide the functionality of your mod. The files and folders in here have `README.txt` to explain what they do and how to use them.
 - `.csproj` Tells `dotnet` how to build your mod and what reference/libraries to use.
 - `Plugin.cs` Primary code of your plugin. Its preloaded with the some of the minium code you need in. Read the comments in this file and edit it.
 	- `namespace ModTemplate;` is not set by `setup.bat`, change this manually to something Short and recognisable. 
 	- `MyGUID`and `PluginName` are also not set by `setup.bat`. Change this to fit the formatting described by the comments.
	- `VersionString` is updated on every build by `build.bat`, leave it and only change the version inside of `settings.bat`


#### src\Modules\
This is the folder where you should put all of the modules you write yourself. You will then use these in other `Modules`, `Plugin.cs` or inside `Patches`.
read more: [src/Modules](https://github.com/Walthzer/MageArenaModTemplate/tree/master/src/Modules)

#### src\Patches\
This is the folder where you should put the Patches you want to apply to the methods of `Mage Arena` itself. <br>
Read more: [src/Patches](https://github.com/Walthzer/MageArenaModTemplate/tree/master/src/Patches)

#### src\Data\
Files and Folders in the `Data` folder are copied and packaged next to the .DLL. You should put your Sprites and other Data files here.<br>

#### src\References\
Any .DLL files (Assembly References) you copy into this folder are automatically included by dotnet and any editor that uses `.csproj` files <br>
Read more: [src/References](https://github.com/Walthzer/MageArenaModTemplate/tree/master/src/References)

#### src\Resources\
Any file you add to this folder: Unity Assetbundles, Images, etc are embedded into your .DLL file. This is usefull in combination with the Asset Management provided by `BlackMagicAPI`, e.g. to load prefabs made in unity into the game without having to manually load them from disk.

