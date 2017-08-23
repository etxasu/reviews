using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFollowsSelected : MonoBehaviour
{
    public Transform AlignTo;
    public Canvas MyCanvas;
    public RectTransform MyTransform;
    public bool TrackPlanets;
    public bool CorrectGraphSelected;
    public Vector3 Offset;
    private string OriginalText;
    private bool _FirstUpdate = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_FirstUpdate)
        {
            ExposeCAPI();
            _FirstUpdate = false;
        }

        if (TrackPlanets)
        {
            RepositionSelf();
        }
    }

    public void ToggleTracking()
    {
        if (!CorrectGraphSelected)
        {
            TrackPlanets = !TrackPlanets;
        }
    }

    public void RepositionSelf()
    {
        //Debug.Log("Aligning " + gameObject.name + " | " + MyTransform.position + " : " + Camera.main.WorldToScreenPoint(AlignTo.position));
        MyTransform.position = Camera.main.WorldToScreenPoint(AlignTo.position) + Offset;
    }

    private void RepositionToCursor()
    {
        if (!CorrectGraphSelected)
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

    private void ExposeCAPI()
    {
        Capi.expose<bool>("UI Controller.Demo Controller." + gameObject.name + ".Enabled", () => { return gameObject.activeSelf; }, (value) => { return CapiHelper.ToggleObject(gameObject, value); });
    }
}
