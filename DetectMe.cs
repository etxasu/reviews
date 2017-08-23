using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMe : MonoBehaviour
{
    public SceneController MySceneController;
    public bool IAmTheSun;
    public float AU_toOutput;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnHover()
    {
        MySceneController.CurrentSelectedUIElement = gameObject;
    }


}
