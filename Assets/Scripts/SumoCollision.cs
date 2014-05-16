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
	
	public AudioClip shotHit;
	public AudioClip powerupGained;
	public AudioClip invincibility;
	public AudioClip rampaging;
	public AudioClip bumper;
	public AudioClip death;
	public AudioClip newRobot;
	
	void Start(){
		jovios = GameManager.jovios;
		playerNumber = transform.parent.GetComponent<Sumo>().playerNumber;
	}
	
	void Update(){
		if(is_ringOut){
			if(ringOutDuration + ringOutTime < Time.time){
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = newRobot;
				GetComponent<AudioSource>().Play();
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
		case "Rampage":
			lastPlayerHit = collision.transform.parent.GetComponent<Sumo>().myPlayer.GetIDNumber();
			rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
			break;
		case "Hand":
			if(collision.transform.GetComponent<Projectile>().playerNumber != transform.parent.GetComponent<Sumo>().myPlayer.GetIDNumber()){
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = shotHit;
				GetComponent<AudioSource>().Play();
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
	
	void OnTriggerEnter(Collider other){
		if(transform.parent != other.transform.parent){
			switch(other.transform.name){
			case "Bounds":
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = death;
				GetComponent<AudioSource>().Play();
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
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = bumper;
				GetComponent<AudioSource>().Play();
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 15 / transform.parent.GetComponent<Sumo>().defense);
				break;
			case "BonusBox":
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = powerupGained;
				GetComponent<AudioSource>().Play();
				transform.parent.GetComponent<Sumo>().is_boostActive = true;
				transform.parent.GetComponent<Sumo>().activeBoost = other.transform.parent.GetComponent<BonusSpawner>().bonusType;
				switch(other.transform.parent.GetComponent<BonusSpawner>().bonusType){
				case BonusType.Immunity:
					GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
					GetComponent<AudioSource>().clip = invincibility;
					GetComponent<AudioSource>().Play();
					break;

				case BonusType.Rampage:
					GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
					GetComponent<AudioSource>().clip = rampaging;
					GetComponent<AudioSource>().Play();
					break;
				}
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
				GetComponent<AudioSource>().volume = MenuManager.sfxVolume;
				GetComponent<AudioSource>().clip = bumper;
				GetComponent<AudioSource>().Play();
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
