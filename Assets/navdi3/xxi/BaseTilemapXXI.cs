namespace navdi3.xxi
{
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using System.Collections;
    using System.Collections.Generic;

    using navdi3.tiled;

    [RequireComponent(typeof(BankLot))]
    [RequireComponent(typeof(SpriteLot))]
    [RequireComponent(typeof(TiledLoader))]

    abstract public class BaseTilemapXXI : BaseSimpleXXI, maze.ITileTTs
    {
        public TiledLoader loader { get { return GetComponent<TiledLoader>(); } }

        public Tilemap tilemap; // must be assigned
        public TextAsset firstLevel; // load first level

        abstract public int[] GetSolidTileIds();
        abstract public int[] GetSpawnTileIds();
        abstract public void SpawnTileId(int TileId, twin TilePos);

        private HashSet<int> _SolidTileIds;
        public HashSet<int> SolidTileIds { get { if (_SolidTileIds == null) _SolidTileIds = new HashSet<int>(GetSolidTileIds()); return _SolidTileIds; } }
        private HashSet<int> _SpawnTileIds;
        public HashSet<int> SpawnTileIds { get { if (_SpawnTileIds == null) _SpawnTileIds = new HashSet<int>(GetSpawnTileIds()); return _SpawnTileIds; } }

        // if loading from loader

        protected void InitializeTileSystem()
        {
            InitializeManualTT();

            loader.PlaceTiles(loader.Load(firstLevel), this, this.SpawnTileId);
        }

        public void InitializeManualTT()
        {
            var grid = tilemap.gameObject.GetComponentInParent<Grid>();
            grid.gameObject.AddComponent<maze.MazeMaster>().IsTileSolid = cell => { return SolidTileIds.Contains(Gett(cell)); };

            if (sprites.Length == 0)
            {
                Dj.Crashf("BaseTilemapXXI '{0}' has no sprites in its SpriteLot '{1}'. TiledLoader.SetupTileset will crash.", this.gameObject.name, sprites.gameObject.name);
            }

            loader.SetupTileset(sprites, SolidTileIds, SpawnTileIds);
            tts = new Dictionary<twin, int>();
        }
        public void ClearAllTiles()
        {
            tts.Clear();
            tilemap.ClearAllTiles();
        }

        Dictionary<twin, int> tts;
        public void Sett(twin cell, int TileId)
        {
            tts[cell] = TileId;
            tilemap.SetTile(cell, loader.tileset[TileId]);
        }
        public int Gett(twin cell)
        {
            if (tts.TryGetValue(cell, out var TileId)) return TileId;
            else return default(int);
        }
    }
}