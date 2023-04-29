using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{   
    public float _timeStart; //time since the start of the trial
    public float _timeResponse;
    public float _trialTime; //is changed
    //alone - sets the starting time of each trial
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callTime()
    {
        _trialTime = _timeStart - _timeResponse;
        //_trialTime -=
        //  Time.fixedDeltaTime; // count down trial time
        Debug.Log(_trialTime);
    }
    
}
