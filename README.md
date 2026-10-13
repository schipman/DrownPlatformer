# Drown Platformer

A port of the original XNA game (D:\PROJECTS\DrownPlatformer) to the MonoFlixel engine (`Flixel/`, MIT; a C#/MonoGame port of HaxeFlixel).
Run with `dotnet run`.

## What is ported
* Title screen (`Game/MainMenuState.cs`): logo bounce, title theme, water wobble.
* 25 rooms (`Game/RoomData.cs` was generated from the original RoomManager switch; maps are the original CSV layers in `Assets/maps`),
  with exits, zone changes (new music, longer fade) and the diver coming in on the opposite side.
* Diver (`Game/Player.cs`): arrows move/aim, X jump, Z fire, C weapon, TAB float; oxygen and health; harpoon and explosive weapons.
* Creatures (`Game/Mobs.cs`, placed from each room's Enemies map): crab, sea snail, sea snake, barracuda, electric eel, man o' war,
  puffer, pain kelp, spike crustacean, floor claw, phantom and the nautilus boss.
* HUD bars and weapon slots, ambience, footsteps and weapon sounds.

* Start beside the crashed ship with a typewriter intro (Z skips/advances), a pause menu (ESC: resume, effects on/off, quit), and a
  Refraction screen effect (Content/Shaders/Refraction.fx, from the original refraction.fx, applied lightly).

* Polish: PLATFORMER rides in under the logo; electric eels and shots glow (`Game/Glow.cs`, additive light sprites); rooms change with the
  engine's tile transition (diamonds sweep the way the diver goes, circles for a zone change).

## Not yet ported
Lighting (darkness + light mask), the intro dialog box, pause, the splash screen and the Xbox load screen.

## Building an installer

`installer/build-installer.ps1 -Version 0.1.0` (needs the .NET 8 SDK and Inno Setup 6) publishes a self-contained win-x64 build, with the .NET runtime bundled, and writes `dist/DrownPlatformer-Setup-<version>.exe`.
