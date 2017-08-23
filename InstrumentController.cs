using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentController : MonoBehaviour
{
    public TransitInstrument[] MyInstruments;
    private List<TransitInstrument> _CurrentlyTransiting = new List<TransitInstrument>();
    public bool AutoSample = true;
    private bool CurrentlySampling = true;

	// Use this for initialization
	void Start ()
    {

	}

    private void Awake()
    {
        StartCoroutine(AutoRecord());
    }

    // Update is called once per frame
    void Update ()
    {
		if(AutoSample && !CurrentlySampling)
        {
            StartCoroutine(AutoRecord());
        }
	}

    public IEnumerator AutoRecord()
    {
        CurrentlySampling = true;

        while(AutoSample)
        {
            TakeSample();
            yield return new WaitForSeconds(1.0f);
        }

        CurrentlySampling = false;

        yield return null;
    }

    public void TakeSample()
    {
        _CurrentlyTransiting.Clear();

        foreach(TransitInstrument _ti in MyInstruments)
        {
            // The sound only plays on transit, so we test against that. ENGINEERING!
            // JOS: 7/14/2017
            if(_ti.SoundPlaying)
            {
                _CurrentlyTransiting.Add(_ti);
            }
        }

        // Only take the highest value. We ain't doing the additive thing like we properly should because it's distracting to the core concept.
        // Students are likely not that quick on the average anyway.
        // JOS: 7/20/2017
        if(_CurrentlyTransiting.Count > 0)
        {
            float _highValue = 0.0f;
            float _tld = 0.0f;

            foreach (TransitInstrument _ti in _CurrentlyTransiting)
            {
                _tld = _ti.TestLightData();

                if (_tld > _highValue)
                {
                    _highValue = _tld;
                }
            }

            /*
            foreach(TransitInstrument _ti in MyInstruments)
            {
                MyInstruments[0].CreateNewData(Time.time, _highValue);
            }
            */
            
        }
        else // nothing going, so grab the first instrument and make it display test data.
        {
            foreach (TransitInstrument _ti in MyInstruments)
            {
                _ti.CreateNewData(Time.time, 1.0f);
            }

            //MyInstruments[0].CreateNewData(Time.time, 1.0f);
        }
    }
}
