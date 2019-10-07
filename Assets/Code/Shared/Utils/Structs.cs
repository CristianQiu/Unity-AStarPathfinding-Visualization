using UnityEngine;

namespace Shared
{
    #region Primitive Representations

    /// <summary>
    /// A simple struct representing a line.
    /// </summary>
    public struct Line
    {
        public Vector3 v1;
        public Vector3 v2;

        public Line(Vector3 v1, Vector3 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    /// <summary>
    /// A simple struct representing a triangle.
    /// </summary>
    public struct Triangle
    {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    /// <summary>
    /// A simple struct representing a quad.
    /// </summary>
    public struct Quad
    {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;
        public Vector3 v4;

        public Quad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }
    }

    #endregion

    #region GL Rendering

    /// <summary>
    /// A simple struct representing a GL line.
    /// </summary>
    public struct LineGL
    {
        private Line line;

        public Color color;
        public DebugRenderChannel channel;

        public Vector3 V1 { get { return line.v1; } set { line.v1 = value; } }
        public Vector3 V2 { get { return line.v2; } set { line.v2 = value; } }

        public LineGL(Vector3 v1, Vector3 v2, Color color, DebugRenderChannel channel)
        {
            line = new Line(v1, v2);
            this.color = color;
            this.channel = channel;
        }
    }

    /// <summary>
    /// A simple struct representing a GL triangle.
    /// </summary>
    public struct TriangleGL
    {
        private Triangle triangle;

        public Color color;
        public DebugRenderChannel channel;

        public Vector3 V1 { get { return triangle.v1; } set { triangle.v1 = value; } }
        public Vector3 V2 { get { return triangle.v2; } set { triangle.v2 = value; } }
        public Vector3 V3 { get { return triangle.v3; } set { triangle.v3 = value; } }

        public TriangleGL(Vector3 v1, Vector3 v2, Vector3 v3, Color color, DebugRenderChannel channel)
        {
            triangle = new Triangle(v1, v2, v3);
            this.color = color;
            this.channel = channel;
        }
    }

    /// <summary>
    /// A simple struct representing a GL quad.
    /// </summary>
    public struct QuadGL
    {
        private Quad quad;

        public Color color;
        public DebugRenderChannel channel;

        public Vector3 V1 { get { return quad.v1; } set { quad.v1 = value; } }
        public Vector3 V2 { get { return quad.v2; } set { quad.v2 = value; } }
        public Vector3 V3 { get { return quad.v3; } set { quad.v3 = value; } }
        public Vector3 V4 { get { return quad.v4; } set { quad.v4 = value; } }

        public QuadGL(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color, DebugRenderChannel channel)
        {
            quad = new Quad(v1, v2, v3, v4);
            this.color = color;
            this.channel = channel;
        }
    }

    #endregion
}