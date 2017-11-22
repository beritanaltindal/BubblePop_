using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

    public Vector2 offset;
    public Transform Following;

    public void Update()
    {
        transform.position = Following.transform.position + (Vector3)offset;
    }
}
