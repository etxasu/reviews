using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignToSlider : MonoBehaviour
{
    public SceneController MySceneController;
    public Transform MyTransformTarget;
    public PositionTestObjectWithSlider MyRepositioner;
    public Button[] MySiblings;
    public ArrowFollowsSelected MyTargeter;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AssignTargetToRepositioner()
    {
        MyRepositioner.ObjectToAlsoReposition = MyTransformTarget;
        
        if(!MyTargeter.gameObject.activeSelf)
        {
            MyTargeter.gameObject.SetActive(true);
        }

        MyTargeter.AlignTo = MyTransformTarget;

        foreach (Button _b in MySiblings)
        {
            _b.interactable = true;

            _b.gameObject.GetComponent<AssignToSlider>().MyTransformTarget.gameObject.GetComponent<AudioSource>().enabled = false;
        }

        gameObject.GetComponent<AssignToSlider>().MyTransformTarget.gameObject.GetComponent<AudioSource>().enabled = true;
        gameObject.GetComponent<Button>().interactable = false;

        MySceneController.CurrentSelectedUIElement = gameObject;
        Capi.set("UI Controller.TopDown View.Last Selected Planet", MySceneController.CurrentSelectedUIElement.name);
    }
}
