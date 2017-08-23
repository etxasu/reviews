using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindMyInstrument : MonoBehaviour
{
    public GameObject ThingToColorFlash;
    public SceneController MySceneController;
    public string MyOrbitToFind;
    private bool _Connected = false;

    private bool _FirstUpdate = false;

	// Use this for initialization
	void Start ()
    {
        ThingToColorFlash = gameObject;    

        if(MySceneController == null)
        {
            MySceneController = GameObject.Find("Scene Controller").GetComponent<SceneController>();
        }

        MySceneController.MyFindPlanetTarget = gameObject;

        //MySceneController.MyPlanets.Add(gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
		if(!_FirstUpdate)
        {
            transform.localScale = MySceneController.GetDisplaySizeForWorld(transform.localScale);
            _FirstUpdate = true;
        }
	}

    private void OnEnable()
    {
        ConnectToInstrument();
    }

    public void ConnectToInstrument()
    {
        if (!_Connected)
        { 
            TransitInstrument MyHelper = GameObject.Find(MyOrbitToFind).GetComponent<TransitInstrument>();

            MyHelper.SpaceObject = gameObject;

            _Connected = true;
        }

        //transform.localScale = MySceneController.GetDisplaySizeForWorld(transform.localScale);
    }
}
