using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lokel.Orbit;
using System.Linq;

public class TransitInstrument : MonoBehaviour
{
    public GameObject Sun;
    public GameObject SpaceObject;
    public Camera MyCamera;
    public WMG_Axis_Graph MyGraph;
    public WMG_Series MySeries;
    public SceneController MySceneController;
    private List<GameObject> _MyPoints;
    private int _PointIndex;
    public bool AllowAutoSampling = false;
    private bool CurrentlySampling = false;
    [SerializeField]
    private float _castRadius = 2.0f;
    [SerializeField]
    private float _castDistance = 1000.0f;

    private float _lightValue;
    private float _dump;
    private float _transitSize;
    [SerializeField]
    private bool FirstTransit = true;

    [Header("UX Properties")]
    public AudioSource MySounder;
    public float PitchOffset;
    public bool SoundPlaying;
    public bool AllowSamples = true;
    public AudioClip MyClip;
    public bool IsTestObject;

    public bool NearToCamera; // Relatively speaking

	// Use this for initialization
	void Start ()
    {
		if(MyCamera == null)
        {
            MyCamera = Camera.main;
        }
	}

    private void Awake()
    {
        if(IsTestObject)
        {
            MySceneController.SunSize = CalculateScreenPercentage(Sun);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // dem tests
        if (SpaceObject != null)
        { 
            CheckPositionRelativeToCamera();
        }   

        if(AllowAutoSampling)
        {
            AllowAutoSampling = false;
            CurrentlySampling = true;
            StartCoroutine(CheckForTransits());           
        }
	}

    private IEnumerator CheckForTransits()
    {
        while(CurrentlySampling)
        {
            //Debug.Log("Sampling on " + gameObject.name);
            yield return new WaitForSeconds(MySceneController.InstrumentSamplingSpeed);
            AutoTestForTransit();
        }

        AllowAutoSampling = true;

        yield return null;
    }

    // This function checks to see if the object on the camera's side of the orbit. 
    // This lets us do less calculations.
    public void CheckPositionRelativeToCamera()
    {
        if(Vector3.Distance(SpaceObject.transform.position, MyCamera.transform.position) <= Vector3.Distance(MyCamera.transform.position, Sun.transform.position))
        {
            NearToCamera = true;
        }
        else
        {
            NearToCamera = false;
        }
    }

    public void CreateTestData()
    {
        _PointIndex++;

        // Create dat data
        Vector2 _MyPoint = new Vector2(_PointIndex, TestLightData());

        MySeries.pointValues.Add(_MyPoint);
    }

    public void CreateNewData(float _time, float _lv)
    {
        Vector2 _MyPoint = new Vector2(_time, _lv);
        MySeries.pointValues.Add(_MyPoint);
    }

    public void AutoTestForTransit()
    {
        _dump = TestLightData();
    }

    public float TestLightData()
    {
        float _newpoint = 1.0f;

        //Debug.Log(BoundsExtensions.DistanceAndDiameterToPixelSize(Vector3.Distance(MyCamera.transform.position, SpaceObject.transform.position), SpaceObject.GetComponent<MeshRenderer>().bounds.size.x));

        RaycastHit _SphereHit;

        Ray _ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(Sun.transform.position));

        if(!SpaceObject)
        {
            //Error checking for assignments
            Debug.Log("No transform assigned to " + gameObject.name + "'s instrument. Returning 1.0f as test data."); 
            return 1.0f;
        }

        if(Physics.SphereCast(_ray, _castRadius, out _SphereHit, _castDistance))
        {
            if(_SphereHit.transform == SpaceObject.transform)
            {          
                if(FirstTransit)
                {
                    _transitSize = CalculateScreenPercentage(SpaceObject);
                    FirstTransit = false;
                }


                if(!SoundPlaying)
                {
                    SetSounderPitch();
                    SoundPlaying = true;
                }

                if (CurrentlySampling)
                {
                    _newpoint = _lightValue; // test data

                    if(AllowSamples)
                    {
                        CreateNewData(Time.time, _newpoint);
                    }

                    CurrentlySampling = false;
                }
            }
            else
            {
                if(SoundPlaying)
                {

                    StartCoroutine(AudioFadeOut.FadeOut(MySounder, 0.5f));
                    SoundPlaying = false;
                }
                //Debug.Log("Enabling auto sampling for " + gameObject.name);
                AllowAutoSampling = true;
            }
        }

        return _newpoint;
    }

    public float HelperDistance(float x, float y)
    {
        return Mathf.Abs(x - y);
    }

    private void SetSounderPitch()
    {
        if (IsTestObject)
        {
            /*
            List<GameObject> MyPlanets = new List<GameObject>();

            List<float> _pradii = new List<float>();

            float _tradius = gameObject.GetComponent<SoloObjectOrbiter>().radius;

            foreach (GameObject _planet in gameObject.GetComponent<SoloObjectOrbiter>().OtherPlanets)
            {
                MyPlanets.Add(_planet);
                _pradii.Add(HelperDistance(_planet.GetComponent<SoloObjectOrbiter>().radius, _tradius));
            }

            float minima = 20;
            int mindex = 0;

            for (int i = 0; i < MyPlanets.Count; i++)
            {
                if (_pradii[i] < minima)
                {
                    minima = _pradii[i];
                    mindex = i;
                }
            }

            //Debug.Log("Closest sounder is " + MyPlanets[mindex].name + " so says " + gameObject.name);
            MySounder.pitch = MyPlanets[mindex].GetComponent<TransitInstrument>().MySounder.pitch + PitchOffset;
            */
            MySounder.PlayOneShot(MyClip);
        }
        else
        {
            _lightValue = (MySceneController.SunSize - _transitSize) / MySceneController.SunSize;

            // Sun totally occluded
            if (_lightValue <= 0)
            {
                _lightValue = 0;
            }

            if ((MySceneController.SunSize / 2) > _transitSize)
            {
                // The final pitch value ends up between 1 and 3.
                
                float _pitchMod = ((40 * _transitSize) / MySceneController.SunSize);
                //Debug.Log("<color=red>" + _transitSize + " | " + MySceneController.SunSize + "</color>");
                //Debug.Log(gameObject.name + " pitch is " + _pitchMod);
                MySounder.pitch = 1 + ((1 - _pitchMod)) + PitchOffset;
            }
            else
            {
                // // The final pitch value ends up between 0.5 and 1.
                float _pitchMod = _transitSize / MySceneController.SunSize;
                MySounder.pitch = 1.0f - (0.5f * _pitchMod) + PitchOffset;

                if (MySounder.pitch < 0.5f)
                {
                    MySounder.pitch = 0.5f;
                }
            }
            MySounder.PlayOneShot(MyClip);
        }
        
    }

    private float CalculateScreenPercentage(GameObject _go)
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        Bounds bounds = _go.GetComponent<Renderer>().bounds;
        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        Vector3[] corners = new Vector3[8];

        corners[0] = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        corners[1] = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        corners[2] = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        corners[3] = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        corners[4] = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        corners[5] = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        corners[6] = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        corners[7] = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

        for (var i = 0; i < corners.Length; i++)
        {
            var corner = transform.TransformPoint(corners[i]);
            corner = Camera.main.WorldToScreenPoint(corner);
            if (corner.x > maxX) maxX = corner.x;
            if (corner.x < minX) minX = corner.x;
            if (corner.y > maxY) maxY = corner.y;
            if (corner.y < minY) minY = corner.y;
            minX = Mathf.Clamp(minX, 0, Screen.width);
            maxX = Mathf.Clamp(maxX, 0, Screen.width);
            minY = Mathf.Clamp(minY, 0, Screen.height);
            maxY = Mathf.Clamp(maxY, 0, Screen.height);
        }

        // Kludge for bad scans
        // JOS: 8/16/2017
        if(minY == 0)
        {
            minY = minX;
        }

        if(maxY == 0)
        {
            maxY = maxX;
        }

        var width = maxX - minX;
        var height = maxY - minY;
        float area = width * height;

        double percentage = area / (Screen.width * Screen.height) * 100.0;

        //Debug.Log("Percentage of coverage of " + gameObject.name + " is " + percentage.ToString());
        //Debug.Log("<color=cyan>" + area + "|" + maxX + "|" + minX + "|" + maxY + "|" + minY + "</color>");

        return (float)percentage;
    }

}

public static class BoundsExtensions
{
    //Get the screen size of an object in pixels, given its distance and diameter.
    public static float DistanceAndDiameterToPixelSize(float distance, float diameter)
    {

        float pixelSize = (diameter * Mathf.Rad2Deg * Screen.height) / (distance * Camera.main.fieldOfView);
        return pixelSize;
    }

    //Get the distance of an object, given its screen size in pixels and diameter.
    public static float PixelSizeAndDiameterToDistance(float pixelSize, float diameter)
    {

        float distance = (diameter * Mathf.Rad2Deg * Screen.height) / (pixelSize * Camera.main.fieldOfView);
        return distance;
    }

    //Get the diameter of an object, given its screen size in pixels and distance.
    public static float PixelSizeAndDistanceToDiameter(float pixelSize, float distance)
    {

        float diameter = (pixelSize * distance * Camera.main.fieldOfView) / (Mathf.Rad2Deg * Screen.height);
        return diameter;
    }
}

public static class AudioFadeOut
{
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}