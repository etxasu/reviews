using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseHelper : MonoBehaviour
{
    public AnimatorInstructions[] MyAnimations;
    public DemoController MyDemoController;
    public GameObject Me;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void CloseOutHelper()
    {
        StartCoroutine(RunCloseOut());
    }

    public IEnumerator RunCloseOut()
    {
        foreach(AnimatorInstructions _i in MyAnimations)
        {
            yield return new WaitForSeconds(_i.MyDelay);
            
            _i.MyObject.GetComponent<Animator>().Play(_i.MyAnimation);
        }

        yield return new WaitForSeconds(1.0f);
        // eww
        // JOS: 7/17/2017
        foreach(AnimatorInstructions _i in MyAnimations)
        {
            _i.MyObject.SetActive(false);
        }

        MyDemoController.EnableOtherTooltips(Me);

        yield return null;
    }
}

[System.Serializable]
public class AnimatorInstructions
{
    public GameObject MyObject;
    public string MyAnimation;
    public float MyDelay;
}
