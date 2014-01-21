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
	public TextMesh character;
	public TextMesh score;
	public bool is_ready = false;
	public int chosenArena;
		
	private Transform body;
	
	private Jovios jovios;
	
	bool IJoviosControllerListener.ButtonEventReceived(JoviosButtonEvent e){
		OnButton(e.GetResponse());
		return false;
	}
	
	public void SetMyPlayer (JoviosPlayer playerInfo){
		if(!is_ready){
			jovios = GameManager.jovios;
			jovios.AddControllerListener(this, playerNumber: playerInfo.GetPlayerNumber());
			playerNumber = jovios.GetPlayerCount() - 1;
			transform.parent = GameObject.Find ("PlayerStatus").transform;
			if(playerNumber < 4){
				transform.localPosition = new Vector3(-4.5F + (playerNumber -1) * 4, -1.75F, 0);
			}
			else{
				transform.localPosition = new Vector3(-4.5F + (playerNumber -5) * 4, -3F, 0);
			}
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
			GameManager.score.Add(jovios.GetPlayer(playerNumber).GetUserID(), 0);
		}
		myPlayer = jovios.GetPlayer(playerNumber).GetUserID();
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
		if(jovios.GetPlayer(playerNumber).GetPlayerObject() != null){
			jovios.GetPlayer(playerNumber).GetPlayerObject().GetComponent<Sumo>().SetMyPlayer(playerInfo);
		}
		if(!is_ready){
			JoviosControllerStyle controllerStyle = new JoviosControllerStyle(JoviosControllerOverallStyle.BasicButtons, "Would you like to play?", new string[] {"Join Game"});
			jovios.SetControls(myPlayer, controllerStyle);
		}
	}
	
	private void OnButton(string button){
		switch(button){
		case "Join Game":
			switch(MenuManager.gameState){
			case GameState.Countdown:
				Ready ();
				StartRound();
				break;
			
			case GameState.ChooseArena:
				Ready ();
				JoviosControllerStyle controllerStyle1 = new JoviosControllerStyle(JoviosControllerAreaStyle.Joystick, "Move Character", JoviosControllerAreaStyle.Button1, "Select Level");
				jovios.SetControls(myPlayer, controllerStyle1);
				break;
				
			case GameState.GameOn:
				Ready ();
				StartRound();
				break;
				
			case GameState.GameEnd:
				JoviosControllerStyle controllerStyle2 = new JoviosControllerStyle(JoviosControllerOverallStyle.BasicButtons, "Would you like to play this game again?", new string[] {"Play Again!"});
				jovios.SetControls(myPlayer, controllerStyle2);
				break;
				
			case GameState.Menu:
				break;
			}
			break;
			
		case "Play Again!":
			if(MenuManager.gameState != GameState.ChooseArena && MenuManager.gameState != GameState.Countdown){
				GameManager.EndRound();
			}
			OnButton("Join Game");
			break;
			
		case "Select Level":
			GameManager.ChooseArena(chosenArena);
			break;
			
		default:
			Debug.Log (button);
			break;
		}
	}
	
	public void Ready(){
		xMark.renderer.enabled = false;
		checkMark.renderer.enabled = true;
		if(jovios.GetPlayer(playerNumber).GetPlayerObject() == null){
			GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
			newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / (playerNumber + 1) * jovios.GetPlayerCount());
			newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / (playerNumber + 1) * jovios.GetPlayerCount()));
			newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
			newPlayerObject.GetComponent<Sumo>().SetMyPlayer(jovios.GetPlayer(playerNumber));
			jovios.GetPlayer(playerNumber).SetPlayerObject(newPlayerObject);
		}
		is_ready = true;
	}
	
	public void StartRound(){
		score.text = "0";
		score.color = Color.white;
		xMark.renderer.enabled = false;
		checkMark.renderer.enabled = false;
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle(JoviosControllerAreaStyle.Joystick, "Move Character", JoviosControllerAreaStyle.Joystick, "Aim by Moving\nHold to Charge\nRelease to Fire");
		jovios.SetControls(myPlayer, controllerStyle);
	}
	
	public void Reset(int newPlayerNumber){
	}
}
