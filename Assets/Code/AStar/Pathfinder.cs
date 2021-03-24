using Shared;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    /// <summary>
    /// The class used to do pathfinding.
    /// </summary>
    public static class Pathfinder
    {
        #region Public Attributes

        public static readonly BinaryHeap<GridNode> OpenSet = new BinaryHeap<GridNode>(MaxHeapSize);
        public static readonly HashSet<GridNode> ClosedSet = new HashSet<GridNode>();

        // the closer the relation (gCost / hCost) to 0, the more accurate, but less performant
        // path. On the contrary, the higher, the less accurate and more greedy, but more performant
        public static int hCostMultiplier = 1;

        public static int gCostMultiplier = 1;

        #endregion

        #region Private Attributes

        private const int StraightCost = 10;
        private const int DiagonalCost = 14;
        private const int MaxHeapSize = 512;

        #endregion

        #region Pathfinding Methods

        /// <summary>
        /// Find the path from the given position to the end position and store it in the list
        /// passed as parameter. The A* algorithm is used to find the path.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="intoResult"></param>
        public static void FindPath(Vector3 startPos, Vector3 endPos, List<GridNode> intoResult)
        {
            GridNode startNode = GridMaster.Instance.PosToNode(startPos);
            GridNode endNode = GridMaster.Instance.PosToNode(endPos);

            FindPath(startNode, endNode, intoResult);
        }

        /// <summary>
        /// Find the path from the given node to the end node and store it in the list passed as parameter.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="intoResult"></param>
        private static void FindPath(GridNode startNode, GridNode endNode, List<GridNode> intoResult)
        {
            // clear the sets
            intoResult.Clear();
            OpenSet.Clear();
            ClosedSet.Clear();

            // TODO: Maybe we want to find the path to the closest location to the end node ?
            if (startNode == null || endNode == null || !startNode.Walkable || !endNode.Walkable)
                return;

            // initalize the values and add the initial node
            startNode.GCost = 0;
            startNode.HCost = GetHeuristic(startNode, endNode);
            OpenSet.Add(startNode);

            // keep going until there are no nodes in the open set or we find the final node
            while (OpenSet.Count > 0)
            {
                GridNode currNode = OpenSet.ExtractRoot();

                // we are done if it is the end node
                if (currNode == endNode)
                {
                    BuildPath(startNode, endNode, intoResult);
                    return;
                }

                // expand the current node neighbors
                for (int i = 0; i < currNode.Neighbors.Length; ++i)
                {
                    GridNode neighbor = currNode.Neighbors[i];

                    // if we can not pass through or it is already in the closed set, skip this
                    if (neighbor == null || !neighbor.Walkable || ClosedSet.Contains(neighbor))
                        continue;

                    // calculate the new gCost for this neighbor from where we are coming
                    int newNeighborGCost = currNode.GCost + GetIncrementalGCost(currNode, neighbor);
                    bool notInOpenSet = neighbor.HeapIndex == -1;

                    // if the node is not yet in the open set or the new gCost of that neighbor is lower
                    if (notInOpenSet || newNeighborGCost < neighbor.GCost)
                    {
                        // update the gCost, hCost, and the parent
                        neighbor.GCost = newNeighborGCost;
                        neighbor.HCost = GetHeuristic(neighbor, endNode);
                        neighbor.Parent = currNode;

                        if (notInOpenSet)
                            OpenSet.Add(neighbor);
                        else
                            OpenSet.UpdateElementWithChangedVal(neighbor); // < the GCost decrease made the FCost to decrease, so update the element in the heap
                    }
                }

                ClosedSet.Add(currNode);
            }
        }

        /// <summary>
        /// Build the path from the end node to the start node, saving it in the list passed as parameter.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="intoResult"></param>
        private static void BuildPath(GridNode startNode, GridNode endNode, List<GridNode> intoResult)
        {
            GridNode currNode = endNode;

            // iterate over the nodes to build the path
            while (currNode != startNode)
            {
                intoResult.Add(currNode);
                currNode = currNode.Parent;
            }

            // reverse it because it is built from the end to the start
            intoResult.Reverse();
        }

        #endregion

        #region Cost Methods

        /* Note: Cost functions are only taking into account the 2D space (positions in XZ).
        It may be interesting to add a 3D modifier that takes into account the Y height too.
        We are also lacking cost modifiers such as mud, water, etc, which may also be interesting.
        And finally, no smoothing techniques are used, which may become useful for some projects.*/

        /// <summary>
        /// Get the heuristic from the origin node to the target node.
        /// </summary>
        /// <param name="origNode"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        private static int GetHeuristic(GridNode origNode, GridNode targetNode)
        {
            int minRow = Mathf.Min(origNode.Row, targetNode.Row);
            int maxRow = Mathf.Max(origNode.Row, targetNode.Row);

            int minCol = Mathf.Min(origNode.Col, targetNode.Col);
            int maxCol = Mathf.Max(origNode.Col, targetNode.Col);

            int rowOffset = maxRow - minRow;
            int colOffset = maxCol - minCol;

            int h = 0;

            if (GridMaster.Instance.NeighboringType == GridNeighboring.EightNeighbors)
            {
                // diagonal distance
                int max = Mathf.Max(rowOffset, colOffset);
                int min = Mathf.Min(rowOffset, colOffset);

                int offset = max - min;

                h = offset * StraightCost + min * DiagonalCost;
            }
            else
            {
                // manhattan distance
                h = (rowOffset + colOffset) * StraightCost;
            }

            return h * hCostMultiplier;
        }

        /// <summary>
        /// Get the distance from the origin node to its neighbor, that is, the incremental GCost.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        private static int GetIncrementalGCost(GridNode node, GridNode neighbor)
        {
            bool sameRow = node.Row == neighbor.Row;
            bool sameCol = node.Col == neighbor.Col;

            // this works both for four and eight neighbors
            int result = (sameRow || sameCol) ? StraightCost : DiagonalCost;

            return result * gCostMultiplier;
        }

        #endregion
    }
}