using UnityEditor.Build.Player;
using UnityEngine;

namespace View
{
    public class TileVisualization : MonoBehaviour
    {
        [HideInInspector]
        public MapVisualisation parent;
        [HideInInspector]
        public int xCoord;
        [HideInInspector]
        public int yCoord;

        void OnMouseDown()
        {
            parent.respondToTileClick(new Vector2Int(xCoord, yCoord));
        }
    }
}
