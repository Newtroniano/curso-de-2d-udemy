using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;

public class BulletController : MonoBehaviour
{
    public ShootsProfile profile;


    PlayerController playerController;
    [SerializeField]  BossType ceillingType;
    [SerializeField]  GroundTypes wallType;
    [SerializeField]  WallType leftWallType;
    [SerializeField]  WallType rightWallType;
    [SerializeField]  ShootType shotType;
    [SerializeField]  Rigidbody2D _rigidbody;
    [SerializeField]  CapsuleCollider2D _capsuleCollider;
    [SerializeField]  LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
         //Destroy(gameObject, lifeTime);
    }

    private void OnEnable()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();

        _capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();

        playerController = GetComponentInParent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        
        _rigidbody.velocity = transform.right * profile.bulletSpeed;
        //_rigidbody.velocity = profile.moveDir * profile.bulletSpeed;
        CheckCollisionSurfaces();
        CheckHitShoot();

    }



    void CheckHitShoot()
    {
        if (shotType == ShootType.shoot8 && ceillingType == BossType.Boss1)
        {
         

            Debug.Log("Vc me feriu gravimente To puto");

        }
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


    private WallType DetermineWallType(Collider2D collider)
    {
        if (collider.GetComponent<WallEffector>())
        {
            WallEffector wallEffector = collider.GetComponent<WallEffector>();
            return wallEffector.wallType;
        }
        else
        {
            return WallType.Normal;
        }
    }

    private void CheckCollisionSurfaces()
    {
        RaycastHit2D leftHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.75f, 0f, Vector2.left,
           profile.raycastDistance * 2f, layerMask);


        if (leftHit.collider)
        {

            leftWallType = DetermineWallType(leftHit.collider);
           
        }
        else
        {
            leftWallType = WallType.None;
           
        }


        RaycastHit2D rigthHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.75f, 0f, Vector2.right,
         profile.raycastDistance * 2f, layerMask);


        if (rigthHit.collider)
        {
            rightWallType = DetermineWallType(rigthHit.collider);
           
        }
        else
        {
            rightWallType = WallType.None;
           
        }



        RaycastHit2D aboveHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, CapsuleDirection2D.Vertical,
      0f, Vector2.up, profile.raycastDistance, layerMask);

        if (aboveHit.collider)
        {

            ceillingType = DetermineBossType(aboveHit.collider);
           
        }
        else
        {
            ceillingType = BossType.None;
            
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }
}


