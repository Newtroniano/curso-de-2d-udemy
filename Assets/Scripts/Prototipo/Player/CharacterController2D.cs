using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;

public class CharacterController2D : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] float raycastDistance = 0.2f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] ControllerMoveType moveType = ControllerMoveType.nonPhysicsBased;
    [SerializeField] float slopeAngleLimit = 45f;
    [SerializeField] float downForceAdjustmente = 1.2f;

    [Header("Collision Flags")]
    [SerializeField] bool below;
    [SerializeField] bool left;
    [SerializeField] bool rigth;
    [SerializeField] bool above;
    [SerializeField] bool hitGrundThisFrames;
    [SerializeField] bool hitWallthisFrames;


    [Header("Collision Information")]
    [SerializeField] GroundTypes groundTypes;
    [SerializeField] WallType leftWallType;
    [SerializeField] bool leftIsRunnable;
    [SerializeField] bool isLeftJumpable;
    [SerializeField] float leftSlideModifier;
    [SerializeField] WallType rightWallType;
    [SerializeField] bool rightIsRunnable;
    [SerializeField] bool isRighJumpable;
    [SerializeField] float righSlideModifier;
    [SerializeField] GroundTypes ceillingType;
    [SerializeField] WallEffector leftWallEfector;
    [SerializeField] WallEffector rightWallEfector;
    [SerializeField] float jumpPadAmount;
    [SerializeField] float jumpPadUperLimit;

    [Header("Air Effector Information")]
    [SerializeField] bool inAirEffctor;
    [SerializeField] AirEffectorType airEffectorType;
    [SerializeField] float airEffectorSpeed;
    [SerializeField] Vector2 airEffectorDirection;

    [Header("Water Effector Information")]
    [SerializeField] bool inWater;
    [SerializeField] bool isSubmerged;






    Vector2 _moveAmount;
    Vector2 _currentPostion;
    Vector2 _lastPosition;
    Rigidbody2D _rigidbody;
    CapsuleCollider2D _capsuleCollider;
    Vector2[] _raycastPosition = new Vector2[3];
    RaycastHit2D[] _raycastHits = new RaycastHit2D[3];
    bool _disableGroundCheck;
    Vector2 _slopeNormal;
    float _slopeAngle;
    bool _inAirLastFrame;
    bool _noSlideCollisionLastFrame;
    Transform _tempMovingPlatform;
    Vector2 _movingPlatformVelocity;
    AirEffects _airEffector;


    #region properties
    public float RaycastDistance { get => raycastDistance; }
    public LayerMask LayerMask { get => layerMask; }
    public float SlopeAngleLimit { get => slopeAngleLimit; }
    public float DownForceAdjustmente { get => downForceAdjustmente; }
    public bool Below { get => below; }
    public bool Left { get => left; }
    public bool Rigth { get => rigth; }
    public bool Above { get => above; }
    public bool HitGrundThisFrames { get => hitGrundThisFrames; }
    public bool HitWallthisFrames { get => hitWallthisFrames; }
    public GroundTypes GroundTypes { get => groundTypes; }
    public WallType LeftWallType { get => leftWallType; }
    public bool LeftIsRunnable { get => leftIsRunnable; }
    public bool IsLeftJumpable { get => isLeftJumpable; }
    public float LeftSlideModifier { get => leftSlideModifier; }
    public WallType RightWallType { get => rightWallType; }
    public bool RightIsRunnable { get => rightIsRunnable; }
    public float RighSlideModifier { get => righSlideModifier; }
    public GroundTypes CeillingType { get => ceillingType; }
    public WallEffector LeftWallEfector { get => leftWallEfector; }
    public WallEffector RightWallEfector { get => rightWallEfector; }
    public float JumpPadAmount { get => jumpPadAmount; }
    public float JumpPadUperLimit { get => jumpPadUperLimit; }
    public bool InAirEffctor { get => inAirEffctor; }
    public AirEffectorType AirEffectorType { get => airEffectorType; }
    public float AirEffectorSpeed { get => airEffectorSpeed; }
    public Vector2 AirEffectorDirection { get => airEffectorDirection; }
    public bool InWater { get => inWater; }
    public bool IsSubmerged { get => isSubmerged; }
    public bool IsRighJumpable { get => isRighJumpable; }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _inAirLastFrame = !below;

        _noSlideCollisionLastFrame = (!rigth && !left);

        _lastPosition = _rigidbody.position;

        //slope adjustment
        if (_slopeAngle != 0 && below == true)
        {
            if ((_moveAmount.x > 0f && _slopeAngle > 0f) || (_moveAmount.x < 0f && _slopeAngle < 0f))
            {
                _moveAmount.y = -Mathf.Abs(Mathf.Tan(_slopeAngle * Mathf.Deg2Rad) * _moveAmount.x);
                _moveAmount.y *= downForceAdjustmente;
            }
        }

        //moving platform adjustment
        if (groundTypes == GroundTypes.MovingPlatform)
        {

            Vector2 moveAjudsment = MovingPlatformAdjust();


            //offset the player`s moviment on the X with moving platform velocity
            _moveAmount.x += MovingPlatformAdjust().x;

            //if platform is moving down
            if (moveAjudsment.y < 0f)
            {
                _moveAmount.y += moveAjudsment.y;
                //_moveAmount.y *= downForceAdjustmente;
            }


        }

        if (groundTypes == GroundTypes.CollapsablePlatform)
        {
            if (MovingPlatformAdjust().y < 0f)
            {
                _moveAmount.y += MovingPlatformAdjust().y;

                if(!_disableGroundCheck && below)
                  _moveAmount.y *= downForceAdjustmente * 4;
            }
        }


        //tractor beam adjustment
        if (_airEffector && airEffectorType == AirEffectorType.TractorBeam)
        {
            Vector2 aireEffectorVector = airEffectorDirection * airEffectorSpeed;
            _moveAmount = Vector2.Lerp(_moveAmount, aireEffectorVector, Time.deltaTime);
        }

        //move  character controller
        if (moveType.Equals(ControllerMoveType.nonPhysicsBased))
        {
            _currentPostion = _lastPosition + _moveAmount;
            _rigidbody.MovePosition(_currentPostion);
        }
        else if (moveType.Equals(ControllerMoveType.physicBased))
        {
            if (_rigidbody.velocity.magnitude < 10f)
            {
                _rigidbody.AddForce(_moveAmount * 300f);
            }

        }

        _moveAmount = Vector2.zero;


        if (!_disableGroundCheck)
        {
            CheckGrounded();

        }
        CheckOtherCollision();

        if (below && _inAirLastFrame)
        {
            hitGrundThisFrames = true;
        }
        else
        {
            hitGrundThisFrames = false;
        }

        if ((rigth || left) && _noSlideCollisionLastFrame)
        {
            hitWallthisFrames = true;
        }
        else
        {
            hitWallthisFrames = false;
        }
    }

    public void Move(Vector2 movement)
    {
        _moveAmount += movement;
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, CapsuleDirection2D.Vertical,
            0f, Vector2.down, raycastDistance, layerMask);


        if (hit.collider)
        {
            groundTypes = DetermineGrpundType(hit.collider);

            _slopeNormal = hit.normal;
            _slopeAngle = Vector2.SignedAngle(_slopeNormal, Vector2.up);

            if (_slopeAngle > slopeAngleLimit || _slopeAngle < -slopeAngleLimit)
            {
                below = false;
            }
            else
            {
                below = true;
            }

            if (groundTypes == GroundTypes.JumpPad)
            {
                JumpPad jumpPad = hit.collider.GetComponent<JumpPad>();
                jumpPadAmount = jumpPad.jumpPadAmount;
                jumpPadUperLimit = jumpPad.jumpPadUpperLimit;
            }



        }

        else
        {
            groundTypes = GroundTypes.None;
            below = false;

            if (_tempMovingPlatform)
            {
                _tempMovingPlatform = null;
            }
        }
    }

    private void CheckOtherCollision()
    {
        //check left 
        Vector2 raycastLeftOrigin = _rigidbody.position - new Vector2(_capsuleCollider.size.x / 2, 0);

        Vector2 raycaseUpperLeft = raycastLeftOrigin + (Vector2.up * _capsuleCollider.size.y * 0.35f);
        Vector2 raycaseLowerLeft = raycastLeftOrigin + (Vector2.down * _capsuleCollider.size.y * 0.35f);

        Debug.DrawRay(raycaseUpperLeft, Vector2.left * raycastDistance, Color.green);
        Debug.DrawRay(raycaseLowerLeft, Vector2.left * raycastDistance, Color.green);

        RaycastHit2D hitUpperLeft = Physics2D.Raycast(raycaseUpperLeft, Vector2.left, raycastDistance, layerMask);
        RaycastHit2D hitLowerLeft = Physics2D.Raycast(raycaseUpperLeft, Vector2.left, raycastDistance, layerMask);

        if(hitUpperLeft.collider && hitLowerLeft.collider)
        {
            leftWallType = DetermineWallType(hitLowerLeft.collider);
            left = true;
            leftWallEfector = hitLowerLeft.collider.GetComponent<WallEffector>();

            if (leftWallEfector)
            {
                leftIsRunnable = leftWallEfector.isRunnable;
                isLeftJumpable = leftWallEfector.isJumpable;
                leftSlideModifier = leftWallEfector.wallSideAmount;
            }

        }
        else
        {
            leftWallType = WallType.None;
            left = false;

        }

        /*
        RaycastHit2D leftHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.75f, 0f, Vector2.left,
            raycastDistance * 2f, layerMask);


        if (leftHit.collider)
        {

            leftWallType = DetermineWallType(leftHit.collider);
            left = true;
            leftWallEfector = leftHit.collider.GetComponent<WallEffector>();

            if (leftWallEfector)
            {
                leftIsRunnable = leftWallEfector.isRunnable;
                isLeftJumpable = leftWallEfector.isJumpable;
                leftSlideModifier = leftWallEfector.wallSideAmount;
            }
        }
        else
        {
            leftWallType = WallType.None;
            left = false;
        }*/



        //check rigth
        Vector2 raycastRigthOrigin = _rigidbody.position + new Vector2(_capsuleCollider.size.x / 2, 0);

        Vector2 raycaseUpperRigth = raycastRigthOrigin + (Vector2.up * _capsuleCollider.size.y * 0.35f);
        Vector2 raycaseLowerRigth = raycastRigthOrigin + (Vector2.down * _capsuleCollider.size.y * 0.35f);

        Debug.DrawRay(raycaseUpperRigth, Vector2.right * raycastDistance, Color.green);
        Debug.DrawRay(raycaseLowerRigth, Vector2.right * raycastDistance, Color.green);

        RaycastHit2D hitUpperRigth = Physics2D.Raycast(raycaseUpperRigth, Vector2.right, raycastDistance, layerMask);
        RaycastHit2D hitLowerRigth = Physics2D.Raycast(raycaseLowerRigth, Vector2.right, raycastDistance, layerMask);

        if (hitUpperRigth.collider && hitLowerRigth.collider)
        {
            rightWallType = DetermineWallType(hitLowerRigth.collider);
            rigth = true;
            rightWallEfector = hitLowerRigth.collider.GetComponent<WallEffector>();

            if (rightWallEfector)
            {
                rightIsRunnable = rightWallEfector.isRunnable;
                isRighJumpable = rightWallEfector.isJumpable;
                righSlideModifier = rightWallEfector.wallSideAmount;
            }

        }
        else
        {
            rightWallType = WallType.None;
            rigth = false;

        }





        /*
        RaycastHit2D rigthHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.75f, 0f, Vector2.right,
        raycastDistance * 2f, layerMask);


        if (rigthHit.collider)
        {
            rightWallType = DetermineWallType(rigthHit.collider);
            rigth = true;

            rightWallEfector = rigthHit.collider.GetComponent<WallEffector>();

            if (rightWallEfector)
            {
                rightIsRunnable = rightWallEfector.isRunnable;
                isRighJumpable = rightWallEfector.isJumpable;
                righSlideModifier = rightWallEfector.wallSideAmount;
            }
        }
        else
        {
            rightWallType = WallType.None;
            rigth = false;
        }*/



        //check above 
        RaycastHit2D aboveHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, CapsuleDirection2D.Vertical,
              0f, Vector2.up, raycastDistance, layerMask);

        if (aboveHit.collider)
        {

            ceillingType = DetermineGrpundType(aboveHit.collider);
            above = true;
        }
        else
        {
            ceillingType = GroundTypes.None;
            above = false;
        }
    }

    /* private void CheckGrounded()
     {
         Vector2 raycastOrigin = _rigidbody.position - new Vector2(0, _capsuleCollider.size.y * 0.5f);

         _raycastPosition[0] = raycastOrigin + (Vector2.left * _capsuleCollider.size.x * 0.25f + Vector2.up * 0.1f);
         _raycastPosition[1] = raycastOrigin;
         _raycastPosition[2] = raycastOrigin + (Vector2.right * _capsuleCollider.size.x * 0.25f + Vector2.up * 0.1f);

         DrawDebugRays(Vector2.down, Color.green);

         int numberOfGroundHits = 0; 

         for (int i = 0; i < _raycastPosition.Length; i++)
         {
             RaycastHit2D hit = Physics2D.Raycast(_raycastPosition[i], Vector2.down, raycastDistance, layerMask);

             if (hit.collider)
             {
                 _raycastHits[i] = hit;
                 numberOfGroundHits++;
             }
         }

         if (numberOfGroundHits > 0)
         {
             if (_raycastHits[1].collider)
             {
                 groundTypes = DetermineGrpundType(_raycastHits[1].collider);
                 _slopeNormal = _raycastHits[1].normal;
                 _slopeAngle = Vector2.SignedAngle(_slopeNormal, Vector2.up);
             }
             else
             {
                 for (int i=0; i < _raycastHits.Length; i++)
                 {
                     if (_raycastHits[i].collider)
                     {
                         groundTypes = DetermineGrpundType(_raycastHits[i].collider);
                         _slopeNormal = _raycastHits[i].normal;
                         _slopeAngle = Vector2.SignedAngle(_slopeNormal, Vector2.up);
                     }
                 }
             }

             if(_slopeAngle > slopeAngleLimit || _slopeAngle < -slopeAngleLimit)
             {
                 below = false;
             }
             else
             {
                 below = true;
             }

         }
         else
         {
             groundTypes = GroundTypes.None;
             below = false;
         }
         System.Array.Clear(_raycastHits, 0, _raycastHits.Length);

     }*/

    private void DrawDebugRays(Vector2 direction, Color color)
    {
        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            Debug.DrawRay(_raycastPosition[i], direction * raycastDistance, color);
        }
    }


    public void DisableGroundCheck()
    {
        below = false;
        _disableGroundCheck = true;
        StartCoroutine("EnableGroundCheck");
    }


    IEnumerator EnableGroundCheck()
    {
        yield return new WaitForSeconds(0.1f);
        _disableGroundCheck = false;
    }

    private GroundTypes DetermineGrpundType(Collider2D collider)
    {
        if (collider.GetComponent<GroundEffector>())
        {
            GroundEffector groundEffector = collider.GetComponent<GroundEffector>();
            if (groundTypes == GroundTypes.MovingPlatform || groundTypes == GroundTypes.CollapsablePlatform)
            {
                //if (!_tempMovingPlatform)
               // {
                    _tempMovingPlatform = collider.transform;

                    if (groundTypes == GroundTypes.CollapsablePlatform)
                    {
                        if(_tempMovingPlatform.TryGetComponent<CollapsePlatform>(out CollapsePlatform cp)) { 
}                           cp.CollapsedPlatform();
                        
                    }


                //}
            }
            return groundEffector.groundTypes;
        }
        else
        {
            if (_tempMovingPlatform)
            {
                _tempMovingPlatform = null;
            }
            return GroundTypes.LevelGeometry;
        }
    }


    private WallType DetermineWallType(Collider2D collider)
    {
        if (collider.GetComponent<WallEffector>())
        {
            WallEffector wallEffector = collider.GetComponent<WallEffector>();
            return wallEffector.wallType;
        }
        else
        {
            return WallType.Normal;
        }
    }

    private Vector2 MovingPlatformAdjust()
    {
        if (_tempMovingPlatform && groundTypes == GroundTypes.MovingPlatform)
        {
           if(_tempMovingPlatform.TryGetComponent<MovingPlatform>(out MovingPlatform mp))
            {
                _movingPlatformVelocity = mp.difference;
                return _movingPlatformVelocity;
            }
            else
            {
                return Vector2.zero;
            }

        }

        else if (_tempMovingPlatform && groundTypes == GroundTypes.CollapsablePlatform)
        {
            if(_tempMovingPlatform.TryGetComponent<CollapsePlatform>(out CollapsePlatform cp))
            {
                _movingPlatformVelocity = cp.difference;
                return _movingPlatformVelocity;
            }
            else
            {
                return Vector2.zero;
            }
          
        }
        else
        {
            return Vector2.zero;
        }
    }

    /*
    public void ClearMovingPlatform()
    {
        if (_tempMovingPlatform)
        {
            _tempMovingPlatform = null;
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BuoyancyEffector2D>())
        {
            inWater = true;
            moveType = ControllerMoveType.physicBased;
        }

        if (collision.gameObject.GetComponent<AirEffects>())
        {
            inAirEffctor = true;
            _airEffector = collision.gameObject.GetComponent<AirEffects>();
            airEffectorSpeed = _airEffector.speed;
            airEffectorDirection = _airEffector.direction;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*
        if (collision.bounds.Contains(_capsuleCollider.bounds.min) &&
            collision.bounds.Contains(_capsuleCollider.bounds.max) &&
            collision.gameObject.GetComponent<BuoyancyEffector2D>())
        {
            isSubmerged = true;
        }
        else
        {
            isSubmerged = false;
        }*/

        if (collision.bounds.Contains(_capsuleCollider.bounds.center) &&
            collision.gameObject.GetComponent<BuoyancyEffector2D>())
        {
            isSubmerged = true;
        }
        else
        {
            isSubmerged = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BuoyancyEffector2D>())
        {
            _rigidbody.velocity = Vector2.zero;
            inWater = false;
            moveType = ControllerMoveType.nonPhysicsBased;
        }

        if (collision.gameObject.GetComponent<AirEffects>())
        {
            inAirEffctor = false;
            _airEffector.DeactiveEffector();
            _airEffector = null;
            airEffectorType = AirEffectorType.None;
            airEffectorSpeed = 0f;
            airEffectorDirection = Vector2.zero;

        }
    }

    public void DeactivateAirEffector()
    {
        if (_airEffector)
        {
            _airEffector.DeactiveEffector();
        }
    }

}
