# VR BIM

This repository contains experiments in creating a live-link between Revit and Unity to enable live editing of BIM data in VR. This is not production ready code. It's a proof-of-concept created as part of my BSc thesis at the Eindhoven University of Technology to validate technical feasibility and perform user studies. 

It does not adhere to any best practices, but it might provide some valuable insight into how to build an application that strives to have this type of functionality, which is the reason I made this open-source. 

In terms of codebase architecture, the project consists of three modules:

- **`revit-to-vr-common`**
- **`revit-to-vr-plugin`**
- **`unity-revit-app`**

## revit-to-vr-common

Common class library built to the `unity-revit-app/Assets/revit-to-vr-common-build` folder and included as a dependency in both the `revit-to-vr-plugin` and the `unity-revit-app` project. This enables simple interchange of data from and to json between the Revit plugin and the VR Application, as it is exactly the same C# objects and the same configuration for the JsonSerializer. 

## revit-to-vr-plugin

Revit plugin (a .NET 4.8 .dll that gets loaded by Revit). 

### Installing Revit plugin for development purposes

#### Debug
This only copies the .addin file, but keeps the .dll in its original location. This script only needs to be run once, because when the .dll gets recompiled, the .addin now refers to the new one. 

```bat
python scripts/install_addin_debug.py
```

To debug the Revit plugin from Visual Studio, inside `Properties > Debug` of the `revit-to-vr-plugin` project, set **`Start Action`** to `Start External Program` and the path to the Revit .exe, e.g. `C:\Program Files\Autodesk\Revit 2024\Revit.exe`. 

#### Release
This creates a directory in the AddIns directory that contains the copied .dll. 

```bat
python scripts/install_addin_release.py
```

#### Deinstall
This removes both Debug and Release addins from the AddIns directory. 

```bat
python scripts/deinstall_addins.py
```

## unity-revit-app

Unity app that receives data from the Revit plugin. 

The target platform for the app is the Meta Quest Pro, but any Meta Quest (e.g. 2 or 3) should work. 