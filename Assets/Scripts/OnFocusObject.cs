using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnFocusObject : MonoBehaviour, IFocusable {

    private Color originalColor = Color.red;
    private Color focusedColor = Color.blue;

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", focusedColor);
    }

    public void OnFocusExit()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
    }
}
