using UnityEngine;
using UnityEngine.Rendering;

namespace Shared
{
    /// <summary>
    /// Simple class for procedural meshes.
    /// </summary>
    public sealed class ProceduralMesh
    {
        #region Properties

        private GameObject Go { get; } = null;
        public MeshFilter Filter { get; } = null;
        public MeshRenderer Renderer { get; } = null;

        public Mesh Mesh { get { return Filter.mesh; } }

        public Vector3[] Vertices { get; } = null;
        public int[] Triangles { get; } = null;
        public Color[] Colors { get; } = null;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="emptyGo"></param>
        /// <param name="meshName"></param>
        /// <param name="numVertices"></param>
        /// <param name="numTriangles"></param>
        public ProceduralMesh(GameObject emptyGo, string meshName, int numVertices, int numTriangles)
        {
            Go = emptyGo;
            Filter = emptyGo.AddComponent<MeshFilter>();
            Renderer = emptyGo.AddComponent<MeshRenderer>();

            // set a simple debug material so that we can manually color the mesh
            Renderer.material = Utils.GetNewDebugMaterial();

            Mesh.name = meshName;
            Mesh.MarkDynamic();

            Vertices = new Vector3[numVertices];
            Triangles = new int[numTriangles];
            Colors = new Color[numVertices];

            // use the appropiate format to support meshes with more than 65534 vertices
            if (numVertices >= Mathf.Pow(2, 16) - 2)
                Mesh.indexFormat = IndexFormat.UInt32;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the vertices, triangles and colors of the mesh. Call this when you have changed any of those and want to see the changes.
        /// </summary>
        public void SetMesh()
        {
            Mesh.vertices = Vertices;
            Mesh.triangles = Triangles;
            Mesh.colors = Colors;
        }

        /// <summary>
        /// Recalculate the bounds and normals.
        /// </summary>
        public void Recalculate()
        {
            Mesh.RecalculateNormals();
            Mesh.RecalculateBounds();
        }

        /// <summary>
        /// Destroy the GameObject with its MeshFilter and MeshRenderer components.
        /// </summary>
        public void Destroy()
        {
            Utils.DestroyProper(Go);
        }

        #endregion
    }
}