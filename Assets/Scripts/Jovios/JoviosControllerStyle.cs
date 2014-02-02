using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosControllerStyle{
	public JoviosControllerStyle(){
		backgroundUrl = "none";
	}
	//this is a list of the areas that are being used on the controller.  Rigth now it is left and right with the arbitrary ones living in another list
	private Dictionary<string, JoviosControllerAreaStyle> areaStyles = new Dictionary<string, JoviosControllerAreaStyle>();
	public JoviosControllerAreaStyle GetAreaStyle(string side){
		return areaStyles[side.ToLower()];
	}
	//this is a list of the arbitrary buttons that being stored.  eventually it sohuld be added to the list above with the areas being given names by the game
	private List<JoviosControllerAreaStyle> arbitraryAreaStyles = new List<JoviosControllerAreaStyle>();
	public List<JoviosControllerAreaStyle> GetArbitraryAreaStyle(){
		return arbitraryAreaStyles;
	}
	//this is the overall style, if set it should override any of the areas.  it should be updated to not override
	private JoviosControllerOverallStyle overallStyle;
	public JoviosControllerOverallStyle GetOverallStyle(){
		return overallStyle;
	}
	//the following will add areas, they can only take right or left, but should be updated to take any arbitrary location information
	public void AddRelativeJoystick(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeJoystick(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddAbsoluteJoystick(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().AbsoluteJoystick(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddRelativeDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddAbsoluteDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().AbsoluteDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddRelativeDiagonalDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeDiagonalDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddAbsoluteDiagonalDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().AbsoluteDiagonalDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddButton1(string sideRightOrLeft, string description, string response){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle().Button1(sideRightOrLeft.ToLower(), description, response);
		areaStyles.Add(sideRightOrLeft.ToLower(), areaStyle);
		is_splitScreen = true;
	}
	public void AddButton2(string sideRightOrLeft, string[] description_2, string[] response_2){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().Button2(sideRightOrLeft.ToLower(), description_2, response_2));
		is_splitScreen = true;
	}
	public void AddCardinalSwipes(string sideRightOrLeft, string[] description_4, string[] response_4){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().CardinalSwipes(sideRightOrLeft.ToLower(), description_4, response_4));
		is_splitScreen = true;
	}
	public void AddAllTouches(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().AllTouches(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	//this is the kind of arbitrary definition that should be added to all of the above area styles.  The below overall styles should also be changed to support area inputs
	public void AddArbitraryButton(int[] buttonRect, string description, string response){
		arbitraryAreaStyles.Add(new JoviosControllerAreaStyle().ArbitraryButton(buttonRect, description, response));
		is_splitScreen = true;
	}
	
	//these are overall styles that will override any area inputs.  These should be updated to also support area styles
	public void SetBasicButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().BasicButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetSingleButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().SingleButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetMultiButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().MultiButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetTextInput(string question, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().TextInput(question, submitButton);
		is_splitScreen = false;
	}
	public void SetNumericInput(string question, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().NumericInput(question, submitButton);
		is_splitScreen = false;
	}
	
	//this is the accelerometer information.  it is currently either on or off, but should have intermediate states added in.
	private JoviosControllerAccelerometerStyle accelerometerStyle;
	public void SetAccelerometerStyle(JoviosControllerAccelerometerStyle setAccelerometerStyle){
		accelerometerStyle = setAccelerometerStyle;
	}
	public JoviosControllerAccelerometerStyle GetAccelerometerStyle(){
		return accelerometerStyle;
	}

	//this will set the background image on the controller
	private string backgroundUrl;
	public void SetBackgroundImage(string url){
		backgroundUrl = url;
	}
	public string GetBackgroundImage(){
		return backgroundUrl;
	}


	// this currently defines if a controller is using the overall style or area styles.  this should be removed and the styles should be updated to return overall information in themselves
	private bool is_splitScreen;
	public bool IsSplitScreen(){
		return is_splitScreen;
	}
}