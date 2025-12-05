using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorTimer : MonoBehaviour
{
    [Header("General")]
    public float currentTime = 0F;
    public Dictionary<float, string> eventTimeline;
    

    [SerializeField]
    private float maxTolerance = .4f;
    // Update is called once per frame
    void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        currentTime += Time.deltaTime;
        if(Mathf.Abs(eventTimeline.Keys.ToList()[0]-currentTime)< maxTolerance)
        {
            Debug.Log(eventTimeline.Values.ToList()[0]);
            eventTimeline.Remove(eventTimeline.Keys.ToList()[0]);
        }
    }
}
