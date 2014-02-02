using System;

public class JoviosControllerOverallStyle{
	//these are the setters for the overall styles
	public JoviosControllerOverallStyle BasicButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "BasicButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle SingleButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "SingleButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle MultiButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "MultiButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle TextInput(string question, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.submit = submitButton;
		overallStyle.type = "TextInput";
		return overallStyle;
	}
	public JoviosControllerOverallStyle NumericInput(string question, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.submit = submitButton;
		overallStyle.type = "NumericInput";
		return overallStyle;
	}
	private string type;
	public string GetOverallType(){
		return type;
	}
	//this is the question or prompt displayed at the top of the controller
	private string questionPrompt;
	public string GetQuestionPrompt(){
		return questionPrompt;
	}
	//this is an array of up to 8 items that are displayed as individual options or responses.  if text input the option 0 will be the greyed out text
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
	// this is the submit button for multi, single, numeric, and text inputs options
	private string submit;
	public string GetSubmit(){
		return submit;
	}
}