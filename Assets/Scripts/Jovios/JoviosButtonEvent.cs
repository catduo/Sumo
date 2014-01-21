using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosButtonEvent {
	public JoviosButtonEvent(string buttonIn, JoviosControllerStyle controllerStyle, string thisSide, string actionIn = "", string typeIn = ""){
		response = buttonIn;
		style = controllerStyle;
		side = thisSide;
	}
	private string side;
	public string GetSide(){
		return side;
	}
	private string response;
	public string GetResponse(){
		return response;
	}
	private JoviosControllerStyle style;
	public JoviosControllerStyle GetControllerStyle(){
		return style;
	}
}
