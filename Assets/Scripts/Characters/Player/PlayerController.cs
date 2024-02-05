using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GlobalTypes;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float damage;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject Wepons;

    public PlayerDirection playerDirection;
    public float moveSpeed = 1f;
    public static PlayerController Instance;

    private Rigidbody2D rb;
    private AnimationControler aninController;
    private Vector2 moveInput;
    private bool isAttacking;

    public Vector2 MoveInput{get { return moveInput; }set { moveInput = value; }}

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    public void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aninController = GetComponent<AnimationControler>();
        //charAnim = GetComponentInChildren<Animator>();

    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
            UpdatePosition();

        }
        

    }

    void OnMove(InputValue value)
    {

        moveInput = value.Get<Vector2>();
    }
     


    void OnFire(InputValue value)
    {
        //Debug.Log("estou batendo .. yeai...");
        //anim.SetTrigger("SwordTrigger");
        //charAnim.SetTrigger("Attack");

        if (!isAttacking)
        {
            isAttacking = true;
            aninController.AttackTrigger();

        }
    }

    void OnShoot(InputValue value)
    {
        Debug.Log("Teste"); 
       
        //Instantiate(shot, shotPoint.position, shotPoint.rotation).moveDir = new Vector2(playerController.Direction, 0f);
        GameObject bullet = ObjectPool.instance.GetNormalShootPool();
        if (bullet != null)
        {
            //bullet.GetComponent<BulletController>().profile.moveDir = new Vector2(playerController.Direction, 0f);

            bullet.transform.position = bulletTransform.position;
            //bullet.transform.rotation = shotPoint.rotation;
            HealthManager.Instance.TakeCorsedDamage(10f);

            bullet.SetActive(true);
        }

    }

    void UpdatePosition()
    {
        
            if (moveInput.x > 0)
            {
               
                playerDirection = PlayerDirection.Right;
                Wepons.transform.rotation = Quaternion.Euler(0, 0, -90);
               
             }
            else if (moveInput.x < 0)
            {
                
                playerDirection = PlayerDirection.Left;
                Wepons.transform.rotation = Quaternion.Euler(-180, 0, 90);
            }
            else if (moveInput.y > 0)
            {

                playerDirection = PlayerDirection.Up;
                Wepons.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else if (moveInput.y < 0)
            {
                playerDirection = PlayerDirection.Down; 
                Wepons.transform.rotation = Quaternion.Euler(0, 180, 180);
            }
        
     
    }


    // Update is called once per frame
    void Update()
    {
       

    }


}
