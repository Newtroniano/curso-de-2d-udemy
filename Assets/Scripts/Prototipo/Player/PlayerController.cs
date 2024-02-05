using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GlobalTypes;
using System;

public class PlayerController : MonoBehaviour
{

    // public PlayerObj player;

    #region public properties
    public PlayerProfile profile;
    public PlayerAnimatorController profile_animator;
    public static PlayerController Instance;





    [Header("Player State")]
    [SerializeField] bool isJumping;
    [SerializeField] bool isDoubleJumping;
    [SerializeField] bool isTripleJumping;
    [SerializeField] bool isWallJumping;
    [SerializeField] bool isWallRunning;
    [SerializeField] bool isWallSliding;
    [SerializeField] bool isDucking;
    [SerializeField] bool isCreeping;
    [SerializeField] bool isGliding;
    [SerializeField] bool isPowerJump;
    [SerializeField] bool isDashing;
    [SerializeField] bool isGroundSlam;
    [SerializeField] bool isSwinming;

    public bool isAttacking = false;
    public bool attacking = false;

    [SerializeField] bool isAirAttack;
    [SerializeField] float _tempMoveSpeed;


    //Melee events
    public bool canBeceiveInput;
    public bool inputReceived;

    //events
    public event EventHandler OnDoubleJump;
    public event EventHandler OnPowerJump;
    public event EventHandler OnStomp;
    public event EventHandler onStartDash;

    public event EventHandler onAirAttack;
    //public event EventHandler onAttack;
    #endregion

    #region private properties
    //input flags
    private bool _startJumping;
    private bool _realseJumping;
    private bool _holdJump;



    private Vector2 _input;
    public Vector2 _moveDirection;
    private int direction;

    //private Animator anim;

    private CharacterController2D _characterController;
    public bool _ableToWallRun = true;
    private CapsuleCollider2D _capsuleCollider;
    private Vector2 _orinalColliderSize;
    //TODO: remove later when not needed

    private SpriteRenderer _spriteRenderer;
    private float _currentGlideTime;
    private bool _startGlide = true;
    private float _powerJumpingTimer;
    //private bool _facingRight;
    private float _dashTimer;
    private float _jumpPadAmunt = 15f;
    private float _jumpPadAdjustment = 0f;
    private Vector2 _tempVelocity;
    private bool _inAirControl = true;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;



    #region StatesEncapsulemnt
    public Vector2 MoveDirection { get => _moveDirection; }
    public bool IsJumping { get => isJumping; }
    public bool IsDoubleJumping { get => isDoubleJumping; }
    public bool IsTripleJumping { get => isTripleJumping; }
    public bool IsWallJumping { get => isWallJumping; }
    public bool IsWallRunning { get => isWallRunning; }
    public bool IsWallSliding { get => isWallSliding; }
    public bool IsDucking { get => isDucking; }
    public bool IsCreeping { get => isCreeping; }
    public bool IsGliding { get => isGliding; }
    public bool IsPowerJump { get => isPowerJump; }
    public bool IsDashing { get => isDashing; }
    public bool IsGroundSlam { get => isGroundSlam; }
    public bool IsSwinming { get => isSwinming; }
    public int Direction { get => direction; }

    public bool IsAirAttack { get => isAirAttack; set => isAirAttack = value; }


    public static PlayerController instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        // anim = GetComponent<Animator>();
        _characterController = gameObject.GetComponent<CharacterController2D>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _orinalColliderSize = _capsuleCollider.size;
        direction = 1;



    }





    // Update is called once per frame
    void Update()
    {

        if (_dashTimer > 0)
        {
            _dashTimer -= Time.deltaTime;
        }

        ApplyDeadZones();

        ProcessHorizontalMovement();

        if (_startJumping)
            _jumpBufferCounter = profile.jumpBufferTime;
        else
            _jumpBufferCounter -= Time.deltaTime;

        // Debug.Log(_moveDirection.y);

        if (_characterController.Below)//On the ground
        {
            OnGround();

        }

        else if (_characterController.InAirEffctor)
        {
            InAirEffector();
        }

        else if (_characterController.InWater)
        {
            InWater();
        }
        else //In the air
        {
            InAir();

        }


        if (!attacking && !HurtBoxDetction.instance.hit)
            _characterController.Move(_moveDirection * Time.deltaTime);

    }

    #region Movement
    void OnGround()
    {

        _coyoteTimeCounter = profile.coyoteTime;

        if (_characterController.AirEffectorType == AirEffectorType.Ladder)
        {
            InAirEffector();
            return;
        }

        if (_characterController.HitGrundThisFrames)
        {
            _tempVelocity = _moveDirection;
        }

        //clear any downard motion when ground
        _moveDirection.y = 0f;

        ClearAirAbilityFlags();

        Jump();

        DuckingAndCreeping();

        JumpPad();



    }

    void InAir()
    {
        if (_coyoteTimeCounter > 0)
        {
            Jump();
            _coyoteTimeCounter -= Time.deltaTime;
        }

        ClearGroundAbilityFlags();

        AirJump();

        WallRunn();

        GravityCalculations();

        if (isGliding && _input.y <= 0f)
        {
            isGliding = false;
        }
    }

    void InWater()
    {
        ClearGroundAbilityFlags();

        AirJump();

        if (_input.y != 0f && profile.canSwim && !_holdJump)
        {
            if (_input.y > 0 && !_characterController.IsSubmerged)
            {
                _moveDirection.y = 0f;
            }
            else
            {
                _moveDirection.y = (_input.y * profile.swimSpeed); //* Time.deltaTime;
            }


        }
        else if (_moveDirection.y < 0 && _input.y == 0f)
        {
            _moveDirection.y += 2f;
        }

        if (_characterController.IsSubmerged && profile.canSwim)
        {
            isSwinming = true;
        }
        else
        {
            isSwinming = false;
        }


    }

    void ProcessHorizontalMovement()
    {


        if (!_inAirControl || (isWallJumping && _input.x == 0))
            return;

        else
        {
            if (_input.x != 0)
            {
                _tempMoveSpeed = Mathf.Lerp(_tempMoveSpeed, _input.x, profile.accelerationSpeed);
            }
            else
            {
                _tempMoveSpeed = Mathf.Lerp(_tempMoveSpeed, 0f, profile.decelaraionSpeed);
            }
            _moveDirection.x = _tempMoveSpeed;

            if (_moveDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                direction = -1;
                //_facingRight = false;

            }
            else if (_moveDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                direction = 1;
                //_facingRight = true;
            }

            if (isDashing)
            {
                if (transform.rotation.y == 0)
                {
                    _moveDirection.x = profile.dashSpeed;

                }
                else
                {
                    _moveDirection.x = -profile.dashSpeed;
                }
                _moveDirection.y = 0;


            }
            else if (isCreeping)
            {
                _moveDirection.x *= profile.creepSpeed;
            }

            else
            {
                _moveDirection.x *= profile.walkSpeed;
            }

        }


    }

    void GravityCalculations()
    {
        //detects if something above player
        if (_moveDirection.y > 0f && _characterController.Above)
        {
            if (_characterController.CeillingType == GroundTypes.OneWayPlatform)
            {
                StartCoroutine(DisableOneWayPltform(false));
            }
            else
            {
                _moveDirection.y = 0f;
            }

        }

        //apply wall slide adjustment
        if (profile.canWallSlide && (_characterController.Left || _characterController.Rigth))
        {
            if (_characterController.HitGrundThisFrames)
            {
                _moveDirection.y = 0f;
            }

            if (_moveDirection.y < 0)
            {
                if (_characterController.Left && _characterController.LeftWallEfector)
                {
                    _moveDirection.y -= (profile.gravity * _characterController.LeftSlideModifier) * Time.deltaTime;

                }

                else if (_characterController.Rigth && _characterController.RightWallEfector)
                {
                    _moveDirection.y -= (profile.gravity * _characterController.LeftSlideModifier) * Time.deltaTime;
                }
                else
                {
                    _moveDirection.y -= (profile.gravity * profile.wallSlidAmount) * Time.deltaTime;
                }


            }
            else
            {
                _moveDirection.y -= profile.gravity * Time.deltaTime;
            }
        }
        else if (profile.canGlide && _input.y > 0f && _moveDirection.y < 0.2f)//glide adjustment
        {
            if (_currentGlideTime > 0f)
            {
                isGliding = true;

                if (_startGlide)
                {
                    _moveDirection.y = 0;
                    _startGlide = false;
                }

                _moveDirection.y -= profile.glideDescentAmount * Time.deltaTime;
                _currentGlideTime -= Time.deltaTime;
            }
            else
            {
                isGliding = false;
                _moveDirection.y -= profile.gravity * Time.deltaTime;
            }


        }
        // else if (canGrundSlam && !isPowerJump && _input.x < 0f && _moveDirection.y < 0f) 
        else if (isGroundSlam && !isPowerJump && _moveDirection.y < 0f)
        {
            OnStomp?.Invoke(this, EventArgs.Empty);
            _moveDirection.y = -profile.groundSlamSpeed;
        }
        else if (!isDashing) //regular gravity
        {
            _moveDirection.y -= profile.gravity * Time.deltaTime;
        }

    }

    void ClearAirAbilityFlags()
    {
        //clear flags for in air abilities
        isJumping = false;
        isDoubleJumping = false;
        isTripleJumping = false;
        isWallJumping = false;
        isGroundSlam = false;
        _currentGlideTime = profile.glideTime;
        _startGlide = true;
        isGliding = false;
        isAirAttack = false;
    }

    void ClearGroundAbilityFlags()
    {
        if ((isDucking || isCreeping) && _moveDirection.y > 0)
        {
            StartCoroutine("ClearDuckingState");
        }
        //clear powerJumpTimer
        _powerJumpingTimer = 0f;
    }

    void Jump()
    {
        //Jumping

        if (_jumpBufferCounter > 0f)
        {
            _startJumping = false;
            _coyoteTimeCounter = 0;


            if (profile.canPowerJump && isDucking &&
                _characterController.GroundTypes != GroundTypes.OneWayPlatform && (_powerJumpingTimer > profile.powerJumpWaitTime))
            {
                OnPowerJump?.Invoke(this, EventArgs.Empty);
                _moveDirection.y = profile.powerJumpSpeed;
                StartCoroutine("PowerJumpWaiter");
            }

            //check to see if we are one way platform
            else if (isDucking && _characterController.GroundTypes == GroundTypes.OneWayPlatform)
            {
                StartCoroutine(DisableOneWayPltform(true));
            }
            else
            {
                _moveDirection.y = profile.jumpSpeed;
            }

            isJumping = true;

            _characterController.DisableGroundCheck();
            //_characterController.ClearMovingPlatform();
            _ableToWallRun = true;
        }

    }

    void AirJump()
    {
        if (_realseJumping)
        {
            _realseJumping = false;

            if (_moveDirection.y > 0)
            {
                _moveDirection.y *= 0.5f;
            }
        }

        //pressed jump button in air
        if (_startJumping)
        {

            //triple jump

            if (profile.canTripleJump && (!_characterController.Left && !_characterController.Rigth))
            {
                if (isDoubleJumping && !isTripleJumping && isAirAttack == false)
                {

                    OnDoubleJump?.Invoke(this, EventArgs.Empty);

                    _moveDirection.y = profile.doubleJumpSpeed;
                    isTripleJumping = true;

                }
            }
            //double jump 
            if (profile.canDoubleJump && (!_characterController.Left && !_characterController.Rigth))
            {
                if (!isDoubleJumping)
                {

                    OnDoubleJump?.Invoke(this, EventArgs.Empty);
                    _moveDirection.y = profile.doubleJumpSpeed;
                    isDoubleJumping = true;
                }
            }



            //jump in water 

            if (_characterController.InWater)
            {
                isDoubleJumping = false;
                isTripleJumping = false;
                _moveDirection.y = profile.jumpSpeed;
            }

            //wall jump

            if (profile.canWallJump && (_characterController.Left || _characterController.Rigth))
            {
                if (_characterController.Left && _characterController.LeftWallEfector && !_characterController.IsLeftJumpable)
                {
                    return;
                }
                else if (_characterController.Rigth && _characterController.RightWallEfector && !_characterController.IsRighJumpable)
                {
                    return;
                }

                if (_moveDirection.x <= 0 && _characterController.Left)
                {
                    _moveDirection.x = profile.xWallJumpSpeed;
                    _moveDirection.y = profile.yWallJumpSpeed;
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else if (_moveDirection.x >= 0 && _characterController.Rigth)
                {

                    _moveDirection.x = -profile.xWallJumpSpeed;
                    _moveDirection.y = profile.yWallJumpSpeed;
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }

                //sWallJumping = true;
                StartCoroutine("WallJumpWaiter");


                if (profile.canJumpAfeterWallJump)
                {
                    isDoubleJumping = false;
                    isTripleJumping = false;

                }
            }




            _startJumping = false;
        }
    }

    void InAirEffector()
    {
        if (_startJumping)
        {
            _characterController.DeactivateAirEffector();
            Jump();
        }


        //process movement when on ladder
        if (_characterController.AirEffectorType == AirEffectorType.Ladder)
        {
            if (_input.y > 0f)
            {
                _moveDirection.y = _characterController.AirEffectorSpeed;
                Debug.Log("Caralho porra sube");
            }
            else if (_input.y < 0f)
            {
                _moveDirection.y = -_characterController.AirEffectorSpeed;
                Debug.Log("Caralho porra desce");

            }
            else
            {
                _moveDirection.y = 0f;
            }
        }

        //process movement when in tactor beam
        if (_characterController.AirEffectorType == AirEffectorType.TractorBeam)
        {
            if (_moveDirection.y != 0f)
            {
                _moveDirection.y = Mathf.Lerp(_moveDirection.y, 0f, Time.deltaTime * 4f);
            }
        }

        //process movement when in tactor beam
        if (_characterController.AirEffectorType == AirEffectorType.TractorBeam)
        {
            if (_moveDirection.y != 0f)
            {
                _moveDirection.y = Mathf.Lerp(_moveDirection.y, 0f, Time.deltaTime * 4f);
            }
        }

        //process movement when gliding in an updraft
        if (_characterController.AirEffectorType == AirEffectorType.Updraft)
        {
            if (_input.y < -0f)
            {
                isGliding = false;
            }

            if (isGliding)
            {
                _moveDirection.y = _characterController.AirEffectorSpeed;
            }
            else
            {
                InAir();
            }
        }
    }

    void JumpPad()
    {
        if (_characterController.GroundTypes == GroundTypes.JumpPad)
        {
            _jumpPadAmunt = _characterController.JumpPadAmount;


            //if downards velocity is greater then jump pad amount
            if (-_tempVelocity.y > _jumpPadAmunt)
            {
                _moveDirection.y = -_tempVelocity.y * 0.91f;
            }
            else
            {
                _moveDirection.y = _jumpPadAmunt;
            }





            //if holding jump bitton add a little each time we bounce 

            if (_holdJump)
            {
                _jumpPadAdjustment += _moveDirection.y * 0.1f;
                _moveDirection.y += _jumpPadAdjustment;
            }
            else
            {
                _jumpPadAdjustment = 0f;
            }

            //impose an upper limit to stop exponential jump height
            if (_moveDirection.y > _characterController.JumpPadUperLimit)
            {
                _moveDirection.y = _characterController.JumpPadUperLimit;
            }

        }
    }

    void WallRunn()
    {
        if (_characterController.HitWallthisFrames)
        {
            ClearAirAbilityFlags();
        }
        //WallRuning 
        if (profile.canWallRun && (_characterController.Left || _characterController.Rigth))
        {
            //isGliding = false;
            if (_characterController.Left && _characterController.LeftWallEfector && !_characterController.LeftIsRunnable)
            {
                return;
            }
            else if (_characterController.Rigth && _characterController.RightWallEfector && !_characterController.RightIsRunnable)
            {
                return;
            }

            if (_input.y > 0 && _ableToWallRun)
            {
                _moveDirection.y = profile.wallRunAmount;
                if (_characterController.Left && !isWallJumping)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (_characterController.Rigth && !isWallJumping)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                }

                StartCoroutine("WallRunning");
            }
        }
        else
        {
            if (profile.canMultipliWallRun)
            {
                StopCoroutine("WallRunning");
                _ableToWallRun = true;
                isWallRunning = false;
            }
        }

        //canGlideAfeterWallContact
        if ((_characterController.Left || _characterController.Rigth) && profile.canWallRun)
        {
            if (profile.canGlideAfeterWallContact)
            {
                _currentGlideTime = profile.glideTime;
            }
            else
            {
                _currentGlideTime = 0;
            }
        }
    }

    void DuckingAndCreeping()
    {
        //ducking and creeping
        if (_input.y < 0f)
        {
            if (!isDucking && !isCreeping)
            {
                _capsuleCollider.size = new Vector2(_capsuleCollider.size.x, _capsuleCollider.size.y / 2);
                _capsuleCollider.offset = new Vector2(0f, -_capsuleCollider.size.y / 2);
                //transform.position = new Vector2(transform.position.x, transform.position.y - (_orinalColliderSize.y / 4));
                isDucking = true;

                _spriteRenderer.sprite = Resources.Load<Sprite>("directionSpriteUp_crouching");
            }

            _powerJumpingTimer += Time.deltaTime;

        }
        else
        {
            if (isDucking || isCreeping)
            {
                RaycastHit2D hitCelling = Physics2D.CapsuleCast(_capsuleCollider.bounds.center,
                    transform.localScale, CapsuleDirection2D.Vertical, 0f, Vector2.up, _orinalColliderSize.y / 2,
                    _characterController.LayerMask);

                if (!hitCelling.collider)
                {
                    _capsuleCollider.size = _orinalColliderSize;
                    _capsuleCollider.offset = new Vector2(0f, 0f);
                    //transform.position = new Vector2(transform.position.x, transform.position.y + (_orinalColliderSize.y / 4));
                    _spriteRenderer.sprite = Resources.Load<Sprite>("directionSpriteUp");
                    isDucking = false;
                    isCreeping = false;
                }

            }

            _powerJumpingTimer = 0f;
        }

        if (isDucking && _moveDirection.x != 0)
        {
            isCreeping = true;

        }
        else
        {
            isCreeping = false;
        }
    }

    void ApplyDeadZones()
    {
        if (_input.x > -profile.deadZoneValue && _input.x < profile.deadZoneValue)
        {
            _input.x = 0f;
        }

        if (_input.y > -profile.deadZoneValue && _input.y < profile.deadZoneValue)
        {
            _input.y = 0f;
        }
    }
    #endregion

    #region Imput Methods
    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();

    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _startJumping = true;
            _realseJumping = false;
            _holdJump = true;
            isDashing = false;
            isAttacking = false;
            isAirAttack = false;
            attacking = false;
        }
        else if (context.canceled)
        {
            _realseJumping = true;
            _startJumping = false;
            _holdJump = false;
        }

        /*if (isDashing && _input.x != 0)
        {

            Debug.Log("Hey");
            profile.walkSpeed = profile.dashSpeed;

        }*/
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && _dashTimer <= 0)
        {
            if ((profile.canAirDash && !_characterController.Below)
                || (profile.canGroundDash && _characterController.Below))
            {
                StartCoroutine("Dash");
            }



        }

    }

    public void OnSlam(InputAction.CallbackContext context)
    {

        if (context.performed && _input.y < 0f)
        {
            if (profile.canGrundSlam)
            {
                isGroundSlam = true;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!isDucking) { if (_characterController.Below) { isAttacking = true; attacking = true; } }

            if (!attacking) { isAttacking = false; }

            if (!_characterController.Below)
            {
                isAirAttack = true;
                onAirAttack?.Invoke(this, EventArgs.Empty);
            }

        }
    }


    public void InputManger()
    {
        if (!canBeceiveInput)
        {
            canBeceiveInput = true;
        }
        else
        {
            canBeceiveInput = false;
        }
    }
    #endregion

    #region courotines
    //coroutines
    IEnumerator WallJumpWaiter()
    {
        isWallJumping = true;
        _inAirControl = false;
        yield return new WaitForSeconds(profile.wallJumpDelay);
        _inAirControl = true;
        // isWallJumping = false;
    }

    IEnumerator WallRunning()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(0.5f);
        isWallRunning = false;
        if (isWallJumping)
        {
            _ableToWallRun = false;
        }
    }

    IEnumerator ClearDuckingState()
    {
        yield return new WaitForSeconds(0.05f);
        RaycastHit2D hitCeilling = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, transform.localScale,
            CapsuleDirection2D.Vertical, 0f, Vector2.up, _orinalColliderSize.y / 2, _characterController.LayerMask);

        if (!hitCeilling.collider)
        {
            _capsuleCollider.size = _orinalColliderSize;
            _capsuleCollider.offset = new Vector2(0f, 0f);
            //transform.position = new Vector2(transform.position.x, transform.position.y + (_orinalColliderSize.y / 4));
            _spriteRenderer.sprite = Resources.Load<Sprite>("directionSpriteUp");
            isDucking = false;
            isCreeping = false;
        }
    }

    IEnumerator PowerJumpWaiter()
    {
        isPowerJump = true;
        _startJumping = false;
        yield return new WaitForSeconds(0.8f);
        isPowerJump = false;
    }

    IEnumerator Dash()
    {
        onStartDash?.Invoke(this, EventArgs.Empty);
        isDashing = true;
        yield return new WaitForSeconds(profile.dashTime);
        isDashing = false;
        _dashTimer = profile.dashCooldownTime;
    }


    IEnumerator DisableOneWayPltform(bool checkBelow)
    {
        bool orinalCanGorundSlam = profile.canGrundSlam;
        GameObject tempOneWayPlatform = null;

        if (checkBelow)
        {
            Vector2 raycastBellow = transform.position - new Vector3(0, _capsuleCollider.size.y, 0);
            Debug.DrawRay(raycastBellow, Vector2.down * _characterController.RaycastDistance, Color.red, 1);
            RaycastHit2D hit = Physics2D.Raycast(raycastBellow, Vector2.down,
                _characterController.RaycastDistance, _characterController.LayerMask);

            if (hit.collider)
            {
                tempOneWayPlatform = hit.collider.gameObject;
            }
        }
        else
        {
            Vector2 raycastAbove = transform.position + new Vector3(0, _capsuleCollider.size.y * 0.5f, 0);
            RaycastHit2D hit = Physics2D.Raycast(raycastAbove, Vector2.up,
                _characterController.RaycastDistance, _characterController.LayerMask);

            if (hit.collider)
            {
                tempOneWayPlatform = hit.collider.gameObject;
            }
        }

        if (tempOneWayPlatform)
        {
            tempOneWayPlatform.GetComponent<EdgeCollider2D>().enabled = false;
            profile.canGrundSlam = false;
        }

        yield return new WaitForSeconds(0.5f);

        if (tempOneWayPlatform)
        {
            tempOneWayPlatform.GetComponent<EdgeCollider2D>().enabled = true;
            profile.canGrundSlam = orinalCanGorundSlam;
        }
    }
    #endregion

}
