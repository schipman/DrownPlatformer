using System;
using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>The game: the diver in a room; walking into an exit loads the next room.</summary>
public class PlayState : FlxState
{
    public const int Width = 320, Height = 240, Zoom = 3;

    static readonly string[] Bgm = { "music/Coral_zone.wav", "music/kelp_forest_fix.wav" };

    readonly int _startRoom;
    DrownInput _input = null!;
    Player _player = null!;
    Room _room = null!;
    EnemyManager? _enemies;
    FlxSprite _backdrop = null!;
    ProgressBar _health = null!, _oxygen = null!;
    FlxGroup _roomLayer = null!, _hud = null!;
    bool _switching;
    FlxSound? _ambience;

    public PlayState(int startRoom = 1) { _startRoom = startRoom; }

    public override void Create()
    {
        FlxG.Cameras.BgColor = Color.Black;
        FlxG.Mouse.Visible = false;
        _input = new DrownInput();

        _backdrop = new FlxSprite(0, 0) { ScrollFactor = Vector2.Zero };
        Add(_backdrop);

        _roomLayer = new FlxGroup();
        Add(_roomLayer);

        _player = new Player(_input, 0, 0);
        _hud = new FlxGroup();
        _health = new ProgressBar(20, 5, Color.Red);
        _oxygen = new ProgressBar(20, 15, Color.LightBlue);
        _hud.Add(_health);
        _hud.Add(_oxygen);
        _hud.Add(new WeaponSlot(95, 6, _player, _player.Harpoon));
        _hud.Add(new WeaponSlot(115, 6, _player, _player.Explosive));
        for (int i = 2; i < 7; i++) _hud.Add(new WeaponSlot(95 + i * 20, 6, _player, null));

        // the water wobble over the picture
        FlxG.PostProcess.Set(FlxPostProcess.Load("Wiggle", ("Amplitude", 0.0025f), ("Frequency", 18f), ("Speed", 2.5f)));
        _ambience = FlxG.Sound.Play("sfx/layered_dive_ambience.wav", 0.5f, looped: true, autoDestroy: false);

        LoadRoom(_startRoom, null, playMusic: true);
        FlxG.Camera.Flash(Color.Black, 1.5f);
    }

    /// <summary>Builds a room. <paramref name="enteredBy"/> is the side of the new room the diver came in by (null: the first room).</summary>
    void LoadRoom(int index, int? enteredBy, bool playMusic)
    {
        var def = RoomData.Get(index) ?? throw new InvalidOperationException($"no room {index}");
        _room?.Destroy();
        _roomLayer.Clear();

        _room = new Room(def);
        _backdrop.LoadGraphic(def.Backdrop == "kelpBackdrop" ? "images/backdrop_test.png" : "images/backdrop.png");

        _roomLayer.Add(_room.Back);
        _enemies = def.HasEnemies ? new EnemyManager(_room.Collide, _player, def.Map) : null;
        if (_enemies != null) _roomLayer.Add(_enemies);
        _roomLayer.Add(_player);
        _roomLayer.Add(_player.Bubbles);
        _roomLayer.Add(_player.Harpoon);
        _roomLayer.Add(_player.Explosive);
        _roomLayer.Add(_room.Front);
        _roomLayer.Add(_hud);

        var at = _room.EntryFor(enteredBy ?? Room.Left);
        _player.Reset(at.X, at.Y);
        _player.Harpoon.ResetShots();
        _player.Explosive.ResetShots();
        if (enteredBy == Room.Down) _player.Velocity.Y = -100;      // a little push going through

        FlxG.Camera.Bounds = new Rectangle(0, 0, (int)_room.Collide.Width, (int)_room.Collide.Height);
        FlxG.Camera.Follow(_player, 0.1f);

        if (playMusic) PlayBgm(def.Bgm);
    }

    static void PlayBgm(string name) =>
        FlxG.Sound.PlayMusic(name.StartsWith("kelp") ? Bgm[1] : Bgm[0], 0.6f);

    public override void Update(float elapsed)
    {
        _health.Value = _player.Health / _player.MaxHealth * 100f;
        _oxygen.Value = _player.Oxygen / _player.MaxOxygen * 100f;

        base.Update(elapsed);

        if (!_switching)
        {
            FlxG.Collide(_room.Collide, _player);
            CollideShots(_player.Harpoon);
            CollideShots(_player.Explosive);
            CheckExits();

            // keep the diver inside the room (but he may fall out of the bottom)
            float w = _room.Collide.Width;
            if (_player.X < 0) _player.X = 0;
            else if (_player.X + _player.Width > w) _player.X = w - _player.Width;
            if (_player.Y > _room.Collide.Height || _player.IsDead) Die();
        }
    }

    void CollideShots(Weapon weapon)
    {
        foreach (var p in weapon.Members)
        {
            if (!p.Exists || p.Dying) continue;
            if (FlxG.Collide(_room.Collide, p)) { p.Explode(); continue; }
            if (_enemies == null) continue;
            foreach (var m in _enemies.Mobs)
                if (m.Exists && m.Alive && FlxObject.OverlapsPair(p, m))
                {
                    if (m.HitByShot(p.Damage)) p.Explode();
                    break;
                }
        }
    }

    void CheckExits()
    {
        for (int side = 0; side < 4; side++)
        {
            var exit = _room.Exits[side];
            if (exit == null || _room.Neighbour(side) <= 0 || !FlxObject.OverlapsPair(_player, exit)) continue;

            // only when pushing into it (the diver may arrive standing beside the exit of the room he came from)
            bool pushing = side switch
            {
                Room.Left => _player.Acceleration.X < 0,
                Room.Right => _player.Acceleration.X > 0,
                Room.Up => _player.Velocity.Y < 0,
                _ => _player.Velocity.Y > 0,
            };
            if (pushing) { Transition(side); return; }
        }
    }

    void Transition(int side)
    {
        _switching = true;
        _player.ControlsEnabled = false;
        _player.Moves = false;                    // hold still (no sinking through the rock) while the screen fades
        _player.Velocity = Vector2.Zero;
        _player.Acceleration = Vector2.Zero;
        _player.Harpoon.ResetShots();
        _player.Explosive.ResetShots();

        bool zone = side == _room.Def.ZoneFlag;
        int next = _room.Neighbour(side);
        float fade = zone ? 2f : 0.5f;
        if (zone) FlxG.Sound.Music?.Stop();

        FlxG.Camera.Fade(Color.Black, fade, onComplete: () =>
        {
            // the diver comes in on the opposite side
            int enteredBy = side switch { Room.Left => Room.Right, Room.Right => Room.Left, Room.Up => Room.Down, _ => Room.Up };
            LoadRoom(next, enteredBy, playMusic: zone);
            FlxG.Camera.Fade(Color.Black, zone ? 1.5f : 0.5f, fadeIn: true);   // the fade-out is still holding the screen black
            _player.Moves = true;
            _player.ControlsEnabled = true;
            _switching = false;
        });
    }

    void Die()
    {
        _switching = true;
        _player.ControlsEnabled = false;
        _player.Moves = false;
        FlxG.Camera.Fade(Color.Black, 1f, onComplete: () => FlxG.SwitchState(new PlayState(1), Width, Height, Zoom));
    }

    public override void Destroy()
    {
        _ambience?.Stop();
        FlxG.PostProcess.Clear();
        base.Destroy();
    }
}
