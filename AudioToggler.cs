using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggler : MonoBehaviour
{
    public AudioSource[] AudioToToggle;
    public Image ButtonImage;
    public Sprite OnImage;
    public Sprite OffImage;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ToggleAudio()
    {
        foreach(AudioSource _as in AudioToToggle)
        {
            _as.enabled = !_as.enabled;
            Capi.set("Audio Controller." + _as.name + ".Enabled", _as.enabled);
        }

        if(ButtonImage.sprite == OnImage)
        {
            ButtonImage.sprite = OffImage;
        }
        else
        {
            ButtonImage.sprite = OnImage;
        }
    }
}
