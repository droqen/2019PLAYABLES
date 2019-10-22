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

    abstract public class BaseTilemapXXI : BaseSimpleXXI
    {
        public TiledLoader loader { get { return GetComponent<TiledLoader>(); } }

        public Tilemap tilemap; // must be assigned
        public TextAsset firstLevel; // load first level

        abstract public int[] GetSolidTileIds();
        abstract public int[] GetSpawnTileIds();
        abstract public void SpawnTileId(int TileId, Vector3Int TilePos);

        int[] _SolidTileIds;
        int[] SolidTileIds { get { if (_SolidTileIds == null) _SolidTileIds = GetSolidTileIds(); return _SolidTileIds; } }
        int[] _SpawnTileIds;
        int[] SpawnTileIds { get { if (_SpawnTileIds == null) _SpawnTileIds = GetSpawnTileIds(); return _SpawnTileIds; } }

        // if loading from loader

        protected void InitializeTileSystem()
        {
            loader.SetupTileset(sprites, SolidTileIds, SpawnTileIds);
            loader.PlaceTiles(loader.Load(firstLevel), tilemap, this.SpawnTileId);
        }

        // if using the 'tt' system

        public void InitializeManualTT()
        {
            loader.SetupTileset(sprites, SolidTileIds, SpawnTileIds);
            tts = new Dictionary<twin, int>();
        }
        public void ClearAllTilesTT()
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