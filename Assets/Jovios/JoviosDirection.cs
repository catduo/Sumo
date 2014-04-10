using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosDirection{
	private string description;
	private string response;
	private Vector2 position;
	private Vector2 scale;
	private string anchor;
	private string simplePosition;
	private string JSON;
	public string GetJSON(){
		return JSON;
	}
	public JoviosDirection(string setDescription, string setSimplePosition, string setResponse = ""){
		description = setDescription;
		if(setResponse == ""){
			response = setDescription;
		}
		else{
			response = setResponse;
		}
		direction = Vector2.zero;
		simplePosition = setSimplePosition;
		JSON = "{'type':'joystick','position':'"+simplePosition+"', 'description':['"+description+"'],'response':['"+response+"']}";
	}
	public JoviosDirection(Vector2 setPostion, Vector2 setScale, string setAnchor, string setDescription, string setResponse = ""){
		description = setDescription;
		if(setResponse == ""){
			response = setDescription;
		}
		else{
			response = setResponse;
		}
		direction = Vector2.zero;
		position = setPostion;
		scale = setScale;
		anchor = setAnchor;
		JSON = "{'type':'joystick','position':["+position.x+","+position.y+","+scale.x+","+scale.y+"], 'anchor':'"+anchor+"', 'description':['"+description+"'],'response':['"+response+"']}";
	}

	//this is for any directional inputs, will eventually support arbitrary definitions
	private Vector2 direction;
	public Vector2 GetDirection(){
		return direction;
	}
	public void SetDirection(Vector2 newDirection){
		direction = newDirection;
	}
}

