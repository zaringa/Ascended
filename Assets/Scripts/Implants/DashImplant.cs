using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class DashImplant : BenzoImplantBase
{
    [SerializeField] private float affectedRange = 1.5f;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        
        Gizmos.DrawSphere(this.transform.position, affectedRange);
    }
    protected override void Start()
    {
        a_action = AffectedAction.dash;

        base.Start();
    }
    public override void Use(bool bEnable)
    {

        if (IsEquiped)
        {
            if (bEnable)
            {
                Debug.Log("Implant enabled");
                SphereCollider scol = this.gameObject.AddComponent<SphereCollider>();
                scol.radius = affectedRange;
                scol.isTrigger = true;
                var c = Physics.OverlapSphere(this.transform.position, affectedRange);
                foreach(Collider c_ in c)
                {
                    if (c_.gameObject.tag.Equals("Enemy"))
                        Debug.Log("HURT!");
                }
                /*var colliders = Physics.SphereCast(this.gameObject.transform.position, 5f);
                foreach(Collider o in colliders)
                {
                    
                }*/
            }
            else
            {
                Destroy(this.gameObject.GetComponent<SphereCollider>());
            }
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
            Debug.Log("HURT");
    }

}
