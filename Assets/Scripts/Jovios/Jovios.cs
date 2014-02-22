using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

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
			userIDToPlayerNumber.Add(userID, playerNumber);
		}
		else{
			userIDToPlayerNumber[userID] = playerNumber;
		}
		if(networkingState == JoviosNetworkingState.Unity){
			gameObject.GetComponent<JoviosUnityNetworking>().SetNetworkPlayer(playerNumber);
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerConnected(players[playerNumber])){
				break;
			}
		}
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
		switch(networkingState){
		case JoviosNetworkingState.Unity:
			GetPlayer(jUID).SetControllerStyle(controllerStyle);
			if(controllerStyle.IsSplitScreen()){
				networkView.RPC ("SentControls", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), controllerStyle.GetAreaStyle("left").GetAreaType(), controllerStyle.GetAreaStyle("left").GetResponse()[0], controllerStyle.GetAreaStyle("left").GetDescription()[0], controllerStyle.GetAreaStyle("right").GetAreaType(), controllerStyle.GetAreaStyle("right").GetResponse()[0], controllerStyle.GetAreaStyle("right").GetDescription()[0], controllerStyle.GetBackgroundImage());
				foreach(JoviosControllerAreaStyle arbitraryArea in controllerStyle.GetArbitraryAreaStyle()){
					Debug.Log ("arbitrary set");
					networkView.RPC ("SentArbitraryUIElement", GetPlayer(jUID).GetNetworkPlayer(), arbitraryArea.GetRect()[0], arbitraryArea.GetRect()[1], arbitraryArea.GetRect()[2], arbitraryArea.GetRect()[3], arbitraryArea.GetDescription()[0], arbitraryArea.GetResponse()[0]);
				}
			}
			else{
				networkView.RPC ("SentButtons", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), controllerStyle.GetOverallStyle().GetOverallType(), controllerStyle.GetOverallStyle().GetQuestionPrompt(),  controllerStyle.GetOverallStyle().GetSubmit(), controllerStyle.GetOverallStyle().GetResponse(0), controllerStyle.GetOverallStyle().GetResponse(1), controllerStyle.GetOverallStyle().GetResponse(2), controllerStyle.GetOverallStyle().GetResponse(3), controllerStyle.GetOverallStyle().GetResponse(4), controllerStyle.GetOverallStyle().GetResponse(5), controllerStyle.GetOverallStyle().GetResponse(6), controllerStyle.GetOverallStyle().GetResponse(7), controllerStyle.GetBackgroundImage());
			}
			break;
			
		default:
			break;
		}
	}
	//this will set the controlls of all players
	public void SetControls(JoviosControllerStyle controllerStyle){
		foreach(JoviosPlayer player in players){
			JoviosUserID jUID = player.GetUserID();
			switch(networkingState){
			case JoviosNetworkingState.Unity:
				GetPlayer(jUID).SetControllerStyle(controllerStyle);
				if(controllerStyle.IsSplitScreen()){
					networkView.RPC ("SentControls", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), controllerStyle.GetAreaStyle("left").GetAreaType(), controllerStyle.GetAreaStyle("left").GetResponse()[0], controllerStyle.GetAreaStyle("left").GetDescription()[0], controllerStyle.GetAreaStyle("right").GetAreaType(), controllerStyle.GetAreaStyle("right").GetResponse()[0], controllerStyle.GetAreaStyle("right").GetDescription()[0], controllerStyle.GetBackgroundImage());
					foreach(JoviosControllerAreaStyle arbitraryArea in controllerStyle.GetArbitraryAreaStyle()){
						networkView.RPC ("SentArbitraryUIElement", GetPlayer(jUID).GetNetworkPlayer(), arbitraryArea.GetRect()[0], arbitraryArea.GetRect()[1], arbitraryArea.GetRect()[2], arbitraryArea.GetRect()[3], arbitraryArea.GetDescription()[0], arbitraryArea.GetResponse()[0]);
					}
				}
				else{
					networkView.RPC ("SentButtons", GetPlayer(jUID).GetNetworkPlayer(),(int)controllerStyle.GetAccelerometerStyle(), controllerStyle.GetOverallStyle().GetOverallType(), controllerStyle.GetOverallStyle().GetQuestionPrompt(),  controllerStyle.GetOverallStyle().GetSubmit(), controllerStyle.GetOverallStyle().GetResponse(0), controllerStyle.GetOverallStyle().GetResponse(1), controllerStyle.GetOverallStyle().GetResponse(2), controllerStyle.GetOverallStyle().GetResponse(3), controllerStyle.GetOverallStyle().GetResponse(4), controllerStyle.GetOverallStyle().GetResponse(5), controllerStyle.GetOverallStyle().GetResponse(6), controllerStyle.GetOverallStyle().GetResponse(7), controllerStyle.GetBackgroundImage());
				}
				break;
				
			default:
				break;
			}
		}
	}
	
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