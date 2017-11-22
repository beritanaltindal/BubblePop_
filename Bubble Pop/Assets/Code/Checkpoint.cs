using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public void SpawnPlayer(Player player) {
        player.RespawnAt(transform);


    }
}
