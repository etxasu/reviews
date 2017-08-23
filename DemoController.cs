using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoController : MonoBehaviour
{
    public GameObject TalkingHead;
    public GameObject TalkBubble;
    public GameObject CloseButton;

    public Button[] TooltipButtons;

    public string AnimationToPlay;

    [Header("Debug & Unit Testing")]
    public string MyDebugAnimation;
    private bool ExposedMyCapi = false;

    private void OnValidate()
    {
        if(MyDebugAnimation != AnimationToPlay)
        {
            Debug.Log("Playing Debug Animation: <color=#00ffffff>" + MyDebugAnimation + "</color>");
            PlayAnimationSpecified(MyDebugAnimation);
            MyDebugAnimation = AnimationToPlay;
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void BringOnScreen()
    {
        TalkingHead.SetActive(true);
        TalkBubble.SetActive(true);
        CloseButton.SetActive(true);
    }

    public void EnableOtherTooltips(GameObject _me)
    {
        if(_me.GetComponent<Button>() == null)
        {
            return;
        }

        foreach(Button _b in TooltipButtons)
        {
            if(_b != _me.GetComponent<Button>())
            {
                _b.interactable = true;
            }
        } 
    }

    private string PlayAnimationSpecified(string _anim)
    {
        gameObject.GetComponent<Animator>().Play(_anim);

        return _anim;
    }

    public void ExposeMyCapi()
    {
        if (!ExposedMyCapi)
        {
            Capi.expose<string>("Demo Controller.Play Animation", () => { return AnimationToPlay; }, (value) => { return PlayAnimationSpecified(value); });
            ExposedMyCapi = true;
        }
        else
        {
            Debug.Log("<color=yellow>Someone just tried to re-expose Demo Controller CAPI! Blocked.</color>");
        }       
    }
}
