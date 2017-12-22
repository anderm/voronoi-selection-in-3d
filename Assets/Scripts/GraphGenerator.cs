using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour {
    
    // Values are in meters
    public int NumberOfNodes = 30;
    public int SpawnRadius = 5;
    public float SphereRadius = 0.1f;
    
	// Spawn the graph objects
	void Start () {

		for (var i=0; i < this.NumberOfNodes; i++)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.layer = 8;
            sphere.transform.SetParent(transform);
            // Scale to 0.1 meters in radius
            sphere.transform.localScale = Vector3.one * SphereRadius;
            sphere.transform.position = Random.insideUnitSphere * this.SpawnRadius;
            sphere.AddComponent<HandDraggable>();
            sphere.AddComponent<OnFocusObject>();
        }
	}
	
	void Update () {
		
	}
}
