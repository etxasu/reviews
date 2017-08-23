using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public float WorldSpeed = 0.0f;
    public float InstrumentSamplingSpeed = 1.0f;
    public float WorldSizes = 1.0f;
    public float PeriodMeasured = 0.0f;
    public string CurrentWorkingGraph = "";

    [Header("Controllers")]
    public InstrumentController MyInstrumentController;
    public DemoController MyDemoController;
    public UIController MyUIController;
    public List<GameObject> MyPlanets;
    public AudioSource[] MyPlanetSounders;

    [Header("Scene Management")]
    [SerializeField]
    private bool CAPIExposed = false;
    public bool UseCustomTooltipFormatting;
    public GameObject CurrentSelectedUIElement;
    public float SunSize = 0.0f;
    public bool AllowSimTriggers = true;
    public float TriggerDeferment = 0.0f;

    [Header("Debug Objects")]
    public GameObject MyFindPlanetTarget;
    public TestCAPIEnum MuhEnum;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!CAPIExposed)
        {
            ExposeMyCAPI();
            CAPIExposed = true;
        }		
	}

    private void OnValidate()
    {
        //ResizeWorlds();
    }

    public void FindPlanetTest()
    {
        MyFindPlanetTarget.GetComponent<Animation>().Play();
    }

    public Vector3 GetDisplaySizeForWorld(Vector3 _currentScale)
    {
        Vector3 _newScale = new Vector3();

        _newScale.x = _currentScale.x * WorldSizes;
        _newScale.y = _currentScale.y * WorldSizes;
        _newScale.z = _currentScale.z * WorldSizes;

        return _newScale;
    }

    public void TriggerCheckEvent()
    {
        if (AllowSimTriggers)
        {
            //TriggerDeferment = 0.0f;
            StartCoroutine(DeferTriggerCheck());
            //Debug.Log("<color=yellow>Triggering check event normally!</color>");
        }
        else
        {
            Debug.Log("Sim not allowed to trigger check events.");
        }
    }

    // ONLY USE THIS FUNCTION IF YOU KNOW WHAT THE SCREEN STATE WILL BE!
    // JOS: 11/8/2016
    public void TriggerCheckEventNOW()
    {
        TriggerDeferment = 0.0f;
        StartCoroutine(DeferTriggerCheck());
        //Debug.Log("<color=green>Triggering check event quickly!</color>");
    }

    public void TriggerDelayedCheckEvent()
    {
        if (AllowSimTriggers)
        {
            StartCoroutine(DeferTriggerCheck());
            AllowSimTriggers = false;
            Capi.set("System.Allow Sim Triggers", false);
            Debug.Log("Delayed trigger initiated");
        }
        else
        {
            Debug.Log("Sim not allowed to trigger deferred check events.");
        }
    }

    private IEnumerator ResetTriggers()
    {
        yield return new WaitForSeconds(1.0f);

        AllowSimTriggers = true;

        yield return null;
    }


    private IEnumerator DeferTriggerCheck()
    {
        yield return new WaitForSeconds(TriggerDeferment);

        Application.ExternalCall("SendMessageToUnity", false);

        Debug.Log("Check event triggered by Small Worlds Viewer!");

        StartCoroutine(SuspendTriggerEvents());

        yield return null;
    }

    private IEnumerator SuspendTriggerEvents()
    {
        //Debug.Log("SuspendTriggerEvents()");
        AllowSimTriggers = false;
        Capi.set("System.AllowSimTriggers", AllowSimTriggers);

        yield return new WaitForSeconds(1.0f);

        AllowSimTriggers = true;
        Capi.set("System.AllowSimTriggers", AllowSimTriggers);

        yield return null;
    }

    private void ExposeMyCAPI()
    {
        Capi.expose<float>("System.World Speeds", () => { return WorldSpeed; }, (value) => { return WorldSpeed = value; });
        Capi.expose<float>("System.World Sizes", () => { return WorldSizes; }, (value) => { return WorldSizes = value; });
        Capi.expose<bool>("System.AllowSimTriggers", () => { return AllowSimTriggers; }, (value) => { return AllowSimTriggers = value; });
        Capi.expose<float>("UI Controller.Graph View.Period Measured", () => { return PeriodMeasured; }, (value) => { return PeriodMeasured = value; });
        Capi.expose<string>("UI Controller.Graph View.Selected Graph", () => { return CurrentWorkingGraph; }, (value) => { return CurrentWorkingGraph = value; });
        Capi.expose<string>("UI Controller.TopDown View.Last Selected Planet", () => { return CurrentSelectedUIElement.name; }, (value) => { return CurrentSelectedUIElement.name; }); // no set

        foreach (AudioSource _as in MyPlanetSounders)
        {
            Capi.expose<bool>("Audio Controller." + _as.name + ".Enabled", () => { return _as.enabled; }, (value) => { return _as.enabled = value; });
        }

        foreach (GameObject _go in MyPlanets)
        {
            Capi.expose<bool>("System.Planets." + _go.name + ".Enabled", () => { return _go.activeSelf; }, (value) => { return CapiHelper.TogglePlanetObject(_go, value); });
        }

        //Capi.expose<TestCAPIEnum>("System.Debug.Test Enum", () => { return MuhEnum; }, (value) => { return MuhEnum = value ; } ); // this doesn't work

        MyDemoController.ExposeMyCapi();
        MyUIController.ExposeMyCapi();
    }
}

public enum ToolTipMode
{
    Transits,
    Exoplanets,
    Kepler
}

public enum TestCAPIEnum
{
    Herp,
    Derp,
    leRp,
    gerp
}
