using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ITakeDamage {

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
    public Projectile Projectile;
    public Transform ProjectileFireLocation;
    public GameObject FireProjectileEffect;
    public float FireRate = 0.5f;
    private float _CanFireIn;

    public AudioClip PlayerHitSound;
    public AudioClip PlayerShootSound;
    public AudioClip PlayerHealthSound;
    public Animator Animator;
    public void Awake()
    {
        Health = MaxHealth;
    }
    public void FinishLevel()
    {
        enabled = false;
        _controller.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _isFacingRight = transform.localScale.x > 0;
    }
    public void Update()
    {
        _CanFireIn -= Time.deltaTime;

        if(!IsDead)
        HandleInput();
        var movementFactor = _controller.State.IsGrounded ?SpeedAccelerationOnGround : SpeedAccelerationInAir;
        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x,  _normalizedHorizantalSpeed*MaxSpeed, Time.deltaTime*movementFactor));


        Animator.SetBool("IsGrounded", _controller.State.IsGrounded);
        Animator.SetBool("IsDead", IsDead);
        Animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x)/MaxSpeed);
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
    public void TakeDamage(int damage, GameObject instigator)
    {
        AudioSource.PlayClipAtPoint(PlayerHitSound, transform.position);


        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();

        FloatingText.Show(string.Format("-{0}", damage), "PlayerTakeDamageText",
            new FromWorldPointTextPositioner(Camera.main, transform.position, 3f, 60f));
    }
    public void GiveHealth(int health, GameObject instigator)
    {
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);

        FloatingText.Show(string.Format("+ {0}", health), "PlayerGotHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position,2f,60f));
        Health = Mathf.Min(Health + health, MaxHealth);
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

        if (Input.GetMouseButtonDown(0))
            FireProjectile();
    }

    private void FireProjectile()
    {

        if (_CanFireIn > 0)
            return;
        if (FireProjectileEffect != null)
        {
            var effect = (GameObject)Instantiate(FireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            effect.transform.parent = transform;
        }

      var projectile=(Projectile)  Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        var direction = _isFacingRight ? Vector2.right : -Vector2.left;

        projectile.Initialize(gameObject, direction, _controller.Velocity);

        _CanFireIn = FireRate;
        AudioSource.PlayClipAtPoint(PlayerShootSound, transform.position);

        Animator.SetTrigger("Fire");

    }

    
}
