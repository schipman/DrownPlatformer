using DrownPlatformer;
using MonoFlixel;

using var game = new FlxGame(MainMenuState.Width, MainMenuState.Height, () => new MainMenuState(), zoom: MainMenuState.Zoom,
    scaleMode: FlxScaleMode.Integer, framerate: 60, title: "Drown Platformer");
game.Run();
