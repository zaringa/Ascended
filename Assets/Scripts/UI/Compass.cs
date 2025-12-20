using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;
using System.Linq;

public class Compass : MonoBehaviour
{    
    public Image _img;
    public Transform _target;
    
    void Update()
    {
        float minX = _img.GetPixelAdjustedRect().width/2;
        float maxX = Screen.width - minX;
        float minY = _img.GetPixelAdjustedRect().height/2;
        float maxY = Screen.height - minY;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        //Debug.Log(_target.position );
        //if(_img.rectTransform.position.x < 1.5f)
        if (Vector3.Dot((_target.position - transform.position), transform.forward) < 0)
        {
            if(screenPos.x < Screen.width/2)
            {
                //change to arrow..
                screenPos.x = maxX;
            }
            else
            {
                screenPos.x = minX;
            }
        }
        _img.rectTransform.position = screenPos;
    }

    // Update is called once per frame

}
