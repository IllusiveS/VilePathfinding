using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    using TileCoordinate = Vector2Int;
    
    public enum PathfindingMethod
    {
        DepthFirst,
        BreadthFirst
    }

    public struct PathAggregator
    {
        public PathAggregator(Vector2Int coord, Vector2Int prevCoord)
        {
            Coord = coord;
            PrevCoord = prevCoord;
        }

        public bool Equals(PathAggregator other)
        {
            return Coord.Equals(other.Coord);
        }

        public override bool Equals(object obj)
        {
            return obj is PathAggregator other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Coord.GetHashCode();
        }

        public TileCoordinate Coord;
        public TileCoordinate PrevCoord;
        
    }
    
    public struct VisualizationCommand
    {
        public TileCoordinate Coordinate;

        public VisualizationCommand(Vector2Int coordinate)
        {
            Coordinate = coordinate;
        }
    }
    
    public struct PathfindingResult
    {
        public bool IsResultFound;
        public TileCoordinate targetTile;
        public List<TileCoordinate> Path;
        public HashSet<TileCoordinate> UnvisitedSet;
        public HashSet<TileCoordinate> VisitedSet;
        public Dictionary<TileCoordinate, TileCoordinate> Aggregator;
        public Queue<VisualizationCommand> Visualizations;

        public void ExtractPathFromAggregator()
        {
            TileCoordinate currentTile = targetTile;
            TileCoordinate finalTile = new Vector2Int(-1, -1);
            
            do
            {
                Path.Add(currentTile);
                currentTile = Aggregator[currentTile];
            } while (currentTile != finalTile);
            Path.Reverse();
        }
    }
    
    public interface IPathfindingCommand
    {
        PathfindingResult CalculatePath(Map map, TileCoordinate from, TileCoordinate to);
    }
}
