using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboHits : MonoBehaviour
{

    public Animator myAnim;
    public bool isAttacking = false;
    public static ComboHits instance;


    private void Update()
    {
       
    }

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        myAnim = GetComponentInChildren<Animator>();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            isAttacking = true;
        }
    }

    public void InputManger()
    {
    
    }

 
}
