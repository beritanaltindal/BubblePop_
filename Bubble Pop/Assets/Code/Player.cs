using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private CharacterController2D _controller;
    private float _normalizedHorizantalSpeed;
    private bool _isFacingRight;
    public float MaxSpeed=8;
    public float SpeedAccelerationOnGround=10f;
    public float SpeedAccelerationInAir = 5f;
    public bool IsDead { get; set; }
    public GameObject OuchEffect;
    public int MaxHealth = 100;
    public int Health { get; private set; }

    public void Awake()
    {
        Health = MaxHealth;
    }
    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _isFacingRight = transform.localScale.x > 0;
    }
    public void Update()
    {
        if(!IsDead)
        HandleInput();
        var movementFactor = _controller.State.IsGrounded ?SpeedAccelerationOnGround : SpeedAccelerationInAir;
        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x,  _normalizedHorizantalSpeed*MaxSpeed, Time.deltaTime*movementFactor));

    }
    public void Kill()
    {
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;
        _controller.SetForce(new Vector2(0,10));
        Health = 0;

    }
    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }
   public void RespawnAt(Transform spawnPoint)
    {
        if (!_isFacingRight)
            Flip();
        IsDead = false;
        GetComponent<Collider2D>().enabled = true;
        _controller.HandleCollisions = true;
        transform.position = spawnPoint.position;
        Health = MaxHealth;
       
    }
    public void TakeDamage(int damage)
    {

        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizantalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizantalSpeed = -1;
            if (_isFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizantalSpeed = 0;
        }
        if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();

        }
        
    }

    
}
