using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerProfile", menuName = "CharacterController2D/PlayerProfile")]
public class PlayerProfile : ScriptableObject
{
    [Header("Player Properties")]
    public float walkSpeed = 20f;
    public float accelerationSpeed = 1;
    public float decelaraionSpeed = 1f;
    public float creepSpeed = 5f;
    public float gravity = 40f;
    public float jumpSpeed = 40f;
    public float doubleJumpSpeed = 10f;
    public float xWallJumpSpeed = 15f;
    public float yWallJumpSpeed = 15f;
    public float wallRunAmount = 8f;
    public float wallSlidAmount = 0.1f;
    public float glideTime = 2f;
    public float glideDescentAmount = 2f;
    public float powerJumpSpeed = 40f;
    public float powerJumpWaitTime = 1.5f;
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldownTime = 1f;
    public float groundSlamSpeed = 60f;
    public float deadZoneValue = 0.15f;
    public float swimSpeed = 150f;
    public float wallJumpDelay = 0.4f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float attackCooldown=0.8f;

    [Header("Player Abilities")]
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool canWallJump;
    public bool canJumpAfeterWallJump;
    public bool canWallRun;
    public bool canMultipliWallRun;
    public bool canWallSlide;
    public bool canGlide;
    public bool canGlideAfeterWallContact;
    public bool canPowerJump;
    public bool canGroundDash;
    public bool canAirDash;
    public bool canGrundSlam;
    public bool canSwim;
    public bool canMelee;
    public bool canAirAttack;


}
