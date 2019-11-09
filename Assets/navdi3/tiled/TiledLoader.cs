namespace navdi3.tiled
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    using System.Xml;
    using System.Xml.Serialization;

    public class TiledLoader : MonoBehaviour
    {
        //public TextAsset ass;
        public Tile[] tileset;
        [HideInInspector] public HashSet<int> spawnTileSet;
        private void Start()
        {
            //var tld = Load(ass);
            //Dj.Tempf("tld debug: {0}x{1}, {2}", tld.width, tld.height, tld.tile_ids[0]);
        }

        public void SetupTileset(SpriteLot tilesetSprites, int[] solidTileIds, int[] spawnTileIds)
        {
            this.SetupTileset(tilesetSprites, new HashSet<int>(solidTileIds), new HashSet<int>(spawnTileIds));
        }

        public void SetupTileset(SpriteLot tilesetSprites, HashSet<int> solidTileSet, HashSet<int> spawnTileSet)
        {
            if (tilesetSprites.Length == 0)
            {
                Dj.Crashf("TiledLoader.SetupTileset was passed empty SpriteLot {0}.", tilesetSprites.gameObject.name);
            }

            //HashSet<int> solidTileSet = new HashSet<int>(solidTileIds);
            this.spawnTileSet = spawnTileSet;

            tileset = new Tile[tilesetSprites.Length];
            for (int i = 0; i < tileset.Length; i++)
            {
                tileset[i] = ScriptableObject.CreateInstance<Tile>();
                tileset[i].sprite = tilesetSprites[i];
                tileset[i].colliderType = solidTileSet.Contains(i) ? Tile.ColliderType.Grid : Tile.ColliderType.None;
            }
        }

        public void PlaceTiles(TiledLevelData levelData, maze.ITileTTs tilemap, System.Action<int, twin> spawnFunction)
        {
            tilemap.ClearAllTiles();

            twin tile_pos = new twin(0, levelData.height - 1);
            foreach (var tile_id in levelData.tile_ids)
            {
                if (spawnTileSet.Contains(tile_id))
                {
                    spawnFunction(tile_id, tile_pos);
                    tilemap.Sett(tile_pos, 0);
                } else if (tile_id >= 0)
                {
                    tilemap.Sett(tile_pos, tile_id);
                }

                tile_pos.x++;
                if (tile_pos.x >= levelData.width)
                {
                    tile_pos.x = 0;
                    tile_pos.y--;
                }
            }
        }

        public TiledLevelData Load(TextAsset xmlTiledExportAsset) {
            XmlDocument xmlDoc;
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTiledExportAsset.text);
            var mapDataNode = xmlDoc.SelectSingleNode("/map");
            var levelDataNode = xmlDoc.SelectSingleNode("/map/layer/data");
            var tile_id_strings = levelDataNode.InnerText.Split(',');
            int[] tile_ids = new int[tile_id_strings.Length];
            for(int i = 0; i < tile_ids.Length; i++)
            {
                tile_ids[i] = int.Parse(tile_id_strings[i])-1;
            }

            return new TiledLevelData
            {
                width = int.Parse(mapDataNode.Attributes.GetNamedItem("width").InnerText),
                height = int.Parse(mapDataNode.Attributes.GetNamedItem("height").InnerText),
                tile_ids = tile_ids,
            };
        }
    }

    public struct TiledLevelData
    {
        public int width;
        public int height;
        public int[] tile_ids;
    }

}
