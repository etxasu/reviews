using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlignButtonToPlanet : MonoBehaviour
{
    public AlignmentTarget AlignWith;
    [SerializeField]
    private TransitInstrument MyInstrument;
    public Transform AlignTo;
    public Canvas MyCanvas;
    public RectTransform MyTransform;
    public RectTransform MyDropTarget;
    public bool TrackPlanets;
    public bool CorrectGraphSelected;
    private string OriginalText;
    private Color OriginalColor;
    private Color OriginalTextColor;
    public Color ErrorColor;
    public Color Transparency;
    public RectTransform Blocker;

    private bool _FirstUpdate = true;

	// Use this for initialization
	void Start ()
    {
        OriginalText = transform.GetChild(0).GetComponent<Text>().text;
        OriginalTextColor = transform.GetChild(0).GetComponent<Text>().color;
        OriginalColor = gameObject.GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_FirstUpdate)
        {      
            if (AlignTo.GetComponent<TransitInstrument>().SpaceObject != null)
            {
                MyInstrument = AlignTo.GetComponent<TransitInstrument>();
                Debug.Log("Altering AlignTo for " + gameObject.name);
                AlignTo = AlignTo.GetComponent<TransitInstrument>().SpaceObject.transform;
                ExposeCAPI();
                _FirstUpdate = false;             
            }            
        }

        if (TrackPlanets)
        {
            RepositionSelf();
        }
	}

    public void ToggleTracking()
    {
        if(!CorrectGraphSelected)
        {
            TrackPlanets = !TrackPlanets;
        }      
    }

    public void RepositionSelf()
    {
        //Debug.Log("Aligning " + gameObject.name + " | " + MyTransform.position + " : " + Camera.main.WorldToScreenPoint(AlignTo.position));
        MyTransform.position = Camera.main.WorldToScreenPoint(AlignTo.position);

        if(MyTransform.GetComponent<BoxCollider2D>().IsTouching(Blocker.GetComponent<CircleCollider2D>()) && !MyInstrument.NearToCamera)
        {
            gameObject.GetComponent<Image>().color = Transparency;
            MyTransform.GetChild(0).GetComponent<Text>().color = Transparency;
            //Debug.Log("derp " + gameObject.name);
        }
        else
        {
            gameObject.GetComponent<Image>().color = OriginalColor;
            MyTransform.GetChild(0).GetComponent<Text>().color = OriginalTextColor;
            //Debug.Log("herp " + gameObject.name);
        }

    }

    private void RepositionToCursor()
    {
        if(!CorrectGraphSelected)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas.transform as RectTransform, Input.mousePosition, MyCanvas.worldCamera, out pos);
            MyTransform.position = MyCanvas.transform.TransformPoint(pos);
        }
    }

    public void BeginButtonDrag()
    {
        
    }

    public void DuringDrag()
    {
        RepositionToCursor();
    }

    public void EndButtonDrag()
    {
        if (MyTransform.GetComponent<BoxCollider2D>().IsTouching(MyDropTarget.GetComponent<BoxCollider2D>()))
        {
            //Debug.Log("HI");
            MyTransform.GetComponent<Button>().interactable = false;
            CorrectGraphSelected = true;   
        }
        else
        {
            //Debug.Log("BYE:" + MyTransform.name + "|" + MyDropTarget.name);
            MyTransform.GetComponent<Button>().interactable = true;
            CorrectGraphSelected = false;
            StartCoroutine(ResetTrackingToPlanet());
        }
        Capi.set("UI Controller.Graph View." + gameObject.name + " Correct", CorrectGraphSelected);
    }

    private IEnumerator ResetTrackingToPlanet()
    {
        gameObject.GetComponent<Image>().color = ErrorColor;

        transform.GetChild(0).GetComponent<Text>().text = ":(";

        yield return new WaitForSeconds(1.0f);

        ToggleTracking();

        transform.GetChild(0).GetComponent<Text>().text = OriginalText;
        gameObject.GetComponent<Image>().color = OriginalColor;

        yield return null;
    }

    private void ExposeCAPI()
    {
        Capi.expose<bool>("UI Controller.Graph View." + gameObject.name + " Correct", () => { return CorrectGraphSelected; }, (value) => { return CorrectGraphSelected = value; });
    }

}

public enum AlignmentTarget
{
    NormalView,
    TopDownView
}
