using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTreeBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        PlayerController.instance.isAttacking = false;
       

    }

}
