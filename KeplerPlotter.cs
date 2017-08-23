using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeplerPlotter : MonoBehaviour
{
    public WMG_Series MySeries;
    public Slider MyDistanceSlider;
    public Slider MyTimeSlider;
    public Text MyDistanceSliderLabel;
    public Text MyTimeSliderLabel;

    private WMG_List<Vector2> MyOriginalSeries;

	// Use this for initialization
	void Start ()
    {
        Debug.Log(MySeries.gameObject.name + " | " + MySeries.pointValues.Count);
        MyOriginalSeries = MySeries.pointValues;

        SetPointsToZero();
	}

    private void SetPointsToZero()
    {
        WMG_List<Vector2> NewPoints = new WMG_List<Vector2>();
        
        foreach (Vector2 _point in MyOriginalSeries)
        {
            Vector2 _newData = new Vector2();
            _newData.y = 0.0f;
            _newData.x = _point.x;
            //Debug.Log(_newData.y.ToString());

            NewPoints.Add(_newData);
        }
        Debug.Log("Setting data for " + transform.parent.name + " | Points: " + MySeries.pointValues.Count);
        MySeries.pointValues.SetList(NewPoints);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SolveForDistance()
    {

    }

    public void SolveForTime()
    {

    }

    public void SolveForSet()
    {
        WMG_List<Vector2> NewPoints = new WMG_List<Vector2>();

        MyDistanceSliderLabel.text = "Distance = " + MyDistanceSlider.value.ToString();
        MyTimeSliderLabel.text = "Time = " + MyTimeSlider.value.ToString();

        foreach (Vector2 _point in MyOriginalSeries)
        {
            Vector2 _newData = new Vector2();
            _newData.y = Mathf.Pow(Mathf.Pow(_point.x, MyDistanceSlider.value), (1.0f / MyTimeSlider.value) );
            if(_newData.y > 30.0f)
            {
                _newData.y = 30.0f;
            }
            _newData.x = _point.x;
            //Debug.Log(_newData.y.ToString());

            NewPoints.Add(_newData);
        }

        MySeries.pointValues.SetList(NewPoints);

        Capi.set("UI Controller.Kepler Graph View.Distance Slider Value", MyDistanceSlider.value);
        Capi.set("UI Controller.Kepler Graph View.Time Slider Value", MyTimeSlider.value);

    }
}
