<img width="617" height="275" alt="easyCMSLogo" src="https://github.com/user-attachments/assets/f18e8d27-e5bf-49e1-aaea-fd2a1b026c8b" />

# :wrench: EasyCMS18
EasyCMS is a toolset created to simplify porting Automation creations exported to BeamNG to Car Mechanic Simulator 2018 and Car Mechanic Simulator VR. The tool automates porting BeamNG materials to Unity BRP materials and manages Asset Bundle creation and exporting to CMS

![202511~2](https://github.com/user-attachments/assets/c92d9014-402f-408d-aa07-895d8cca70d9)

* [:stop_sign: Limitations](#stop_sign-limitations)
* [:spiral_notepad: Prerequisites](#spiral_notepad-prerequisites)
* [:building_construction: Setup](#building_construction-setup)
* [:blue_car: Usage](#blue_car-usage)
* [:oncoming_automobile: Extras](#oncoming_automobile-extras)

# :stop_sign: Limitations
Before we proceed to the installation and usage guides, let us go over things EasyCMS **cannot** do:
* EasyCMS does not manage the models outside of generating configs based on imported materials. The models have to be set up manually to the requirements of CMS
* Materials created by EasyCMS do not utilize CMS material features such as painting, rust or light functions
* Glass parts do not get cut out fully, some glass reflection effects remain on parts of glass that are supposed to be cut out
* Because EasyCMS materials are mostly transparent to support Automation cutouts, some objects may appear through these materials
* Unity 5.6, used by CMS 2018 and VR, does not support single models with a vertex or face count of over 65534
# :spiral_notepad: Prerequisites
* An installed instance of the Unity 5.6 editor
* A reference of the CMS18 modding PDF Guide `Car Mechanic Simulator 2018\ModdingTools\PDFGuides\CMS18 Car Modding Guide.pdf` (You do **not** need to install CMS modding tools for Unity)
* 3D editing software that supports Collada (.dae) and FBX (.fbx) files (Blender recommended)
* Car Mechanic Simulator 2018 (Even when exporting to CMS VR, CMS VR does not have its own Car Editor)

# :building_construction: Setup
1. Download the latest EasyCMS18 Template release from the `Releases` section in this GitHub repository
2. Unpack the archive
3. Open Unity and add the unpacked folder as a project. Make sure the selected folder has folders named `Assets`, `Library` and `ProjectSettings` immediately inside it

> [!NOTE]
> If the project gets stuck infinitely loading - close it via task manager, in Unity Hub navigate to `Installs` and find your Unity 5.6 install. Right click on it and select `Show In Explorer`. In the opened directory, open `Unity.exe`

# :blue_car: Usage
While the guide below seems rather long, the process to making a basic working car can take approximately 5-10 minutes
## Car setup
First, let's prepare the Automation car itself. Make sure your car does not use modded materials that do not support exporting
* Remove the steering wheel and the seats. Car Mechanic Simulator adds its own
* Avoid fixtures going over the body's panel seams. This will not be an issue unless you want to separate the panels to work in CMS properly. Regardless, not all bodies are made the same and some of them will be a hassle to separate panels from
* Proceed to the export menu and select BeamNG.drive. **Make sure to uncheck `Zip-Pack Mod`**, it is also **highly** recommended to check `Merge All Fixtures` to simplify working with the exported models. Now export the car to BeamNG
## Set up the car project inside the Unity project
* In the Unity Editor's filesystem at the bottom, go to `Assets/Cars` and create a new folder for your car (`Right-click > Create > Folder`). The name of the folder will be the internal name of your car
* Right-click inside this folder and select `Create > EasyCMS > Car Manager` (do not mistake the `EasyCMS` option for `Create > EasyCMS`). This will create the core manager for your car, you can name this file anything you want

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/ceef5ce6-6edc-4bb8-967d-fcda8a568db5" />

* Select the newly created `Car Manager` file, fill out the `CMS Executable` field with the path to your CMS 2018 .exe file (you can click the `...` button next to it to open a file dialog). You should only need to do this once, any Car Managers you create after will look for other Car Managers and copy this path on creation
* Now fill out the `BeamNG Materials` path with the path to your car's `[UID].materials.json` file. By default, this is located in `C:\Users\[USER]\AppData\Local\BeamNG\BeamNG.drive\current\mods\unpacked\[MOD NAME]\vehicles\[CAR NAME]`

<img width="385" height="511" alt="image" src="https://github.com/user-attachments/assets/91fb42e3-a9cf-4ec6-b0e2-307a7a687fcf" />

* Once both paths are set up, click through all of the buttons in the `Import` section in order. This will copy the materials file and the textures related to it from your car and then build Unity BRP materials from them
* Once materials are built, you will see `Paint_[X]` assets created in your car's folder. These files are created for each of the paint slots found in the materials file. You can use these to change the paint of the car. This cannot be done in the game, so this is where you decide on the color and the paint parameters. In the `Materials` folder you will also find all materials imported from the materials file, you can edit each one separately here if you so desire

<img width="386" height="169" alt="image" src="https://github.com/user-attachments/assets/f31dc423-8eda-4680-bd8e-7abb9769f237" />

## Car model setup
* Open your 3D editing software of choice and import the car's model file from the exported files. By default this will be in `C:\Users\[USER]\AppData\Local\BeamNG\BeamNG.drive\current\mods\unpacked\[MOD NAME]\vehicles\[CAR NAME]\[UID]\[UID]_bodymesh.dae`. In Blender, make sure not to import the same car multiple times in one project as this will mess up the material names
* As per CMS requirements, there need to be at least 2 model files: `model.fbx` - containing all the models of the car excluding alternate body parts, and `collider.fbx` - containing just one model for the collider. Let's start with the `model.fbx` file
* CMS requires that `model.fbx` contains at least 2 models: one named `body` and one named `details`. The official documentation lists all valid object names, other names must not be present here. For a quick working example we can select the main body model and rename it to `body` and select the chassis model and rename it to `details`. All other objects should now we deleted. Select all remaining objects and apply all their transforms (in Blender, hit `Ctrl+A` and select `All Transforms`)
* We also want a way into the driver's seat, so if the body is enclosed it's a good idea to separate the driver's window (in Blender, go into `Edit Mod` (default hotkey: `Tab`), then into face mode (hotkey: `3`) and press `L` when hovering over the window, then press `P` and select `Selection`), name the new object `window_door_front_left` or `window_door_front_right` depending on the side of the car. We will talk about openable panels in a [separate section](#openable-panels)
* This is enough for a basic car mesh, export the models as FBX into your project at path `Assets/Cars/[CAR NAME]/model.fbx`. Make sure to export only the required models (in Blender, use `Selected Objects` or `Active Collection` in the export settings)
* Now let's create the collider model. In Blender, it is a good idea to create a separate collection for it. For a quick and simple collider we can simply create a cube and size it to the car's dimensions. Remember to apply transforms on it again if you sized it in object mode. The name of the new object must be `collider`
* Export the collider model to `Assets/Cars/[CAR NAME]/collider.fbx`. Once again make sure to export only the collider, not the entire car with it
* Now in Unity, you can drag the car out of the filesystem into the scene to make sure the model exported correctly and that materials correctly applied to it. This is also where you can find if any materials are wrong and can adjust them with a good reference

> [!IMPORTANT]
> Unity 5.6 does not support single models with a vertex or face count of over 65534. Most automation bodies combined with all fixtures are over this number. Unity will automatically separate these models into several - this will cause the models to not work properly in CMS. It is recommended to separate the body into panels so each model has smaller size

Your car folder should now look something like this:
<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/59ce0ce4-4354-495c-ab05-046fa50b37d8" />

## Generating configs
* Navigate to the `Car Manager` object for your car and click through the buttons in the `Templates` section. This will generate all config files required and used by CMS with EasyCMS templates that you can edit later. Instead of generating the `Car Config` (`config.txt`), you can grab one from the game files off a car that closer matches the car you are  porting. CMS car files can be found in `Car Mechanic Simulator 2018\cms2018_Data\StreamingAssets\Cars`. We will talk about what each config file does in a [further section](#config-files)
* Later you will likely edit these inside the game files, the `Updating Configs` section of the `Car Manager` can bring your project's config files up to date with the ones in your game files (exporting the car to the game files will also prompt you for this if applicable)
## Exporting and gameside edits
* Once ready to export the car to CMS, first click `Build Asset Bundle`. This will create the package that contains your car models so they can be loaded into CMS
* Now click `Copy Car Assets To Game`. This will take your car package and config files associated with it and copy them over to your game's directory so they are ready to be used in-game
* Open the CMS Car Editor and select your car there. Here you can properly set up mostly everything about the car itself. Make sure to periodically click `Save` in the bottom left corner as you make changes - **there is no autosave**
* In the bottom right corner you will find buttons to generate images for the car thumbnail and the car parts. Click them both

> [!IMPORTANT]
> Car Mechanic Simulator VR does **not** support CMS 2018 DLC. If you attempt to load a car that uses CMS 18 DLC content in CMS VR you will be stuck on a black screen. If this happens - it is a good indicator that the car likely uses DLC content

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/19c7cf05-e965-4327-8ef4-ef9d06bfec00" />

* Now you are ready to open the game itself and find your car in places defined by the config

> [!NOTE]
> In CMS 18 taking a body part off a car and putting it back on may change its color to white. Reloading the scene fixes that

# :oncoming_automobile: Extras
## Config files
There is a total of 4 config files that we are using
1. `name.txt` - This is simply the display name of the car, including the brand name but excluding the configuration name (the latter is part of the `config.txt` file)
2. `config.txt` - This is the main file of the car that manages most of the technical stuff. You mostly edit it via the CMS Car Editor.
3. `bodyconfig.txt` - This file manages links between body parts. The `unmount_with` section defines which parts are attached to other parts, which means the attached parts will be removed when the part they are attached to gets removed, and this also manages part attachment to openable parts. This file also manages alternate positions of license plates that are used if you have alternate parts license plates attach to. In the config generated by EasyCMS the front license plate attaches to the front bumper and the rear license plate attaches to the trunk by default. EasyCMS also automatically generates links between doors, door windows and mirrors
4. `parts.txt` - This file defines prices for the car's body parts. To be able to appear in the in-game part shop the part must be defined here and must have a price above 0. EasyCMS generates a price of 300 credits for each valid part found in the model file

## Openable panels
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/b13d664a-c30a-4768-8079-9bffe8222116" />

Openable panels are separate objects set up in a slightly different way from other body parts. Here, working in Blender will be referenced specifically, working in other software may be different.
To make openable doors, hoods, trunks etc. one must first separate these panels from the body. Liberate use of the `L` key when in face edit mode in Blender helps quickly select large surfaces, however be wary that the parts of the panels that go inside panel gaps are not usually picked up by this. With other more complex objects the X-Ray mode (default hotkey: `Alt-Z` or can be found as a button in the top right corner) paired with box or circle select (`B` or `C` hotkeys respectively) can help. Temporarily hiding faces with `H` can help get other faces out of the way so you don't accidentally select them along with the faces you want, you can unhide them again with `Alt-H`

Making the panels open correctly gets a bit weird. First, you need to set the object's origin point to the point around which the panel will rotate. The fastest (although inaccurate) way to do this is to `Shift-Right Click` on the point where you want the origin to be, this will set the 3D cursor to that point, then bring up the search menu (default hotkey: `F3`) and find and click `Origin to 3D Cursor` with the object selected. But we are not done yet, the axis needs to be set up in an unintuitive way

To set up the axis, first navigate to the top menu and change `Transformation Orientation` to `Local`. Now select the object you want to set up. You will now have to do different things depending on what panel this is:
* Hood: Rotate on the `Y` axis `-90 degrees` to open the hood forwards or `90 degrees` for backwards. Hit `Ctrl+A`, then `Rotation`. Rotate on the `Y` axis `90 degrees` or `-90 degrees` depending on what you did earlier respectively. Ensure that rotating the object on the `Z` axis to **negative** values opens the panel the correct way
* Trunk: Rotate on the `Y` axis `90 degrees` to open the trunk forwards or `-90 degrees` for backwards. Hit `Ctrl+A`, then `Rotation`. Rotate on the `Y` axis `-90 degrees` or `90 degrees` depending on what you did earlier respectively. Ensure that rotating the object on the `Z` axis to **positive** values opens the trunk the correct way
* Left Doors: Rotate on the `X` axis `-90 degrees`. Hit `Ctrl+A`, then `Rotation`. Rotate on the `X` axis `90 degrees`. Ensure that rotating the object on the `Y` axis to **negative** values opens the panel the correct way
* Right Doors: Rotate on the `X` axis `90 degrees`. Hit `Ctrl+A`, then `Rotation`. Rotate on the `X` axis `-90 degrees`. Ensure that rotating the object on the `Y` axis to **negative** values opens the panel the correct way

## Exporting to CMS VR
To export to CMS VR the car should first be exported to CMS 2018, where it can be edited via its Car Editor. Once you are ready to send the car to CMS VR, find it in `Car Mechanic Simulator 2018\cms2018_Data\StreamingAssets\Cars` and copy it over to `Car Mechanic Simulator 2018 - VR\cms_Data\StreamingAssets\Cars`

Also note that high poly areas of the car cause the game to start lagging heavily during physics interactions: this includes any physics objects, liquids and even your hands. Try to keep high poly areas a good distance away from areas that are to be used often
