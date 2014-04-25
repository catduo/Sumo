using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ControlStyle{
	Cursor,
	Robot,
	PlayAgain
}

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
	public List<GameObject> controlStyleObjects = new List<GameObject>();
	public List<Texture2D> exportTextures = new List<Texture2D>();
	
	void Awake(){
		jovios = Jovios.Create();
		jovios.iconURL = "http://jovios.com/icons/botaboom.png";
		//jovios.assetBundleURL = "http://jovios.com/bundles/botaboom";
	}
	
	// Use this for initialization
	void Start () {
		jovios.StartServer(controlStyleObjects, exportTextures, "BotABoom");
		jovios.AddPlayerListener(this);
		arenas = new GameObject[] {arenaSelection, arena1, arena2, arena3, arena4, tieBreaker}; 
		chosenArena = (GameObject) GameObject.Instantiate(arenaSelection, Vector3.zero, Quaternion.identity);
		GameObject.Find("GameCode").GetComponent<UILabel>().text = jovios.gameCode.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
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
		if(winner.Contains(p.GetUserID().GetIDNumber())){
			winner.Remove(p.GetUserID().GetIDNumber());
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
		GameObject.Find ("PlayerStatus").transform.localPosition = new Vector3(0,5,10);
		GameObject.Find ("PlayerStatusAreas").transform.localPosition = new Vector3(0,5,10);
		Destroy(chosenArena);
		MenuManager.gameState = GameState.ChooseArena;
		//play again controls
		chosenArena = (GameObject) GameObject.Instantiate(arenas[0], Vector3.zero, Quaternion.identity);
		for(int i = 0; i < jovios.GetPlayerCount(); i++){
			jovios.GetPlayer(i).GetStatusObject().transform.FindChild("Immunity").renderer.enabled = false;
			jovios.GetPlayer(i).GetStatusObject().transform.FindChild("Range").renderer.enabled = false;
			jovios.GetPlayer(i).GetStatusObject().transform.FindChild("Rampage").renderer.enabled = false;
			jovios.GetPlayer(i).GetStatusObject().transform.FindChild("Speed").renderer.enabled = false;
			jovios.GetPlayer(i).GetStatusObject().transform.FindChild("Strength").renderer.enabled = false;
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
			for(int i = 0; i < GameObject.Find ("PlayerObjects").transform.childCount; i++){
				GameObject.Find ("PlayerObjects").transform.GetChild(i).position = GameObject.Find("PlayerSpawners").transform.GetChild(Mathf.FloorToInt(GameObject.Find("PlayerSpawners").transform.childCount * Random.value)).transform.position;
			}
			bonusSpawners = chosenArena.transform.FindChild("BonusSpawners");
			bonusSpawnTimer = Time.time;
		}
	}
	
	public static void SetVictoryPlayer (JoviosPlayer player){
		GameObject.Find ("PlayerStatus").transform.localPosition = new Vector3(1000,5,10);
		GameObject.Find ("PlayerStatusAreas").transform.localPosition = new Vector3(1000,5,10);
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
	}

	public void ExitGame(){
		Application.Quit();
	}
}
