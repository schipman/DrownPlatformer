using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// The pause menu (ESC): resume, switch screen effects on and off (the choice is kept for next time), or quit the game.
/// Up/Down choose, ENTER, Z or the pad's A switch the item, ESC or Start resumes.
/// </summary>
public sealed class PauseState : FlxSubState
{
    /// <summary>The effects on the menu (names from FlxPostProcessLibrary), besides the water wobble.</summary>
    static readonly string[] Effects = { "Refraction", "Bloom", "Vignette", "ChromaticAberration", "Desaturate", "NightVision", "VHS", "CRT" };

    readonly PlayState _play;
    readonly List<(string Label, System.Func<string> Value, System.Action Activate)> _items = new();
    readonly List<FlxText> _labels = new();
    int _selected;

    public PauseState(PlayState play) : base(new Color(0, 0, 20, 170)) { _play = play; }

    public override void Create()
    {
        _items.Add(("RESUME", () => "", Close));
        _items.Add(("Water wobble", () => _play.WaterWobble ? "ON" : "OFF", () => _play.WaterWobble = !_play.WaterWobble));
        foreach (var name in Effects)
        {
            string n = name;
            _items.Add((Nice(n), () => FlxG.PostProcess.FindGlobal(n) != null ? "ON" : "OFF", () => Toggle(n)));
        }
        _items.Add(("QUIT GAME", () => "", () => FlxG.Game.Exit()));

        int w = PlayState.Width;
        Add(new FlxText(0, 24, w, "PAUSED") { Alignment = FlxTextAlign.Center, Size = 16, Tint = new Color(190, 235, 255) });
        float y = 54;
        foreach (var _ in _items)
        {
            var t = new FlxText(0, y, w, "") { Alignment = FlxTextAlign.Center };
            _labels.Add(t);
            Add(t);
            y += 15;
        }
        Add(new FlxText(0, PlayState.Height - 16, w, "Up/Down: choose   Enter or Z: switch   Esc: resume") { Alignment = FlxTextAlign.Center, Tint = new Color(110, 160, 190) });
        // the camera is somewhere in the room: everything here stays on the screen
        foreach (var m in Members) if (m is FlxObject o) o.ScrollFactor = Vector2.Zero;
        Refresh();
    }

    static string Nice(string name) => name switch
    {
        "ChromaticAberration" => "Chromatic aberration",
        "NightVision" => "Night vision",
        _ => name,
    };

    static void Toggle(string name)
    {
        var existing = FlxG.PostProcess.FindGlobal(name);
        if (existing != null) FlxG.PostProcess.RemoveGlobal(existing);
        else FlxG.PostProcess.AddGlobal(FlxPostProcessLibrary.Create(name));
    }

    void Refresh()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            var (label, value, _) = _items[i];
            string v = value();
            _labels[i].Text = (i == _selected ? "> " : "  ") + label + (v.Length > 0 ? ":  " + v : "") + (i == _selected ? " <" : "  ");
            _labels[i].SetFormat(8, i == _selected ? Color.Yellow : new Color(170, 200, 220), FlxTextAlign.Center);
        }
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);

        var pad = FlxG.Gamepad;
        if (FlxG.Keys.JustPressed(Keys.Down) || pad.JustPressed(FlxGamepadInputID.DPadDown)) { _selected = (_selected + 1) % _items.Count; Refresh(); }
        if (FlxG.Keys.JustPressed(Keys.Up) || pad.JustPressed(FlxGamepadInputID.DPadUp)) { _selected = (_selected + _items.Count - 1) % _items.Count; Refresh(); }
        if (FlxG.Keys.AnyJustPressed(Keys.Enter, Keys.Z, Keys.Space) || pad.JustPressed(FlxGamepadInputID.A))
        {
            _items[_selected].Activate();
            Refresh();
        }
        if (FlxG.Keys.AnyJustPressed(Keys.Escape, Keys.P) || pad.JustPressed(FlxGamepadInputID.Start)) Close();
    }
}
