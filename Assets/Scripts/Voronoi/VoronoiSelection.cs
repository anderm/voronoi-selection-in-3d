using csDelaunay;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiSelection : Singleton<VoronoiSelection> {

    // We are using a constant weight distributor for the voronoi diagram
    // This allows easy access to objects that are at the back
    // In a data viz scenario, this is useful
    [Tooltip("Constant weight distributor for each Voronoi site. [0 - 1.0] value range.")]
    public double WeightDistributor = 0.8;

    // We are using a constant for the sphere cast radius. 0.3 meters is enough for most scenarios.
    // Increase this value if your objects are very far away from each other. 
    public float SphereCastRadius = 0.3f; 

    private List<Edge> edges;
    private Rectf bounds;
    private Voronoi voronoi;

    // Use this for initialization
    void Start () {
        // Create the bounds of the voronoi diagram based on the screen resolution
        bounds = new Rectf(0, 0, Screen.width, Screen.height);
    }

    private void Update()
    {        
        if (GazeManager.Instance.HitObject && InputManager.Instance.OverrideFocusedObject != null)
        {
            InputManager.Instance.OverrideFocusedObject.SendMessage("OnFocusExit");

            InputManager.Instance.OverrideFocusedObject = null;
        }
        else if (!GazeManager.Instance.IsGazingAtObject)
        {
            // Only check objects in layer 8
            var layerMask = 1 << 8;

            // Cast a sphere to see what nodes are in radius of the head orientation
            var hitObjects = Physics.SphereCastAll(GazeManager.Instance.GazeOrigin, SphereCastRadius, GazeManager.Instance.GazeTransform.forward, GazeManager.Instance.MaxGazeCollisionDistance, layerMask);
            var voronoiNodes = new List<Vector2>();

            if (hitObjects.Length > 0)
            {
                int objectindex = -1;
                if (hitObjects.Length == 1)
                {
                    // There only one object in range, no need for Voronoi.
                    objectindex = 0;
                }
                else
                {
                    foreach (var hit in hitObjects)
                    {
                        var screenCoordinates = WorldTo2DHelper.GUICoordinatesWithObject(hit.collider.gameObject);
                        voronoiNodes.Add(screenCoordinates);
                    }

                    objectindex = this.CreateNewVoronoiSelection(voronoiNodes, new Vector2(Screen.width / 2, Screen.height / 2));
                }

                if (objectindex != -1 && hitObjects.Length > objectindex)
                {
                    var hitObject = hitObjects[objectindex].collider.gameObject;
                    if (hitObject != InputManager.Instance.OverrideFocusedObject)
                    {
                        hitObject.SendMessage("OnFocusEnter");

                        if (InputManager.Instance.OverrideFocusedObject != null)
                        {
                            InputManager.Instance.OverrideFocusedObject.SendMessage("OnFocusExit");
                        }
                        
                        InputManager.Instance.OverrideFocusedObject = hitObject;
                    }
                }
            }
        }
        else
        {
            if (InputManager.Instance.OverrideFocusedObject != null)
            {
                InputManager.Instance.OverrideFocusedObject.SendMessage("OnFocusExit");
                InputManager.Instance.OverrideFocusedObject = null;
            }
        }
    }

    // Selects the closest node to the center point using the Voronoi diagram
    public int CreateNewVoronoiSelection(List<Vector2> voronoiNodes, Vector2 centerPoint)
    {
        var points = new List<Vector2f>();
        foreach(var node in voronoiNodes)
        {
            points.Add(new Vector2f(node.x, node.y));
        }

        if(voronoi != null)
        {
            voronoi.Dispose();
        }

        // There are two ways you can create the voronoi diagram: with or without the lloyd relaxation. 
        // We do not want lloyd relaxation as it does not represent the real location of the 3D objects.
        voronoi = new Voronoi(points, bounds, WeightDistributor);
        edges = voronoi.Edges;
        if(edges.Count < 1)
            return -1;

        // Returns the closest site based on a point.
        // The algorithm calculates the closest edge and picks the closest adjacent site of that edge.
        var closestSite = GetClosestSiteByPoint(new Vector2f(centerPoint.x, centerPoint.y));
        if(closestSite == null)
            return -1;

        // The site index does not match the initial points index, we need to check for the correct index.
        for(int i =0; i<points.Count; i++)
        {
            if(points[i] == closestSite.Coord)
                return i;
        }

        return -1;
    }

    private Site GetClosestSiteByPoint(Vector2f point)
    {
        var minDistance = float.MaxValue;
        var closestEdgeIndex = -1;
        for (int i = 0; i < edges.Count; i++)
        {
            // If clipped ends are null, it means the edge is outside the bounds.
            if (edges[i].ClippedEnds == null)
            {
                continue;
            }
            // Get the minimum distance between the point and edge
            // -1 is returned when the point projection does not lie on the edge
            var currentDistance = MinimumDistanceFromPointToEdge(edges[i], new Vector2(point.x, point.y));
            if (currentDistance != -1 && currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestEdgeIndex = i;
            }
        }

        if (closestEdgeIndex == -1)
            return null;

        // Get the adjacent sites and return the nearest one.
        var leftSite = edges[closestEdgeIndex].LeftSite;
        var rightSite = edges[closestEdgeIndex].RightSite;
        var distLeft = leftSite.Coord.DistanceSquare(point);
        var distRight = rightSite.Coord.DistanceSquare(point);

        return distLeft < distRight ? leftSite : rightSite;
    }

    private float MinimumDistanceFromPointToEdge(Edge line, Vector2 p)
    {
        var v = new Vector2(line.ClippedEnds[LR.LEFT].x, line.ClippedEnds[LR.LEFT].y);
        var w = new Vector2(line.ClippedEnds[LR.RIGHT].x, line.ClippedEnds[LR.RIGHT].y);

        var vf = new Vector2f(v.x, v.y);
        var wf = new Vector2f(w.x, w.y);
        var pf = new Vector2f(p.x, p.y);

        // Return minimum distance between line segment vw and point p
        float l2 = vf.DistanceSquare(wf);
        if (l2 == 0.0) return pf.DistanceSquare(vf);
        float t = Vector2.Dot((p - v), (w - v)) / l2;

        if (t < 0 || t > 1)
            return -1;

        // Projection falls on the segment
        Vector2 projection = v + t * (w - v);
        var prrojectionF = new Vector2f(projection.x, projection.y);
        return pf.DistanceSquare(prrojectionF);
    }
}
