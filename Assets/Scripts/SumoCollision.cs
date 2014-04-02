using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SumoCollision : MonoBehaviour {
	
	public int lastPlayerHit = -1;
	private float ringOutTime;
	private float ringOutDuration = 1.5F;
	public bool is_ringOut = false;
	public Vector3 startPosition;
	public int playerNumber;
	
	private Jovios jovios;
	
	void Start(){
		jovios = GameManager.jovios;
		playerNumber = transform.parent.GetComponent<Sumo>().playerNumber;
	}
	
	void Update(){
		if(is_ringOut){
			if(ringOutDuration + ringOutTime < Time.time){
				is_ringOut = false;
				transform.position = GameObject.Find("PlayerSpawners").transform.GetChild(Mathf.FloorToInt(GameObject.Find("PlayerSpawners").transform.childCount * Random.value)).transform.position;
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				if(MenuManager.gameState != GameState.SuddenDeath){
					transform.parent.GetComponent<Sumo>().boostDuration = 2;
					transform.parent.GetComponent<Sumo>().boostStart = Time.time;
					transform.parent.GetComponent<Sumo>().activeBoost = BonusType.Immunity;
					transform.parent.GetComponent<Sumo>().is_boostActive = true;
					lastPlayerHit = -1;
				}
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		switch(collision.transform.name){
		case "Arena1Platform":
			for(int i = 0; i < GameObject.Find("Arena1").transform.childCount; i ++){
				if(GameObject.Find("Arena1").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena1").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().selectedArenaTile;
				}
			}
			jovios.GetPlayer(playerNumber).GetStatusObject().GetComponent<Status>().chosenArena = 1;
			break;
		case "Arena2Platform":
			for(int i = 0; i < GameObject.Find("Arena2").transform.childCount; i ++){
				if(GameObject.Find("Arena2").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena2").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().selectedArenaTile;
				}
			}
			jovios.GetPlayer(playerNumber).GetStatusObject().GetComponent<Status>().chosenArena = 2;
			break;
		case "Arena3Platform":
			for(int i = 0; i < GameObject.Find("Arena3").transform.childCount; i ++){
				if(GameObject.Find("Arena3").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena3").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().selectedArenaTile;
				}
			}
			jovios.GetPlayer(playerNumber).GetStatusObject().GetComponent<Status>().chosenArena = 3;
			break;
		case "Arena4Platform":
			for(int i = 0; i < GameObject.Find("Arena4").transform.childCount; i ++){
				if(GameObject.Find("Arena4").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena4").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().selectedArenaTile;
				}
			}
			jovios.GetPlayer(playerNumber).GetStatusObject().GetComponent<Status>().chosenArena = 4;
			break;
		case "Platform":
			//collision.transform.renderer.enabled = true;
			jovios.GetPlayer(playerNumber).GetStatusObject().GetComponent<Status>().chosenArena = 0;
			break;
		case "Rampage":
			lastPlayerHit = collision.transform.parent.GetComponent<Sumo>().myPlayer.GetIDNumber();
			rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
			break;
		case "Hand":
			if(collision.transform.GetComponent<Projectile>().playerNumber != transform.parent.GetComponent<Sumo>().myPlayer.GetIDNumber()){
				lastPlayerHit = collision.transform.GetComponent<Projectile>().playerNumber;
				rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
				if(!collision.transform.GetComponent<Projectile>().is_fired){
					collision.transform.parent.GetComponent<Sumo>().attackPower = 0;
				}
				else{
					Destroy(collision.gameObject);
				}
			}
			break;
		default:
			break;
		}
	}
	
	void OnCollisionExit(Collision collision){
		switch(collision.transform.name){
		case "Arena1Platform":
			for(int i = 0; i < GameObject.Find("Arena1").transform.childCount; i ++){
				if(GameObject.Find("Arena1").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena1").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().deselectedArenaTile;
				}
			}
			break;
		case "Arena2Platform":
			for(int i = 0; i < GameObject.Find("Arena2").transform.childCount; i ++){
				if(GameObject.Find("Arena2").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena2").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().deselectedArenaTile;
				}
			}
			break;
		case "Arena3Platform":
			for(int i = 0; i < GameObject.Find("Arena3").transform.childCount; i ++){
				if(GameObject.Find("Arena3").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena3").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().deselectedArenaTile;
				}
			}
			collision.transform.renderer.enabled = false;
			break;
		case "Arena4Platform":
			for(int i = 0; i < GameObject.Find("Arena4").transform.childCount; i ++){
				if(GameObject.Find("Arena4").transform.GetChild(i).name == "Plane"){
					GameObject.Find("Arena4").transform.GetChild(i).renderer.material = GameObject.Find("MainCamera").GetComponent<GameManager>().deselectedArenaTile;
				}
			}
			break;
		default:
			break;
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(transform.parent != other.transform.parent){
			switch(other.transform.name){
			case "Bounds":
				is_ringOut = true;
				ringOutTime = Time.time;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x/4, rigidbody.velocity.y/4,0);
				transform.parent.GetComponent<Sumo>().attackPower = 0;
				if(lastPlayerHit > -1){
					GameManager.score[lastPlayerHit]++;
					if(GameManager.winner.Count>0){
						if(GameManager.score[lastPlayerHit] > GameManager.score[GameManager.winner[0]] || GameManager.winner[0] == lastPlayerHit){
							for(int i = 0; i < jovios.GetPlayerCount(); i++){
								jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().crown.renderer.enabled = false;
								jovios.GetPlayer(i).GetPlayerObject().GetComponent<Sumo>().crown.renderer.enabled = false;
							}
							GameManager.winner = new List<int>();
							GameManager.winner.Add(lastPlayerHit);
							jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetStatusObject().GetComponent<Status>().crown.renderer.enabled = true;
							jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetPlayerObject().GetComponent<Sumo>().crown.renderer.enabled = true;
						}
						else if(GameManager.score[lastPlayerHit] == GameManager.score[GameManager.winner[0]]){
							jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetStatusObject().GetComponent<Status>().crown.renderer.enabled = true;
							jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetPlayerObject().GetComponent<Sumo>().crown.renderer.enabled = true;
							GameManager.winner.Add(lastPlayerHit);
						}
					}
					else{
						GameManager.winner = new List<int>();
						GameManager.winner.Add(lastPlayerHit);
						jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetStatusObject().GetComponent<Status>().crown.renderer.enabled = true;
						jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetPlayerObject().GetComponent<Sumo>().crown.renderer.enabled = true;
					}
					jovios.GetPlayer(new JoviosUserID(lastPlayerHit)).GetStatusObject().GetComponent<Status>().score.text = GameManager.score[lastPlayerHit].ToString();
					lastPlayerHit = -1;
				}
				break;
			case "Bumper(Clone)":
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 15 / transform.parent.GetComponent<Sumo>().defense);
				break;
			case "BonusBox":
				transform.parent.GetComponent<Sumo>().is_boostActive = true;
				transform.parent.GetComponent<Sumo>().activeBoost = other.transform.parent.GetComponent<BonusSpawner>().bonusType;
				transform.parent.GetComponent<Sumo>().boostStart = Time.time;
				transform.parent.GetComponent<Sumo>().boostDuration = 5;
				other.transform.FindChild(other.transform.parent.GetComponent<BonusSpawner>().bonusType.ToString()).GetChild(0).renderer.enabled = false;
				other.transform.parent.GetComponent<BonusSpawner>().bonusType = BonusType.None;
				break;
			}
		}
	}
	
	void OnTriggerStay(Collider other){
		if(transform.parent != other.transform.parent){
			switch(other.transform.name){
			case "Bumper":
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 15 / transform.parent.GetComponent<Sumo>().defense);
				break;

			case "GreenCollider":
				if(!TutorialArena.playersInsideGreenArea.Contains(playerNumber)){
					TutorialArena.playersInsideGreenArea.Add(playerNumber);
				}
				break;
			}
		}
	}
}
