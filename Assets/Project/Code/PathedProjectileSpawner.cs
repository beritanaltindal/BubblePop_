using UnityEngine;
using System.Collections;

public class PathedProjectileSpawner : MonoBehaviour {
    public Transform Destination;
    public PathedProjectile Projectile;

    public float Speed = 6f;
    public float FireRate = 2f;
    public GameObject spawnEffect;
    public AudioClip SpawnProjectileSound;
    public Animator Animator;

    private float _nextShotInSeconds;
    public void Start()
    {
        _nextShotInSeconds = FireRate;
    }
    public void Update()
    {
        if ((_nextShotInSeconds -= Time.deltaTime) > 0)
            return;
        _nextShotInSeconds = FireRate;
        var projectile = (PathedProjectile)Instantiate(Projectile, transform.position, transform.rotation);

        projectile.Initialize(Destination, Speed);
        if (spawnEffect != null)
            Instantiate(spawnEffect, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);

        if (Animator != null)
            Animator.SetTrigger("Fire");
    }
    public void OnDrawGizmos()
    {
        if (Destination == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Destination.position);
    }

}
