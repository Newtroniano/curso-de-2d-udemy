using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTwoBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

       
        if (PlayerController.instance.isAttacking)
        {
            PlayerAnimatorController.instance.animator.Play("Attack3");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.isAttacking = false;

    }
}
