namespace navdi3.maze {

    using UnityEngine;
    using UnityEngine.Tilemaps;
    using System.Collections;
    using System.Collections.Generic;

    public interface ITileGettable
    {
        int Gett(twin cell);
    }

    public interface ITileTTs : ITileGettable
    {
        void ClearAllTiles();
        void Sett(twin cell, int TileId);
    }

    public class TilemapTTWrapper : ITileTTs
    {
        Tile[] tileset;
        Tilemap tilemap;
        Dictionary<twin, int> tts;
        public TilemapTTWrapper(Tile[] tileset, Tilemap tilemap)
        {
            this.tileset = tileset;
            this.tilemap = tilemap;
            this.tts = new Dictionary<twin, int>();
        }
        public void ClearAllTiles()
        {
            tilemap.ClearAllTiles();
        }
        public int Gett(twin cell)
        {
            if (tts.TryGetValue(cell, out var TileId)) return TileId;
            else return default(int);
        }
        public void Sett(twin cell, int TileId)
        {
            tts[cell] = TileId;
            tilemap.SetTile(cell, tileset[TileId]);
        }

    }
}