using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IJoviosPlayerListener {
	
	private Transform modifiers;
	public static Dictionary<JoviosUserID, int> score = new Dictionary<JoviosUserID, int>();
	public static int[] winner = new int[] {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
	public static GameObject chosenArena;
	public static GameObject[] arenas;
	public GameObject arena1;
	public GameObject arena2;
	public GameObject arena3;
	public GameObject arena4;
	public GameObject arenaSelection;	
	public static Transform bonusSpawners;
	public static float bonusSpawnTimer = 0;
	public GameObject statusObject;
	public static Jovios jovios;
	
	void Awake(){
		jovios = Jovios.Create();
	}
	
	// Use this for initialization
	void Start () {
		jovios.AddPlayerListener(this);
		arenas = new GameObject[] {arenaSelection, arena1, arena2, arena3, arena4}; 
		chosenArena = (GameObject) GameObject.Instantiate(arenaSelection, Vector3.zero, Quaternion.identity);
		modifiers = GameObject.Find ("Modifiers").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(bonusSpawnTimer + 8 < Time.time && MenuManager.gameState == GameState.GameOn){
			bonusSpawnTimer = Time.time;
			bonusSpawners.GetChild(Mathf.FloorToInt(bonusSpawners.childCount * Random.value)).FindChild("BonusSpawner").GetComponent<BonusSpawner>().bonusType = (BonusType)Mathf.FloorToInt(Random.value * 5);
		}
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		GameObject newStatusObject = (GameObject) GameObject.Instantiate(statusObject, Vector3.zero, Quaternion.identity);
		p.SetStatusObject(newStatusObject);
		newStatusObject.GetComponent<Status>().SetMyPlayer(p);
		return false;
	}
	bool IJoviosPlayerListener.PlayerUpdated(JoviosPlayer p){
		p.GetStatusObject().GetComponent<Status>().SetMyPlayer(p);
		return false;
	}
	bool IJoviosPlayerListener.PlayerDisconnected(JoviosPlayer p){
		return false;
	}
	
	public static void StartRound(){
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			if(jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().is_ready){
				jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().StartRound();
			}
		}
	}
	
	public static void EndRound(){
		Destroy(chosenArena);
		MenuManager.gameState = GameState.ChooseArena;
		chosenArena = (GameObject) GameObject.Instantiate(arenas[0], Vector3.zero, Quaternion.identity);
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().xMark.renderer.enabled = true;
			jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().checkMark.renderer.enabled = false;
		}
	}
	
	public static void ChooseArena(int selectedArena){
		if(selectedArena > 0){
			Destroy (chosenArena);
			chosenArena = (GameObject) GameObject.Instantiate(arenas[selectedArena], Vector3.zero, Quaternion.identity);
			MenuManager.lastTickTime = Time.time;
			MenuManager.gameState = GameState.Countdown;
			GameManager.StartRound();
			bonusSpawners = chosenArena.transform.FindChild("BonusSpawners");
			bonusSpawnTimer = Time.time;
		}
	}
}
