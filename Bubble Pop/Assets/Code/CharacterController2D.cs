using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

    private Vector2 _velocity;
    public Vector2 Velocity { get { return _velocity; } }
    private BoxCollider2D _boxCollider;
    private Vector3 _localScale;
    private Vector3 _raycastBottomRight;//sağ alttaki ışının gönderildiği nokta
    private Vector3 _raycastBottomLeft;
    private Vector3 _raycastTopLeft;
    private Transform _transform;
    private const float SkinWidth = 0.02f;
    public LayerMask PlatformMask;
    private const int TotalHorizontalRays=8;
    private const int TotalVerticalRays = 4;
    private float _verticalDistanceBetweenRays;
    private float _horizontalDistanceBetweenRays;
    private ControllersParameters2D _OverrideParameters;
    private float _JumpIn;
    private static readonly float SlopeLimitTangant = Mathf.Tan(75f*Mathf.Deg2Rad);
    private Vector3 _activeGlobalPlatformPoint;
    private Vector3 _activeLocalPlatformPoint;
    private GameObject _lastStandingOn;
    public ControllersParameters2D DefaultParameters;
    public ControllersParameters2D Parameters { get { return _OverrideParameters ?? DefaultParameters; } }

    public ControllerState2D State { get; private set; }
    public GameObject StandingOn { get; private set; }
    public bool HandleCollisions { get; set; }
    public void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState2D();
        _boxCollider = GetComponent<BoxCollider2D>();
        _localScale = transform.localScale;
        _transform = transform;
        var ColliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y)-(2*SkinWidth); ;
        var ColliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2*SkinWidth); ;
        _verticalDistanceBetweenRays = ColliderHeight / (TotalHorizontalRays-1);
        _horizontalDistanceBetweenRays=ColliderWidth/(TotalVerticalRays-1);
        
    }
    public void Jump()
    {
        AddForce(new Vector2(0, Parameters.JumpMagnitude));
        _JumpIn = Parameters.JumpFrequency;
    }
    public void AddForce(Vector2 force)
    {
        _velocity += force;

    }
    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }
    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;

        _OverrideParameters = parameters.Parameters;
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;
    }
    public void LateUpdate()
        
    {
        _JumpIn -= Time.deltaTime;
        _velocity.y += Parameters. Gravity*Time.deltaTime;
        Move(Velocity * Time.deltaTime);

    }
    public bool CanJump{
        get {
            if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehaviour.CanJumpAnywhere)
                return _JumpIn <= 0;
            if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehaviour.CanJumpOnGround)
                return State.IsGrounded;
            return false;

        }
    }
    private void Move(Vector2 deltaMovement)
    {


        var wasGrounded = State.IsCollidingBelow;
        State.Reset();
        if (HandleCollisions)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.x < 0 && wasGrounded)
                HandleVerticalSlope(ref deltaMovement);

            if (Mathf.Abs(deltaMovement.x) > 0.001f)
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);
            CorrectHorizontalPlacament(ref deltaMovement, true);
            CorrectHorizontalPlacament(ref deltaMovement, false);

        }
        _transform.Translate(deltaMovement, Space.World);
        if (Time.deltaTime > 0)
            _velocity = deltaMovement / (Time.deltaTime);

        if (State.IsMovingUpSlope)
            _velocity.y = 0;

        if (StandingOn != null){
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
            if (_lastStandingOn !=StandingOn)
            {
                StandingOn.SendMessage("ControllerEnter2D" , this, SendMessageOptions.DontRequireReceiver);
                _lastStandingOn = StandingOn;

            }
        }



    }
    private void HandlePlatforms()
    {
      if(StandingOn != null)
        {
            var newGlobalPlatformsPoint= StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            var moveDistance = newGlobalPlatformsPoint - _activeGlobalPlatformPoint;
            if (moveDistance != Vector3.zero)
                transform.Translate(moveDistance, Space.World);
        }
        StandingOn = null;
    }
    private void CorrectHorizontalPlacament(ref Vector2 deltaMovement, bool isRight)
    {
        var halfWidth = (_boxCollider.size.x*_localScale.x)/2f;
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        if (isRight)
            rayOrigin.x += SkinWidth - halfWidth;
        else
            rayOrigin.x += -SkinWidth + halfWidth;
        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;
           
            for(var i=1; i<TotalHorizontalRays-1; i++)
        {
            var rayVektor = new Vector2(rayOrigin.x+deltaMovement.x, deltaMovement.y+rayOrigin.y+i*(_verticalDistanceBetweenRays));
            var raycastHit = Physics2D.Raycast(rayVektor, rayDirection, halfWidth, PlatformMask);
            if (!raycastHit)
                continue;

            offset = isRight ? (raycastHit.point.x - _transform.position.x) - halfWidth : (halfWidth - (_transform.position.x - raycastHit.point.x));
        }//for
        deltaMovement.x += offset;
    }//Correction
    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x)+SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        for (var i = 0; i < TotalHorizontalRays; i++) {
            var rayVektor = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

            var raycastHit = Physics2D.Raycast(rayVektor, rayDirection, rayDistance, PlatformMask);
            if (!raycastHit)
                continue;

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(raycastHit.normal, Vector2.up), isGoingRight))
                break;
            deltaMovement.x = raycastHit.point.x - rayVektor.x;
            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

        }//for
    }
    private bool HandleHorizontalSlope (ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angle) == 90)
            return false;

        if (angle > Parameters.SlopeLimit)
        {
            deltaMovement.x = 0;
            return true;
        }
        if (_JumpIn>0)
            return true;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad))* deltaMovement.x;

        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;

        return true;

    }
    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        var direction = -Vector2.up;
        var slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
        var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);
        var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        if (!raycastHit)
            return;

        var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x)==Mathf.Sign(deltaMovement.x);
        if (!isMovingDownSlope)
            return;
        var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        if (Mathf.Abs(angle)< .0001f)
        return;

        State.IsMovingDownSlope = true;
        State.SlopeAngle = angle;
        deltaMovement.y = raycastHit.point.y-slopeRayVector.y;


    }
    private void MoveVertically(ref Vector2 deltaMovement)
    {
        var isGoingUp =deltaMovement.y>0;
        var rayDistance = Mathf.Abs(deltaMovement.y)+SkinWidth;
        var rayDirection = isGoingUp? Vector2.up : -Vector2.up;
        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
        rayOrigin.x += deltaMovement.x;

        for (var i=0; i<TotalVerticalRays; i++)
        {
            var rayVektor = new Vector2(rayOrigin.x+(i*_horizontalDistanceBetweenRays), rayOrigin.y);
            var raycastHit = Physics2D.Raycast(rayVektor, rayDirection, rayDistance, PlatformMask); //ışınların çıkış vektörü, hangi yöne, ne kadar uzaklığa, hangi layer tabakasındaki objelere çarpması isteniyor bilgisi
            if (!raycastHit)
                continue;
            if (!isGoingUp)
            {
                StandingOn = raycastHit.collider.gameObject;
               
            }

            deltaMovement.y = raycastHit.point.y - rayVektor.y;//çarpma noktasından çarpma noktasına olan mesafe
            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }
            if (!isGoingUp && deltaMovement.y > 0.001f)
                State.IsCollidingBelow = true;
        }//for
    }
   private void CalculateRayOrigins()
    {
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x-size.x+SkinWidth, center.y-size.y+SkinWidth);
        _raycastTopLeft = _transform.position + new Vector3(center.x-size.x+SkinWidth, center.y+size.y-SkinWidth);
    }
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }
}
