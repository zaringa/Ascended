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
    // Update is called once per frame
    void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        currentTime += Time.deltaTime;
        if(Mathf.Abs(eventTimeline[0].triggerTime-currentTime)< maxTolerance)
        {
            Debug.Log(eventTimeline[0].Event_);
            eventTimeline.RemoveAt(0);
        }
    }
}
