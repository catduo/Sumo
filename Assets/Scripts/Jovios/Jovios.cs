using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using JoviosSimpleJSON;

//TODO List
//take in array of userids to change player ordering
//versioning with the controller so that it always works with the games (think about how it would work, not actually doing it)
//look into unit testing on unity
//more comments and grouping
//IJovios for API help (list of all public functions)
//UI for 2 button and directional swiping
//Add non-relative directional inputs


public class Jovios : MonoBehaviour {
	
	//this is the type of networking that is being used, only Unity based is implemented
	private static JoviosNetworkingState networkingState = JoviosNetworkingState.Unity;
	
	//this is the connection string for the player to type into the controller
	private string gameName;
	public string GetGameName(){
		return gameName;
	}
	public void SetGameName(string newGameName){
		gameName = newGameName;
	}
	
	//this will call the approriate jovios object creation code, such that it will work properly with Unity
	public static Jovios Create(){
		switch(networkingState){
		case JoviosNetworkingState.Unity:
			return JoviosUnityNetworking.Create();
			break;
		default:
			return new Jovios();
			break;
		}
	}
	
	//this will start the game server so that players can start connections.
	public void StartServer(string thisGameName = ""){
		switch(networkingState){
		case JoviosNetworkingState.Unity:
			gameObject.GetComponent<JoviosUnityNetworking>().StartServer(thisGameName);
			break;
		default:
			
			break;
		}
	}
	
	//Players are stored in a list and you can get the player calling GetPlayer(id) with id being either the PlayerNumber or the JoviosUserID
	private List<JoviosPlayer> players = new List<JoviosPlayer>();
	private Dictionary<int, int> userIDToPlayerNumber = new Dictionary<int, int>();
	public JoviosPlayer GetPlayer(int playerNumber){
		if(playerNumber < players.Count){
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
			return players.Count;
		}
		else{
			return 0;
		}
	}
	
	
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
	
	//This will be called by the connection scripts and will manage player connections
	public void PlayerConnected(int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName, int userID){
		players.Add(new JoviosPlayer(players.Count, new JoviosUserID(userID), playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1)));
		if(!userIDToPlayerNumber.ContainsKey(userID)){
			Debug.Log(userID);
			Debug.Log (playerNumber);
			Debug.Log(players.Count - 1);
			userIDToPlayerNumber.Add(userID, players.Count - 1);
			packetJSON.Add(userID, "");
			networkingStates.Add(userID, JoviosNetworkingState.Unity);
			connectionJSON.Add(userID, "\"connection\":{}");
		}
		else{
			userIDToPlayerNumber[userID] = playerNumber;
		}
		if(networkingState == JoviosNetworkingState.Unity){
			gameObject.GetComponent<JoviosUnityNetworking>().SetNetworkPlayer(userID, playerNumber);
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerConnected(GetPlayer(new JoviosUserID(userID)))){
				break;
			}
		}
		//string encodedString = "{\"packet\":{\"controlStyle\":[{\"type\": \"button1\", \"position\": [0.75,0.8,1.5,1.5], \"anchor\":\"bl\", \"response\": \"[left]\", \"content\": \"[string]\"},{\"type\": \"button4\", \"position\": [-0.75,1,1.5,2], \"anchor\":\"br\", \"response\": [\"Whats up?\",\"A\",\"B\",\"C\",\"D\",\"E\",\"F\",\"G\",\"H\",\"Submit\",\"Cancel\"], \"content\": [\"Whats up?\",\"A\",\"B\",\"C\",\"D\",\"E\",\"F\",\"G\",\"H\",\"Submit\",\"Cancel\"]]}}}";
		//networkView.RPC("SendPacket", GetPlayer(new JoviosUserID(userID)).GetNetworkPlayer(), encodedString);
	}
	
	// this will be triggered when information about a player is updated, like colors or names
	public void PlayerUpdated(int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName, int userID){
		players[playerNumber].NewPlayerInfo(players.Count, playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1));
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerUpdated(players[playerNumber])){
				break;
			}
		}
	}
	
	// this will trigger when a player disconnects,
	public void PlayerDisconnected(JoviosPlayer p){
		players.Remove(p);
		packetJSON.Remove(p.GetUserID().GetIDNumber());
		networkingStates.Remove(p.GetUserID().GetIDNumber());
		connectionJSON.Remove(p.GetUserID().GetIDNumber());
		userIDToPlayerNumber.Remove(p.GetUserID().GetIDNumber());
		for(int i = 0; i < userIDToPlayerNumber.Count; i++){
			userIDToPlayerNumber[GetPlayer(i).GetUserID().GetIDNumber()] = i;
			players[i].NewPlayerInfo(i, players[i].GetPlayerName(), players[i].GetColor("primary"), players[i].GetColor("secondary"));
		}
		for(int i = 0; i < p.PlayerObjectCount(); i++){
			Destroy(p.GetPlayerObject(i));
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerDisconnected(p)){
				break;
			}
		}
	}
	void OnPlayerDisconnected(NetworkPlayer player){
		for(int i = 0; i < players.Count; i++){
			if(players[i].GetNetworkPlayer() == player){
				PlayerDisconnected(players[i]);
				break;
			}
		}
	}
	
	
	//listening to each player's controller
	public List<IJoviosControllerListener> GetControllerListeners(JoviosUserID jUID){
		return GetPlayer(jUID).GetControllerListeners();
	}
	public List<IJoviosControllerListener> GetControllerListeners(int playerNumber){
		return GetPlayer(playerNumber).GetControllerListeners();
	}
	public List<IJoviosControllerListener> GetControllerListeners(){
		List<IJoviosControllerListener> allControllerListeners = new List<IJoviosControllerListener>();
		foreach(JoviosPlayer player in players){
			foreach(IJoviosControllerListener listener in player.GetControllerListeners()){
				allControllerListeners.Add(listener);
			}
		}
		return allControllerListeners;
	}
	public void AddControllerListener(IJoviosControllerListener listener){
		foreach(JoviosPlayer player in players){
			player.AddControllerListener(listener);
		}
	}
	public void AddControllerListener(IJoviosControllerListener listener, int playerNumber){
		GetPlayer(playerNumber).AddControllerListener(listener);
	}
	public void AddControllerListener(IJoviosControllerListener listener, JoviosUserID jUID){
		GetPlayer(jUID).AddControllerListener(listener);
	}
	public void RemoveControllerListener(IJoviosControllerListener listener){
		foreach(JoviosPlayer player in players){
			player.RemoveControllerListener(listener);
		}
	}
	public void RemoveControllerListener(IJoviosControllerListener listener, int playerNumber){
		GetPlayer(playerNumber).RemoveControllerListener(listener);
	}
	public void RemoveControllerListener(IJoviosControllerListener listener, JoviosUserID jUID){
		GetPlayer(jUID).RemoveControllerListener(listener);
	}
	public void RemoveAllControllerListeners(){
		foreach(JoviosPlayer player in players){
			player.RemoveAllControllerListeners();
		}
	}
	public void RemoveAllControllerListeners(int playerNumber){
		GetPlayer(playerNumber).RemoveAllControllerListeners();
	}
	public void RemoveAllControllerListeners(JoviosUserID jUID){
		GetPlayer(jUID).RemoveAllControllerListeners();
	}
	
	//this will set the controlls of a given player
	public void SetControls(JoviosUserID jUID, JoviosControllerStyle controllerStyle){
		GetPlayer(jUID).SetControllerStyle(controllerStyle);
		AddToPacket(jUID, controllerStyle.GetJSON());
	}
	//this will set the controlls of all players
	public void SetControls(JoviosControllerStyle controllerStyle){
		foreach(JoviosPlayer player in players){
			JoviosUserID jUID = player.GetUserID();
			GetPlayer(jUID).SetControllerStyle(controllerStyle);
			AddToPacket(jUID, controllerStyle.GetJSON());
		}
	}
	
	private Dictionary<int, string> packetJSON = new Dictionary<int, string>();
	private Dictionary<int, JoviosNetworkingState> networkingStates = new Dictionary<int, JoviosNetworkingState>();
	private Dictionary<int, string> connectionJSON = new Dictionary<int, string>();
	//this sends out the packets as they are generated
	void FixedUpdate(){
		foreach(int key in userIDToPlayerNumber.Keys){
			if(packetJSON[key] != ""){
				packetJSON[key] += "}}";
				//update to switch case by player for network connections other than unity networking
				networkView.RPC("SendPacket", GetPlayer(new JoviosUserID(key)).GetNetworkPlayer(), packetJSON[key]);
				packetJSON[key] = "";
			}
		}
	}
	public void AddToPacket(JoviosUserID jUID, string addition){
		if(packetJSON[jUID.GetIDNumber()] == ""){
			packetJSON[jUID.GetIDNumber()] += "{\"packet\":{" + addition;
		}
		else{
			packetJSON[jUID.GetIDNumber()] += "," + addition;
		}
		Debug.Log (packetJSON[jUID.GetIDNumber()].ToString());
		Debug.Log (jUID.GetIDNumber().ToString());
	}
	//this picks up the Unity Networking connections
	[RPC] void SendPacket(string packet){
		ParsePacket(packet);
	}
	//this parses the incoming packets
	private void ParsePacket(string packet){
		var myJSON = JSON.Parse(packet);
		if(myJSON["packet"]["response"] != null){
			for(int i = 0; i < myJSON["packet"]["response"].Count; i++){
				switch(myJSON["packet"]["response"][i]["type"]){
				case "button":
					JoviosButtonEvent e = new JoviosButtonEvent(myJSON["packet"]["response"][i]["button"], GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle(), myJSON["packet"]["response"][i]["action"]);
					foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerListeners()){
						if(listener.ButtonEventReceived(e)){
							return;
						}
					}
					break;
					
				case "direction":
					switch(myJSON["packet"]["response"][i]["action"]){
					case "hold":
						GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle().GetDirection(myJSON["packet"]["response"][i]["direction"]).SetDirection(new Vector2(myJSON["packet"]["response"][i]["position"][0].AsFloat, myJSON["packet"]["response"][i]["position"][1].AsFloat));
						break;
						
					case "press":
						JoviosButtonEvent e1 = new JoviosButtonEvent(myJSON["packet"]["response"][i]["direction"], GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle(), myJSON["packet"]["response"][i]["action"]);
						foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerListeners()){
							if(listener.ButtonEventReceived(e1)){
								return;
							}
						}
						break;
						
					case "release":
						JoviosButtonEvent e2 = new JoviosButtonEvent(myJSON["packet"]["response"][i]["direction"], GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle(), myJSON["packet"]["response"][i]["action"]);
						foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerListeners()){
							if(listener.ButtonEventReceived(e2)){
								return;
							}
						}
						break;
						
					default:
						break;
					}
					break;
					
				case "accelerometer":
					var accInfo = myJSON["packet"]["response"][i]["values"];
					GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle().GetAccelerometer().SetGyro(new Quaternion(-accInfo[0].AsFloat, -accInfo[1].AsFloat, accInfo[2].AsFloat, accInfo[3].AsFloat));
					GetPlayer(new JoviosUserID(myJSON["userID"].AsInt)).GetControllerStyle().GetAccelerometer().SetAcceleration(new Vector3(accInfo[4].AsFloat, accInfo[5].AsFloat, accInfo[6].AsFloat));
					break;
					
				default:
					Debug.Log ("wrong response type");
					break;
				}
			}
		}
		if(myJSON["packet"]["playerConnected"] != null){
			PlayerConnected(myJSON["packet"]["playerConnected"]["playerNumber"].AsInt, myJSON["packet"]["playerConnected"]["primaryR"].AsFloat, myJSON["packet"]["playerConnected"]["primaryG"].AsFloat, myJSON["packet"]["playerConnected"]["primaryB"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryR"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryG"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryB"].AsFloat, myJSON["packet"]["playerConnected"]["playerName"], myJSON["packet"]["playerConnected"]["userID"].AsInt);
		}
	}
	//{"packet":{"response":{[{"action":"press","button":""}]}}}
	//string encodedString = "{\"packet\":{\"controlStyle\":[{\"type\": \"joystick\", \"position\": [0.75,0.8,1.5,1.5], \"anchor\":\"bl\", \"response\": \"[left]\", \"content\": \"[string]\"},{\"type\": \"button4\", \"position\": [-0.75,1,1.5,2], \"anchor\":\"br\", \"response\": [\"Whats up?\",\"A\",\"B\",\"C\",\"D\",\"E\",\"F\",\"G\",\"H\",\"Submit\",\"Cancel\"], \"content\": [\"Whats up?\",\"A\",\"B\",\"C\",\"D\",\"E\",\"F\",\"G\",\"H\",\"Submit\",\"Cancel\"]]}}}";
	
	
	//When a controller connects it will check the version so that it can know if the controller is out of date.  If the game is out of date the controller should still work with it (only 1.0.0 and greater)
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
}