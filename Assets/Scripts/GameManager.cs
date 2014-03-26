﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IJoviosPlayerListener {

	public static Dictionary<int, float> score = new Dictionary<int, float>();
	public static List<int> winner = new List<int>();
	public static GameObject chosenArena;
	public static GameObject[] arenas;
	public GameObject arena1;
	public GameObject arena2;
	public GameObject arena3;
	public GameObject arena4;
	public GameObject arenaSelection;	
	public GameObject tieBreaker;	
	public static Transform bonusSpawners;
	public static float bonusSpawnTimer = 0;
	public GameObject statusObject;
	public static Jovios jovios;
	public Material selectedArenaTile;
	public Material deselectedArenaTile;
	
	void Awake(){
		jovios = Jovios.Create();
	}
	
	// Use this for initialization
	void Start () {
		jovios.AddPlayerListener(this);
		arenas = new GameObject[] {arenaSelection, arena1, arena2, arena3, arena4, tieBreaker}; 
		chosenArena = (GameObject) GameObject.Instantiate(arenaSelection, Vector3.zero, Quaternion.identity);
		GameObject.Find("GameCode").GetComponent<UILabel>().text = jovios.gameCode.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(bonusSpawnTimer + 8 < Time.time && MenuManager.gameState == GameState.GameOn && bonusSpawners.childCount > 0){
			bonusSpawnTimer = Time.time;
			bonusSpawners.GetChild(Mathf.FloorToInt(bonusSpawners.childCount * Random.value)).FindChild("BonusSpawner").GetComponent<BonusSpawner>().bonusType = (BonusType)Mathf.FloorToInt(Random.value * 5);
		}
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		GameObject newStatusObject = null;
		GameObject ps = GameObject.Find("PlayerStatus");
		for(int i = 0; i < ps.transform.childCount; i++){
			if(ps.transform.GetChild(i).GetComponent<Status>().myPlayer.GetIDNumber() == p.GetUserID().GetIDNumber()){
				newStatusObject = ps.transform.GetChild(i).gameObject;
				newStatusObject.GetComponent<Status>().is_ready = false;
			}
		}
		if(newStatusObject == null){
			newStatusObject = (GameObject) GameObject.Instantiate(statusObject, Vector3.zero, Quaternion.identity);
		}
		p.SetStatusObject(newStatusObject);
		newStatusObject.GetComponent<Status>().SetMyPlayer(p);
		return false;
	}
	bool IJoviosPlayerListener.PlayerUpdated(JoviosPlayer p){
		p.GetStatusObject().GetComponent<Status>().SetMyPlayer(p);
		return false;
	}
	bool IJoviosPlayerListener.PlayerDisconnected(JoviosPlayer p){
		Destroy(p.GetStatusObject());
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().SetMyPlayer(jovios.GetPlayer(i));
		}
		return false;
	}
	
	public static void StartRound(){
		winner = new List<int>();
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			if(jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().is_ready){
				jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().StartRound();
			}
		}
	}
	
	public void EndRound(){
		Destroy(chosenArena);
		MenuManager.gameState = GameState.ChooseArena;
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
		controllerStyle.AddButton1(new Vector2(0, 0.2F), new Vector2(2, 1.2F), "mc", "Play Again!", "Join Game");
		jovios.SetControls(controllerStyle);
		chosenArena = (GameObject) GameObject.Instantiate(arenas[0], Vector3.zero, Quaternion.identity);
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().xMark.renderer.enabled = true;
			jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().checkMark.renderer.enabled = false;
		}
		Dictionary<int, float> newScore = new Dictionary<int, float>();
		foreach (KeyValuePair<int, float> kvp in score){
			newScore.Add(kvp.Key, 0);
		}
		score = newScore;
	}
	
	public static void ChooseArena(int selectedArena){
		if(selectedArena > 0){
			Destroy (chosenArena);
			chosenArena = (GameObject) GameObject.Instantiate(arenas[selectedArena], Vector3.zero, Quaternion.identity);
			GameObject.Find ("Countdown").GetComponent<Countdown>().StartCountdown(3);
			MenuManager.gameState = GameState.Countdown;
			GameManager.StartRound();
			bonusSpawners = chosenArena.transform.FindChild("BonusSpawners");
			bonusSpawnTimer = Time.time;
		}
	}
	
	public static void SetVictoryPlayer (JoviosPlayer player){
		Debug.Log("start");
		GameObject.Find("Victory").GetComponent<UIPanel>().enabled = true;
		GameObject.Find("VictoryRobot").transform.position = Vector3.zero;
		GameObject.Find("VictoryName").GetComponent<UILabel>().text = player.GetPlayerName();
		Color primary = player.GetColor("primary");
		Color secondary = player.GetColor("secondary");
		Transform body = GameObject.Find("VictoryRobot").transform.FindChild("Body");
		Transform robot = body.FindChild("Robot1");
		for(int i = 0; i < robot.childCount; i++){
			if(robot.GetChild(i).name == "Sphere_008"){
				robot.GetChild(i).GetChild(0).renderer.material.color = primary;
				robot.GetChild(i).GetChild(1).renderer.material.color = primary;
				robot.GetChild(i).GetChild(2).renderer.material.color = primary;
			}
			else if(robot.GetChild(i).name == "Sphere_009"){
				robot.GetChild(i).GetChild(0).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(1).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(2).renderer.material.color = secondary;
			}
			else{
				robot.GetChild(i).renderer.material.color = Color.grey;
			}
		}
		Debug.Log("start");
	}
}
