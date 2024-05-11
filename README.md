# VR BIM

This repository contains experiments with Revit and Unity, and enabling live editing of BIM data inside VR. This is not production ready code. 

It currently consists of two parts:

## revit-to-vr-plugin

Revit plugin (a .NET .dll that gets loaded by Revit). 

Written in C#. 

The following tutorial was used to create a plugin:

https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7I2bC1zUr4VjJ3U31uM66K.html

## unity-revit-app

Unity app that receives data from the Revit plugin. 