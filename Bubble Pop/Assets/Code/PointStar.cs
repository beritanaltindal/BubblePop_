﻿using UnityEngine;
using System.Collections;

public class PointStar : MonoBehaviour {

    public GameObject Effect;
    public int PointsToAdd = 10;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;
        GameManager.Instance.AddPoints(PointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
