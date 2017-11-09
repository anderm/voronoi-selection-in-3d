using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTo2DHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Vector2 GUICoordinatesWithObject(GameObject go)
    {
        Vector3 cen = go.GetComponent<Renderer>().transform.position;
        return WorldToGUIPoint(cen);
    }

    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        return screenPoint;
    }
}
