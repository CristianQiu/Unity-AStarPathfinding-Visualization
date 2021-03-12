using Shared;
using System.Diagnostics;
using UnityEngine;

namespace Graphs
{
    /// <summary>
    /// The main class that is used to store the nodes and useful parameters for their creation.
    /// </summary>
    public sealed class GridMaster : MonoBehaviourSingleton<GridMaster>
    {
        #region Public Attributes

        [Range(0.0f, 0.4f)]
        public float ObstacleProbability = 0.2f;

        #endregion

        #region Private Attributes

        private const float MinNodeRadius = 0.1f;
        private const float MaxNodeRadius = 5.0f;

        private const int MaxRows = 50;
        private const int MaxCols = 50;

        private const float NodeDebugRadiusFactor = 0.8f;
        private static readonly Vector3 NodeDebugConnectionsUp = Vector3.up * 0.01f;

        private ProceduralMesh nodesMeshInfo = null;
        private ProceduralMesh connectionsMeshInfo = null;

        [Header("Grid configuration"), Range(MinNodeRadius, MaxNodeRadius), SerializeField]
        private float nodeRadius = 0.5f;

        [Range(5, MaxRows), SerializeField]
        private int numRows = 50;

        [Range(5, MaxCols), SerializeField]
        private int numCols = 50;

        [SerializeField]
        private GridNeighboring neighboringType = GridNeighboring.FourNeighbors;

        [Header("Grid debug"), SerializeField]
        private Color walkableNodeColor = new Color(0.0f, 0.0f, 0.0f, 192.0f / 255.0f);

        [SerializeField]
        private Color unwalkableNodeColor = new Color(1.0f, 0.0f, 0.0f, 192.0f / 255.5f);

        [SerializeField]
        private Color connectionsColor = Color.red;

        [Range(0.01f, 0.1f), SerializeField]
        private float connectionsWidthFactor = 0.02f;

        private Camera cam = null;

        #endregion

        #region Properties

        protected override bool DestroyOnLoad { get { return true; } }

        public GridNode[,] Nodes { get; private set; } = null;
        public float NodeRadius { get { return nodeRadius; } }
        public float NodeDiameter { get { return nodeRadius * 2.0f; } }
        public GridNeighboring NeighboringType { get { return neighboringType; } }
        public float NodeDebugRadius { get { return nodeRadius * NodeDebugRadiusFactor; } }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Called the very first time the singleton instance is accessed, and thus, lazily
        /// instanced. This is automatically called in Awake() too.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public override bool Init(bool force)
        {
            bool proceed = base.Init(force);

            if (!proceed)
                return false;

            cam = FindObjectOfType<Camera>();
            Asserter.Assert(cam != null, "There is no camera in the scene!");
            gameObject.hideFlags = HideFlags.NotEditable;

            CreateGrid();
            BakeNeighbors();
            BakeObstacles();

            InitDebugRender();
            BuildGridMesh();

            return true;
        }

        /// <summary>
        /// Create the grid.
        /// </summary>
        private void CreateGrid()
        {
            Nodes = new GridNode[numRows, numCols];

            Vector3 startPos = GetGridBotLeftNodePos();
            Vector3 currPos = startPos;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    // create the node and update the position
                    Nodes[i, j] = new GridNode(i, j, currPos);
                    currPos.x += NodeDiameter;
                }

                // update the Z position and reset the X position
                currPos.z += NodeDiameter;
                currPos.x = startPos.x;
            }
        }

        /// <summary>
        /// Get the bottom left corner of the grid, considering the grid is centered at the
        /// Vector3.zero world position.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetGridBotLeftCornerPos()
        {
            // calculate if we have pair or impair rows and columns
            bool pairRows = (numRows % 2) == 0;
            bool pairCols = (numCols % 2) == 0;

            // get the offsets to go to the bottom left corner of the grid
            float xOffsetFromMid = NodeDiameter * Mathf.FloorToInt((float)((float)numCols / 2.0f));
            float zOffsetFromMid = NodeDiameter * Mathf.FloorToInt((float)((float)numRows / 2.0f));

            // if the columns are not pair we must add half a node
            xOffsetFromMid = !pairCols ? (xOffsetFromMid + nodeRadius) : xOffsetFromMid;
            zOffsetFromMid = !pairCols ? (zOffsetFromMid + nodeRadius) : zOffsetFromMid;

            return new Vector3(-xOffsetFromMid, 0.0f, -zOffsetFromMid);
        }

        /// <summary>
        /// Get the bottom left node position of the grid.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetGridBotLeftNodePos()
        {
            return GetGridBotLeftCornerPos() + new Vector3(nodeRadius, 0.0f, nodeRadius);
        }

        /// <summary>
        /// Initialize the nodes neighbors.
        /// </summary>
        private void BakeNeighbors()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    GridNode n = Nodes[i, j];
                    n.InitNeighbors();
                }
            }
        }

        /// <summary>
        /// Bake the nodes obstacles to set their walkability.
        /// </summary>
        private void BakeObstacles()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    GridNode n = Nodes[i, j];
                    n.BakeObstacle();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert a position to the corresponding node covering that position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GridNode PosToNode(Vector3 pos)
        {
            Vector3 botLeft = GetGridBotLeftCornerPos();
            Vector3 offset = pos + (-botLeft);

            int row = Mathf.FloorToInt(offset.z / NodeDiameter);
            int col = Mathf.FloorToInt(offset.x / NodeDiameter);

            return GetNodeAt(row, col);
        }

        /// <summary>
        /// Get a node that is supposed to be at the given row and column. Returns null if it is out
        /// of bounds.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public GridNode GetNodeAt(int row, int col)
        {
            GridNode n = null;

            if (row < 0 || row >= numRows || col < 0 || col >= numCols)
                return n;

            return Nodes[row, col];
        }

        #endregion

        #region Debug Render Methods

        /// <summary>
        /// Initialize the stuff for the debug rendering of the grid.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void InitDebugRender()
        {
            // calculate the number of vertices and triangles
            int numQuads = numRows * numCols;
            int numVerts = numQuads * 4;
            int numTris = numQuads * 2 * 3;

            GameObject gridRenderer = new GameObject("Grid Renderer");
            gridRenderer.hideFlags = HideFlags.HideInHierarchy;

            nodesMeshInfo = new ProceduralMesh(gridRenderer, "Grid Mesh", numVerts, numTris);

            GameObject gridConnections = new GameObject("Grid Connections Renderer");
            gridConnections.hideFlags = HideFlags.HideInHierarchy;

            // let's consider that the maximum number of vertices is constrained by the eight
            // neighboring type
            numQuads = numRows * numCols * 8;
            numVerts = numQuads * 4;
            numTris = numQuads * 2 * 3;
            connectionsMeshInfo = new ProceduralMesh(gridConnections, "Grid Connections Mesh", numVerts, numTris);
        }

        /// <summary>
        /// Build the grid debug mesh.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void BuildGridMesh()
        {
            int vertNodesIndex = 0;
            int triNodesIndex = 0;

            int vertConnectionsIndex = 0;
            int triConnectionsIndex = 0;

            // get the references to the mesh stuff
            Vector3[] nodesVerts = nodesMeshInfo.Vertices;
            Color[] nodesCols = nodesMeshInfo.Colors;
            int[] nodesTris = nodesMeshInfo.Triangles;
            Mesh nodesMesh = nodesMeshInfo.Mesh;

            Vector3[] connectionsVerts = connectionsMeshInfo.Vertices;
            Color[] connectionsCols = connectionsMeshInfo.Colors;
            int[] connectionsTris = connectionsMeshInfo.Triangles;
            Mesh connectionsMesh = connectionsMeshInfo.Mesh;

            // calculate half the width of the line that represents the neighbor connections
            float lineWidthRadius = connectionsWidthFactor * nodeRadius;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    GridNode n = Nodes[i, j];

                    // first we set up the quads corresponding to the node visual representation
                    Vector3 v1 = n.Pos + (Vector3.left + Vector3.back) * NodeDebugRadius;
                    Vector3 v2 = n.Pos + (Vector3.right + Vector3.back) * NodeDebugRadius;
                    Vector3 v3 = n.Pos + (Vector3.left + Vector3.forward) * NodeDebugRadius;
                    Vector3 v4 = n.Pos + (Vector3.right + Vector3.forward) * NodeDebugRadius;

                    Color col = n.Walkable ? walkableNodeColor : unwalkableNodeColor;

                    nodesVerts[vertNodesIndex] = v1;
                    nodesVerts[vertNodesIndex + 1] = v2;
                    nodesVerts[vertNodesIndex + 2] = v3;
                    nodesVerts[vertNodesIndex + 3] = v4;

                    nodesCols[vertNodesIndex] = col;
                    nodesCols[vertNodesIndex + 1] = col;
                    nodesCols[vertNodesIndex + 2] = col;
                    nodesCols[vertNodesIndex + 3] = col;

                    nodesTris[triNodesIndex++] = vertNodesIndex;
                    nodesTris[triNodesIndex++] = vertNodesIndex + 2;
                    nodesTris[triNodesIndex++] = vertNodesIndex + 1;

                    nodesTris[triNodesIndex++] = vertNodesIndex + 2;
                    nodesTris[triNodesIndex++] = vertNodesIndex + 3;
                    nodesTris[triNodesIndex++] = vertNodesIndex + 1;

                    vertNodesIndex += 4;

                    // now we set up the connections to the neighbors
                    for (int k = 0; k < n.Neighbors.Length; k++)
                    {
                        GridNode neighbor = n.Neighbors[k];

                        if (neighbor == null)
                            continue;

                        Vector3 offsetToNeighbor = neighbor.Pos - n.Pos;

                        Vector3 left = Vector3.Cross(offsetToNeighbor, -cam.transform.forward).normalized * lineWidthRadius;

                        v1 = n.Pos + left + NodeDebugConnectionsUp;
                        v2 = n.Pos - left + NodeDebugConnectionsUp;
                        v3 = n.Pos + left + offsetToNeighbor + NodeDebugConnectionsUp;
                        v4 = n.Pos - left + offsetToNeighbor + NodeDebugConnectionsUp;

                        connectionsVerts[vertConnectionsIndex] = v1;
                        connectionsVerts[vertConnectionsIndex + 1] = v2;
                        connectionsVerts[vertConnectionsIndex + 2] = v3;
                        connectionsVerts[vertConnectionsIndex + 3] = v4;

                        connectionsCols[vertConnectionsIndex] = connectionsColor;
                        connectionsCols[vertConnectionsIndex + 1] = connectionsColor;
                        connectionsCols[vertConnectionsIndex + 2] = connectionsColor;
                        connectionsCols[vertConnectionsIndex + 3] = connectionsColor;

                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex;
                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex + 2;
                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex + 1;

                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex + 2;
                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex + 3;
                        connectionsTris[triConnectionsIndex++] = vertConnectionsIndex + 1;

                        vertConnectionsIndex += 4;
                    }
                }
            }

            nodesMeshInfo.SetMesh();
            nodesMeshInfo.Recalculate();

            connectionsMeshInfo.SetMesh();
            connectionsMeshInfo.Recalculate();
        }

        #endregion
    }
}