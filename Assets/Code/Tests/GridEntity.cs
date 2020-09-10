using Graphs;
using Shared;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    #region Attributes

    public int firstTravelPointIndex = 0;
    public Transform[] travelPoints;

    public float speed = 6.0f;
    public Transform target;

    private int currTravelPointIndex = 0;
    private List<GridNode> pathResult = new List<GridNode>(128);

    #endregion

    #region Methods

    private void Start()
    {
        currTravelPointIndex = firstTravelPointIndex;
        currTravelPointIndex %= travelPoints.Length;

        transform.position = travelPoints[currTravelPointIndex].position;
    }

    private void Update()
    {
        Transform currTravelPoint = travelPoints[currTravelPointIndex];

        Vector3 newPos = Vector3.MoveTowards(transform.position, currTravelPoint.position, speed * Time.deltaTime);
        transform.position = newPos;

        if (Vector3.Distance(newPos, currTravelPoint.position) <= speed * Time.deltaTime)
        {
            currTravelPointIndex++;
            currTravelPointIndex %= travelPoints.Length;
        }

        if (target == null)
            return;

        GridSearcher.FindPath(transform.position, target.position, pathResult);

        // Debug
        float sideFactor = Graphs.GridGraph.Instance.NodeRadius * 0.6f;
        Vector3 upFactor = Vector3.up * 0.04f;

        for (int i = 0; i < GridSearcher.OpenSet.Elements.Length; i++)
        {
            GridNode n = GridSearcher.OpenSet.Elements[i];

            if (n == null)
                continue;

            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, new Color(90.0f / 255.0f, 176.0f / 255.0f, 1.0f, 1.0f), DebugRenderChannel.GridGraph);
        }

        foreach (GridNode n in GridSearcher.ClosedSet)
        {
            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, new Color(1.0f, 147.0f / 255.0f, 61.0f / 255.0f, 1.0f), DebugRenderChannel.GridGraph);
        }

        sideFactor *= 0.9f;
        upFactor = Vector3.up * 0.05f;

        for (int i = 0; i < pathResult.Count; i++)
        {
            GridNode n = pathResult[i];
            Vector3 v4 = n.Pos + upFactor + (Vector3.right + Vector3.forward) * sideFactor;
            Vector3 v2 = n.Pos + upFactor + (Vector3.right - Vector3.forward) * sideFactor;
            Vector3 v1 = n.Pos + upFactor + (Vector3.left - Vector3.forward) * sideFactor;
            Vector3 v3 = n.Pos + upFactor + (Vector3.left + Vector3.forward) * sideFactor;

            DebugRenderer.Instance.DrawQuad(v1, v2, v3, v4, new Color(109.0f / 255.0f, 1.0f, 107.0f / 255.0f, 1.0f), DebugRenderChannel.GridGraph);
        }
    }

    #endregion
}