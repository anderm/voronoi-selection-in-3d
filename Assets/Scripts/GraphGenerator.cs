using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour {

    public int NumberOfNodes = 30;
    public int SpawnRadius = 5;
    
	// Spawn the graph objects
	void Start () {

		for (var i=0; i < this.NumberOfNodes; i++)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.layer = 8;
            s.transform.SetParent(transform);
            s.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            s.transform.position = Random.insideUnitSphere * this.SpawnRadius;
            s.AddComponent<HandDraggable>();
            s.AddComponent<OnFocusObject>();
        }
	}
	
	void Update () {
		
	}
}
