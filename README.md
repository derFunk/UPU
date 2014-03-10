UPU - .unityPackage unpacker
===

This little Windows tool helps you to unpack Unity Packages from [Unity 3D](http://www.unity3d.com/ "Unity 3D"), and avoids that you have to open the Unity Editor in order to access the precious asset files bundled within the package.

It also can add a context menu handler for the Windows Explorer which makes extraction of the files a lot easier.

What's working already?
---

1. You can extract the whole unity package to a defined directory, or the same directory where the package resides in.
2. Add/Remove Windows Explorer context menu handlers for *.unitypackage files.

Usage
---
upu.exe [options]

**Options:**<br /> 
-i, --input: Unitypackage input file.<br /> 
-o, --output: The output path of the extracted unitypackage.<br /> 
-r, --register: Register context menu handler.<br /> 
-u, --unregister: Unregister context menu handler.<br /> 


Roadmap (Todos)
---

1. Add option to only display the file structure which the package contains to be able to extract only certain files on command line.
2. Add graphical user interface with the option to only extract certain files.

Download
---
Version 0.0.0.1: https://github.com/ChimeraEntertainment/UPU/releases/download/0.0.0.1/UnityPackageUnpacker-0.0.0.1.zip

Search Engine Keywords:
unity3d, unitypackage, unpack, extract, deflate, assets
