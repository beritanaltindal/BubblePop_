using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {

    public PathDefinition path;
    public float speed = 5f;
    public float maxDistanceToGoal = 0.1f;

    public enum FollowType { MoveTowards, Lerp}
    public FollowType type = FollowType.MoveTowards;

    private IEnumerator<Transform> _currentPoint;

    public void Start()
    {
        if(path==null )
        {
            Debug.LogError("Path boş bırakıldı!", gameObject);
            return;
        }
        _currentPoint = path.GetPathEnumerator();
        _currentPoint.MoveNext();
         transform.position = _currentPoint.Current.position;
    }

    public void Update()
    {
        if (_currentPoint == null || _currentPoint.Current == null)
            return;
        if (type == FollowType.MoveTowards)
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);

        }
        else if (type == FollowType.Lerp)
        {
            transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
        }
        var distanceSquared =(transform.position-_currentPoint.Current.position).sqrMagnitude;
        if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal)
            _currentPoint.MoveNext();
    }
}
