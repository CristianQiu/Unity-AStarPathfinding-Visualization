using Shared;
using UnityEngine;

namespace Graphs
{
    /// <summary>
    /// The class that is used to represent the nodes in the grid. They are used to do pathfinding tasks.
    /// </summary>
    public class GridNode : IBinaryHeapable<GridNode>
    {
        #region Properties

        public int Row { get; } = -1;
        public int Col { get; } = -1;
        public Vector3 Pos { get; } = Vector3.zero;
        public bool Walkable { get; set; } = false;
        public GridNode[] Neighbors { get; private set; } = null;

        public int GCost { get; set; } = 0;
        public int HCost { get; set; } = 0;
        public int FCost { get { return GCost + HCost; } }

        public GridNode Parent { get; set; } = null;

        public int HeapIndex { get; set; } = BinaryHeap<GridNode>.InvalidIndex;
        public ulong NumHeapItem { get; set; } = 0;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="pos"></param>
        /// <param name="walkable"></param>
        public GridNode(int row, int col, Vector3 pos, bool walkable = true)
        {
            Row = row;
            Col = col;
            Pos = pos;
            Walkable = walkable;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize this node neighbors.
        /// </summary>
        public void InitNeighbors()
        {
            int i = Row;
            int j = Col;

            GridMaster gm = GridMaster.Instance;

            // initialize the neighbors array
            Neighbors = new GridNode[(int)gm.NeighboringType];

            int counter = 0;
            bool fourNeighbors = gm.NeighboringType == GridNeighboring.FourNeighbors;

            for (int y = i - 1; y <= i + 1; ++y)
            {
                for (int x = j - 1; x <= j + 1; ++x)
                {
                    GridNode neighbor = GridMaster.Instance.GetNodeAt(y, x);
                    bool skip = neighbor == null || neighbor == this;

                    if (skip)
                        continue;

                    skip = fourNeighbors && (neighbor.Row != i && neighbor.Col != j);

                    if (skip)
                        continue;

                    // index this way so that it is possible to get them back using the Direction enums
                    Neighbors[counter] = neighbor;
                    counter++;
                }
            }
        }

        /// <summary>
        /// TODO: Real implementation
        /// </summary>
        public void BakeObstacle()
        {
            float prob = GridMaster.Instance.ObstacleProbability;
            prob = 1.0f - prob;

            Walkable = Random.Range(0.0f, 1.0f) <= prob;
        }

        #endregion

        #region Binary Heap Methods

        /// <summary>
        /// Used to get whether this node has priority over other node when being shift to the top
        /// of the heap.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasPriorityToShiftUp(GridNode other)
        {
            bool equalCosts = FCost == other.FCost && HCost == other.HCost;
            bool hasPriority = FCost < other.FCost || (FCost == other.FCost && HCost < other.HCost);

            return equalCosts ? NumHeapItem > other.NumHeapItem : hasPriority;
        }

        /// <summary>
        /// Used to get whether this node has priority over other nodewhen being shift to the bottom
        /// of the heap.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasPriorityToShiftDown(GridNode other)
        {
            bool equalCosts = FCost == other.FCost && HCost == other.HCost;
            bool hasPriority = FCost < other.FCost || (FCost == other.FCost && HCost < other.HCost);

            return equalCosts ? NumHeapItem < other.NumHeapItem : hasPriority;
        }

        #endregion
    }
}