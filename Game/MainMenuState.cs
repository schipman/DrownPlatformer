using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// The title screen, from the original game's main menu: the underwater picture with the water wobble over it, the title theme, and
/// the DROWN logo that rises from the bottom and bounces into place over ten seconds, ending with a flash and a thump.
/// Z, ENTER, SPACE or the pad's A starts the game.
/// </summary>
public class MainMenuState : FlxState
{
    public const int Width = 320, Height = 240, Zoom = 3;

    FlxSprite _logo = null!;
    FlxText _prompt = null!, _subtitle = null!;
    bool _landed, _going;
    float _time;

    public override void Create()
    {
        FlxG.Game.Window.Title = "Drown";
        FlxG.Cameras.BgColor = new Color(0, 20, 50);
        FlxG.Mouse.Visible = false;

        var backdrop = new FlxSprite(0, 0).LoadGraphic("images/mainscreen.png");
        backdrop.Moves = false;
        Add(backdrop);

        // the logo starts below the screen and rises 170 pixels, bouncing as it lands
        _logo = new FlxSprite(Width / 2f - 100, Height).LoadGraphic("images/DrownLogo.png");
        _logo.DrawAfterPostProcess = true;
        _logo.Moves = false;
        Add(_logo);
        FlxTween.Tween(_logo, new { Y = Height - 170f }, 10f, new TweenOptions { Ease = FlxEase.BounceOut, OnComplete = _ => Land() });

        // PLATFORMER rides in under the logo
        _subtitle = new FlxText(0, Height + 44, Width, "P L A T F O R M E R") { Alignment = FlxTextAlign.Center, Size = 16, Tint = new Color(150, 230, 255) };
        _subtitle.SetBorderStyle(FlxTextBorderStyle.Outline, new Color(0, 40, 90), 1f);
        Add(_subtitle);

        // the water wobble over the picture (not over the text)
        FlxG.PostProcess.Set(FlxPostProcess.Load("Wiggle", ("Amplitude", 0.0035f), ("Frequency", 18f), ("Speed", 2.5f)));

        _prompt = new FlxText(0, Height - 40, Width, "") { Alignment = FlxTextAlign.Center, Tint = new Color(190, 235, 255) };
        var credits = new FlxText(0, Height - 25, Width, "A game by Scott Chipman and Gavin Allen") { Alignment = FlxTextAlign.Center, Tint = new Color(170, 215, 235) };
        var hint = new FlxText(0, Height - 12, Width, "ESC: quit") { Alignment = FlxTextAlign.Center, Tint = new Color(110, 160, 190) };
        Add(_prompt);
        Add(credits);
        Add(hint);

        // the title theme; the first frame can take a moment while the MP3 is decoded
        FlxG.Sound.PlayMusic("music/title_theme.mp3", 0.4f);

        FlxG.Camera.Fade(Color.Black, 1.5f, fadeIn: true);
    }

    void Land()
    {
        _landed = true;
        FlxG.Sound.Play("sfx/drown_logobump.wav");
        FlxG.Camera.Flash(Color.Aqua, 0.5f);
    }

    void Start()
    {
        if (_going) return;
        _going = true;
        FlxG.Sound.Music?.FadeOut(1f);
        FlxG.Camera.Fade(Color.Black, 1f, onComplete: () =>
        {
            FlxG.Sound.Music?.Stop();
            FlxG.PostProcess.Clear();
            FlxG.SwitchState(new PlayState(), PlayState.Width, PlayState.Height, PlayState.Zoom);
        });
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        _time += elapsed;
        _subtitle.Y = _logo.Y + 44;
        if (_landed)
        {
            // after landing it shimmers: the color slides between aqua and white
            float k = 0.5f + 0.5f * MathF.Sin(_time * 3f);
            _subtitle.Tint = Color.Lerp(new Color(110, 215, 255), Color.White, k);
        }

        // "press Z" blinks once the logo is down
        _prompt.Text = _landed ? (_time % 1.2f < 0.8f ? "Press Z  (or A) to start" : "") : "";

        if (_going) return;
        if (FlxG.Keys.AnyJustPressed(Keys.Z, Keys.Enter, Keys.Space) || FlxG.Gamepad.JustPressed(FlxGamepadInputID.A)) Start();
        if (FlxG.Keys.JustPressed(Keys.Escape)) FlxG.Game.Exit();
    }
}
