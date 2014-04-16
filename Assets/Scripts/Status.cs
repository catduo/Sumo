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
	public GameObject cursorObject;
	public GameObject myCursorObject;
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
			if(jovios.GetPlayer(myPlayer).PlayerObjectCount() > 0 && myPlayerObject != null){
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
			OnButton("Join Game", "release");
		}
		transform.position = GameObject.Find ("PlayerStatusArea" + (playerNumber + 1).ToString()).transform.position;
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
		if(myPlayerObject != null){
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
				if(jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left") != null){
					jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").SetDirection(Vector2.zero);
				}
				break;
			}
			break;

		case "right":
			switch(action){
			case "press":
				break;
				
			case "release":
				if(jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("right") != null){
					jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("right").SetDirection(Vector2.zero);
					myPlayerObject.GetComponent<Sumo>().Attack();
				}
				break;
			default:
				break;
			}
			break;

		case "Join Game":
			if(action == "release"){
				switch(MenuManager.gameState){
				case GameState.Countdown:
					Ready ();
					StartRound();
					break;
					
				case GameState.ChooseArena:
					jovios.SetControls(myPlayer, "Cursor");
					Ready ();
				break;
					
				case GameState.GameOn:
					Ready ();
					StartRound();
					break;
					
				case GameState.GameEnd:
					jovios.SetControls(myPlayer, "PlayAgain");
					break;
					
				case GameState.Menu:
					break;
				}
			}
			break;
			
		case "PlayAgain":
			if(action == "release"){
				if(MenuManager.gameState != GameState.ChooseArena && MenuManager.gameState != GameState.Countdown){
					Camera.main.transform.GetComponent<GameManager>().EndRound();
				}
				Ready ();
				jovios.SetControls(myPlayer, "Cursor");
			}
			break;
			
		case "Click":
			Debug.Log("click");
			if(action == "press"){
				CursorClick();
			}
			break;

		default:
			Debug.Log (button);
			break;
		}
	}
	
	public void Ready(){
		if(MenuManager.gameState == GameState.ChooseArena){
			SpawnCursor();
			checkMark.renderer.enabled = true;
		}
		else{
			SpawnRobot();
		}
		is_ready = true;
		xMark.renderer.enabled = false;
	}

	void SpawnCursor(){
		if(myPlayerObject != null){
			jovios.GetPlayer(myPlayer).RemovePlayerObject(myPlayerObject);
			Destroy (myPlayerObject);
		}
		if(jovios.GetPlayer(myPlayer).PlayerObjectCount() == 0){
			GameObject newPlayerObject = (GameObject) GameObject.Instantiate(cursorObject, Vector3.zero, Quaternion.identity);
			newPlayerObject.GetComponent<PlayerCursor>().SetMyPlayer(jovios.GetPlayer(myPlayer));
			jovios.GetPlayer(myPlayer).AddPlayerObject(newPlayerObject);
			myCursorObject = newPlayerObject;
		}
	}
	void CursorClick(){
		RaycastHit hitInfo;
		UICamera.Raycast(GameObject.Find ("Camera").camera.WorldToScreenPoint(myCursorObject.transform.position), out hitInfo);
		if(hitInfo.collider != null){
			hitInfo.collider.transform.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void SpawnRobot(){
		if(myCursorObject != null){
			jovios.GetPlayer(myPlayer).RemovePlayerObject(myCursorObject);
			Destroy (myCursorObject);
		}
		if(jovios.GetPlayer(myPlayer).PlayerObjectCount() == 0){
			GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
			newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / (playerNumber + 1) * jovios.GetPlayerCount());
			newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / (playerNumber + 1) * jovios.GetPlayerCount()));
			newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
			newPlayerObject.GetComponent<Sumo>().SetMyPlayer(jovios.GetPlayer(myPlayer));
			jovios.GetPlayer(myPlayer).AddPlayerObject(newPlayerObject);
			myPlayerObject = newPlayerObject;
		}
	}
	
	public void StartRound(){
		score.text = "0";
		score.color = Color.white;
		xMark.renderer.enabled = false;
		checkMark.renderer.enabled = false;
		jovios.SetControls(myPlayer, "Robot");
		Ready();
	}
	
	public void BreakTie(int position){
		GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, GameManager.chosenArena.transform.FindChild("PlayerSpawners").GetChild(position).position, Quaternion.identity);
		newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
		newPlayerObject.GetComponent<Sumo>().SetMyPlayer(jovios.GetPlayer(myPlayer));
		jovios.GetPlayer(myPlayer).AddPlayerObject(newPlayerObject);
		myPlayerObject = newPlayerObject;
	}
	
	public void Reset(int newPlayerNumber){
	}
}
