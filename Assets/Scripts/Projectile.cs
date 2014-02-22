using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public Vector3 facing = Vector3.zero;
	public int playerNumber = -1;
	public bool is_fired = false;
	public float fireTime;
	public float fireDuration = 1;
	public bool is_strong = false;
	public bool is_range = false;
	
	// Use this for initialization
	void Start () {
		playerNumber = transform.parent.GetComponent<Sumo>().myPlayer.GetIDNumber();
		gameObject.GetComponent<LineRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(facing.magnitude > 0){
			gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);
			gameObject.GetComponent<LineRenderer>().SetPosition(1, transform.position + facing * -1);
			if(!is_fired){
				fireTime = Time.time;
				is_fired = true;
			}
			rigidbody.velocity = facing * 10;
			if(fireTime + fireDuration / 2 < Time.time){
				Destroy(gameObject);
			}
		}
	}

	public void Range(){
		gameObject.GetComponent<LineRenderer>().enabled = true;
	}

	public void Strong(){
		for(int i = 0; i < transform.childCount; i++){
			transform.GetChild(i).renderer.enabled = true;
		}
		(collider as SphereCollider).radius = 1.1F;
	}

	public void Normal(){
		for(int i = 0; i < transform.childCount - 1; i++){
			transform.GetChild(i).renderer.enabled = false;
		}
		(collider as SphereCollider).radius = 0.65F;
		gameObject.GetComponent<LineRenderer>().enabled = false;
	}
}
