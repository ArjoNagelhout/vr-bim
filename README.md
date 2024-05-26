# VR BIM

This repository contains experiments with Revit and Unity, and enabling live editing of BIM data inside VR. This is not production ready code. 

It currently consists of three parts:
- `revit-to-vr-common`
- `revit-to-vr-plugin`
- `unity-revit-app`

## revit-to-vr-common

Common class library built to the `unity-revit-app/Assets/revit-to-vr-common-build` folder and included as a dependency in th revit-to-vr-plugin. This enables simple interchange of data from and to json. 

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