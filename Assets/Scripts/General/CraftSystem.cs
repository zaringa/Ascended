using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;


public class CraftSystem : MonoBehaviour
{
    public bool isActive;
    private bool isWorking;
    public List<CraftableItem> AvailibleCrafts;
    private float timeRemaining;
    public CraftableItem bufferItem;
    [SerializeField] private TMP_Text text_UI;
    private string out_naym = "";

   public void SwitchActive()
    {
        if(bufferItem == null)
        {
            if(isActive)
            {
                out_naym = "";
                isActive  =false;
            }
            else
            {
                out_naym = "Availible Crafts:\n";
                int ind = 0;
                foreach(CraftableItem v in AvailibleCrafts)
                {
                    out_naym += (ind+1)+". "+ v.GetComponent<CraftableItem>().lename+"\n";
                    ind++;
                }
                text_UI.text = out_naym;
                isActive = true;

            }
        }       
        else
        {
            //гарантируем что в начале случайно не используем нескрафтившийся предмет
            if(timeRemaining <=0F)
            {
                try
                {
                    bufferItem.ResultItem.Execute();
                }
                catch
                {
                    Debug.Log("Womp Womp"); 
                } 
                bufferItem = null;
                out_naym = "";
            }
        
        }
    }

    public void TryToCraft(int CraftbleID)
    {
        if(isActive && CraftbleID <= AvailibleCrafts.Count())
        {
            Debug.Log("Craft of" + AvailibleCrafts[CraftbleID].lename + " begun");
            //timeRemaining = AvailibleCrafts[CraftbleID].craftTime;
            timeRemaining = 4.5f;
            bufferItem = AvailibleCrafts[CraftbleID].GetComponent<CraftableItem>();
            AvailibleCrafts.RemoveAt(CraftbleID);
            isActive = false;
            isWorking = true;
            out_naym = bufferItem.lename + "will be ready in "+ timeRemaining;
        }


    }
    void Update()
    {
        text_UI.text = out_naym;
        if(isWorking && timeRemaining>0F)
        {
            timeRemaining -= Time.deltaTime;
            out_naym = bufferItem.lename + " will be ready in "+ timeRemaining;

        }
        else
        {
            if(bufferItem != null)
            {
            out_naym = bufferItem.lename + "ready to deploy";
            }
            isWorking = false;
            timeRemaining = 0F;

        }
    }
}
