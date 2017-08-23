using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private bool ExposedMyCapi = false;
    public GameObject DragAndMeasureTool;
    public GameObject AutoPlotterGraph;
    public GameObject SliderMatch;
    public GameObject GraphView;
    public GameObject KeplerGraphView;
    public GameObject TopDownSlider;
    public GameObject ButtonContainer;
    public GameObject ClickAndDragButtons;
    public GameObject FadePanel;
    public Slider MyDistanceSlider;
    public Slider MyTimeSlider;
    public string AutoPlotterGraphAnim;
    public string TopDownViewAnim;
    public string GraphViewAnim;
    public string KeplerGraphViewAnim;
    public string FadePanelAnim;
    public string DragAndMeasureAnim;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ExposeMyCapi()
    {
        if (!ExposedMyCapi)
        {
            //Capi.expose<string>("UI Controller.Auto Plotter Graph.Play Animation", () => { return AutoPlotterGraphAnim; }, (value) => { return PlayAnimationSpecified(AutoPlotterGraph, value); });
            Capi.expose<string>("UI Controller.Fade Panel.Play Animation", () => { return FadePanelAnim; }, (value) => { return CapiHelper.PlayAnimationSpecified(FadePanel, value); });
            Capi.expose<string>("UI Controller.TopDown View.Play Animation", () => { return TopDownViewAnim; }, (value) => { return CapiHelper.PlayAnimationSpecified(SliderMatch, value); });
            Capi.expose<bool>("UI Controller.TopDown View.Show Slider", () => { return TopDownSlider.activeSelf; }, (value) => { return CapiHelper.ToggleObject(TopDownSlider, value); });
            Capi.expose<float>("UI Controller.TopDown View.Slider Value", () => { return TopDownSlider.GetComponent<Slider>().value; }, (value) => { return TopDownSlider.GetComponent<Slider>().value = value; }); // no set
            Capi.expose<bool>("UI Controller.TopDown View.Show Buttons", () => { return ButtonContainer.activeSelf; }, (value) => { return CapiHelper.ToggleObject(ButtonContainer, value); });
            Capi.expose<float>("UI Controller.TopDown View.Number of Times Slider Used", () => { return TopDownSlider.GetComponent<PositionTestObjectWithSlider>().NumberOfTimesTestObjectPositioned; }, (value) => { return TopDownSlider.GetComponent<PositionTestObjectWithSlider>().NumberOfTimesTestObjectPositioned = (int)value; });

            Capi.expose<string>("UI Controller.Graph View.Play Animation", () => { return GraphViewAnim; }, (value) => { return CapiHelper.PlayAnimationSpecified(GraphView, value); });
            Capi.expose<bool>("UI Controller.Graph View.Show Click and Drag Buttons", () => { return ClickAndDragButtons.activeSelf; }, (value) => { return CapiHelper.ToggleObject(ClickAndDragButtons, value); });

            Capi.expose<string>("UI Controller.Kepler Graph View.Play Animation", () => { return KeplerGraphViewAnim; }, (value) => { return CapiHelper.PlayAnimationSpecified(KeplerGraphView, value); });
            Capi.expose<float>("UI Controller.Kepler Graph View.Distance Slider Value", () => { return MyDistanceSlider.value; }, (value) => { return MyDistanceSlider.value = value; });
            Capi.expose<float>("UI Controller.Kepler Graph View.Time Slider Value", () => { return MyTimeSlider.value; }, (value) => { return MyTimeSlider.value = value; });

            Capi.expose<string>("UI Controller.Drag and Measure Tool.Play Animation", () => { return DragAndMeasureAnim; }, (value) => { return CapiHelper.PlayAnimationSpecified(DragAndMeasureTool, value); });
            Capi.expose<bool>("UI Controller.Drag and Measure Tool.Allow Dragging", () => { return DragAndMeasureTool.activeSelf; }, (value) => { return CapiHelper.ToggleObject(DragAndMeasureTool, value); });

            ExposedMyCapi = true;
        }
        else
        {
            Debug.Log("<color=yellow>Someone just tried to re-expose UI Controller CAPI! Blocked.</color>");
        }
    }

    
}

public static class CapiHelper
{
    public static bool ToggleObject(GameObject _go, bool _toggle)
    {
        _go.SetActive(_toggle);

        //DragAndMeasureTool.GetComponent<Animator>().Play("FadeOut");

        return _toggle;
    }

    public static string PlayAnimationSpecified(GameObject _go, string _anim)
    {
        if(_anim != "null")
        { 
            _go.GetComponent<Animator>().Play(_anim);
        }

        return "null";
    }

    public static bool TogglePlanetObject(GameObject _go, bool _toggle)
    {
        _go.GetComponent<TransitInstrument>().enabled = _toggle;
        _go.GetComponent<AudioSource>().enabled = _toggle;
        Capi.set("Audio Controller." + _go.name + ".Enabled", _toggle);
        _go.GetComponent<SoloObjectOrbiter>().enabled = _toggle;
        _go.transform.GetChild(0).gameObject.SetActive(_toggle);

        return _toggle;
    }
}
