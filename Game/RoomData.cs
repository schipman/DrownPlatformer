using Microsoft.Xna.Framework;

namespace DrownPlatformer;

/// <summary>One room of the world (generated from the original RoomManager switch): its maps, exits and neighbours.</summary>
public sealed class RoomDef
{
    public int Index;
    public string Tileset = "", Map = "", Bgm = "", Backdrop = "";
    /// <summary>Which of the four sides has an exit. Order everywhere: Left, Right, Up, Down.</summary>
    public bool ExitLeft, ExitRight, ExitUp, ExitDown;
    public Point ExitLeftAt, ExitRightAt, ExitUpAt, ExitDownAt;
    /// <summary>Where the diver appears when entering through that side.</summary>
    public Point? EntryLeft, EntryRight, EntryTop, EntryBottom;
    /// <summary>The room behind each exit (0: none).</summary>
    public int ToLeft, ToRight, ToUp, ToDown;
    /// <summary>Leaving by this side (0 left, 1 right, 2 up, 3 down) changes zone (new music, longer fade); -1: never.</summary>
    public int ZoneFlag = -1;
    public bool HasEnemies, HasEntities;
}

public static class RoomData
{
    public static RoomDef? Get(int index) => All.TryGetValue(index, out var r) ? r : null;

    public static readonly System.Collections.Generic.Dictionary<int, RoomDef> All = new()
    {
        [1] = new RoomDef
        {
            Index = 1, Tileset = "world1_atlas", Map = "Coral1", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(16, 208), ExitRightAt = new(944, 256), ExitUpAt = new(0, 0), ExitDownAt = new(144, 304),
            EntryLeft = new(32, 208), EntryRight = new(912, 256), EntryTop = null, EntryBottom = new(160, 272),
            ToLeft = 13, ToRight = 2, ToDown = 4, ToUp = 0,
            ZoneFlag = 0, HasEnemies = true, HasEntities = true,
        },
        [2] = new RoomDef
        {
            Index = 2, Tileset = "world1_atlas", Map = "Coral2", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 256), ExitRightAt = new(624, 160), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 256), EntryRight = new(608, 176), EntryTop = null, EntryBottom = null,
            ToLeft = 1, ToRight = 3, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [3] = new RoomDef
        {
            Index = 3, Tileset = "world1_atlas", Map = "Coral3", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 160), ExitRightAt = new(944, 256), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 160), EntryRight = null, EntryTop = null, EntryBottom = null,
            ToLeft = 2, ToRight = -1, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [4] = new RoomDef
        {
            Index = 4, Tileset = "world1_atlas", Map = "Coral4", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = false, ExitRight = false, ExitUp = true, ExitDown = true,
            ExitLeftAt = new(0, 0), ExitRightAt = new(0, 0), ExitUpAt = new(208, 0), ExitDownAt = new(48, 624),
            EntryLeft = null, EntryRight = null, EntryTop = new(208, 16), EntryBottom = new(48, 592),
            ToLeft = 0, ToRight = 0, ToDown = 5, ToUp = 1,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [5] = new RoomDef
        {
            Index = 5, Tileset = "world1_atlas", Map = "Coral5", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = false, ExitRight = true, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 0), ExitRightAt = new(944, 848), ExitUpAt = new(736, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = new(928, 848), EntryTop = new(736, 16), EntryBottom = null,
            ToLeft = 0, ToRight = 6, ToDown = 0, ToUp = 4,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [6] = new RoomDef
        {
            Index = 6, Tileset = "world1_atlas", Map = "Coral6", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 224), ExitRightAt = new(944, 224), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 224), EntryRight = new(928, 224), EntryTop = null, EntryBottom = null,
            ToLeft = 5, ToRight = 7, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [7] = new RoomDef
        {
            Index = 7, Tileset = "world1_atlas", Map = "Coral7", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = false, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(0, 1168), ExitRightAt = new(0, 0), ExitUpAt = new(0, 0), ExitDownAt = new(1184, 1264),
            EntryLeft = new(16, 1168), EntryRight = null, EntryTop = null, EntryBottom = new(1184, 1216),
            ToLeft = 6, ToRight = 0, ToDown = 8, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [8] = new RoomDef
        {
            Index = 8, Tileset = "world1_atlas", Map = "Coral8", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 912), ExitRightAt = new(304, 912), ExitUpAt = new(32, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 912), EntryRight = new(288, 912), EntryTop = new(32, 16), EntryBottom = null,
            ToLeft = 9, ToRight = 10, ToDown = 0, ToUp = 7,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [9] = new RoomDef
        {
            Index = 9, Tileset = "world1_atlas", Map = "Coral9", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = false, ExitRight = true, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(0, 0), ExitRightAt = new(944, 272), ExitUpAt = new(0, 0), ExitDownAt = new(16, 304),
            EntryLeft = null, EntryRight = new(928, 272), EntryTop = null, EntryBottom = new(16, 288),
            ToLeft = 0, ToRight = 8, ToDown = 11, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [10] = new RoomDef
        {
            Index = 10, Tileset = "world1_atlas", Map = "Coral10", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = false, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 208), ExitRightAt = new(0, 0), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 208), EntryRight = null, EntryTop = null, EntryBottom = null,
            ToLeft = 8, ToRight = 0, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [11] = new RoomDef
        {
            Index = 11, Tileset = "world1_atlas", Map = "Coral11", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = true, ExitRight = false, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 240), ExitRightAt = new(0, 0), ExitUpAt = new(592, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 240), EntryRight = null, EntryTop = new(592, 16), EntryBottom = null,
            ToLeft = 12, ToRight = 0, ToDown = 0, ToUp = 9,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [12] = new RoomDef
        {
            Index = 12, Tileset = "world1_atlas", Map = "Coral12", Bgm = "coral_zone_bgm", Backdrop = "coralBackdrop",
            ExitLeft = false, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 0), ExitRightAt = new(0, 0), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = new(608, 368), EntryTop = null, EntryBottom = null,
            ToLeft = 0, ToRight = 11, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [13] = new RoomDef
        {
            Index = 13, Tileset = "kelp_zone_tileset", Map = "Forest1", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 240), ExitRightAt = new(624, 224), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 240), EntryRight = new(608, 224), EntryTop = null, EntryBottom = null,
            ToLeft = 14, ToRight = 1, ToDown = 0, ToUp = 0,
            ZoneFlag = 1, HasEnemies = true, HasEntities = false,
        },
        [14] = new RoomDef
        {
            Index = 14, Tileset = "kelp_zone_tileset", Map = "Forest2", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 624), ExitRightAt = new(944, 96), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 624), EntryRight = new(928, 96), EntryTop = null, EntryBottom = null,
            ToLeft = 15, ToRight = 13, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [15] = new RoomDef
        {
            Index = 15, Tileset = "kelp_zone_tileset", Map = "Forest3", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 208), ExitRightAt = new(624, 128), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 208), EntryRight = new(608, 128), EntryTop = null, EntryBottom = null,
            ToLeft = 16, ToRight = 14, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [16] = new RoomDef
        {
            Index = 16, Tileset = "kelp_zone_tileset", Map = "Forest4", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(0, 336), ExitRightAt = new(304, 64), ExitUpAt = new(0, 0), ExitDownAt = new(144, 944),
            EntryLeft = new(16, 336), EntryRight = new(288, 64), EntryTop = null, EntryBottom = new(144, 912),
            ToLeft = 24, ToRight = 15, ToDown = 17, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [17] = new RoomDef
        {
            Index = 17, Tileset = "kelp_zone_tileset", Map = "Forest5", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 560), ExitRightAt = new(944, 448), ExitUpAt = new(64, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 560), EntryRight = new(928, 448), EntryTop = new(64, 16), EntryBottom = null,
            ToLeft = 22, ToRight = 18, ToDown = 0, ToUp = 16,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [18] = new RoomDef
        {
            Index = 18, Tileset = "kelp_zone_tileset", Map = "Forest6", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 176), ExitRightAt = new(624, 208), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 176), EntryRight = new(608, 208), EntryTop = null, EntryBottom = null,
            ToLeft = 17, ToRight = 19, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [19] = new RoomDef
        {
            Index = 19, Tileset = "kelp_zone_tileset", Map = "Forest7", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(0, 464), ExitRightAt = new(1264, 496), ExitUpAt = new(0, 0), ExitDownAt = new(144, 624),
            EntryLeft = new(16, 464), EntryRight = new(1248, 496), EntryTop = null, EntryBottom = new(144, 592),
            ToLeft = 18, ToRight = 20, ToDown = 25, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [20] = new RoomDef
        {
            Index = 20, Tileset = "kelp_zone_tileset", Map = "Forest8", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = false, ExitUp = false, ExitDown = true,
            ExitLeftAt = new(0, 176), ExitRightAt = new(624, 208), ExitUpAt = new(0, 0), ExitDownAt = new(176, 304),
            EntryLeft = new(16, 176), EntryRight = new(608, 208), EntryTop = null, EntryBottom = new(176, 272),
            ToLeft = 19, ToRight = 0, ToDown = 21, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [21] = new RoomDef
        {
            Index = 21, Tileset = "kelp_zone_tileset", Map = "Forest9", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = false, ExitRight = false, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 176), ExitRightAt = new(624, 208), ExitUpAt = new(96, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = null, EntryTop = new(96, 16), EntryBottom = null,
            ToLeft = 0, ToRight = 0, ToDown = 0, ToUp = 20,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [22] = new RoomDef
        {
            Index = 22, Tileset = "kelp_zone_tileset", Map = "Forest10", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = true, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 192), ExitRightAt = new(304, 192), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = new(16, 192), EntryRight = new(288, 192), EntryTop = null, EntryBottom = null,
            ToLeft = 23, ToRight = 17, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [23] = new RoomDef
        {
            Index = 23, Tileset = "kelp_zone_tileset", Map = "Forest11", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = false, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 0), ExitRightAt = new(944, 832), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = new(928, 832), EntryTop = null, EntryBottom = null,
            ToLeft = 0, ToRight = 22, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [24] = new RoomDef
        {
            Index = 24, Tileset = "kelp_zone_tileset", Map = "Forest12", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = false, ExitRight = true, ExitUp = false, ExitDown = false,
            ExitLeftAt = new(0, 0), ExitRightAt = new(304, 192), ExitUpAt = new(0, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = new(288, 192), EntryTop = null, EntryBottom = null,
            ToLeft = 0, ToRight = 16, ToDown = 0, ToUp = 0,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
        [25] = new RoomDef
        {
            Index = 25, Tileset = "kelp_zone_tileset", Map = "Forest13", Bgm = "kelp_forest_bgm", Backdrop = "kelpBackdrop",
            ExitLeft = false, ExitRight = false, ExitUp = true, ExitDown = false,
            ExitLeftAt = new(0, 0), ExitRightAt = new(0, 0), ExitUpAt = new(256, 0), ExitDownAt = new(0, 0),
            EntryLeft = null, EntryRight = null, EntryTop = new(256, 16), EntryBottom = null,
            ToLeft = 0, ToRight = 0, ToDown = 0, ToUp = 19,
            ZoneFlag = -1, HasEnemies = true, HasEntities = false,
        },
    };
}
