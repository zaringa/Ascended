using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterIK : MonoBehaviour
{
    protected Animator _animator;
    [SerializeField] private float maxReload;
    [SerializeField] private Transform targetGun;
    [SerializeField] private LayerMask _mask;

    private Vector3 floor;
    private Vector3 ogGunpos;

    private float timeLeft;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = transform.GetComponent<Animator>();
        ogGunpos = targetGun.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeLeft>0.0f)
        {
            timeLeft -= Time.deltaTime;
            targetGun.position = new Vector3(ogGunpos.x,ogGunpos.y,ogGunpos.z - timeLeft);
        }
    
            RaycastHit _hit;
            if(Physics.Raycast(transform.position,-transform.up,out _hit,700.0f, _mask))
            {
                floor = _hit.point;
            }

    }
    public void Reset()
    {
        timeLeft = maxReload;
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(_animator)
        {
            Debug.Log("IK on");
            Vector3 leftFootPos = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            Vector3 rightFootPos = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, .9f);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, .9f);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);             

            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(rightFootPos.x,floor.y,rightFootPos.z));
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(leftFootPos.x,floor.y,leftFootPos.z));

            _animator.SetIKPosition(AvatarIKGoal.LeftHand, new Vector3(targetGun.position.x,targetGun.position.y,targetGun.position.z));
            _animator.SetIKPosition(AvatarIKGoal.RightHand, new Vector3(targetGun.position.x,targetGun.position.y,targetGun.position.z));

            _animator.SetLookAtWeight(1f); 

        }
    }


}
