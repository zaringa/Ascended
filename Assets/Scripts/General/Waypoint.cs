using UnityEngine;

public interface IWaypoint
{
    public Vector3 GetPosition();
    public void Purge();

}
public class Waypoint : MonoBehaviour, IWaypoint, IUseable
{
    public Compass compassRef;
    public Transform _tr;
    void Start()
    {
        //compassRef.AddWaypoint(this);
    }
    public void Execute()
    {
        Purge();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Purge()
    {
        Destroy(gameObject);
    }
}
