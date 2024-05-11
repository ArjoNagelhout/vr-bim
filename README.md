# VR BIM

This repository contains experiments with Revit and Unity, and enabling live editing of BIM data inside VR. This is not production ready code. 

It currently consists of two parts:

## revit-to-vr-plugin

Revit plugin (a .NET .dll that gets loaded by Revit). 

Written in C#, .NET 4.8

The following tutorial was used to create the plugin:

https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7I2bC1zUr4VjJ3U31uM66K.html

## Installing revit plugin for development purposes

Needs administrator privileges to access the C:\Program Files\Autodesk\Revit 2024\AddIns directory. 

### Debug
This only copies the .addin file, but keeps the .dll in its original location. 

```batch
python install_addin_debug.py
```

### Release
This creates a directory in the AddIns directory that contains the copied .dll. 

```batch
python install_addin_release.py
```

### Deinstall
This removes both Debug and Release addins from the AddIns directory. 

```bat
python deinstall_addins.py
```

## unity-revit-app

Unity app that receives data from the Revit plugin. 