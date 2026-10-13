using System;
using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// Base of the creatures. They hurt the diver while touching him (a hit every half second), and die when their health runs out.
/// Most pictures face left; a creature whose picture faces right calls <see cref="SetArtFacesRight"/>. It is flipped to face where it moves.
/// </summary>
public abstract class Mob : FlxSprite
{
    protected readonly Player Diver;
    protected readonly FlxTilemap Map;
    protected int ContactDamage = 1;
    float _hurtTime;

    /// <summary>True for the ones that bump into the rock (the enemy manager collides them with the map).</summary>
    public bool CollidesWithMap;

    protected Mob(float x, float y, FlxTilemap map, Player player) : base(x, y)
    {
        Map = map;
        Diver = player;
        SetArtFacesRight(false);
    }

    protected void SetArtFacesRight(bool right)
    {
        SetFacingFlip(FlxDirection.Right, !right, false);
        SetFacingFlip(FlxDirection.Left, right, false);
        Facing = FlxDirection.Right;
    }

    protected void FaceMoving(float vx)
    {
        if (vx > 0) Facing = FlxDirection.Right;
        else if (vx < 0) Facing = FlxDirection.Left;
    }

    protected float DistanceToDiver => Vector2.Distance(new Vector2(X, Y), new Vector2(Diver.X, Diver.Y));

    public override void Update(float elapsed)
    {
        if (Health <= 0) { Kill(); return; }
        base.Update(elapsed);
        Think(elapsed);
        if (Exists && Alive && CanHurt && FlxObject.OverlapsPair(Diver, this)) Touch(elapsed);
    }

    protected virtual bool CanHurt => true;

    protected abstract void Think(float elapsed);

    protected virtual void Touch(float elapsed)
    {
        _hurtTime += elapsed;
        if (_hurtTime >= 0.5f)
        {
            _hurtTime = 0;
            Diver.TakeDamage(ContactDamage);
            OnHurtDiver();
        }
    }

    protected virtual void OnHurtDiver() { }

    /// <summary>Hit by a shot. Returns true if the shot should burst.</summary>
    public virtual bool HitByShot(int damage) { Hurt(damage); return true; }
}

/// <summary>Walks along the ground and turns around at walls (crabs and snails).</summary>
public class WalkerMob : Mob
{
    float _dir = -1;

    public WalkerMob(float x, float y, FlxTilemap map, Player player, string image) : base(x, y, map, player)
    {
        LoadGraphic(image, animated: true, frameWidth: 16, frameHeight: 16);
        Animation.Add("walk", new[] { 0, 1 }, 10);
        Animation.Play("walk");
        Acceleration.Y = 50;
        Drag.X = 40;
        Health = 1;
        CollidesWithMap = true;
    }

    protected override void Think(float elapsed)
    {
        // turn around when the next tile ahead is solid
        float probeX = _dir < 0 ? X - 1 : X + Width + 1;
        if (Map.GetTileAt(probeX, Y + Height * 0.5f) > 0 || IsTouching(_dir < 0 ? FlxDirection.Left : FlxDirection.Right)) _dir = -_dir;
        Velocity.X = _dir * 30;
        FaceMoving(Velocity.X);
    }
}

public sealed class Crab : WalkerMob { public Crab(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, "images/crab.png") { } }
public sealed class SeaSnail : WalkerMob { public SeaSnail(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, "images/seasnail.png") { } }

/// <summary>Sits still and hurts whatever touches it.</summary>
public sealed class PainKelp : Mob
{
    public PainKelp(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p) { LoadGraphic("images/painkelp.png"); Health = 9999; }
    protected override void Think(float elapsed) { }
    public override bool HitByShot(int damage) => true;
}

/// <summary>Bobs up and down on the spot.</summary>
public class FloaterMob : Mob
{
    readonly float _startY, _range;

    public FloaterMob(float x, float y, FlxTilemap m, Player p, float range) : base(x, y, m, p)
    {
        _startY = y;
        _range = range;
        Acceleration.Y = 10;
        MaxVelocity.Y = 5;
        Drag.Y = 40;
        Health = 4;
    }

    protected override void Think(float elapsed)
    {
        if (Y >= _startY + _range) Acceleration.Y = -20;
        if (Y <= _startY - _range) Acceleration.Y = 20;
    }
}

public sealed class ManOWar : FloaterMob
{
    public ManOWar(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, 10) { LoadGraphic("images/manowar.png"); }
}

public sealed class Puffer : FloaterMob
{
    public Puffer(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, 4)
    {
        LoadGraphic("images/puffer.png", animated: true, frameWidth: 32, frameHeight: 32);
        Animation.Add("float", new[] { 0, 1, 2, 3 }, 4);
        Animation.Play("float");
        SetArtFacesRight(true);
        ContactDamage = 10;
        CollidesWithMap = true;
    }

    protected override void Touch(float elapsed)
    {
        FlxG.Sound.Play("sfx/puffer.wav", 0.4f);
        base.Touch(elapsed);
    }
}

/// <summary>Swims straight at the diver once he is close (barracudas and sea snakes).</summary>
public class SeekerMob : Mob
{
    readonly float _range = 80, _speed = 30;

    public SeekerMob(float x, float y, FlxTilemap m, Player p, int damage) : base(x, y, m, p)
    {
        Drag = new Vector2(25, 25);
        MaxVelocity = new Vector2(75, 75);
        Health = 2;
        ContactDamage = damage;
        CollidesWithMap = true;
    }

    protected override void Think(float elapsed)
    {
        if (DistanceToDiver >= _range) return;
        var dir = new Vector2(Diver.X - X, Diver.Y - Y);
        if (dir == Vector2.Zero) return;
        dir.Normalize();
        Velocity = dir * _speed;
        FaceMoving(dir.X);
    }
}

public sealed class Barracuda : SeekerMob
{
    public Barracuda(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, 5) { LoadGraphic("images/barracuda.png"); }
}

public sealed class SeaSnake : SeekerMob
{
    public SeaSnake(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p, 5)
    {
        LoadGraphic("images/sea_snake.png", animated: true, frameWidth: 32, frameHeight: 16);
        Animation.Add("walk", new[] { 0, 1 }, 5);
        Animation.Play("walk");
    }
}

/// <summary>Swims back and forth along a stretch of water; its shock stuns the diver for a moment.</summary>
public sealed class ElectricEel : Mob
{
    readonly float _homeX;
    int _direction;

    public ElectricEel(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p)
    {
        _homeX = x;
        LoadGraphic("images/electric_eel.png", animated: true, frameWidth: 32, frameHeight: 16);
        Animation.Add("walk", new[] { 0, 1 }, 5);
        Animation.Play("walk");
        SetArtFacesRight(true);
        Drag = new Vector2(25, 25);
        MaxVelocity = new Vector2(48, 48);
        Health = 2;
        ContactDamage = 5;
        CollidesWithMap = true;
    }

    protected override void Think(float elapsed)
    {
        if (MathF.Abs(X - _homeX) >= 5 * Player.TileSize || Velocity.X == 0) _direction = 1 - _direction;
        Velocity.X = _direction == 0 ? 45 : -45;
        FaceMoving(Velocity.X);
    }

    protected override void OnHurtDiver()
    {
        if (Diver.StatusTimer <= 0) Diver.StatusTimer = 1.5f;
        Diver.Velocity.X /= 2;
    }
}

/// <summary>Waits on a wall, floor or roof; when the diver is near it flashes across the water at him and sticks there.</summary>
public sealed class SpikeCrustacean : Mob
{
    enum State { Idle, Extend, Prep }

    State _state = State.Idle;
    bool _inverted;
    float _preAttack, _setSpeed, _cooldown;

    public FlxEmitter Bubbles { get; }

    public SpikeCrustacean(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p)
    {
        LoadGraphic("images/spike_crust.png", animated: true, frameWidth: 8, frameHeight: 16);
        Animation.Add("idle", new[] { 0 });
        Animation.Add("prep", new[] { 1 });
        Animation.Add("fly", new[] { 2 });
        Health = 2;
        ContactDamage = 2;
        CollidesWithMap = true;

        Bubbles = new FlxEmitter(x, y);
        Bubbles.Width = 4;
        Bubbles.LoadParticles("images/bubble_particle.png", 12, multiple: true);
        Bubbles.Lifespan.Set(0.4f, 0.8f);
    }

    protected override void Think(float elapsed)
    {
        Bubbles.X = X + 3;
        if (_inverted) { Angle = 180; Bubbles.Y = Y; Bubbles.Velocity.Set(-5, -70, 5, -25); }
        else { Angle = 0; Bubbles.Y = Y + 12; Bubbles.Velocity.Set(-5, 25, 5, 50); }

        if (_cooldown > 0) { _cooldown -= elapsed; Animation.Play("idle"); return; }

        switch (_state)
        {
            case State.Idle:
                Animation.Play("idle");
                if (MathF.Abs(Diver.X - X) <= 20) { _state = State.Prep; _preAttack = 0.5f; }
                break;
            case State.Prep:
                Animation.Play("prep");
                _preAttack -= elapsed;
                if (_preAttack < 0)
                {
                    _setSpeed = Diver.Y > Y ? 175 : -175;
                    _state = State.Extend;
                }
                break;
            case State.Extend:
                Animation.Play("fly");
                Velocity.Y = _setSpeed;
                if (!Bubbles.Emitting) Bubbles.Start(false, 0.05f);
                if (IsTouching(FlxDirection.Down)) Land(false);
                else if (IsTouching(FlxDirection.Up)) Land(true);
                break;
        }
    }

    void Land(bool inverted)
    {
        _inverted = inverted;
        _state = State.Idle;
        Bubbles.Emitting = false;
        _cooldown = 1.5f;
    }

    public override void Kill()
    {
        Bubbles.Emitting = false;
        base.Kill();
    }
}

/// <summary>A claw on a long tentacle: shoots up out of the floor when the diver is above it, and carries him down when it catches him.</summary>
public sealed class FloorClaw : Mob
{
    enum State { Idle, Extend, Retract }

    public readonly FlxSprite Body, Tentacle;
    readonly float _startY;
    State _state = State.Idle;
    bool _latched;
    float _timer;
    const float AttackSpeed = 250;

    public FloorClaw(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p)
    {
        _startY = y;
        LoadGraphic("images/claw_claw.png", animated: true, frameWidth: 10, frameHeight: 10);
        Health = 2;
        ContactDamage = 2;
        Body = new FlxSprite(x - 11, y).LoadGraphic("images/claw_body.png");
        Tentacle = new FlxSprite(x + 2, y + 8).LoadGraphic("images/claw_tentacle.png");
        Body.Moves = Tentacle.Moves = false;
    }

    /// <summary>A shot makes it let go and pull back.</summary>
    public override bool HitByShot(int damage)
    {
        _state = State.Retract;
        ResetClaw();
        return true;
    }

    void ResetClaw()
    {
        _latched = false;
        _timer = 1.5f;
        Diver.ControlsEnabled = true;
        FlxFlicker.Flicker(this, 1.5f);
    }

    protected override void Think(float elapsed)
    {
        Tentacle.Y = Y + 8;
        if (_timer > 0) _timer -= elapsed;

        switch (_state)
        {
            case State.Idle:
                if (_latched) ResetClaw();
                Y = _startY;
                if (MathF.Abs(Diver.X - X) <= 15 && _timer <= 0) _state = State.Extend;
                break;
            case State.Extend:
                if (_latched) ResetClaw();
                if (Y < Diver.Y || Y < _startY - 160) _state = State.Retract;
                else Velocity.Y = -AttackSpeed;
                break;
            case State.Retract:
                if (Y < _startY)
                {
                    if (_latched)
                    {
                        Diver.ControlsEnabled = false;
                        Diver.Y = Y - 16;
                        Diver.Velocity.X = 0;
                        Velocity.Y = AttackSpeed * 1.5f;
                    }
                    else Velocity.Y = AttackSpeed / 5;
                }
                else
                {
                    Velocity.Y = 0;
                    if (_latched) { _latched = false; Diver.ControlsEnabled = true; }
                    _state = State.Idle;
                }
                break;
        }
    }

    protected override void Touch(float elapsed)
    {
        if (_state == State.Extend) { _state = State.Retract; _latched = true; }
        base.Touch(elapsed);
    }

    public override void Kill()
    {
        if (_latched) Diver.ControlsEnabled = true;
        Body.Kill();
        Tentacle.Kill();
        base.Kill();
    }
}

/// <summary>Invisible until the diver comes near; then it fades in, aims, and darts at where he was.</summary>
public sealed class Phantom : Mob
{
    enum State { Waiting, Reveal, Aim, Seeking }

    State _state = State.Waiting;
    float _timer = 0.5f;
    Vector2 _dir;
    const float Range = 100, Speed = 175;

    public Phantom(float x, float y, FlxTilemap m, Player p) : base(x, y, m, p)
    {
        LoadGraphic("images/phantom.png", animated: true, frameWidth: 16, frameHeight: 16);
        Animation.Add("wait", new[] { 3 });
        Animation.Add("reveal", new[] { 3, 2, 1, 0 }, 25, looped: false);
        Animation.Play("wait");
        SetArtFacesRight(true);
        MaxVelocity = new Vector2(Speed, Speed);
        Alpha = 0;
        Health = 1;
        ContactDamage = 5;
    }

    protected override bool CanHurt => _state != State.Waiting;

    protected override void Think(float elapsed)
    {
        FaceMoving(Velocity.X);

        if (_state == State.Waiting && MathF.Abs(X - Diver.X) <= Range && MathF.Abs(Y - Diver.Y) <= Range) _state = State.Reveal;

        if (_state == State.Reveal)
        {
            if (Alpha < 0.85f) Alpha += elapsed;
            else
            {
                Alpha = 1;
                Animation.Play("reveal");
                if (Animation.Finished) _state = State.Aim;
            }
        }

        if (_state == State.Aim)
        {
            _timer -= elapsed;
            if (_timer <= 0)
            {
                _dir = new Vector2(Diver.X - X, Diver.Y - Y);
                if (_dir != Vector2.Zero) _dir.Normalize();
                _state = State.Seeking;
            }
        }

        if (_state == State.Seeking) Acceleration = _dir * Speed;
        if (!IsOnScreen()) Kill();
    }
}

/// <summary>The nautilus boss: sleeps until the diver comes near, wakes with a quake and sends a swarm of snails.</summary>
public sealed class BossNautilus : Mob
{
    readonly EnemyManager _manager;
    bool _awake;
    const float Range = 190;

    public BossNautilus(float x, float y, FlxTilemap m, Player p, EnemyManager manager) : base(x, y, m, p)
    {
        _manager = manager;
        LoadGraphic("images/nautilus.png", animated: true, frameWidth: 130, frameHeight: 96);
        Animation.Add("idle", new[] { 0 }, 5);
        Animation.Add("wake", new[] { 0, 1, 2, 3 }, 5, looped: false);
        Animation.Play("idle");
        Drag = new Vector2(25, 25);
        MaxVelocity = new Vector2(40, 40);
        Health = 30;
        Scale = new Vector2(2, 2);
        ContactDamage = 5;
    }

    protected override void Think(float elapsed)
    {
        if (_awake || MathF.Abs(X - Diver.X) > Range || MathF.Abs(Y - Diver.Y) > Range) return;
        _awake = true;                       // let the boss fight begin
        FlxG.Cameras.Shake(0.035f, 4f);
        Animation.Play("wake");
        for (int i = 0; i < 20; i++)
        {
            var snail = new SeaSnail(X + 30, Y - 100, Map, Diver);
            snail.Acceleration.X += 30;
            _manager.AddMob(snail);
        }
    }
}
