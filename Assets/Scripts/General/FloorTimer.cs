using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class evento
{
    [SerializeField]

    public float triggerTime;
    public string Event_;
}
public class FloorTimer : MonoBehaviour
{
    [Header("General")]
    public float currentTime = 0F;
    public List<evento> eventTimeline = new List<evento>();
    [SerializeField]
    private float maxTolerance = .4f;
    [SerializeField]
    private float maximumTime = 24F;
    int c__ = 0;
    // Update is called once per frame
    void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        if(currentTime < maximumTime)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            
            while (c__<=0)
            {
                FlushTimer();
                c__ ++;
            }
        }
        if(eventTimeline.Count>0)
            if(Mathf.Abs(eventTimeline[0].triggerTime-currentTime)< maxTolerance)
            {
                Debug.Log(eventTimeline[0].Event_);
                eventTimeline.RemoveAt(0);
            }

    }

    private void FlushTimer()
    {
        
    }
}
