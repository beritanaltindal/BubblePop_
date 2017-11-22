using UnityEngine;
using System.Collections;

public class PointStar : MonoBehaviour, IPlayerRespawnListener {

    public GameObject Effect;
    public int PointsToAdd = 10;
    public AudioClip HitStarSound;
    public Animator Animator;
    private bool _isCollected;

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (_isCollected)
            return;
        if (other.GetComponent<Player>() == null)
            return;
        AudioSource.PlayClipAtPoint(HitStarSound, transform.position);

        GameManager.Instance.AddPoints(PointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);
        gameObject.SetActive(false);

        FloatingText.Show(string.Format("+{0}!", PointsToAdd), "PointStarText",
            new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));

        _isCollected = true;
        Animator.SetTrigger("Collect");
       
    }
    public void FinishAnimationEvent()
    {
        
    }

    public void OnPlayerRespawnInThisCheckpoint()
    {
        _isCollected = false;
        gameObject.SetActive(true);

    }
}
