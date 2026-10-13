using Microsoft.Xna.Framework;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// One room: four tilemap layers made from the CSV files (a scrolling backdrop layer, background decoration, the solid layer and the
/// foreground that is drawn over the diver) and an invisible trigger for every exit.
/// </summary>
public sealed class Room
{
    public const int Left = 0, Right = 1, Up = 2, Down = 3;

    public readonly RoomDef Def;
    public readonly FlxTilemap Parallax = new(), Background = new(), Collide = new(), Foreground = new();
    public readonly FlxGroup Back = new(), Front = new();
    public readonly FlxSprite?[] Exits = new FlxSprite?[4];

    public Room(RoomDef def)
    {
        Def = def;
        string tiles = $"images/{def.Tileset}.png";
        FlxTilemap Load(FlxTilemap map, string layer) => map.LoadMapFromCsvFile(FlxAssets.Get($"maps/mapCSV_{def.Map}_{layer}.csv"), tiles, Player.TileSize, Player.TileSize);

        Load(Parallax, "Parallax").ScrollFactor = new Vector2(0.5f, 0.5f);
        Load(Background, "Background");
        Load(Collide, "Collide");
        Load(Foreground, "Foreground");

        Back.Add(Parallax);
        Back.Add(Background);
        Back.Add(Collide);
        Front.Add(Foreground);

        MakeExit(Left, def.ExitLeft, def.ExitLeftAt, 16, 32);
        MakeExit(Right, def.ExitRight, def.ExitRightAt, 16, 32);
        MakeExit(Up, def.ExitUp, def.ExitUpAt, 32, 16);
        MakeExit(Down, def.ExitDown, def.ExitDownAt, 32, 16);
    }

    void MakeExit(int side, bool has, Point at, int w, int h)
    {
        if (!has) return;
        var s = new FlxSprite(at.X, at.Y) { Visible = false, Moves = false };
        s.MakeGraphic(w, h, Color.Transparent);
        Exits[side] = s;
        Back.Add(s);
    }

    public int Neighbour(int side) => side switch { Left => Def.ToLeft, Right => Def.ToRight, Up => Def.ToUp, _ => Def.ToDown };

    /// <summary>Where the diver stands after coming in through <paramref name="side"/> (the side of THIS room he came in by).</summary>
    public Point EntryFor(int side)
    {
        Point? p = side switch { Left => Def.EntryLeft, Right => Def.EntryRight, Up => Def.EntryTop, _ => Def.EntryBottom };
        return p ?? new Point((int)Collide.Width / 2, (int)Collide.Height / 2);
    }

    public void Destroy()
    {
        Back.Destroy();
        Front.Destroy();
    }
}
