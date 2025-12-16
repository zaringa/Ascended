using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;
using System.Linq;

public class Waypoint_UI: Image
{
    public Waypoint wpCartridge;
    public float angle = 0F;
    private Vector2 plyrAngle;
    private PlayerLook plyr;
    public void Init(Waypoint _wp)
    {
        this.wpCartridge = _wp;
        plyr = FindFirstObjectByType<PlayerLook>().GetComponent<PlayerLook>();
    }
    void Update()
    {
        try
        {
            rectTransform.position = GetComponentInChildren<Camera>().WorldToScreenPoint(wpCartridge.transform.position);
            Debug.Log(gameObject.transform.localPosition);
        }
        catch (NullReferenceException e)
        {
            //Destroy(gameObject);
        }
    }
}

public class Compass : MonoBehaviour
{    
    public float angle = 0F;
    public List<GameObject> regist = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AddWaypoint(Waypoint _ref)
    {
        if (_ref != null)
        {
            GameObject o = Instantiate(new GameObject("Waypoint"), gameObject.transform);
            o.AddComponent<Waypoint_UI>().Init(_ref);
            regist.Add(o);
        }
    }

    // Update is called once per frame

}
