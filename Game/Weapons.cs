using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

public enum ProjectileType { Harpoon, Explosive }

public enum AimDirection { Left, Right, Up, Down, NorthEast, NorthWest, SouthEast, SouthWest }

/// <summary>A shot: flies, explodes on rock or when something takes it.</summary>
public sealed class Projectile : FlxSprite
{
    public int Damage = 1;
    public bool Dying { get; private set; }
    ProjectileType _type;

    public Projectile(ProjectileType type) : base(0, 0)
    {
        _type = type;
        LoadGraphic(type == ProjectileType.Harpoon ? "images/k_beam.png" : "images/e_beam.png", animated: true, frameWidth: 16, frameHeight: 16);
        Animation.Add("fire", FlxAnimationController.Range(0, 5), 45, looped: false);
        Animation.Add("explode", new[] { 6, 7 }, 15, looped: false);
        CenterOrigin();
        Exists = false;
    }

    public void Fire(float x, float y, AimDirection dir, float speed)
    {
        Reset(x, y);
        Dying = false;
        Solid = true;
        Visible = true;
        Acceleration = Vector2.Zero;
        float s = speed;
        float d = speed * 0.75f;
        (Velocity, Angle) = dir switch
        {
            AimDirection.Left => (new Vector2(-s, 0), 180f),
            AimDirection.Right => (new Vector2(s, 0), 0f),
            AimDirection.Up => (new Vector2(0, -s), -90f),
            AimDirection.Down => (new Vector2(0, s), 90f),
            AimDirection.NorthEast => (new Vector2(d, -d), -45f),
            AimDirection.SouthEast => (new Vector2(d, d), 45f),
            AimDirection.NorthWest => (new Vector2(-d, -d), -135f),
            _ => (new Vector2(-d, d), 135f),
        };
        if (_type == ProjectileType.Explosive) { Acceleration.Y = 100; AngularVelocity = 350; }
        else AngularVelocity = 0;
        Animation.Play("fire", true);
    }

    /// <summary>Stop and play the burst; the shot is gone when it finishes.</summary>
    public void Explode(bool sound = true)
    {
        if (Dying) return;
        Dying = true;
        Solid = false;
        Velocity = Acceleration = Vector2.Zero;
        Angle = 0;
        AngularVelocity = 0;
        Animation.Play("explode", true);
        if (sound) FlxG.Sound.Play("sfx/explode1.wav", 0.6f);
    }

    public override void Update(float elapsed)
    {
        if (Dying && Animation.Finished) { Exists = false; return; }
        base.Update(elapsed);
        if (!Dying && !IsOnScreen()) Exists = false;
    }
}

/// <summary>A pool of shots of one kind.</summary>
public sealed class Weapon : FlxTypedGroup<Projectile>
{
    public readonly ProjectileType Type;
    public readonly string Icon;
    public readonly float Speed;
    public readonly int Damage;
    public readonly string FireSound;

    public Weapon(ProjectileType type, int pool = 40)
    {
        Type = type;
        Icon = type == ProjectileType.Harpoon ? "images/UI_K_Beam_Select.png" : "images/UI_E_Beam_Select.png";
        Speed = type == ProjectileType.Harpoon ? 250 : 120;
        Damage = 1;
        FireSound = type == ProjectileType.Harpoon ? "sfx/laser_shoot1.wav" : "sfx/launcher.wav";
        for (int i = 0; i < pool; i++) Add(new Projectile(type) { Damage = Damage });
    }

    public void Fire(float x, float y, AimDirection dir)
    {
        var p = GetFirstAvailable();
        if (p == null) return;
        p.Fire(x, y, dir, Speed);
        FlxG.Sound.Play(FireSound, 0.6f);
    }

    public void ResetShots()
    {
        foreach (var p in Members) p.Exists = false;
    }
}
