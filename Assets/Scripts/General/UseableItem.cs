using UnityEngine;
public interface IUseable
{
   void Execute();
}

public class UseableItem : MonoBehaviour, IUseable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Execute()
    {
        Debug.Log("Haaaaaiii :3");
    }
}
