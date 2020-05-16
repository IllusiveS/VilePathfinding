using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using Logic.Pathfinding;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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
                var tileGameObject = Instantiate(passableTile) as GameObject;
                tileGameObject.transform.parent = transform;
                tileGameObject.transform.position = new Vector3(sizeBetweenTiles * x, 0, sizeBetweenTiles * y);
                _visualizations.Add(tileGameObject);
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        RunPathfinding();
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
        var result = pathfinding.CalculatePath(Instantiate(visualisedMap), startPos, endPos);
        StartCoroutine("VisualizePathfindingResult", result);
    }
    

    private IEnumerator VisualizePathfindingResult(PathfindingResult result)
    {
        _currentVisualizations.ForEach(DestroyImmediate);
        _currentVisualizations.Clear();
        do
        {
            var currentCommand = result.Visualizations.Dequeue();
            var currentVis = Instantiate(singleStepVisualizationObject) as GameObject;
            currentVis.transform.parent = transform;
            currentVis.transform.position = new Vector3(sizeBetweenTiles * currentCommand.Coordinate.x, 0.0f, sizeBetweenTiles * currentCommand.Coordinate.y);
            _currentVisualizations.Add(currentVis);
            yield return new WaitForSeconds(timeBetweenTicks);
        } while (result.Visualizations.Count > 0);
        
        var foundPath = result.Path;
        _lineRenderer.positionCount = foundPath.Count;
        
        _lineRenderer.SetPositions(foundPath.Select(coord => new Vector3(coord.x * sizeBetweenTiles, 0.0f, coord.y * sizeBetweenTiles)).ToArray());
    }
}
