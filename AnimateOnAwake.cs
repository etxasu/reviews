using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnAwake : MonoBehaviour
{
    public string AnimationToPlay;
    public float AnimationDelay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        StartCoroutine(DelayAnimation());
    }

    private IEnumerator DelayAnimation()
    {
        yield return new WaitForSeconds(AnimationDelay);

        gameObject.GetComponent<Animator>().Play(AnimationToPlay);

        yield return null;
    }
}
