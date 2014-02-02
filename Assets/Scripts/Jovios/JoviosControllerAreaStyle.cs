using System;

public class JoviosControllerAreaStyle{
	//here are the setters for the areas, they need to be updated to support arbitrary areas
	public JoviosControllerAreaStyle RelativeJoystick(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeJoystick";
		return areaStyle;
	}
	public JoviosControllerAreaStyle AbsoluteJoystick(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "AbsoluteJoystick";
		return areaStyle;
	}
	public JoviosControllerAreaStyle RelativeDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle AbsoluteDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "AbsoluteDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle RelativeDiagonalDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeDiagonalDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle AbsoluteDiagonalDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "AbsoluteDiagonalDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle Button1(string nSide, string nDescription, string nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "Button1";
		return areaStyle;
	}
	public JoviosControllerAreaStyle Button2(string nSide, string[] nDescription, string[] nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.description = new string[2];
		areaStyle.response = new string[2];
		for(int i = 0; i < 2; i++){
			if(nDescription.Length > i){
				areaStyle.description[i] = nDescription[i];
			}
			else{
				areaStyle.description[i] = "";
			}
			if(nResponse.Length > i){
				areaStyle.response[i] = nResponse[i];
			}
			else{
				areaStyle.response[i] = "";
			}
		}
		areaStyle.side = nSide;
		areaStyle.type = "Button2";
		return areaStyle;
	}
	public JoviosControllerAreaStyle CardinalSwipes(string nSide, string[] nDescription, string[] nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.description = new string[4];
		areaStyle.response = new string[4];
		for(int i = 0; i < 4; i++){
			if(nDescription.Length >= i){
				areaStyle.description[i] = nDescription[i];
			}
			else{
				areaStyle.description[i] = "";
			}
			if(nResponse.Length >= i){
				areaStyle.response[i] = nResponse[i];
			}
			else{
				areaStyle.response[i] = "";
			}
		}
		areaStyle.side = nSide;
		areaStyle.type = "CardinalSwipes";
		return areaStyle;
	}
	public JoviosControllerAreaStyle AllTouches(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "AllTouches";
		return areaStyle;
	}
	//this is the first image supporting and arbitrary area supporting item, other options should be updated to take information like this
	public JoviosControllerAreaStyle ArbitraryButton(int[] nRect, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.rect = nRect;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "Button";
		return areaStyle;
	}
	// for arbitrary areas this is the rect definitions on the controller screen with the middle point being 0,0 and the top right corner being 12.5,10
	// wider screens may go up to the top right corner being 16, 10
	private int[] rect;
	public int[] GetRect(){
		return rect;
	}
	private string type;
	public string GetAreaType(){
		return type;
	}
	//this is only if the split screen is set to a right and left split
	private string side;
	public string GetSide(){
		return side;
	}
	//descriptions are the text or image shown on the controller
	private string[] description;
	public string[] GetDescription(){
		return description;
	}
	//the response is the text sent back when the button is pressed on the controller
	private string[] response;
	public string[] GetResponse(){
		return response;
	}
}