using UnityEngine;
using System.Collections;

public enum GameState{
	GameOn,
	ChooseArena,
	Menu,
	Countdown,
	GameEnd
}

public class MenuManager : MonoBehaviour {
	
	public static GameState gameState = GameState.ChooseArena;
	public static GameState previousGameState = GameState.Menu;
	static public bool is_credits = false;
	static public bool is_loadingNewGame = false;
	static public int countdownTime = 3;
	static public int timer = countdownTime;
	static public float lastTickTime;
	public Font font;
	private Transform mainCamera;
	private string muteText = "Mute";
	
	static public int roundDuration = 180;
	private float roundStart;
	
	private bool is_roundStarted = false;
	
	private Jovios jovios;
	
	void Start(){
		//iPhoneSettings.screenCanDarken = false;
		mainCamera = GameObject.Find ("MainCamera").transform;
		jovios = GameManager.jovios;
		jovios.StartServer();
	}
	
	void OnGUI(){
		GUI.skin.font = font;
		GUI.skin.box.wordWrap = true;
		GUI.skin.button.wordWrap = true;
		GUI.skin.box.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.button.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textArea.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textField.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		switch(gameState){
		case GameState.Menu:
			GUI.Box(new Rect(Screen.width/2 - Screen.width/5,0,Screen.width/2.5F,Screen.height/20), "Menu");
			if(!Screen.fullScreen){
				if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10,Screen.width/5,Screen.height/5), "FullScreen")){
					Screen.fullScreen = !Screen.fullScreen;
				}
			}
			if(Application.isWebPlayer){
				if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10*4,Screen.width/5,Screen.height/5), "Reload Player")){
					jovios.NewUrl(Application.absoluteURL);
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/20,Screen.height - Screen.height/20,Screen.width/10,Screen.height/20), "Menu")){
				gameState = previousGameState;
			}
			break;
			
			
		case GameState.ChooseArena:
			GUI.Box(new Rect(Screen.width - Screen.width/2.5F,0,Screen.width/5,Screen.height/5), "Choose Arena");
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/20,Screen.height - Screen.height/20,Screen.width/10,Screen.height/20), "Menu")){
				gameState = GameState.Menu;
				previousGameState = GameState.ChooseArena;
			}
			break;
			
			
		case GameState.Countdown:
			GUI.Box(new Rect(Screen.width - Screen.width/2.5F,0,Screen.width/5,Screen.height/5), "Game Starts In");
			GUI.skin.box.fontSize *= 2;
			GUI.Box(new Rect(Screen.width - Screen.width/2.5F,Screen.height/5,Screen.width/5,Screen.height/5), timer.ToString());
			GUI.skin.box.fontSize /= 2;
			if(timer < 1){
				GameManager.StartRound();
				timer = roundDuration;
				gameState = GameState.GameOn;
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/20,Screen.height - Screen.height/20,Screen.width/10,Screen.height/20), "Menu")){
				gameState = GameState.Menu;
				previousGameState = GameState.Countdown;
			}
			break;
			
			
		case GameState.GameOn:
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Time Remaining");
			GUI.skin.box.fontSize *= 2;
			GUI.Box(new Rect(Screen.width - Screen.width/5,Screen.height/5,Screen.width/5,Screen.height/5), timer.ToString());
			GUI.skin.box.fontSize /= 2;
			if(timer < 1){
				timer = countdownTime;
				gameState = GameState.GameEnd;
				for(int i = 0; i < jovios.GetPlayerCount(); i++){
					JoviosControllerStyle controllerStyle = new JoviosControllerStyle(JoviosControllerOverallStyle.BasicButtons, "Would you like to play this game again?", new string[] {"Play Again!"});
					jovios.SetControls(jovios.GetPlayer(i).GetUserID(), controllerStyle);
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
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
			if(!Screen.fullScreen){
				if (GUI.Button(new Rect(Screen.width/10,0,Screen.width/5,Screen.height/20), "FullScreen")){
					Screen.fullScreen = !Screen.fullScreen;
				}
			}
			if(Application.isWebPlayer){
				if (GUI.Button(new Rect(Screen.width/2.5F,0,Screen.width/5,Screen.height/20), "Reload Player")){
					jovios.NewUrl(Application.absoluteURL);
				}
			}
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/20,Screen.height - Screen.height/20,Screen.width/10,Screen.height/20), "Menu")){
				gameState = GameState.Menu;
				previousGameState = GameState.GameOn;
			}
			break;
			
			
		case GameState.GameEnd:
			if(GameManager.winner[0] > -1){
				GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "The Winner is " + jovios.GetPlayer(GameManager.winner[0]).GetPlayerName());
			}
			break;
			
			
		default:
			Debug.Log ("Game State Broken");
			break;
		}
		if (is_loadingNewGame){
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Loading New Game");
		}
		GUI.Box(new Rect(0,Screen.height - Screen.height/8,Screen.width/5,Screen.height/8), "Game Code\n" + jovios.GetGameName());
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
