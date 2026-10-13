using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>An empty main menu: a title and placeholder items to fill in.</summary>
public class MainMenuState : FlxState
{
    public const int Width = 640, Height = 360, Zoom = 2;

    static readonly string[] Items = { "START", "OPTIONS", "QUIT" };
    readonly FlxText[] _labels = new FlxText[Items.Length];
    int _selected;

    public override void Create()
    {
        FlxG.Game.Window.Title = "Drown Platformer";
        FlxG.Cameras.BgColor = new Color(8, 16, 32);

        Add(new FlxText(0, 70, Width, "DROWN PLATFORMER") { Alignment = FlxTextAlign.Center, Size = 16 });
        for (int i = 0; i < Items.Length; i++)
        {
            _labels[i] = new FlxText(0, 160 + i * 24, Width, Items[i]) { Alignment = FlxTextAlign.Center };
            Add(_labels[i]);
        }
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < _labels.Length; i++)
        {
            _labels[i].SetFormat(8, i == _selected ? Color.Yellow : Color.Gray, FlxTextAlign.Center);
            _labels[i].Text = (i == _selected ? "> " : "") + Items[i];
        }
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        if (FlxG.Keys.JustPressed(Keys.Down) || FlxG.Keys.JustPressed(Keys.S)) { _selected = (_selected + 1) % Items.Length; Refresh(); }
        if (FlxG.Keys.JustPressed(Keys.Up) || FlxG.Keys.JustPressed(Keys.W)) { _selected = (_selected + Items.Length - 1) % Items.Length; Refresh(); }
        if (FlxG.Keys.JustPressed(Keys.Enter) || FlxG.Keys.JustPressed(Keys.Space))
        {
            if (_selected == 0) FlxG.SwitchState(new PlayState(), PlayState.Width, PlayState.Height, PlayState.Zoom);
            else if (_selected == Items.Length - 1) FlxG.Game.Exit();
            // START / OPTIONS: nothing yet
        }
        if (FlxG.Keys.JustPressed(Keys.Escape)) FlxG.Game.Exit();
    }
}
