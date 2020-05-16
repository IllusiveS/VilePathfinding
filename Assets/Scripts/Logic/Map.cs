using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Logic
{
    using TileCoordinate = Vector2Int;

    [Serializable]
    public struct TileSerializationHelper
    {
        public List<bool> tiles;
    }
    
    [CreateAssetMenu(menuName = "VilePathfinding/Map")]
    public class Map : ScriptableObject
    {
        public int mapWidth;
        public int mapHeight;

        [Button]
        void generateMap()
        {
            SetMapSize(mapWidth, mapHeight);
        }

        public List<TileSerializationHelper> tilesList = new List<TileSerializationHelper>();

        public void SetMapSize(int width, int height)
        {
            tilesList.Clear();
            for (var x = 0; x < width; x++)
            {
                var helper = new TileSerializationHelper();
                helper.tiles = new List<bool>();
                tilesList.Add(helper);
                for (var y = 0; y < height; y++)
                {
                    var row = tilesList[x];
                    var rowList = row.tiles;
                    rowList.Add(true);
                }
            }
        }


        public TileCoordinate?[] GetNeighbouringTiles(TileCoordinate coord)
        {
            TileCoordinate[] potentialNeighbours =
            {
                new TileCoordinate(-1, 0),
                new TileCoordinate(0, -1),
                new TileCoordinate(1, 0),
                new TileCoordinate(0, 1)
            };
            var result = new TileCoordinate?[4];

            int count = 0;
            foreach (var potentialNeighbour in potentialNeighbours)
            {
                var neighbourCoordinate = coord + potentialNeighbour;
                if (neighbourCoordinate.x < 0 || neighbourCoordinate.x >= mapWidth)
                {
                    result[count] = null;
                } 
                else if (neighbourCoordinate.y < 0 || neighbourCoordinate.y >= mapHeight)
                {
                    result[count] = null;
                } 
                else
                {
                    if (tilesList[neighbourCoordinate.x].tiles[neighbourCoordinate.y] == false)
                    {
                        result[count] = null;
                    }
                    else
                    {
                        result[count] = neighbourCoordinate;
                    }
                }
                count++;
            }
            
            return result;
        }
    }
}
