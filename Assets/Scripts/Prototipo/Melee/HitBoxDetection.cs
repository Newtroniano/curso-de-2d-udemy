using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;

public class HitBoxDetection : MonoBehaviour
{
    [SerializeField] float raycastDistance = 0.2f;


    //PlayerController playerController;
    [SerializeField] BossType ceillingType;
    private  BoxCollider2D _boxCollider;
    [SerializeField] LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, lifeTime);
    }

    private void OnEnable()
    {


        _boxCollider = gameObject.GetComponent<BoxCollider2D>();

        //playerController = GetComponentInParent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

       
        CheckCollisionSurfaces();
      

    }





    private BossType DetermineBossType(Collider2D collider)
    {
        if (collider.GetComponent<ProjectileEffector>())
        {
            ProjectileEffector projectileEffector = collider.GetComponent<ProjectileEffector>();

            return projectileEffector.bossType;
        }
        else
        {
            return BossType.Normal;
        }
    }

    private void CheckCollisionSurfaces()
    {

        RaycastHit2D aboveHit = Physics2D.CapsuleCast(_boxCollider.bounds.center, _boxCollider.size, CapsuleDirection2D.Vertical,
      0f, Vector2.up, raycastDistance, layerMask);

        if (aboveHit.collider)
        {

            ceillingType = DetermineBossType(aboveHit.collider);

        }
        else
        {
            ceillingType = BossType.None;

        }

    }

    void CheckHit()
    {
        if (ceillingType == BossType.Boss1)
        {
            Debug.Log("Vc feriu o inimigo");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject)
        {
           CheckHit();
           //Destroy(other.gameObject);
        }
       
          
        
        
    }
}
