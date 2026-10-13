using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// A soft round light drawn with additive blending (it brightens what is behind it). Put one on something that should glow and call
/// <see cref="CenterOn"/> and <see cref="Draw"/> each frame, or let <see cref="FlxSprite"/>-derived classes own one.
/// </summary>
public sealed class Glow : FlxSprite
{
    const int Size = 64;
    static Texture2D? _texture;

    /// <summary>Strength of the pulse (0: steady) and its speed.</summary>
    public float PulseAmount, PulseSpeed = 8f, BaseAlpha = 0.7f;
    float _time, _phase;

    public Glow(Color color, float radius, float pulseAmount = 0f, float baseAlpha = 0.7f) : base(0, 0)
    {
        LoadGraphic(Texture());
        Tint = color;
        Blend = FlxBlend.Add;
        Scale = new Vector2(radius * 2 / Size);
        Moves = false;
        PulseAmount = pulseAmount;
        BaseAlpha = baseAlpha;
        Alpha = baseAlpha;
        _phase = Random.Shared.NextSingle() * MathF.Tau;
    }

    /// <summary>A white disc that fades out smoothly towards its edge.</summary>
    static Texture2D Texture()
    {
        if (_texture != null) return _texture;
        var pixels = new Color[Size * Size];
        for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                float dx = (x + 0.5f) / Size * 2 - 1, dy = (y + 0.5f) / Size * 2 - 1;
                float d = MathF.Sqrt(dx * dx + dy * dy);
                float a = MathF.Max(0, 1 - d);
                a *= a;
                pixels[y * Size + x] = new Color((byte)255, (byte)255, (byte)255, (byte)(a * 255));
            }
        _texture = new Texture2D(FlxG.GraphicsDevice, Size, Size);
        _texture.SetData(pixels);
        return _texture;
    }

    public void CenterOn(float x, float y)
    {
        X = x - Size * 0.5f;
        Y = y - Size * 0.5f;
    }

    public void CenterOn(FlxObject o) => CenterOn(o.X + o.Width * 0.5f, o.Y + o.Height * 0.5f);

    public override void Update(float elapsed)
    {
        _time += elapsed;
        if (PulseAmount > 0) Alpha = Math.Clamp(BaseAlpha + MathF.Sin(_time * PulseSpeed + _phase) * PulseAmount, 0f, 1f);
        base.Update(elapsed);
    }
}
