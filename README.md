# Drown Platformer

A port of the original XNA game (DrownPlatformer) to the MonoFlixel engine (`Flixel/`, MIT; a C#/MonoGame port of HaxeFlixel).
Run with `dotnet run`.

<img width="2558" height="1440" alt="0We4UQeCEw" src="https://github.com/user-attachments/assets/466a21ce-9b54-46fe-a296-5e5ae009c085" />



## Building an installer

`installer/build-installer.ps1 -Version 0.1.0` (needs the .NET 8 SDK and Inno Setup 6) publishes a self-contained win-x64 build, with the .NET runtime bundled, and writes `dist/DrownPlatformer-Setup-<version>.exe`.
