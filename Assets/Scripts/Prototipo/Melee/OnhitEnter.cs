using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnhitEnter : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HurtBoxDetction.instance.hit)
        {
            HurtBoxDetction.instance.hit = true;
            PlayerAnimatorController.instance.animator.Play("hit");

        }
    }

   



}