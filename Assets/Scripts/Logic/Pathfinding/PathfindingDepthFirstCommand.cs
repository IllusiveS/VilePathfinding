using System.Collections.Generic;
using UnityEngine;

namespace Logic.Pathfinding
{
    using TileCoordinate = Vector2Int;
    public class PathfindingDepthFirstCommand : IPathfindingCommand
    {
        public PathfindingResult CalculatePath(Map map, TileCoordinate @from, TileCoordinate to)
        {
            return CalculatePathDepthFirst(map, from, to);
        }
    
        private PathfindingResult CalculatePathDepthFirst(Map map, TileCoordinate from, TileCoordinate to)
        {
            var result = new PathfindingResult();
            result.Path = new List<Vector2Int>();
            result.UnvisitedSet = new HashSet<TileCoordinate>();
            result.VisitedSet = new HashSet<TileCoordinate>();
            result.Aggregator = new Dictionary<Vector2Int, Vector2Int>();
            result.Visualizations = new Queue<VisualizationCommand>();
            result.targetTile = to;

            ProcessNodeDepthFirst(ref result, map, to, from, new TileCoordinate(-1,-1));
            
            result.ExtractPathFromAggregator();
            return result;
        }

        private void ProcessNodeDepthFirst(ref PathfindingResult results, Map map,  TileCoordinate targetCoords, TileCoordinate nodeCoords, TileCoordinate prevNodeCoords)
        {
            if (results.IsResultFound)
            {
                return;
            }
            var aggregator = new PathAggregator(nodeCoords, prevNodeCoords);
            if (results.Aggregator.ContainsKey(nodeCoords)) return;
            
            results.Visualizations.Enqueue(new VisualizationCommand(nodeCoords));
            results.Aggregator.Add(nodeCoords, prevNodeCoords);
            if (nodeCoords == targetCoords)
            {
                results.IsResultFound = true;
                return;
            }
            var neighbours = map.GetNeighbouringTiles(nodeCoords);
            foreach (var potentialNeighbour in neighbours)
            {
                if (!potentialNeighbour.HasValue) continue;
                ProcessNodeDepthFirst(ref results, map, targetCoords, potentialNeighbour.Value, nodeCoords);
            }
        }
    }
}
