using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTwo : StateMachineBehaviour
{
     override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.attacking = true;

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.attacking = false;

    }
    
}
