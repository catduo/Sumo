using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

//move connection items to a different class
//take in array of userids to change player ordering
//versioning with the controller so that it always works with the games (think about how it would work, not actually doing it)
//look into unit testing on unity
//more comments and grouping
//IJovios for API help (list of all public functions)
//Add Button Release
//UI for 2 button and directional swiping
//Add non-relative directional inputs
//Dictionary reset on player disconnect
//list of players instead of array Network and Jovios


public class JoviosUnityNetworking : MonoBehaviour {
	
	private static Jovios jovios;
	private string gameName;

	//this is called to setup the unity networking
	public static Jovios Create(){
		GameObject joviosGameObject = new GameObject();
		joviosGameObject.AddComponent<Jovios>();
		joviosGameObject.AddComponent<JoviosUnityNetworking>();
		joviosGameObject.AddComponent<NetworkView>();
		joviosGameObject.name = "JoviosObject";
		joviosGameObject.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		joviosGameObject.networkView.observed = joviosGameObject.GetComponent<JoviosUnityNetworking>();
		jovios = joviosGameObject.GetComponent<Jovios>();
		return joviosGameObject.GetComponent<Jovios>();
	}

	//this starts the unity server
	public void StartServer(string thisGameName){
		Application.runInBackground = true;
        Network.InitializeServer(32, 25008, !Network.HavePublicAddress());
		SetGameName(thisGameName);
    } 

	//this is the external IP gained from either the website or the unity system
	private string externalIP;
	IEnumerator GetIP(){
		WWW www = new WWW("http://checkip.dyndns.org");
        yield return www;
		if(www.error == null){
			externalIP=www.text;
			externalIP=externalIP.Substring(externalIP.IndexOf(":")+1);
			externalIP=externalIP.Substring(0,externalIP.IndexOf("<"));
		}
		else{
			externalIP = Network.player.externalIP;
		}
		MasterServer.RegisterHost(typeName, gameName + ";" + externalIP + ";" + "aaaa");
        WWWForm form = new WWWForm();
        form.AddField("action","create");
        form.AddField ("name",gameName);
        WWW post_req = new WWW("http://localhost/foo.php",form);
		if(Application.isWebPlayer){
			Application.ExternalCall("SetGameName", gameName);
		}
		Debug.Log ("game name: " + gameName + ", ipAdress: " + externalIP);
	}
	private WWW wwwData = null;
	private Dictionary<int, NetworkPlayer> networkPlayers = new Dictionary<int, NetworkPlayer>();
	private int networkPlayerCount = 0;
	private const string typeName = "Jovios";

	//this disconnects when the application quits
	void OnApplicationQuit(){
		Network.Disconnect();
	}

	//this sets the gamename
	public void SetGameName(string newGameName){
		if(newGameName.Length >= 4){
			gameName = newGameName;
		}
		else{
            gameName = Guid.NewGuid().ToString().Split('-')[1];
		}
		jovios.SetGameName(gameName);
		StartCoroutine("GetIP");
	}    

	//if this is in the web player it will download a new game
	public IEnumerator WaitForDownload(){    
        yield return wwwData;
        wwwData.LoadUnityWeb();  
    }
	[RPC] public void NewUrl(string url){
		networkView.RPC("NewGame", RPCMode.Others);
        wwwData = new WWW(url);
        StartCoroutine("WaitForDownload");
	}

	//this is the unity newtorkign connection and disconnection information
	void OnPlayerConnected(NetworkPlayer player){
		networkView.RPC ("SendPacket",player,"{\"packet\":{\"playerNumber\":"+networkPlayerCount.ToString()+"}}");
		networkPlayers.Add(networkPlayerCount, player);
		networkPlayerCount ++;
		//networkView.RPC ("PlayerObjectCreated", player, jovios.GetPlayerCount());
	}
	public void SetNetworkPlayer(int playerNumber){
		jovios.GetPlayer(playerNumber).SetNetworkPlayer(networkPlayers[playerNumber]);
	}   
	
	
	//these are the rpc calls for the unity networking
	[RPC] void GetDirection(int userID, float horizontal, float vertical, string side){
		//jovios.GetPlayer(new JoviosUserID(userID)).GetInput(side).SetDirection(new Vector2(horizontal, vertical));
	}
	[RPC] void GetAccelerometer(int userID, float gyroW, float gyroX, float gyroY, float gyroZ, float accX, float accY, float accZ){
		//jovios.GetPlayer(new JoviosUserID(userID)).GetInput("accelerometer").SetGyro(new Quaternion(-gyroX, -gyroY, gyroZ, gyroW));
		//jovios.GetPlayer(new JoviosUserID(userID)).GetInput("accelerometer").SetAcceleration(new Vector3(accX, accZ, accY));
	}
	[RPC] void GetTextResponse(int userID, string buttonPress, string side = "", string action = ""){
		//JoviosButtonEvent e = new JoviosButtonEvent(buttonPress, jovios.GetPlayer(new JoviosUserID(userID)).GetControllerStyle(), side, action);
		//foreach(IJoviosControllerListener listener in jovios.GetPlayer(new JoviosUserID(userID)).GetControllerListeners()){
		//	if(listener.ButtonEventReceived(e)){
		//		return;
		//	}
		//}
	}

	//here is the information returned for instantiating and updating the JoviosPlayer object
	[RPC] public void InstantiatePlayerObject(int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName, int userID){
		if(jovios.GetPlayer(playerNumber) == null){
			jovios.PlayerConnected(playerNumber, primaryR, primaryG, primaryB, secondaryR, secondaryG, secondaryB, playerName, userID);
		}
		else{
			jovios.PlayerUpdated(playerNumber, primaryR, primaryG, primaryB, secondaryR, secondaryG, secondaryB, playerName, userID);
		}
	}

	//blank rpc calls for all the calls going from the game to the controller, unity requries these blanks in order to call then via RPC
	[RPC] private void SentControls(int accelerometerSetting, string lControls, string lControlsResponse, string lControlsDescription, string rControls, string rControlsResponse, string rControlsDescription, string backgroundUrl){}
	[RPC] private void PlayerObjectCreated(int player){}
	[RPC] private void EndOfRound(int player){}
	[RPC] private void NewGame(){}
	[RPC] private void SentButtons (int accelerometerSetting, string type, string question, string actionWord, string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8, string backgroundUrl){}
	[RPC] private void SentArbitraryUIElement(int x, int y, int w, int h, string description, string response){}
}