using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public Transform player;
	public Vector3 _min, _max;
	public BoxCollider2D bounds;
	public Vector2 margin;
	public Vector2 smoothing;
	public float cameraHalfWidth;
    public float camX;
    public float camY;
	public void Start(){
        	_min = bounds.bounds.min;
        	_max = bounds.bounds.max;
        //_min = camX;
        //_max = camY;

	}
    public void Update()
    {
        var x = transform.position.x;
        var y = transform.position.y;

        x = player.position.x +camX;
        y = player.position.y +camY;
        
	    var cameraHalfWidth= GetComponent <Camera>().orthographicSize*((float)Screen.width/Screen.height);

        if(Mathf.Abs (x-player.position.x) > margin.x) 
		x = Mathf.Clamp (x, _min.x+cameraHalfWidth, _max.x-cameraHalfWidth) ;
        if(Mathf.Abs(y-player.position.y + 100.0f) >margin.y)
        y = Mathf.Clamp (y, _min.y+GetComponent <Camera>().orthographicSize, _max.y-GetComponent<Camera>().orthographicSize);

        transform.position = new Vector3(x, y, transform.position.z);

    }
}
