using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterText : MonoBehaviour
{
    public Text textBox;
    //Store all your text in this string array
    public string MessageToDisplay = "Welcome to Transits!";
    public int currentlyDisplayingText = 0;
    public float TypeWriterSpeed = 0.3f;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    
    void Awake()
    {
        StartCoroutine(AnimateText());
    }
    //This is a function for a button you press to skip to the next text
    public void SkipToNextText(string _newText)
    {
        // Only update if the message is new.
        if (_newText != textBox.text)
        {
            StopAllCoroutines();

            MessageToDisplay = _newText;

            StartCoroutine(AnimateText());
        }
    }

    IEnumerator AnimateText()
    {
        for (int i = 0; i < (MessageToDisplay.Length + 1); i++)
        {
            textBox.text = MessageToDisplay.Substring(0, i);
            yield return new WaitForSeconds(TypeWriterSpeed);
        }
    }

}
