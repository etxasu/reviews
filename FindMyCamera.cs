using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMyCamera : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnEnable()
    {
        CameraHelper MyHelper = GameObject.Find("Main Camera").GetComponent<CameraHelper>();

        MyHelper.AssignCameraTarget(transform);
    }
}
