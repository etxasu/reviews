using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndMeasureTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SceneController MySceneController;
    public RectTransform imageRectTransform;
    public EventSystem MyEventSystem;
    public float lineWidth;
    public bool AllowDragging = true;
    public bool TrackToMouseCursor;
    public Canvas MyCanvas;
    public RectTransform TextTransform;
    public DragAndMeasureMode CurrentMode;
    public WMG_Axis_Graph[] PeriodGraphs;
    public float[] CorrectPeriods;

    public Animator _MyAnimator;
    public ArrowFollowsSelected MyTargeter;

    public bool CAPIAllowDragging = true;
    public bool FirstUpdate = true;

    public Vector2 _MouseStartDragPosition;
    public Vector2 _MouseCurrentPosition;

    private WMG_Node _FirstNode;
    private WMG_Node _LastNode;
    private WMG_Series _StartingSeries;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(TrackToMouseCursor)
        {
            RepositionToCursor();
        }
	}

    private void OnEnable()
    {
        // Enable listeners
        foreach(WMG_Axis_Graph _graph in PeriodGraphs)
        {
            _graph.WMG_BeginDrag += GraphBeginDrag;
            _graph.WMG_OnDrag += GraphOnDrag;
            _graph.WMG_OnEndDrag += GraphEndDrag;
            _graph.WMG_MouseEnter += OnGraphEnter;
        }
        //Debug.Log("OnEnable complete.");
    }

    private void OnDisable()
    {
        // Disable listeners
        foreach (WMG_Axis_Graph _graph in PeriodGraphs)
        {
            _graph.WMG_BeginDrag -= GraphBeginDrag;
            _graph.WMG_OnDrag -= GraphOnDrag;
            _graph.WMG_OnEndDrag -= GraphEndDrag;
            _graph.WMG_MouseEnter -= OnGraphEnter;
        }
        //Debug.Log("OnDisable complete.");
    }

    private void OnGraphEnter(WMG_Series _series, WMG_Node _node, bool _state)
    {
        _LastNode = _node;
    }

    private void GraphBeginDrag(WMG_Series _series, WMG_Node _node, PointerEventData eventData)
    {
        _StartingSeries = _series;
        MySceneController.CurrentWorkingGraph = _series.transform.parent.parent.name;
        Capi.set("UI Controller.Graph View.Selected Graph", MySceneController.CurrentWorkingGraph);
        RepositionToCursor();
        _FirstNode = _node;
        Debug.Log(_FirstNode.name);
        AllowDragging = false;
        _MyAnimator.Play("On");
        _MouseStartDragPosition = eventData.position;
        CurrentMode = DragAndMeasureMode.Period;
        //imageRectTransform.GetComponent<Image>().raycastTarget = false;
        imageRectTransform.position = _FirstNode.transform.localPosition;
        //OnBeginDrag(eventData);
    }

    private void GraphOnDrag(WMG_Series _series, WMG_Node _node, PointerEventData eventData)
    {
        //Debug.Log(_FirstNode.transform.position.ToString() + " | " + eventData.position.ToString());
        MeasurePoints(_FirstNode.transform.position, eventData.position, _FirstNode.transform.position);
    }

    private void GraphEndDrag(WMG_Series _series, WMG_Node _node, PointerEventData eventData)
    {
        //Debug.Log(_FirstNode.name + " | " + _LastNode.name);
        MeasurePoints(_FirstNode.transform.position, _LastNode.transform.position, _FirstNode.transform.position);
        FormatText(true, _series);
    }

    public void ChangeRepositionState()
    {
        Debug.Log("Changing state of tracking from " + TrackToMouseCursor.ToString());
        TrackToMouseCursor = !TrackToMouseCursor;
        Debug.Log("State is now " + TrackToMouseCursor.ToString());
    }

    public void RepositionToCursor()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas.transform as RectTransform, Input.mousePosition, MyCanvas.worldCamera, out pos);
        imageRectTransform.position = MyCanvas.transform.TransformPoint(pos);
    }

    private void FormatText(bool _finished, WMG_Series _series)
    {
        switch (CurrentMode)
        {
            case (DragAndMeasureMode.Debug):
                TextTransform.GetComponent<Text>().text = imageRectTransform.rect.width.ToString() + " pixels.";

                return;
            case (DragAndMeasureMode.DistanceInAU):
                TextTransform.GetComponent<Text>().text = "Measuring...";

                if (_finished)
                {
                    DetectMe _me = FirstButtonSelected.GetComponent<DetectMe>();
                    DetectMe _you = MySceneController.CurrentSelectedUIElement.GetComponent<DetectMe>();

                    if(!_me.IAmTheSun && !_you.IAmTheSun)
                    {
                        TextTransform.GetComponent<Text>().text = "Please measure from a planet to the sun.";
                        MySceneController.CurrentSelectedUIElement = MySceneController.gameObject;
                        Capi.set("UI Controller.TopDown View.Last Selected Planet", MySceneController.CurrentSelectedUIElement.name);
                        return;
                    }

                    if(_me.IAmTheSun)
                    {
                        TextTransform.GetComponent<Text>().text = _you.AU_toOutput.ToString() + " AU.";
                    }
                    else
                    {
                        TextTransform.GetComponent<Text>().text = _me.AU_toOutput.ToString() + " AU.";
                    }
                }

                return;
            case (DragAndMeasureMode.Period):
                TextTransform.GetComponent<Text>().text = "Measuring...";

                if(_finished)
                {
                    Debug.Log(_FirstNode.name + " | " + _LastNode.name);
                    //Debug.Log(_FirstNode.seriesRef.name);
                    Vector2 _fnd = _series.getNodeValue(_FirstNode);
                    Vector2 _lnd = _series.getNodeValue(_LastNode);

                    if(CheckLightValues(_series, _FirstNode, _LastNode))
                    {
                        MySceneController.PeriodMeasured = Mathf.Abs(_fnd.x - _lnd.x);

                        int _index = Array.IndexOf(PeriodGraphs, _series.theGraph);

                        if(CloseEnoughForMe(MySceneController.PeriodMeasured, CorrectPeriods[_index], 0.001f))
                        {
                            TextTransform.GetComponent<Text>().text = MySceneController.PeriodMeasured.ToString() + " years.";
                            Capi.set("UI Controller.Graph View.Period Measured", MySceneController.PeriodMeasured);
                        }
                        else
                        {
                            TextTransform.GetComponent<Text>().text = "Measured too many periods. Reduce measurement length.";
                            Capi.set("UI Controller.Graph View.Period Measured", -1.0f);
                        }
                        
                    }
                    else
                    {
                        TextTransform.GetComponent<Text>().text = "Curve data mismatch. Please retry.";
                        Capi.set("UI Controller.Graph View.Period Measured", -1.0f);
                    }

                    
                }

                return;

        }
    }

    private bool CheckLightValues(WMG_Series _series, WMG_Node _n1, WMG_Node _n2)
    {
        Vector2 _fnd = _series.getNodeValue(_n1);
        Vector2 _lnd = _series.getNodeValue(_n2);

        if (_fnd.y != _lnd.y)
        {
            Debug.Log("User mismatched light values");
            return false;
        }
        else
        {
            Debug.Log("User matched light values.");
            return true;
        }        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        AllowDragging = false;
        _MyAnimator.Play("On");

        switch (CurrentMode)
        {
            case DragAndMeasureMode.Debug:                
                _MouseStartDragPosition = eventData.position;
                imageRectTransform.position = _MouseStartDragPosition;

                if (TrackToMouseCursor)
                {
                    ChangeRepositionState();
                }
                return;

            case DragAndMeasureMode.Period:

                imageRectTransform.position = _FirstNode.transform.position;

                return;
        }

               
    }

    public void OnDrag(PointerEventData eventData)
    {
        MeasurePoints(_MouseStartDragPosition, eventData.position, _MouseStartDragPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch(CurrentMode)
        {
            case DragAndMeasureMode.Debug:
                //AllowDragging = true;
                _MyAnimator.Play("Off");
                return;

            case DragAndMeasureMode.Period:

                MeasurePoints(_FirstNode.transform.position, _LastNode.transform.position, _FirstNode.transform.position);

                return;
        }

        //AllowDragging = true;
        //_MyAnimator.Play("FadeOut");
        //ChangeRepositionState();
    }

    public void MeasurePoints(Vector2 pointA, Vector2 pointB, Vector3 start)
    {
        Vector3 differenceVector = pointB - pointA;
        Vector2 offsetPosition = pointB - pointA;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        //imageRectTransfom.pivot = new Vector2(0, 0.5f);
        imageRectTransform.position = new Vector2(start.x + (offsetPosition.x / 2), start.y + (offsetPosition.y / 2));

        TextTransform.position = start + new Vector3(offsetPosition.x / 2, (offsetPosition.y / 2) + 32, TextTransform.position.z);

        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        Capi.set("UI Controller.Drag and Measure Tool.Play Animation", "null");

        FormatText(false, null);
    }

    #region Distance in AU

    public GameObject FirstButtonSelected;
    private Vector2 DistancePositionToUse;

    public void OnBeginDragDistance(BaseEventData _eventData)
    {
        CurrentMode = DragAndMeasureMode.DistanceInAU;
        PointerEventData _ped = (PointerEventData)_eventData;
        DistancePositionToUse = _ped.position;
        FirstButtonSelected = MyEventSystem.currentSelectedGameObject;
        //Debug.Log(FirstButtonSelected.name);
        AllowDragging = false;
        RepositionToCursor();
        _MyAnimator.Play("On");
    }

    public void OnDragDistance(BaseEventData _eventData)
    {
        PointerEventData _ped = (PointerEventData)_eventData;

        //Debug.Log(DistancePositionToUse.ToString() + " | " + _ped.position.ToString());

        MeasurePoints(DistancePositionToUse, _ped.position, DistancePositionToUse);
    }

    public void OnEndDragDistance(BaseEventData _eventData)
    {
        PointerEventData _ped = (PointerEventData)_eventData;

        Debug.Log(FirstButtonSelected.name + " | " + MySceneController.CurrentSelectedUIElement.name);

        MeasurePoints(DistancePositionToUse, MySceneController.CurrentSelectedUIElement.GetComponent<RectTransform>().position, DistancePositionToUse);
        FormatText(true, null); // derp
    }

    #endregion

    static bool CloseEnoughForMe(float value1, float value2, float acceptableDifference)
    {
        return Math.Abs(value1 - value2) <= acceptableDifference;
    }
}

public enum DragAndMeasureMode
{
    Debug,
    Period,
    DistanceInAU
}
