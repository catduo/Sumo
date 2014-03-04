using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour, IJoviosControllerListener {

	public int playerNumber;
	public JoviosUserID myPlayer;
	public Color primary;
	public Color secondary;
	public string playerName;
	public string playerCharacter;
	public Transform checkMark;
	public Transform xMark;
	public Transform crown;
	public GameObject playerObject;
	public GameObject myPlayerObject;
	public TextMesh character;
	public TextMesh score;
	public bool is_ready = false;
	public int chosenArena;
		
	private Transform body;
	
	private Jovios jovios;
	
	bool IJoviosControllerListener.ButtonEventReceived(JoviosButtonEvent e){
		OnButton(e.GetResponse(), e.GetAction());
		return false;
	}

	void Update(){
		if(jovios.GetPlayer(myPlayer) != null){
			if(MenuManager.gameState == GameState.GameOn){
				transform.FindChild("Bracket").renderer.enabled = true;
			}
			else{
				transform.FindChild("Bracket").renderer.enabled = false;
			}
			if(jovios.GetPlayer(myPlayer).PlayerObjectCount() > 0){
				transform.FindChild("Immunity").renderer.enabled = false;
				transform.FindChild("Range").renderer.enabled = false;
				transform.FindChild("Rampage").renderer.enabled = false;
				transform.FindChild("Speed").renderer.enabled = false;
				transform.FindChild("Strength").renderer.enabled = false;
				if(jovios.GetPlayer(myPlayer).GetPlayerObject().GetComponent<Sumo>().activeBoost != BonusType.None){
					transform.FindChild(jovios.GetPlayer(myPlayer).GetPlayerObject().GetComponent<Sumo>().activeBoost.ToString()).renderer.enabled = true;
				}
			}
		}
	}
	
	public void SetMyPlayer (JoviosPlayer playerInfo){
		jovios = GameManager.jovios;
		myPlayer = playerInfo.GetUserID();
		playerNumber = playerInfo.GetPlayerNumber();
		if(!GameManager.score.ContainsKey(myPlayer.GetIDNumber())){
			GameManager.score.Add(myPlayer.GetIDNumber(), 0);
		}
		transform.parent = GameObject.Find ("PlayerStatus").transform;
		if(!is_ready){
			JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
			controllerStyle.AddButton1(new Vector2(0, 0.2F), new Vector2(2, 1.2F), "mc", "Build my Robot (robot appears on main screen)", "Join Game");
			jovios.SetControls(myPlayer, controllerStyle);
			transform.localRotation = Quaternion.identity;
			body = transform.FindChild("Primary");
			score = transform.FindChild("Score").GetComponent<TextMesh>();
			crown = transform.FindChild("Crown");
			xMark = transform.Find("X");
			checkMark = transform.Find("Check");
			xMark.renderer.enabled = true;
			checkMark.renderer.enabled = false;
			crown.renderer.enabled = false;
			score.text = "";
			jovios.AddControllerListener(this, myPlayer);
		}
		if(playerNumber < 4){
			transform.localPosition = new Vector3(-4.5F + (playerNumber -1) * 4, -1.75F, 0);
		}
		else{
			transform.localPosition = new Vector3(-4.5F + (playerNumber -5) * 4, -3F, 0);
		}
		transform.FindChild("Immunity").renderer.enabled = false;
		transform.FindChild("Range").renderer.enabled = false;
		transform.FindChild("Rampage").renderer.enabled = false;
		transform.FindChild("Speed").renderer.enabled = false;
		transform.FindChild("Strength").renderer.enabled = false;
		playerNumber = playerInfo.GetPlayerNumber();
		primary = playerInfo.GetColor("primary");
		secondary = playerInfo.GetColor("secondary");
		playerName = playerInfo.GetPlayerName();
		if(playerName.Length>0){
			playerCharacter = playerName[0].ToString();
		}
		else{
			playerCharacter = "";
		}
		body = transform.FindChild("Primary");
		character = transform.FindChild("Character").GetComponent<TextMesh>();
		character.color = secondary;
		character.text = playerCharacter;
		body.renderer.material.color = primary;
		if(jovios.GetPlayer(myPlayer).PlayerObjectCount() > 0){
			jovios.GetPlayer(myPlayer).GetPlayerObject().GetComponent<Sumo>().SetMyPlayer(playerInfo);
		}
	}
	
	private void OnButton(string button, string action){
		switch(button){
		case "left":
			switch(action){
			case "press":
				break;

			case "release":
				jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").SetDirection(Vector2.zero);
				break;
			}
			break;

		case "right":
			switch(action){
			case "press":
				break;
				
			case "release":
				jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("right").SetDirection(Vector2.zero);
				myPlayerObject.GetComponent<Sumo>().Attack();
				break;
			default:
				Debug.Log(action);
				break;
			}
			break;

		case "Join Game":
			//if(action == "release"){
				switch(MenuManager.gameState){
				case GameState.Countdown:
					Ready ();
					StartRound();
					break;
					
				case GameState.ChooseArena:
					Ready ();
					JoviosControllerStyle controllerStyle1 = new JoviosControllerStyle();
					controllerStyle1.AddJoystick(new Vector2(0.6F, 0.9F), new Vector2(1.2F, 1.8F), "bl", "left", "left");
					controllerStyle1.AddButton1(new Vector2 (1, 0), Vector2.one, "mc", "Select the level my robot is standing on", "Select Level");
					jovios.SetControls(myPlayer, controllerStyle1);
					break;
					
				case GameState.GameOn:
					Ready ();
					StartRound();
					break;
					
				case GameState.GameEnd:
					JoviosControllerStyle controllerStyle2 = new JoviosControllerStyle();
					controllerStyle2.AddButton1(Vector2.zero, new Vector2(2, 0.6F), "mc", "Play Again!", "Play Again!");
					jovios.SetControls(myPlayer, controllerStyle2);
					break;
					
				case GameState.Menu:
					break;
				}
			//}
			break;
			
		case "Play Again!":
			if(action == "press"){
				if(MenuManager.gameState != GameState.ChooseArena && MenuManager.gameState != GameState.Countdown){
					GameManager.EndRound();
				}
				Ready ();
				JoviosControllerStyle controllerStyle1 = new JoviosControllerStyle();
				controllerStyle1.AddJoystick(new Vector2(0.6F, 0.9F), new Vector2(1.2F, 1.8F), "bl", "left");
				controllerStyle1.AddButton1(new Vector2 (1, 0), Vector2.one, "mc", "Play!", "Select Level");
				jovios.SetControls(myPlayer, controllerStyle1);
			}
			break;
			
		case "Select Level":
			if(action == "press"){
				Debug.Log("select level");
				GameManager.ChooseArena(chosenArena);
			}
			break;
			
		default:
			Debug.Log (button);
			break;
		}
	}
	
	public void Ready(){
		xMark.renderer.enabled = false;
		checkMark.renderer.enabled = true;
		if(jovios.GetPlayer(myPlayer).PlayerObjectCount() == 0){
			GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
			newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / (playerNumber + 1) * jovios.GetPlayerCount());
			newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / (playerNumber + 1) * jovios.GetPlayerCount()));
			newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
			newPlayerObject.GetComponent<Sumo>().SetMyPlayer(jovios.GetPlayer(myPlayer));
			jovios.GetPlayer(myPlayer).AddPlayerObject(newPlayerObject);
			myPlayerObject = newPlayerObject;
		}
		is_ready = true;
	}
	
	public void StartRound(){
		score.text = "0";
		score.color = Color.white;
		xMark.renderer.enabled = false;
		checkMark.renderer.enabled = false;
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
		controllerStyle.AddJoystick(new Vector2(0.6F, 0.9F), new Vector2(1.2F, 1.8F), "bl", "left");
		controllerStyle.AddJoystick(new Vector2(-0.6F, 0.9F), new Vector2(1.2F, 1.8F), "br", "right");
		jovios.SetControls(myPlayer, controllerStyle);
	}
	
	public void Reset(int newPlayerNumber){
	}
}
