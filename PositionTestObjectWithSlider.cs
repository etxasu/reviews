using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lokel.Orbit;

public class PositionTestObjectWithSlider : MonoBehaviour
{
    public SceneController MySceneController;
    public SoloObjectOrbiter MyOrbit;
    public Transform MyTestObject;
    public Slider MySlider;
    public Transform ObjectToAlsoReposition;
    public int NumberOfTimesTestObjectPositioned = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void RepositionTestObjectViaSlider()
    {
        //Vector3 MyNewPosition = new Vector3(MyTestObject.position.x, MyTestObject.position.y, -MySlider.value);
        //MyTestObject.position = MyNewPosition;
        MyOrbit.transform.position = new Vector3(MySlider.value, 0.0f, 0.0f);

        if(ObjectToAlsoReposition != null)
        {
            ObjectToAlsoReposition.position = new Vector3(ObjectToAlsoReposition.GetComponent<SoloObjectOrbiter>().radius, 0.0f, 0.0f);
        }

        MyOrbit.GetWorldSpeed();

        float _di = Mathf.Pow((MyOrbit.radius / 10), -3.0f);
        float _pi = Mathf.Sqrt(_di);        

        //Debug.Log(gameObject.name + " " + _di);
        MyOrbit.rotationSpeed = _pi * MySceneController.WorldSpeed;
        
        //MyOrbit.radi

    }

    public void SetTheCapi()
    {
        Capi.set("UI Controller.TopDown View.Slider Value", MySlider.value);
        NumberOfTimesTestObjectPositioned += 1;
        Capi.set("UI Controller.TopDown View.Number of Times Slider Used", NumberOfTimesTestObjectPositioned);
    }
}
