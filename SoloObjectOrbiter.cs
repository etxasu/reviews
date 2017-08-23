using UnityEngine;
using System.Collections;

public class SoloObjectOrbiter : MonoBehaviour
{
    [SerializeField]
    private SceneController _MySceneController;
    public string MySpecialName;
    public bool FixLocation;
    public bool AutoPeformPeriod;
    public Vector3 StagePosition;
    public bool UpdateStagePosition = false;
    public GameObject OrbitalFocus;
    public enum OrbitType { Simple, Quaternion, Trigonometric };
    public OrbitType MyOrbitType;
 
    private bool FirstUpdate = true;
    [SerializeField]
    private bool ExposeCapi = false;

    [Header("Simple Orbital Variables")]
    private Vector3 center;
    public Vector3 axis;
    private Vector3 desiredPosition;
    public float radius;
    public float radiusSpeed;
    public float rotationSpeed;
    public Vector3 PlanarOffset;
    public Vector3 StartingPosition;

    [Header("Other Planets")]
    public GameObject[] OtherPlanets;

    private Vector3 _StartingScale;

    [SerializeField]
    private float WorldSpeed;

    void Start()
    {
        _StartingScale = transform.localScale;
        //_MySceneController = GameObject.Find("Scene Controller").GetComponent<SceneController>();

        StartingPosition = transform.position;

        if (AutoPeformPeriod)
        {
            GetWorldSpeed();
            //InstantiateOrbitalPeriod();
        }



        Initialize();
        //transform.position = (transform.position - center.position).normalized * radius + center.position;
    }

    // Use this when instantiating an object
    public void Initialize()
    {
        //OrbitalFocus = GameObject.Find("Sun");

        center = OrbitalFocus.transform.position + PlanarOffset;
        //Debug.Log(center.ToString());
    }

    // Update material to provided material
    public void UpdateMaterial(Material _m)
    {
        gameObject.GetComponent<MeshRenderer>().material = _m;
    }

    public void GetWorldSpeed()
    {
        WorldSpeed = _MySceneController.WorldSpeed;
        radius = Vector3.Distance(transform.position, center);
        //Debug.Log("Radius is " + radius.ToString());
        InstantiateOrbitalPeriod();
        //transform.position.x = 1.0f;
    }
    public void InstantiateOrbitalPeriod()
    {
        float _di = Mathf.Pow((radius / 10), -3.0f);
        float _pi = Mathf.Sqrt(_di);

        //Debug.Log(gameObject.name + " " + _di);
        rotationSpeed = _pi;
    }

    public void UpdateCapi()
    {

    }

    void Update()
    {
        GetWorldSpeed();

        if (!FixLocation)
        {
            switch (MyOrbitType)
            {
                case OrbitType.Simple:
                    SimpleRotate();
                    break;

                default:
                    // DERP
                    break;
            }
        }

        if (FirstUpdate)
        {

            FirstUpdate = !FirstUpdate;
        }

        if (UpdateStagePosition)
        {
            AdjustStagePosition();
            GetWorldSpeed();
            InstantiateOrbitalPeriod();
            UpdateStagePosition = !UpdateStagePosition;

            UpdateCapi();
        }
    }

    public void AdjustStagePosition()
    {
        transform.position = StagePosition;
    }

    private void SimpleRotate()
    {
        //Debug.Log(name + " is " + (Vector3.Distance(transform.position, Camera.main.transform.position) + " away"));

        //Debug.Log(rotationSpeed.ToString() + " | " + WorldSpeed.ToString());
        float movementSpeed = (rotationSpeed * WorldSpeed);

        //Debug.Log(movementSpeed.ToString());

        transform.RotateAround(center, axis, movementSpeed * -Time.deltaTime);

        desiredPosition = (transform.position - center).normalized * radius + center;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
    }
}