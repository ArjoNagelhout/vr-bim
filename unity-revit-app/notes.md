# notes

https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html

How do we get the websocket-sharp as a dependency into Unity?

We could simply compile to a .dll, but where's the fun in that?

https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html

Assembly definitions in Unity allow specifying to which assembly scripts belong. 

For now, we'll simply use a compiled .dll. 
Maybe a python script for automatically copying the dll might be a good idea. But for now this suffices. 