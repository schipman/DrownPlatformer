using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>Makes the creatures of a room from its Enemies map (a number in a cell is a kind of creature), and keeps them going.</summary>
public sealed class EnemyManager : FlxGroup
{
    readonly FlxTilemap _level;
    readonly Player _player;
    readonly List<Mob> _mobs = new();
    readonly FlxGroup _extras = new();     // claw parts, bubble trails
    readonly FlxGroup _creatures = new();

    public IReadOnlyList<Mob> Mobs => _mobs;

    public EnemyManager(FlxTilemap level, Player player, string mapName)
    {
        _level = level;
        _player = player;
        Add(_extras);
        Add(_creatures);

        var rows = File.ReadAllLines(FlxAssets.Get($"maps/mapCSV_{mapName}_Enemies.csv"));
        for (int ty = 0; ty < rows.Length; ty++)
        {
            var cells = rows[ty].Split(',');
            for (int tx = 0; tx < cells.Length; tx++)
                if (int.TryParse(cells[tx], out int id) && id > 0) Spawn(id, tx * Player.TileSize, ty * Player.TileSize);
        }
    }

    public void AddMob(Mob mob)
    {
        _mobs.Add(mob);
        _creatures.Add(mob);
    }

    void Spawn(int id, int x, int y)
    {
        Mob? mob = id switch
        {
            1 => new SeaSnail(x, y, _level, _player),
            2 => new SeaSnake(x, y, _level, _player),
            3 => new ElectricEel(x, y, _level, _player),
            4 => new Barracuda(x, y, _level, _player),
            5 => new PainKelp(x, y, _level, _player),
            6 => new ManOWar(x, y, _level, _player),
            10 => new Puffer(x, y, _level, _player),
            11 => new Crab(x, y, _level, _player),
            12 => new FloorClaw(x, y, _level, _player),
            13 => new SpikeCrustacean(x, y, _level, _player),
            14 => new Phantom(x, y, _level, _player),
            15 => new BossNautilus(x, y, _level, _player, this),
            _ => null,              // 7-9 (anemone, bad seahorse, angler) were never finished
        };
        if (mob == null) return;

        if (mob is FloorClaw claw) { _extras.Add(claw.Tentacle); _extras.Add(claw.Body); }
        if (mob is SpikeCrustacean crust) _extras.Add(crust.Bubbles);
        AddMob(mob);
    }

    public override void Update(float elapsed)
    {
        foreach (var m in _mobs.ToArray())
            if (m.Exists && m.CollidesWithMap) FlxG.Collide(_level, m);
        base.Update(elapsed);
    }
}
