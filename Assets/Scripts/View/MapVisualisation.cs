using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using Logic.Pathfinding;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using View;

public class MapVisualisation : MonoBehaviour
{
    [BoxGroup("Visualization Settings")]
    [OnValueChanged("OnMapChangedCallback")]
    public Map visualisedMap;

    [BoxGroup("Visualization Settings")]
    public float sizeBetweenTiles = 1.0f;
    
    [BoxGroup("Visualization Settings")]
    public GameObject passableTile;
    
    [BoxGroup("Visualization Settings")]
    [FormerlySerializedAs("_visualization")] [SerializeField, HideInInspector]
    private List<GameObject> _visualizations;
    
    [SerializeField, HideInInspector]
    private List<GameObject> _currentVisualizations;

    [BoxGroup("Pathfinding Visualization Settings")]
    public GameObject singleStepVisualizationObject;

    public AssetReference stepVisualizationObject;
    [BoxGroup("Pathfinding Visualization Settings")]
    public GameObject startPosGameObject;
    [BoxGroup("Pathfinding Visualization Settings")]
    public GameObject endPosGameObject;
    [BoxGroup("Pathfinding Visualization Settings")]
    public float timeBetweenTicks = 0.3f;
    
    [BoxGroup("Pathfinding Visualization Settings")]
    public Vector2Int startPos;
    [BoxGroup("Pathfinding Visualization Settings")]
    public Vector2Int endPos;

    [BoxGroup("Pathfinding Visualization Settings")]
    [Dropdown("GetAlgorithms")]
    public string algorithm;
    
    private List<string> GetAlgorithms { get { return new List<string>() { "Breadth First", "Depth First"}; } }
    

    private IPathfindingCommand depthFirst = new PathfindingDepthFirstCommand();
    private IPathfindingCommand breadthFirst =new PathfindingBreadthFirstSearch();

    private LineRenderer _lineRenderer;


    [Button]
    void OnMapChangedCallback()
    {
        _visualizations.ForEach(DestroyImmediate);
        _visualizations.Clear();
        if (visualisedMap == null) return;

        for (int x = 0; x < visualisedMap.mapWidth; x++)
        {
            var row = visualisedMap.tilesList[x];
            for (int y = 0; y < visualisedMap.mapHeight; y++)
            {
                var tileStatus = row.tiles[y];
                if (tileStatus == false) continue;
                var tileGameObject = Instantiate(passableTile, transform) as GameObject;
                tileGameObject.transform.position = new Vector3(sizeBetweenTiles * x, 0, sizeBetweenTiles * y);
                var tileVis = tileGameObject.GetComponent<TileVisualization>();
                tileVis.xCoord = x;
                tileVis.yCoord = y;
                tileVis.parent = this;
                _visualizations.Add(tileGameObject);
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        startPos = new Vector2Int(-1, -1);
        endPos = new Vector2Int(-1, -1);
        stepVisualizationObject.LoadAssetAsync<GameObject>().Completed += handle => singleStepVisualizationObject = handle.Result;
    }

    public void RunPathfinding()
    {
        IPathfindingCommand pathfinding = null;
        switch (algorithm)
        {
            case "Breadth First":
                pathfinding = breadthFirst;
                break;
            case "Depth First":
                pathfinding = depthFirst;
                break;
        }

        if (pathfinding == null)
        {
            Debug.LogError("Wrong pathfinding algorithm selected");
            return;
        }
        StopCoroutine("VisualizePathfindingResult");
        _currentVisualizations.ForEach(Destroy);
        _currentVisualizations.Clear();
        var result = pathfinding.CalculatePath(visualisedMap, startPos, endPos);
        StartCoroutine("VisualizePathfindingResult", result);
        startPos = new Vector2Int(-1, -1);
        endPos = new Vector2Int(-1, -1);
    }

    public void respondToTileClick(Vector2Int tileCoord)
    {
        if (startPos == new Vector2Int(-1, -1))
        {
            startPos = tileCoord;
        }
        else if (startPos != tileCoord)
        {
            endPos = tileCoord;
            RunPathfinding();
        }
    }
    
    void setStartPoint(Vector2Int startPoint)
    {
        this.startPos = startPoint;
    }
    
    void setEndPoint(Vector2Int endPoint)
    {
        this.endPos = endPoint;
    }

    private IEnumerator VisualizePathfindingResult(PathfindingResult result)
    {
        _currentVisualizations.ForEach(Destroy);
        _currentVisualizations.Clear();
        do
        {
            var currentCommand = result.Visualizations.Dequeue();
            var currentVis = Instantiate(singleStepVisualizationObject, transform) as GameObject;
            currentVis.transform.position = new Vector3(sizeBetweenTiles * currentCommand.Coordinate.x, 0.0f, sizeBetweenTiles * currentCommand.Coordinate.y);
            _currentVisualizations.Add(currentVis);
            yield return new WaitForSeconds(timeBetweenTicks);
        } while (result.Visualizations.Count > 0);
        
        var foundPath = result.Path;
        _lineRenderer.positionCount = foundPath.Count;
        
        _lineRenderer.SetPositions(foundPath.Select(coord => new Vector3(coord.x * sizeBetweenTiles, 0.0f, coord.y * sizeBetweenTiles)).ToArray());
    }
}
