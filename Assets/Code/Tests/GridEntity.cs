using System.Collections.Generic;
using UnityEngine;
using Shared;
using Graphs;

public class GridEntity : MonoBehaviour
{
    #region Public Attributes

    public float speed = 6.0f;
    public Transform target;

    #endregion

    #region Private Attributes

    private List<GridNode> pathResult = new List<GridNode>(128);

    private void Update()
    {
        int up = 0;
        int right = 0;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            up += 1;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            up -= 1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            right += 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            right -= 1;
        }

        transform.position += new Vector3(right, 0.0f, up).normalized * Time.deltaTime * speed;

        if (up != 0 || right != 0)
            FindPath();

        float sideFactor = Graphs.GridGraph.Instance.NodeRadius * 0.8f;

        Vector3 upFactor = Vector3.up * 0.08f;

        for (int i = 0; i < GridSearcher.OpenSet.Elements.Length; i++)
        {
            GridNode n = GridSearcher.OpenSet.Elements[i];

            if (n == null)
                continue;

            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, Color.black, DebugRenderChannel.GridGraph);
        }

        foreach (GridNode n in GridSearcher.ClosedSet)
        {
            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, new Color(0.1f, 0.1f, 0.9f), DebugRenderChannel.GridGraph);
        }

        sideFactor *= 0.8f;
        upFactor = Vector3.up * 0.09f;

        for (int i = 0; i < pathResult.Count; i++)
        {
            GridNode n = pathResult[i];
            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, Color.green, DebugRenderChannel.GridGraph);
        }
    }

    private void FindPath()
    {
        UnityEngine.Profiling.Profiler.BeginSample("PATHFINDING");

        GridSearcher.FindPath(transform.position, target.position, pathResult);

        UnityEngine.Profiling.Profiler.EndSample();

    }

    #endregion
}