using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

public class Compass : MonoBehaviour
{    
    private Image _img;
    private GameObject _wpGO;
    private Transform _target;
    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    void Start()
    {
        _target = transform;
        

        _wpGO = new GameObject("Wp");
        _wpGO.transform.parent = FindFirstObjectByType<Canvas>().gameObject.transform;
        _img = _wpGO.AddComponent<Image>();
        minX = _img.GetPixelAdjustedRect().width/2;
        maxX = Screen.width - minX;
        minY = _img.GetPixelAdjustedRect().height/2;
        maxY = Screen.height - minY;
    }

    void Update()
    {

        Vector2 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        //Debug.Log(_target.position );
        //if(_img.rectTransform.position.x < 1.5f)
        if (Vector3.Dot((_target.position - Camera.main.transform.position), Camera.main.transform.forward) < 0)
        {
            if(screenPos.x < Screen.width/2)
            {
                //_img.rectTransform.sizeDelta = new Vector2(minX,minY);
                //change to arrow..
                screenPos.x = maxX;

            }
            else
            {
                //_img.rectTransform.sizeDelta = new Vector2(minX,minY);
                screenPos.x = minX;
            }
        }
        else
        {
            _img.rectTransform.sizeDelta = new Vector2(minX*2,minY*2);
        }
        _img.rectTransform.position = screenPos;
        if(screenPos.x == maxX || screenPos.y == minY ||screenPos.x == minX || screenPos.y == maxY)
        {
                _img.rectTransform.sizeDelta = new Vector2(minX,minY);
            
        }
        
    }

    // Update is called once per frame

}
