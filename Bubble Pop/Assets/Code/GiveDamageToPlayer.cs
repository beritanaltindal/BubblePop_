using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour {

    public int DamageToGive = 25;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.TakeDamage(DamageToGive);

    }

}
