using UnityEngine;
using System.Collections;

public class SimpleEnemyAi : MonoBehaviour, ITakeDamage {

    public float Speed=9;
    public Projectile Projectile;
    public float FireRate = 1;
    public GameObject DestroyEffect;
    public AudioClip ShootSound;



    private CharacterController2D _controller;
    private Vector2 _direction;
    private float _CanFireIn;

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _direction = new Vector2(-1,0);
    }

    public void Update()
    {
        _controller.SetHorizontalForce(_direction.x * Speed);
        Debug.Log("sol:" + _controller.State.IsCollidingLeft + "--sag:" + _controller.State.IsCollidingRight);

    
        if ((_controller.State.IsCollidingLeft))
        {
            if (_direction.x < 0)
            {
                Debug.Log("B");
                _direction = -_direction;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            _controller.State.IsCollidingLeft = false;

            AudioSource.PlayClipAtPoint(ShootSound, transform.position);

        }
        if ((_controller.State.IsCollidingRight))
        {
            if (_direction.x > 0)
            {
                Debug.Log("A");
                _direction = -_direction;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                _controller.State.IsCollidingRight = false;
            }

        }
        if ((_CanFireIn -= Time.deltaTime) > 0)
            return;
       /* var raycast = Physics2D.Raycast(transform.position, _direction, 30, 1<<LayerMask.NameToLayer("Player"));
        if (!raycast)
            return;*/


        var projectile = (Projectile)Instantiate(Projectile, transform.position, transform.rotation);

        projectile.Initialize(gameObject, _direction, _controller.Velocity);

        _CanFireIn = FireRate;
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        Instantiate(DestroyEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
