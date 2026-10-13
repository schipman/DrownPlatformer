using System;
using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// The diver. Sinks, jumps with X, floats up for a moment with TAB (then has to wait before floating again), shoots with Z and changes
/// weapon with C. Air runs down slowly; at zero the suit starts to take damage.
/// </summary>
public class Player : FlxSprite
{
    public const int TileSize = 16;

    public float MaxOxygen = 100, Oxygen = 100;
    public float MaxHealth = 100;
    public int Gravity = 60;
    public bool ControlsEnabled = true;

    public readonly Weapon Harpoon = new(ProjectileType.Harpoon);
    public readonly Weapon Explosive = new(ProjectileType.Explosive);
    public Weapon CurrentWeapon;
    public readonly FlxEmitter Bubbles;

    /// <summary>Set by the play state: where the player may go.</summary>
    public float StatusTimer;

    readonly DrownInput _input;
    bool _floating;
    float _floatTime, _floatCooldown = FloatCooldownMax, _breath, _suffocate, _hurtFlash;
    int _lastFrame = -1, _step;
    AimDirection _lastHorizontal = AimDirection.Left;

    const float OxygenLossRate = 0.5f, OxygenLossTime = 2f, FloatCooldownMax = 2f, FloatTimeMax = 1.5f, JumpPower = 80;

    public Player(DrownInput input, float x, float y) : base(x, y)
    {
        _input = input;
        Health = MaxHealth;
        CurrentWeapon = Harpoon;

        LoadGraphic("images/diver_atlas.png", animated: true, frameWidth: 16, frameHeight: 32);
        Animation.Add("walk", new[] { 0, 1, 2 }, 15);
        Animation.Add("idle", new[] { 3 });
        Animation.Add("jump", new[] { 4 });
        Animation.Add("aim_up", new[] { 5 });
        SetFacingFlip(FlxDirection.Left, true, false);
        Facing = FlxDirection.Left;

        Width = 13;
        Height = 32;
        Offset = new Vector2(1.5f, 0);

        Drag = new Vector2(100, 200);
        MaxVelocity = new Vector2(40, 77);
        Acceleration.Y = Gravity;

        Bubbles = new FlxEmitter(x, y);
        Bubbles.LoadParticles("images/bubble_particle.png", 10, multiple: true);
        Bubbles.Velocity.Set(-2, -30, 2, -30);
        Bubbles.Acceleration.Set(0, -30, 0, -30);
        Bubbles.Lifespan.Set(1.5f, 3f);
        Bubbles.Start(false, 0.5f);
    }

    public void Respawn(float x, float y)
    {
        Reset(x, y);
        Health = MaxHealth;
        Oxygen = MaxOxygen;
        _floating = false;
        Acceleration.Y = Gravity;
    }

    public bool IsDead => Health <= 0;

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        Health -= amount;
        _hurtFlash = 0.25f;
        FlxG.Sound.Play("sfx/hurt.wav", 0.7f);
    }

    public override void Update(float elapsed)
    {
        // status effects (stuns) block the controls
        if (StatusTimer > 0) StatusTimer -= elapsed;
        bool canAct = ControlsEnabled && StatusTimer <= 0 && !IsDead;

        // the air runs down; with none left the suit takes damage
        _breath += elapsed;
        if (_breath >= OxygenLossTime) { _breath = 0; Oxygen = Math.Max(0, Oxygen - OxygenLossRate); }
        if (Oxygen <= 0)
        {
            _suffocate += elapsed;
            if (_suffocate >= 1) { _suffocate = 0; TakeDamage(2); }
        }

        Bubbles.X = X + Width * 0.5f;
        Bubbles.Y = Y;

        Acceleration.X = 0;
        float mx = canAct ? _input.Move.X : 0, my = canAct ? _input.Move.Y : 0;
        if (mx < -0.5f) { Facing = FlxDirection.Left; Acceleration.X = -Drag.X; _lastHorizontal = AimDirection.Left; }
        else if (mx > 0.5f) { Facing = FlxDirection.Right; Acceleration.X = Drag.X; _lastHorizontal = AimDirection.Right; }

        if (canAct)
        {
            if (_input.NextWeapon.Triggered) CurrentWeapon = CurrentWeapon == Harpoon ? Explosive : Harpoon;
            if (_input.Jump.Triggered && Velocity.Y == 0) Velocity.Y = -JumpPower;
            if (_input.Fire.Triggered) Shoot(mx, my);
            if (_input.Float.Triggered && _floatCooldown >= FloatCooldownMax)
            {
                _floating = true;
                _floatCooldown = 0;
                FlxG.Sound.Play("sfx/air_intake.wav", 0.5f);
            }
        }

        // float up for a little while, then sink again
        if (_floating)
        {
            _floatTime += elapsed;
            Acceleration.Y = -Gravity;
            if (_floatTime >= FloatTimeMax) { _floating = false; _floatTime = 0; }
        }
        else
        {
            Acceleration.Y = Gravity;
            _floatCooldown += elapsed;
        }

        // animation
        string anim = Velocity.Y != 0 ? "jump" : Velocity.X == 0 ? "idle" : "walk";
        Animation.Play(anim);
        if (anim == "walk" && Animation.FrameIndex == 1 && _lastFrame != 1)
            FlxG.Sound.Play(++_step % 2 == 0 ? "sfx/step_1.wav" : "sfx/step_2.wav", 0.3f);
        _lastFrame = Animation.FrameIndex;

        if (_hurtFlash > 0) { _hurtFlash -= elapsed; Tint = ((int)(_hurtFlash * 30) % 2 == 0) ? Color.Red : Color.White; }
        else Tint = Color.White;

        base.Update(elapsed);
    }

    void Shoot(float mx, float my)
    {
        bool up = my < -0.5f, down = my > 0.5f, left = mx < -0.5f, right = mx > 0.5f;
        AimDirection dir = (up, down, left, right) switch
        {
            (true, _, true, _) => AimDirection.NorthWest,
            (true, _, _, true) => AimDirection.NorthEast,
            (_, true, true, _) => AimDirection.SouthWest,
            (_, true, _, true) => AimDirection.SouthEast,
            (true, _, _, _) => AimDirection.Up,
            (_, true, _, _) => AimDirection.Down,
            _ => _lastHorizontal,
        };
        // horizontal shots leave the gun; the rest from the middle of the diver
        float fx = dir == AimDirection.Left ? X - 9 : dir == AimDirection.Right ? X + 9 : X;
        CurrentWeapon.Fire(fx, Y + (dir is AimDirection.Left or AimDirection.Right ? 6 : 0), dir);
    }
}
