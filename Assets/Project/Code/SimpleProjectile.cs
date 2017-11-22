using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile, ITakeDamage
{
    public float TimeToLive = 10;
    public GameObject DestroyedEffect;
    public int Damage = 10;
    public AudioClip DestroySound;
    public void TakeDamage(int damage, GameObject instigator)
    {

    }
    public void Update()
    {
        if((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjecttile();
            return;
        }
        transform.Translate(Direction* ((Mathf.Abs(InitialVelocity.x)+Speed)*Time.deltaTime),Space.World);
    }

    private void DestroyProjecttile()
    {
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);
        if(DestroySound!=null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);
        Destroy(gameObject);
    }
    protected override void OnCollideOther(Collider2D other)
    {
        DestroyProjecttile();
    }
    protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {
        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjecttile();
    }
}
