using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;


public class PlayerAnimatorController : MonoBehaviour
{
    PlayerController playerController;
    CharacterController2D characterController;
    public static PlayerAnimatorController instance;

    public Animator animator;
    public bool hit;

    public void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        characterController = gameObject.GetComponent<CharacterController2D>();
        animator = gameObject.GetComponentInChildren<Animator>();

       
        playerController.OnDoubleJump += PlayDoubleJump;
        playerController.onAirAttack += PLayAirAttack;
        




        playerController.OnPowerJump += PlayPowerJump;
        playerController.OnStomp += PlayStomp;
        playerController.onStartDash += PlayDash;
       
    }

    // Update is called once per frames
    void Update()
    {

        animator.SetBool("hit", HurtBoxDetction.instance.hit);
        //animator.SetBool("isAttack", playerController.IsAttacking);

        if (characterController.Below)
        {
            
            animator.SetBool("isAttack", playerController.IsAirAttack);
        }

        animator.SetBool("onAir", !characterController.Below);
        


        animator.SetFloat("horizontalMovement", Mathf.Abs(playerController.MoveDirection.x));
        animator.SetFloat("verticalMovement", playerController.MoveDirection.y);
      //  Debug.Log(playerController.MoveDirection.x);
        /*if (playerController.MoveDirection.x != 0 || playerController.MoveDirection.y != 0 || playerController.IsDucking==true)
            animator.SetBool("isAttack", false);*/
        
            

        if (characterController.Below)
            animator.SetBool("isGrounded", true);
        else
            animator.SetBool("isGrounded", false);

        if ((characterController.Left || characterController.Rigth) && !characterController.Below)
            animator.SetBool("onWall", true);
        else
            animator.SetBool("onWall", false);

        animator.SetBool("isGliding", playerController.IsGliding);

        animator.SetBool("isCrounching", playerController.IsDucking);

        animator.SetBool("inWater", characterController.IsSubmerged);

    }


    void PlayDoubleJump(object sender, EventArgs e)
    {
        
        animator.SetTrigger("doubleJump");

    }



    void PlayPowerJump(object sender, EventArgs e)
    {
        animator.Play("powerJump");
    }

    void PlayStomp(object sender, EventArgs e)
    {
        animator.Play("stomp");
    }

    void PlayDash(object sender, EventArgs e)
    {
        animator.Play("slide");
    }

    void PLayAirAttack(object sender, EventArgs e)
    {
        animator.Play("airattack");
    }

}
