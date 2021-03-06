﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaSpawning : MonoBehaviour {
	
	public List<Vector3> bonusSpawnLocations;
	public GameObject bonusSpawner;
	public List<Vector3> bumperSpawnLocations;
	public GameObject bumper;
	public List<Vector3> playerSpawnLocations;
	public GameObject playerSpawner;

	// Use this for initialization
	void Start () {
		foreach (Vector3 position in bonusSpawnLocations){
			GameObject newBonusSpawner = (GameObject) GameObject.Instantiate(bonusSpawner, position, Quaternion.identity);
			newBonusSpawner.transform.parent = transform.FindChild("BonusSpawners").transform;
			newBonusSpawner.transform.localPosition = position;
			newBonusSpawner.transform.localScale = new Vector3(1,1,1);
		}
		foreach (Vector3 position in bumperSpawnLocations){
			GameObject newBumper = (GameObject) GameObject.Instantiate(bumper, position, Quaternion.identity);
			newBumper.transform.eulerAngles = new Vector3(90,0,0);
			newBumper.transform.parent = transform.FindChild("Bumpers");
			newBumper.transform.localPosition = position;
			newBumper.transform.localScale = new Vector3(1,0.5F,1);
		}
		foreach (Vector3 position in playerSpawnLocations){
			GameObject newBumper = (GameObject) GameObject.Instantiate(playerSpawner, position, Quaternion.identity);
			newBumper.transform.parent = transform.FindChild("PlayerSpawners");
			newBumper.transform.localPosition = position;
		}
		GameObject.Find ("CountdownCorner").GetComponent<Countdown>().StartCountdown(180);
	}
}
