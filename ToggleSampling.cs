using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSampling : MonoBehaviour
{
    [Header("Controllers")]
    public SceneController MySceneController;
    public InstrumentController MyInstrumentController;
    public TypeWriterText MyTooltipText;

    [Header("Variables")]
    public Text ButtonLabelReference;
    public string ButtonLabelText_On;
    public string ButtonLabelText_Off;
    public string HelpMessage_Off;
    public string HelpMessage_On;
    public bool IsSampling = true;
    public float InputTimeout;

    [Header("UI")]
    public Animator MyGraphAnimator;

    public void SetSamplingState()
    {
        if(IsSampling) // off
        {
            ButtonLabelReference.text = ButtonLabelText_On;
            MyInstrumentController.AutoSample = false;
            MyTooltipText.SkipToNextText(HelpMessage_Off);
            MyGraphAnimator.Play("GraphFlashOff");
        }
        else // on
        {
            ButtonLabelReference.text = ButtonLabelText_Off;
            MyInstrumentController.AutoSample = true;
            MyTooltipText.SkipToNextText(HelpMessage_On);
            MyGraphAnimator.Play("GraphFlashOn");
        }

        gameObject.GetComponent<Button>().interactable = false;

        IsSampling = !IsSampling;
        ToggleIndividualInstruments();
    }

    public void ToggleIndividualInstruments()
    {
        foreach(TransitInstrument _ti in MyInstrumentController.MyInstruments)
        {
            _ti.AllowSamples = !_ti.AllowSamples;
        }
    }

}
