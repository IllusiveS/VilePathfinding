using UnityEngine;

namespace Logic
{
    [CreateAssetMenu(menuName = "VilePathfinding/Tile")]
    public class Tile : ScriptableObject
    {
        public float enterCost = 1;
    }
}
