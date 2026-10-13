using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>A small bar: the empty frame picture with a colored fill that grows with the value (0-100).</summary>
public sealed class ProgressBar : FlxGroup
{
    readonly FlxSprite _fill;
    public float Value = 100;

    public ProgressBar(float x, float y, Color color)
    {
        var frame = new FlxSprite(x, y).LoadGraphic("images/progressBar.png");
        frame.ScrollFactor = Vector2.Zero;
        _fill = new FlxSprite(x, y).MakeGraphic(50, 8, color);
        _fill.Origin = Vector2.Zero;
        _fill.ScrollFactor = Vector2.Zero;
        Add(frame);
        Add(_fill);
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        _fill.Scale = new Vector2(MathHelper.Clamp(Value / 100f, 0f, 1f), 1);
    }
}

/// <summary>A weapon slot icon, big when it is the weapon in hand.</summary>
public sealed class WeaponSlot : FlxSprite
{
    readonly Player _player;
    readonly Weapon? _weapon;

    public WeaponSlot(float x, float y, Player player, Weapon? weapon) : base(x, y)
    {
        _player = player;
        _weapon = weapon;
        LoadGraphic(weapon?.Icon ?? "images/UI_Empty_Select.png");
        ScrollFactor = Vector2.Zero;
        Scale = new Vector2(0.5f, 0.5f);
    }

    public override void Update(float elapsed)
    {
        float s = _weapon != null && _player.CurrentWeapon == _weapon ? 1f : 0.5f;
        Scale = new Vector2(s, s);
        base.Update(elapsed);
    }
}
