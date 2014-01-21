using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class Jovios : MonoBehaviour {
	
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
        MasterServer.RegisterHost(typeName, gameName + externalIP);
        WWWForm form = new WWWForm();
        form.AddField("action","create");
        form.AddField ("name",gameName);
        WWW post_req = new WWW("http://localhost/foo.php",form);
		if(Application.isWebPlayer){
			Application.ExternalCall("SetGameName", gameName);
		}
		Debug.Log ("game name: " + gameName + ", ipAdress: " + externalIP);
	}
	
	private Dictionary<int, int> userIDToPlayerNumber = new Dictionary<int, int>();
	private const string typeName = "Jovios";
	private string gameName;
	public string GetGameName(){
		return gameName;
	}
	private WWW wwwData = null;
	private NetworkPlayer[] networkPlayers = new NetworkPlayer[0];
	
	
	//listening to when players connect, disconnect, and change their information
	private List<IJoviosPlayerListener> playerListeners = new List<IJoviosPlayerListener>();
	public void AddPlayerListener(IJoviosPlayerListener listener){
		playerListeners.Add(listener);
	}
	public void RemovePlayerListener(IJoviosPlayerListener listener){
		playerListeners.Remove(listener);
	}
	public void RemoveAllPlayerListeners(){
		playerListeners = new List<IJoviosPlayerListener>();
	}
	
	
	//listening to each player's controller
	public List<IJoviosControllerListener> GetControllerListeners(JoviosUserID jUID = null, int playerNumber = -1){
		if (playerNumber > -1){
			return GetPlayer(playerNumber).GetControllerListeners();
		}
		else{
			return GetPlayer(jUID).GetControllerListeners();
		}
	}
	public void AddControllerListener(IJoviosControllerListener listener, JoviosUserID jUID = null, int playerNumber = -1){
		if(jUID == null && playerNumber == -1){
			foreach(JoviosPlayer player in players){
				player.AddControllerListener(listener);
			}
		}
		else if (playerNumber > -1){
			GetPlayer(playerNumber).AddControllerListener(listener);
		}
		else{
			GetPlayer(jUID).AddControllerListener(listener);
		}
	}
	public void RemoveControllerListener(IJoviosControllerListener listener, JoviosUserID jUID = null, int playerNumber = -1){
		if(jUID == null && playerNumber == -1){
			foreach(JoviosPlayer player in players){
				player.RemoveControllerListener(listener);
			}
		}
		else if (playerNumber > -1){
			GetPlayer(playerNumber).RemoveControllerListener(listener);
		}
		else{
			GetPlayer(jUID).RemoveControllerListener(listener);
		}
	}
	public void RemoveAllControllerListeners(JoviosUserID jUID = null, int playerNumber = -1){
		if(jUID == null && playerNumber == -1){
			foreach(JoviosPlayer player in players){
				player.RemoveAllControllerListeners();
			}
		}
		else if (playerNumber > -1){
			GetPlayer(playerNumber).RemoveAllControllerListeners();
		}
		else{
			GetPlayer(jUID).RemoveAllControllerListeners();
		}
	}
	
	private JoviosPlayer[] players = new JoviosPlayer[0];
	public JoviosPlayer GetPlayer(int playerNumber){
		if(playerNumber < players.Length){
			return players[playerNumber];
		}
		else{
			return null;
		}
	}
	public JoviosPlayer GetPlayer(JoviosUserID jUID){
		if(userIDToPlayerNumber.ContainsKey(jUID.GetIDNumber())){
			return players[userIDToPlayerNumber[jUID.GetIDNumber()]];
		}
		else{
			return null;
		}
	}
	public int GetPlayerCount(){
		if(players != null){
			return players.Length;
		}
		else{
			return 0;
		}
	}
	
	private string version = "0.0.0";
	public string GetVersion(){
		return version;
	}
	[RPC] public void CheckVersion(string controllerVersion, int userID){
		if(controllerVersion == version){
			Debug.Log ("versions Match");
		}
		else if(int.Parse(version.Split('.')[0])<=int.Parse(controllerVersion.Split('.')[0]) && int.Parse(version.Split('.')[1])<=int.Parse(controllerVersion.Split('.')[1]) && int.Parse(version.Split('.')[2])<=int.Parse(controllerVersion.Split('.')[2])){
			Debug.Log ("controller more advanced version");
		}
		else{
			Debug.Log ("controller out of date");
		}
	}
	
	public static Jovios Create(){
		GameObject joviosGameObject = new GameObject();
		joviosGameObject.AddComponent<Jovios>();
		joviosGameObject.AddComponent<NetworkView>();
		joviosGameObject.name = "JoviosObject";
		joviosGameObject.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		joviosGameObject.networkView.observed = joviosGameObject.GetComponent<Jovios>();
		return joviosGameObject.GetComponent<Jovios>();
	}
	
	private Jovios(){
		
	}
	
    public void StartServer(int maxPlayers = 32, int portNumber = 25000){
		Application.runInBackground = true;
        Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		if(Application.isWebPlayer){
			Application.ExternalCall("GetGameName");
		}
		else{
			SetGameName("aaaa");
		}
    }    
	
    void OnServerInitialized(){
        Debug.Log("Server Initializied");
		//TCPGetMessage("serveron", Network.player.externalIP);
		//udp = new UdpClient();
		//udp.EnableBroadcast = true;
		//groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 15000);
		//IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
	}
	
	void OnApplicationQuit(){
		Network.Disconnect();
	}
	
	private void SetGameName(string newGameName){
		if(newGameName.Length == 4){
			gameName = newGameName;
		}
		else{
            gameName = Guid.NewGuid().ToString().Split('-')[1];
		}
		StartCoroutine("GetIP");
	}    
	
	public IEnumerator WaitForDownload(){    
        yield return wwwData;
        wwwData.LoadUnityWeb();  
    }
	[RPC] public void NewUrl(string url){
		networkView.RPC("NewGame", RPCMode.Others);
        wwwData = new WWW(url);
        StartCoroutine("WaitForDownload");
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		NetworkPlayer[] newPlayers = new NetworkPlayer[networkPlayers.Length + 1];
		for(int i = 0; i < networkPlayers.Length; i++){ 
			newPlayers[i] = networkPlayers[i];
		}
		newPlayers[players.Length] = player;
		networkPlayers = newPlayers;
		networkView.RPC ("PlayerObjectCreated", player, GetPlayerCount());
	}
	
	void OnPlayerDisconnected(NetworkPlayer player){
		JoviosPlayer[] newPlayers = new JoviosPlayer[players.Length - 1];
		NetworkPlayer[] newNetworkPlayers = new NetworkPlayer[players.Length - 1];
		int nextPlayerNum = 0;
		int disconnectedPlayer = 0;
		for(int i = 0; i < players.Length; i++){
			if(players[i].GetNetworkPlayer() == player){
				Destroy (players[i].GetPlayerObject());
				Destroy (players[i].GetStatusObject());
				disconnectedPlayer = i;
			}
			else{
				newNetworkPlayers[nextPlayerNum] = networkPlayers[i];
				newPlayers[nextPlayerNum] = players[i];
				userIDToPlayerNumber[players[i].GetUserID().GetIDNumber()] = nextPlayerNum;
				nextPlayerNum++;
			}
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerDisconnected(players[disconnectedPlayer])){
				break;
			}
		}
		networkPlayers = newNetworkPlayers;
		players = newPlayers;
		//Change
		for(int i = 0; i < players.Length; i++){
			players[i].GetStatusObject().SendMessage("Reset", i, SendMessageOptions.DontRequireReceiver);
		}
    	Network.RemoveRPCs(player);
    	Network.DestroyPlayerObjects(player);
    }
	
	
	
	[RPC] void GetDirection(int userID, float vertical, float horizontal, string side){
		GetPlayer(new JoviosUserID(userID)).GetInput(side).SetDirection(new Vector2(horizontal, vertical));
	}
	
	
	
	[RPC] void GetAccelerometer(int userID, float gyroW, float gyroX, float gyroY, float gyroZ, float accX, float accY, float accZ){
		GetPlayer(new JoviosUserID(userID)).GetInput("accelerometer").SetGyro(new Quaternion(-gyroX, -gyroY, gyroZ, gyroW));
		GetPlayer(new JoviosUserID(userID)).GetInput("accelerometer").SetAcceleration(new Vector3(accX, accZ, accY));
	}
	
	
	
	[RPC] void GetTextResponse(int userID, string buttonPress, string side){
		JoviosButtonEvent e = new JoviosButtonEvent(buttonPress, GetPlayer(new JoviosUserID(userID)).GetControllerStyle(), side);
		foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(userID)).GetControllerListeners()){
			if(listener.ButtonEventReceived(e)){
				return;
			}
		}
	}
	
	[RPC] public void InstantiatePlayerObject(int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName, int userID){
		if(!userIDToPlayerNumber.ContainsKey(userID)){
			JoviosPlayer[] newPlayers = new JoviosPlayer[players.Length + 1];
			for(int i = 0; i < players.Length; i++){ 
				newPlayers[i] = players[i];
			}
			newPlayers[playerNumber] = new JoviosPlayer(players.Length, new JoviosUserID(userID), networkPlayers[playerNumber], playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1));
			players = newPlayers;
			userIDToPlayerNumber.Add(userID, playerNumber);
			foreach(IJoviosPlayerListener listener in playerListeners){
				if(listener.PlayerConnected(players[playerNumber])){
					break;
				}
			}
		}
		else{
			players[playerNumber].NewPlayerInfo(players.Length, new JoviosUserID(userID), networkPlayers[playerNumber], playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1));
			foreach(IJoviosPlayerListener listener in playerListeners){
				if(listener.PlayerUpdated(players[playerNumber])){
					break;
				}
			}
		}
	}
	
	public void SetControls(JoviosUserID jUID, JoviosControllerStyle controllerStyle){
		GetPlayer(jUID).SetControllerStyle(controllerStyle);
		if(controllerStyle.IsSplitScreen()){
			networkView.RPC ("SentControls", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), (int) controllerStyle.GetJoviosControllerLeftStyle(), controllerStyle.GetJoviosControllerLeftDescription(), (int) controllerStyle.GetJoviosControllerRightStyle(), controllerStyle.GetJoviosControllerRightDescription());
		}
		else{
			networkView.RPC ("SentButtons", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), controllerStyle.GetOverallStyle().ToString(), controllerStyle.GetQuestionPrompt(),  controllerStyle.GetSubmit(), controllerStyle.GetResponse(0), controllerStyle.GetResponse(1), controllerStyle.GetResponse(2), controllerStyle.GetResponse(3), controllerStyle.GetResponse(4), controllerStyle.GetResponse(5), controllerStyle.GetResponse(6), controllerStyle.GetResponse(7));
		}
	}
	
	[RPC] private void SentControls(int accelerometerSetting, int lControls, string lControlsDescription, int rControls, string rControlsDescription){}
	[RPC] private void PlayerObjectCreated(int player){}
	[RPC] private void EndOfRound(int player){}
	[RPC] private void NewGame(){}
	[RPC] private void SentButtons (int accelerometerSetting, string type, string question, string actionWord, string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8){}
}