using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButtonWidget : MonoBehaviour
{
    public Text MyLabel;
    public bool AllowClicks = true;
    public float ClickTimeout;
    public GameObject MySceneController;
    public string ResetURL;
    private SceneController _MySceneController;
    private bool FirstUpdate = true;
    [SerializeField]
    private Color _OriginalColor;
    public bool AttemptRestart;
    public bool SimWantsToRestart;
    private bool HaveSound;
    private bool UserIsMashingNext = false;

    // Use this for initialization
    void Start()
    {
        _MySceneController = MySceneController.GetComponent<SceneController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (FirstUpdate)
        {
            ExposeMyCapi();
            FirstUpdate = false;
        }

    }

    public IEnumerator TimeOutUserClicks()
    {
        gameObject.GetComponent<Button>().interactable = false;
        //gameObject.GetComponent<Image>().color = gameObject.GetComponent<Button>().colors.disabledColor;

        yield return new WaitForSeconds(ClickTimeout);

        AllowClicks = true;
        gameObject.GetComponent<Button>().interactable = true;

        yield return null;
    }

    public void TriggerNextEvent()
    {
        if (AllowClicks)
        {
            AllowClicks = false;
            StartCoroutine(TimeOutUserClicks());

            _MySceneController.TriggerCheckEvent();
        }
        else
        {
            UserIsMashingNext = true;
            Debug.Log("User is mashing next");
            StartCoroutine(TimeOutUserClicks());
        }
    }

    public void InitiateSimRestart()
    {
        SimWantsToRestart = true;
        gameObject.GetComponent<Button>().interactable = false;
    }

    public void RestartLesson()
    {
        Debug.Log("Lesson Restart triggered by simulation");

        Application.ExternalCall("UnityResetLesson", ResetURL);
        //Application.OpenURL(ResetURL);
    }

    public void OpenSPROptionsMenu()
    {
        Application.ExternalCall("UnityOpenRestartMenu", 0);
    }

    private void ExposeMyCapi()
    {
        Capi.expose<bool>("UI Controller.Next Button.Shown", () => { return gameObject.activeSelf; }, (value) => { return ReturnActiveState(value, gameObject); });
        Capi.expose<string>("UI Controller.Next Button.Label Text", () => { return MyLabel.text; }, (value) => { return MyLabel.text = value; });
    }

    private bool ReturnActiveState(bool _value, GameObject _go)
    {
        _go.SetActive(_value);

        return _value;
    }

    private bool SetButtonInteractable(bool _value, GameObject _go)
    {
        _go.GetComponent<Button>().interactable = _value;

        return _value;
    }
}
