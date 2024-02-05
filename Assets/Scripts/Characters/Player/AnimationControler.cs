using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControler : MonoBehaviour
{
     private PlayerController playerController;
     private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();    
        playerAnimator =   GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAnimator.SetFloat("Vertical", playerController.MoveInput.y);
        playerAnimator.SetFloat("Horizontal", playerController.MoveInput.x);
        playerAnimator.SetFloat("Speed", playerController.MoveInput.sqrMagnitude);

    }

    public void AttackTrigger()
    {
        if (playerController.IsAttacking)
        {
            playerAnimator.SetTrigger("SwordTrigger");
        }
    } 
}
