using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Shared
{
    /// <summary>
    /// The class that is used to do more customized debug rendering than Gizmos.
    /// </summary>
    public sealed class DebugRenderer : MonoBehaviourSingleton<DebugRenderer>
    {
        #region Private Attributes

#if DEBUG_RENDER
        private const int MaxExpectedLines = 4096;
        private const int MaxExpectedTriangles = 4096;
        private const int MaxExpectedQuads = 2048;
#else
        private const int MaxExpectedLines = 0;
        private const int MaxExpectedTriangles = 0;
        private const int MaxExpectedQuads = 0;
#endif

        private readonly List<LineGL> lines = new List<LineGL>(MaxExpectedLines);
        private readonly List<TriangleGL> triangles = new List<TriangleGL>(MaxExpectedTriangles);
        private readonly List<QuadGL> quads = new List<QuadGL>(MaxExpectedQuads);
        private Material debugMat = null;

        #endregion

        #region Properties

        protected override bool DestroyOnLoad { get { return true; } }

        #endregion

        #region MonoBehaviour Methods

#if DEBUG_RENDER

        private void Start()
        {
            Camera.onPostRender += PostRender;
        }

        private void OnDestroy()
        {
            Camera.onPostRender -= PostRender;
        }

#endif

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
#if DEBUG_RENDER
            bool proceed = base.Init(force);

            if (!proceed)
                return false;

            debugMat = Utils.GetNewDebugMaterial();

            return true;
#else
            return false;
#endif
        }

        #endregion

        #region GL Elements Registering

        /// <summary>
        /// Draw a line this frame using GL immediate mode.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="color"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_RENDER")]
        public void DrawLine(Vector3 v1, Vector3 v2, Color color, DebugRenderChannel channel)
        {
            LineGL l = new LineGL(v1, v2, color, channel);
            lines.Add(l);
        }

        /// <summary>
        /// Draw a triangle this frame using GL immediate mode.
        /// </summary>
        /// <param name="v1">Bottom left.</param>
        /// <param name="v2">Bottom right.</param>
        /// <param name="v3">Top right.</param>
        /// <param name="color"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_RENDER")]
        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color, DebugRenderChannel channel)
        {
            TriangleGL t = new TriangleGL(v1, v2, v3, color, channel);
            triangles.Add(t);
        }

        /// <summary>
        /// Draw a quad this frame using GL immediate mode.
        /// </summary>
        /// <param name="v1">Bottom left.</param>
        /// <param name="v2">Bottom right.</param>
        /// <param name="v3">Top left.</param>
        /// <param name="v4">Top right.</param>
        /// <param name="color"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_RENDER")]
        public void DrawQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color, DebugRenderChannel channel)
        {
            QuadGL q = new QuadGL(v1, v2, v3, v4, color, channel);
            quads.Add(q);
        }

        #endregion

        #region Render Methods

        /// <summary>
        /// Do all the debug rendering related to GL immediate mode.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void DoDebugRenderGL()
        {
            GL.PushMatrix();
            debugMat.SetPass(0);

            RenderStackedLines();
            RenderStackedTriangles();
            RenderStackedQuads();

            GL.PopMatrix();
        }

        /// <summary>
        /// Render all the stacked GL lines.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void RenderStackedLines()
        {
            GL.Begin(GL.LINES);

            while (lines.Count > 0)
            {
                int last = lines.Count - 1;
                LineGL l = lines[last];
                lines.RemoveAt(last);

                if (!DebugRenderChannels.IsChannelActive(l.channel))
                    continue;

                GL.Color(l.color);
                GL.Vertex(l.V1);
                GL.Vertex(l.V2);
            }

            GL.End();
        }

        /// <summary>
        /// Render all the stacked GL triangles.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void RenderStackedTriangles()
        {
            GL.Begin(GL.TRIANGLES);

            while (triangles.Count > 0)
            {
                int last = triangles.Count - 1;
                TriangleGL t = triangles[last];
                triangles.RemoveAt(last);

                if (!DebugRenderChannels.IsChannelActive(t.channel))
                    continue;

                GL.Color(t.color);
                GL.Vertex(t.V3);
                GL.Vertex(t.V2);
                GL.Vertex(t.V1);
            }

            GL.End();
        }

        /// <summary>
        /// Render all the stacked GL quads.
        /// </summary>
        [Conditional("DEBUG_RENDER")]
        private void RenderStackedQuads()
        {
            GL.Begin(GL.QUADS);

            while (quads.Count > 0)
            {
                int last = quads.Count - 1;
                QuadGL q = quads[last];
                quads.RemoveAt(last);

                if (!DebugRenderChannels.IsChannelActive(q.channel))
                    continue;

                GL.Color(q.color);
                GL.Vertex(q.V3);
                GL.Vertex(q.V4);
                GL.Vertex(q.V2);
                GL.Vertex(q.V1);
            }

            GL.End();
        }

        #endregion

        #region Callbacks

        private Camera mainCam;

        /// <summary>
        /// Called right after the camera finishes rendering.
        /// </summary>
        /// <param name="cam"></param>
        private void PostRender(Camera cam)
        {
            if (mainCam == null)
                mainCam = FindObjectOfType<Camera>();

            if (cam == mainCam)
                DoDebugRenderGL();
        }

        #endregion
    }
}