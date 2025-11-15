using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class SlideImplant : BenzoImplantBase
{

    [SerializeField] private float affectedRange = 1.5f;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        
        Gizmos.DrawSphere(this.transform.position, affectedRange);
    }
    protected override void Start()
    {
        base.Start();
        a_action = AffectedAction.slide;
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
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            Debug.Log("HURT");
    }
}