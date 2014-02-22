using UnityEngine;
using System.Collections;

public enum GameState{
	GameOn,
	ChooseArena,
	Menu,
	Countdown,
	GameEnd,
	SuddenDeath
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
	private float musicVolume = 1;
	private float sfxVolume = 1;
	
	static public int roundDuration = 120;
	private float roundStart;
	
	private bool is_roundStarted = false;
	
	private Jovios jovios;

	public Texture2D menuButton;
	public Texture2D restartButton;
	public Texture2D instructionButton;
	public Texture2D resumeButton;
	public Texture2D menuSlider;
	public Texture2D menuBackground;
	public Texture2D menuX;
	public GUISkin defaultSkin;
	
	void Start(){
		//iPhoneSettings.screenCanDarken = false;
		mainCamera = GameObject.Find ("MainCamera").transform;
		jovios = GameManager.jovios;
		jovios.StartServer("Bots");
	}
	
	void OnGUI(){
		if(defaultSkin == null){
			defaultSkin = GUI.skin;
		}
		GUI.skin = defaultSkin;
		GUI.skin.font = font;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.box.normal.textColor = Color.white;
		GUI.skin.box.wordWrap = true;
		GUI.skin.label.wordWrap = true;
		GUI.skin.button.wordWrap = true;
		GUI.skin.label.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.box.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.button.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textArea.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textField.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		switch(gameState){
		case GameState.Menu:
			GUI.skin.box.active.background = menuBackground;
			GUI.skin.box.normal.background = menuBackground;
			GUI.skin.box.focused.background = menuBackground;
			GUI.Box(new Rect(Screen.width /10 ,Screen.height / 5,Screen.width * 0.8F,Screen.height * 0.75F), "");
			GUI.skin.horizontalSliderThumb.active.background = menuSlider;
			GUI.skin.horizontalSliderThumb.hover.background = menuSlider;
			GUI.skin.horizontalSliderThumb.normal.background = menuSlider;
			GUI.skin.horizontalSliderThumb.focused.background = menuSlider;
			GUI.skin.horizontalSlider.active.background = null;
			GUI.skin.horizontalSlider.hover.background = null;
			GUI.skin.horizontalSlider.normal.background = null;
			GUI.skin.horizontalSlider.focused.background = null;
			sfxVolume = GUI.HorizontalSlider(new Rect(Screen.width /1.84F ,Screen.height / 2.35F,Screen.width * 0.275F,40), sfxVolume, 0, 1);
			musicVolume = GUI.HorizontalSlider(new Rect(Screen.width /1.84F ,Screen.height / 1.89F,Screen.width * 0.275F,40), musicVolume, 0, 1);
			GUI.skin.button.active.background = restartButton;
			GUI.skin.button.hover.background = restartButton;
			GUI.skin.button.normal.background = restartButton;
			GUI.skin.button.focused.background = restartButton;
			if(GUI.Button(new Rect(Screen.width * 0.15F ,Screen.height * 0.75F,Screen.width * 0.16F,Screen.height * 0.1F), "")){
				GameManager.EndRound();
			}
			GUI.skin.button.active.background = instructionButton;
			GUI.skin.button.hover.background = instructionButton;
			GUI.skin.button.normal.background = instructionButton;
			GUI.skin.button.focused.background = instructionButton;
			if(GUI.Button(new Rect(Screen.width * 0.38F ,Screen.height * 0.75F,Screen.width * 0.2F,Screen.height * 0.1F), "")){
				
			}
			GUI.skin.button.active.background = resumeButton;
			GUI.skin.button.hover.background = resumeButton;
			GUI.skin.button.normal.background = resumeButton;
			GUI.skin.button.focused.background = resumeButton;
			if(GUI.Button(new Rect(Screen.width * 0.65F ,Screen.height * 0.75F,Screen.width * 0.18F,Screen.height * 0.1F), "")){
				gameState = previousGameState;
			}
			if(Screen.fullScreen){
				GUI.skin.button.active.background = null;
				GUI.skin.button.hover.background = menuX;
				GUI.skin.button.normal.background = menuX;
				GUI.skin.button.focused.background = null;
			}
			else{
				GUI.skin.button.active.background = menuX;
				GUI.skin.button.hover.background = null;
				GUI.skin.button.normal.background = null;
				GUI.skin.button.focused.background = menuX;

			}
			if(GUI.Button(new Rect(Screen.width /1.855F ,Screen.height / 1.61F,Screen.width * 0.025F,Screen.height * 0.04F), "")){
				Screen.fullScreen = !Screen.fullScreen;
			}
			break;
			
			
		case GameState.ChooseArena:
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(Screen.width *1/4,Screen.height*1/3,Screen.width/2,Screen.height/15), "Choose Arena");
			break;
			
			
		case GameState.Countdown:
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.box.fontSize *= 4;
			GUI.Box(new Rect(0,0,Screen.width,Screen.height), "\n\n" + timer.ToString());
			GUI.skin.box.fontSize /= 4;
			if(timer < 1){
				GameManager.StartRound();
				timer = roundDuration;
				gameState = GameState.GameOn;
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
			break;
			
			
		case GameState.GameOn:
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			GUI.Label(new Rect(Screen.width * 7/10 - GUI.skin.label.fontSize * 0.25F,Screen.height - GUI.skin.label.fontSize * 2.5F,Screen.width*3/10,Screen.height/8), "Time Left\n" + timer.ToString());
			if(timer < 1){
				if(GameManager.winner[1] > -1){
					gameState = GameState.SuddenDeath;
					for(int i = 0; i < jovios.GetPlayerCount(); i++){
						JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
						controllerStyle.SetBasicButtons("Sudden Death", new string[] {});
						jovios.SetControls(jovios.GetPlayer(i).GetUserID(), controllerStyle);
					}
					for(int i = 0; i < GameManager.winner.Length; i++){
						if(i < 0){
							break;
						}
						JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
						controllerStyle.AddAbsoluteJoystick("left", "Move Character by tapping and swiping");
						controllerStyle.AddAbsoluteJoystick("right", "Aim by Moving\nHold to Charge\nRelease to Fire");
						jovios.SetControls(jovios.GetPlayer(i).GetUserID(), controllerStyle);
					}
				}
				else{
					timer = countdownTime;
					gameState = GameState.GameEnd;
					for(int i = 0; i < jovios.GetPlayerCount(); i++){
						JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
						controllerStyle.SetBasicButtons("Would you like to play this game again?", new string[] {"Play Again!"});
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
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
			break;
			
			
		case GameState.GameEnd:
			if(GameManager.winner[0] > -1){
				GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "The Winner is " + jovios.GetPlayer(new JoviosUserID(GameManager.winner[0])).GetPlayerName());
			}
			break;

		case GameState.SuddenDeath:
			if(GameObject.Find ("PlayerObjects").transform.childCount < 2){
				timer = countdownTime;
				gameState = GameState.GameEnd;
				for(int i = 0; i < jovios.GetPlayerCount(); i++){
					JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
					controllerStyle.SetBasicButtons("Would you like to play this game again?", new string[] {"Play Again!"});
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
			break;
			
		default:
			Debug.Log ("Game State Broken");
			break;
		}
		if (is_loadingNewGame){
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Loading New Game");
		}
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.Label(new Rect(GUI.skin.label.fontSize * 0.25F,Screen.height - GUI.skin.label.fontSize * 2.5F,Screen.width/5,Screen.height/8), "Game Code\n" + jovios.GetGameName());
		GUI.skin.button.active.background = menuButton;
		GUI.skin.button.hover.background = menuButton;
		GUI.skin.button.normal.background = menuButton;
		GUI.skin.button.focused.background = menuButton;
		if (GUI.Button(new Rect(Screen.width/2-Screen.width/20,Screen.height - Screen.height/20,Screen.width/10,Screen.height/20), "")){
			if(gameState == GameState.Menu){
				gameState = previousGameState;
			}
			else{
				previousGameState = gameState;
				gameState = GameState.Menu;
			}
		}
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
