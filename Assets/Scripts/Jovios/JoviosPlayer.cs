using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosPlayer{
	public JoviosPlayer(int pNumber, JoviosUserID pUserID, NetworkPlayer pNetwork, string pName, Color pPrimary, Color pSecondary){
		playerNumber = pNumber;
		networkPlayer = pNetwork;
		playerName = pName;
		right = new JoviosInput();
		left = new JoviosInput();
		accelerometer = new JoviosInput();
		primary = pPrimary;
		secondary = pSecondary;
		userID = pUserID;
	}
	public void NewPlayerInfo(int pNumber, JoviosUserID pUserID, NetworkPlayer pNetwork, string pName, Color pPrimary, Color pSecondary){
		playerNumber = pNumber;
		networkPlayer = pNetwork;
		playerName = pName;
		right = new JoviosInput();
		left = new JoviosInput();
		accelerometer = new JoviosInput();
		primary = pPrimary;
		secondary = pSecondary;
	}
	private List<IJoviosControllerListener> controllerListeners = new List<IJoviosControllerListener>();
	public List<IJoviosControllerListener> GetControllerListeners(){
		return controllerListeners;
	}
	public void AddControllerListener(IJoviosControllerListener listener){
		controllerListeners.Add(listener);
	}
	public void RemoveControllerListener(IJoviosControllerListener listener){
		controllerListeners.Remove(listener);
	}
	public void RemoveAllControllerListeners(){
		controllerListeners = new List<IJoviosControllerListener>();
	}
	private int playerNumber;
	public int GetPlayerNumber(){
		return playerNumber;
	}
	private GameObject statusObject;
	public GameObject GetStatusObject(){
		return statusObject;
	}
	public void SetStatusObject(GameObject newStatusObject){
		statusObject = newStatusObject;
	}
	private GameObject playerObject;
	public GameObject GetPlayerObject(){
		return playerObject;
	}
	public void SetPlayerObject(GameObject newPlayerObject){
		playerObject = newPlayerObject;
	}
	private JoviosUserID userID;
	public JoviosUserID GetUserID(){
		return userID;
	}
	private string playerName;
	public string GetPlayerName(){
		return playerName;
	}
	private NetworkPlayer networkPlayer;
	public NetworkPlayer GetNetworkPlayer(){
		return networkPlayer;
	}
	private Color primary;
	private Color secondary;
	public Color GetColor(string color){
		switch(color){
		case "primary":
			return primary;
			break;
		case "secondary":
			return secondary;
			break;
		default:
			Debug.Log("could not find color " + color);
			return Color.clear;
			break;
		}
	}
	private JoviosInput left;
	private JoviosInput right;
	private JoviosInput accelerometer;
	public JoviosInput GetInput(string position){
		switch(position){
		case "left":
			return left;
			break;
		case "right":
			return right;
			break;
		case "accelerometer":
			return accelerometer;
			break;
		default:
			Debug.Log("could not find input " + position);
			return null;
			break;
		}
	}
	private JoviosControllerStyle controllerStyle;
	public void SetControllerStyle(JoviosControllerStyle newStyle){
		controllerStyle = newStyle;
	}
	public JoviosControllerStyle GetControllerStyle(){
		return controllerStyle;
	}
}