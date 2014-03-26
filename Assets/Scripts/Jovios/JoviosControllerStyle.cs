using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosControllerStyle{
	public JoviosControllerStyle(){
	}
	//the following will add areas, they can only take right or left, but should be updated to take any arbitrary location information
	public void AddJoystick(string position, string description, string response = ""){
		if(response == ""){
			response = description;
		}
		directions.Add(response, new JoviosDirection(description, position, response));
		AddToJSON(directions[response].GetJSON());
	}
	public void AddJoystick(Vector2 position, Vector2 scale, string anchor, string description, string response = ""){
		if(response == ""){
			response = description;
		}
		directions.Add(response, new JoviosDirection(position, scale, anchor, description, response));
		AddToJSON(directions[response].GetJSON());
	}
	public void AddButton1(Vector2 position, Vector2 scale, string anchor, string description, string response = ""){
		if(response == ""){
			response = description;
		}
		buttons.Add(response, new JoviosButton(position, scale, anchor, "button1", new string[1] {description}, new string[1] {response}));
		AddToJSON(buttons[response].GetJSON());
	}
	
	//this is the accelerometer information.  it is currently either on or off, but should have intermediate states added in.
	public void SetAccelerometerStyle(JoviosAccelerometerStyle setAccelerometerStyle){
		accelerometer = new JoviosAccelerometer(setAccelerometerStyle);
		AddToJSON(accelerometer.JSON);
	}

	
	//these are the currently used inputs for the controller style
	private Dictionary<string, JoviosDirection> directions = new Dictionary<string, JoviosDirection>();
	private Dictionary<string, JoviosButton> buttons = new Dictionary<string, JoviosButton>();
	private JoviosAccelerometer accelerometer;
	public JoviosDirection GetDirection(string response){
		if(directions.ContainsKey(response)){
			return directions[response];
		}
		else{
			return null;
		}
	}
	public JoviosButton
	GetButton(string response){
		if(buttons.ContainsKey(response)){
			return buttons[response];
		}
		else{
			return null;
		}
	}
	public JoviosAccelerometer GetAccelerometer(){
		return accelerometer;
	}

	
	private string JSON;
	public string GetJSON(){
		return (JSON + "]");
	}
	private void AddToJSON(string toAdd){
		if(JSON == "" || JSON == null){
			JSON += "'controlStyle':["+toAdd;
		}
		else{
			JSON += ","+toAdd+"";
		}
	}
}