using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState{
	GameOn,
	ChooseArena,
	Countdown,
	GameEnd,
	SuddenDeath
}

public class MenuManager : MonoBehaviour {
	
	public static GameState gameState = GameState.ChooseArena;
	public static GameState previousGameState = GameState.ChooseArena;
	static public bool is_credits = false;
	static public bool is_loadingNewGame = false;
	static public bool is_menu = false;
	static public float lastTickTime;
	public static float musicVolume = 1;
	public static float sfxVolume = 1;

	private float roundStart;
	
	private bool is_roundStarted = false;
	
	private Jovios jovios;
	
	void Start(){
		//iPhoneSettings.screenCanDarken = false;
		jovios = GameManager.jovios;
		GameObject.Find ("Menu").transform.localPosition = 1000 * Vector3.one;
	}
	
	void Update(){
		switch(gameState){
			
		case GameState.ChooseArena:
			GameObject.Find("VictoryRobot").transform.position = new Vector3(0,50,0);
			GameObject.Find("Victory").GetComponent<UIPanel>().enabled = false;
			break;
			
		case GameState.Countdown:
			if(GameObject.Find ("Countdown").GetComponent<Countdown>().countTime < 1){
				gameState = GameState.GameOn;
			}
			break;
			
		case GameState.GameOn:
			if(GameObject.Find("CountdownCorner").GetComponent<Countdown>().countTime < 1){
				if(GameObject.Find("Tutorial") != null){
					GameObject.Find("Tutorial").GetComponent<UIPanel>().enabled = false;
				}
				if(GameManager.winner.Count > 1){
					Destroy (GameManager.chosenArena);
					GameManager.chosenArena = (GameObject) GameObject.Instantiate(GameManager.arenas[5], Vector3.zero, Quaternion.identity);
					GameObject.Find ("Countdown").GetComponent<Countdown>().StartCountdown(3);
					gameState = GameState.SuddenDeath;
					Transform po = GameObject.Find ("PlayerObjects").transform;
					for(int i = 0; i < po.childCount; i++){
						Destroy(po.GetChild(i).gameObject);
						jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().is_ready = false;
					}
					int j = 0;
					foreach(int i in GameManager.winner){
						Debug.Log (i);
						jovios.GetPlayer(new JoviosUserID(i)).GetStatusObject().GetComponent<Status>().BreakTie(j);
						jovios.SetControls(jovios.GetPlayer(new JoviosUserID(i)).GetUserID(), "Robot");
						j++;
					}
				}
				else{
					gameState = GameState.GameEnd;
					for(int i = 0; i < jovios.GetPlayerCount(); i++){
						jovios.SetControls(jovios.GetPlayer(i).GetUserID(), "PlayAgain");
					}
					Transform po = GameObject.Find ("PlayerObjects").transform;
					for(int i = 0; i < po.childCount; i++){
						Destroy(po.GetChild(i).gameObject);
						jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().is_ready = false;
					}
					Transform mo = GameObject.Find ("Modifiers").transform;
					for(int i = 0; i < mo.childCount; i++){
						Destroy(mo.GetChild(i).gameObject);
					}
				}
			}
			break;
			
		case GameState.GameEnd:
			Destroy(GameManager.chosenArena);
			if(GameManager.winner.Count > 0 && !GameObject.Find("SuddenDeath").GetComponent<UIPanel>().enabled){
				GameManager.SetVictoryPlayer(jovios.GetPlayer(new JoviosUserID(GameManager.winner[0])));
			}
			else{
				GameObject.Find ("NoVictory").GetComponent<UIPanel>().enabled = true;
			}
			break;

		case GameState.SuddenDeath:
			GameObject.Find("SuddenDeath").GetComponent<UIPanel>().enabled = true;	
			if(GameManager.winner.Count == 1){
				GameObject.Find("SuddenDeath").GetComponent<UIPanel>().enabled = false;
				for(int i = 0; i < jovios.GetPlayerCount(); i++){
					Debug.Log ("set cursor");
					jovios.SetControls(jovios.GetPlayer (i).GetUserID(), "PlayAgain");
				}
				Transform po = GameObject.Find ("PlayerObjects").transform;
				for(int i = 0; i < po.childCount; i++){
					Destroy(po.GetChild(i).gameObject);
					jovios.GetPlayer(i).GetStatusObject().GetComponent<Status>().is_ready = false;
				}
				Transform mo = GameObject.Find ("Modifiers").transform;
				for(int i = 0; i < mo.childCount; i++){
					Destroy(mo.GetChild(i).gameObject);
				}
				gameState = GameState.GameEnd;
			}
			break;
			
		default:
			Debug.Log ("Game State Broken");
			break;
		}
	}
	public void ToggleMenu(){
		if(is_menu){
			GameObject.Find ("Menu").transform.localPosition = 1000 * Vector3.one;
		}
		else{
			GameObject.Find ("Menu").transform.localPosition = Vector3.zero;
		}
		GameObject.Find ("Menu").GetComponent<UIPanel>().enabled = !GameObject.Find ("Menu").GetComponent<UIPanel>().enabled;
	}
	public void SFXVolume(){
		sfxVolume = GameObject.Find("SFXSlider").GetComponent<UISlider>().value;
	}
	public void MusicVolume(){
		musicVolume = GameObject.Find("MusicSlider").GetComponent<UISlider>().value;
	}
	public void ToggleFullScreen(){
		Screen.fullScreen = GameObject.Find ("Fullscreen").GetComponent<UIToggle>().value;
	}
		
	
	Rect WorldRect(Rect rect){
		Vector3 pos;
		Vector3 dim;
		pos = Camera.main.WorldToScreenPoint(new Vector2(rect.x, -rect.y));
		dim = Camera.main.WorldToScreenPoint(new Vector2(rect.xMax, -rect.yMax));
		rect = new Rect(pos.x, pos.y, dim.x - pos.x, pos.y - dim.y);
		return rect;
	}
}
