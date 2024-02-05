using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnhiExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        HurtBoxDetction.instance.hit = false;
       
        
        
    }
}
