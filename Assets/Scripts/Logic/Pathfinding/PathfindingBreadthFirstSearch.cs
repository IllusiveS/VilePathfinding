using System.Collections.Generic;
using UnityEngine;

namespace Logic.Pathfinding
{
    using TileCoordinate = Vector2Int;
    public class PathfindingBreadthFirstSearch : IPathfindingCommand
    {
        private Queue<TileCoordinate> pathfindingQueue = new Queue<Vector2Int>();
        public PathfindingResult CalculatePath(Map map, TileCoordinate @from, TileCoordinate to)
        {
            var result = new PathfindingResult();
            result.Path = new List<Vector2Int>();
            result.UnvisitedSet = new HashSet<TileCoordinate>();
            result.VisitedSet = new HashSet<TileCoordinate>();
            result.Aggregator = new Dictionary<Vector2Int, Vector2Int>();
            result.Visualizations = new Queue<VisualizationCommand>();
            result.targetTile = to;

            pathfindingQueue.Enqueue(from);
            result.Aggregator.Add(from, new TileCoordinate(-1, -1));
            do
            {
                var currentCoord = pathfindingQueue.Dequeue();
                result.Visualizations.Enqueue(new VisualizationCommand(currentCoord));
                if (currentCoord == to)
                {
                    result.IsResultFound = true;
                    break;
                }
                var neighbours = map.GetNeighbouringTiles(currentCoord);
                foreach (var potentialNeighbour in neighbours)
                {
                    if (!potentialNeighbour.HasValue) continue;
                    if (result.Aggregator.ContainsKey(potentialNeighbour.Value)) continue;
                    result.Aggregator.Add(potentialNeighbour.Value, currentCoord);
                    pathfindingQueue.Enqueue(potentialNeighbour.Value);
                }
            } while (pathfindingQueue.Count > 0);
            
            result.ExtractPathFromAggregator();
            return result;
        }
    }
}
