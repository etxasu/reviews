using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lokel.Orbit;

public class CameraHelper : MonoBehaviour
{
    public DesignableOrbit OrbitToTarget;
    public Camera MyCamera;

	// Use this for initialization
	void Start ()
    {
        //AssignCameraTarget();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AssignCameraTarget(Transform _targetMe)
    {
        MyCamera.transform.LookAt(_targetMe);
        MyCamera.transform.SetParent(_targetMe);
    }
}
