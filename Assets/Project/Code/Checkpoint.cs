using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{

    public List<IPlayerRespawnListener> _listeners;

    public void Awake()
    {
        _listeners = new List<IPlayerRespawnListener>();
    }

    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);

        foreach (var listener in _listeners)
            listener.OnPlayerRespawnInThisCheckpoint();
    }
    public void AssignObjectToCheckPoint(IPlayerRespawnListener listener)
    {
        _listeners.Add(listener);
    }

    public void PlayerHitCheckPoint()
    {
        StartCoroutine(PlayerHitCheckPointCo(LevelManager.Instance.CurrentTimeBonus));
    }

    public IEnumerator PlayerHitCheckPointCo(int bonus)
    {
        FloatingText.Show("Checkpoint!", "CheckPointText", new CenteredTextPositioner(.5f));
        yield return new WaitForSeconds(0.5f);
        FloatingText.Show(string.Format("+ {0} time bonus!", bonus),"CheckPointText", new CenteredTextPositioner(.5f));
    }

}
