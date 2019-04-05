using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamControl : MonoBehaviour {

	public List<Transform> targets;
	public Vector3 offset;
	public float smoothing = 0.5f;
	private Vector3 velSmooth;

	public float minZoom = 5.0f;
	public float maxZoom = 15.0f;
	public float zoomLimiter = 0.5f;
	

	private Camera cam;

	void Start(){
		cam = GetComponent<Camera>();
	}

	// Update is called once per frame
	void LateUpdate () {
        //Make sure targets isnt empty
		if(targets.Count ==0)
			return;
		Move();
		Zoom();
	}

	void Move(){
		Vector3 centerPoint = getCenterPoint();

		Vector3 newCenter = centerPoint+ offset;
		transform.position= Vector3.SmoothDamp(transform.position, newCenter, ref velSmooth, smoothing);

		
	}
	void Zoom(){

		float newZoom =Mathf.Lerp(minZoom, maxZoom, (getMaxDistance()-5)* zoomLimiter);
		cam.orthographicSize= Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
	}



	float getMaxDistance(){

	    var bounds= new Bounds(targets[0].position, Vector3.zero);
		for (int i =0; i < targets.Count; i++)
			bounds.Encapsulate(targets[i].position);
		
		return bounds.size.magnitude;
	}

	Vector3 getCenterPoint(){

		var bounds= new Bounds(targets[0].position, Vector3.zero);
		for (int i =0; i < targets.Count; i++)
			bounds.Encapsulate(targets[i].position);
		
		return bounds.center;
	}
}
