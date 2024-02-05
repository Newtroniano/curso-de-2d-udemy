using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GlobalTypes;




public class FireBall : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] PlayerDirection playerDirection;

    public float speed = 24f;
    [SerializeField] private Rigidbody2D rb;

    private void OnEnable()
    {

        playerDirection = PlayerController.Instance.playerDirection;

    }

    private void FixedUpdate()
    {
        Debug.Log(PlayerController.Instance.playerDirection);


        
        FireBallPostion();
    }

    void FireBallPostion()
    {
        if (playerDirection == PlayerDirection.Up)
        {
            rb.velocity = Vector2.up * speed;
        }
        if (playerDirection == PlayerDirection.Down)
        {
            rb.velocity = Vector2.down * speed;
        }
        if (playerDirection == PlayerDirection.Right)
        {
            rb.velocity = Vector2.right * speed;
        }
        if (playerDirection == PlayerDirection.Left)
        {
            rb.velocity = Vector2.left * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BossHealthManager.Instance.TakeDamage(15f);
        gameObject.SetActive(false);
    }
}

