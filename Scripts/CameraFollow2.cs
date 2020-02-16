using UnityEngine;
using System.Collections;

public class CameraFollow2 : MonoBehaviour {

	private Vector2 velocity;

	public float smoothTimeY;
	public float smoothTimeX;

	public GameObject player;

	public bool bounds;

	public Vector3 minCamPos;
	public Vector3 maxCamPos;
	
	void Start () 
	{

	}

	void FixedUpdate()
	{

		if (null == player) {
			player = GameObject.FindGameObjectWithTag ("Player");
		} else {
			float posX = Mathf.SmoothDamp (transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
			float posY = Mathf.SmoothDamp (transform.position.y, player.transform.position.y - 0.5f, ref velocity.y, smoothTimeY);

			transform.position = new Vector3 (posX, posY, transform.position.z);

			if (bounds) {
				transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minCamPos.x, maxCamPos.x),
			                                  Mathf.Clamp (transform.position.y, minCamPos.y, maxCamPos.y),
			                                  Mathf.Clamp (transform.position.z, minCamPos.z, maxCamPos.z));
			}
		}


	}
}
