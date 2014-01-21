using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosControllerStyle{
	public JoviosControllerStyle(JoviosControllerOverallStyle controllerStyle, string question, string[] setResponses, string submitButton = ""){
		questionPrompt = question;
		responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			responses = setResponses;
		}
		submit = submitButton;
		splitScreen = false;
	}
	public JoviosControllerStyle(JoviosControllerAreaStyle leftControllerStyle, string leftControlsDescription, JoviosControllerAreaStyle rightControllerStyle, string rightControlsDescription){
		left = leftControllerStyle;
		right = rightControllerStyle;
		leftString = leftControlsDescription;
		rightString = rightControlsDescription;
		splitScreen = true;
	}
	private JoviosControllerAccelerometerStyle accelerometerStyle;
	public void SetAccelerometerStyle(JoviosControllerAccelerometerStyle setAccelerometerStyle){
		accelerometerStyle = setAccelerometerStyle;
	}
	public JoviosControllerAccelerometerStyle GetAccelerometerStyle(){
		return accelerometerStyle;
	}
	private bool splitScreen;
	public bool IsSplitScreen(){
		return splitScreen;
	}
	private JoviosControllerAreaStyle left;
	public JoviosControllerAreaStyle GetJoviosControllerLeftStyle(){
		return left;
	}
	private JoviosControllerAreaStyle right;
	public JoviosControllerAreaStyle GetJoviosControllerRightStyle(){
		return right;
	}
	private string leftString;
	public string GetJoviosControllerLeftDescription(){
		return leftString;
	}
	private string rightString;
	public string GetJoviosControllerRightDescription(){
		return rightString;
	}
	private JoviosControllerOverallStyle overallStyle;
	public JoviosControllerOverallStyle GetOverallStyle(){
		return overallStyle;
	}
	private string questionPrompt;
	public string GetQuestionPrompt(){
		return questionPrompt;
	}
	private string[] responses = new string[8];
	public string[] GetResponses(){
		return responses;
	}
	public string GetResponse(int responseNumber){
		if(responseNumber < responses.Length){
			return responses[responseNumber];
		}
		else{
			return "";
		}
	}
	private string submit;
	public string GetSubmit(){
		return submit;
	}
}