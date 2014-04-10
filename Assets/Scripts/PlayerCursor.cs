using UnityEngine;
using System.Collections;



public class PlayerCursor : MonoBehaviour {
	
	float speed = 0.3F;
	public JoviosUserID myPlayer;
	public int playerNumber;
	public string playerName;
	private Color primary;
	private Color secondary;
	private Jovios jovios;
	private float xMin;
	private float xMax;
	private float yMin;
	private float yMax;

	void Start(){
		transform.parent = GameObject.Find("UI Root").transform;
		transform.localScale = Vector3.one;
		yMin = -720/2;
		yMax = 720/2;
		xMin = - Screen.width/Screen.height * 720/2;
		xMax = Screen.width/Screen.height * 720/2;
	}
	
	void Update(){
		transform.Translate( new Vector3(speed / 10 * jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").GetDirection().x, speed / 10 * jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").GetDirection().y, 0));
		if(transform.localPosition.x < xMin){
			transform.localPosition += new Vector3(xMin - transform.localPosition.x, 0, 0);
		}
		if(transform.localPosition.x > xMax){
			transform.localPosition += new Vector3(xMax - transform.localPosition.x, 0, 0);
		}
		if(transform.localPosition.y < yMin){
			transform.localPosition += new Vector3(0, yMin - transform.localPosition.y, 0);
		}
		if(transform.localPosition.y > yMax){
			transform.localPosition += new Vector3(0, yMax - transform.localPosition.y, 0);
		}
	}
	
	public void SetMyPlayer (JoviosPlayer player){
		jovios = GameManager.jovios;
		myPlayer = player.GetUserID();
		playerNumber = player.GetPlayerNumber();
		primary = player.GetColor("primary");
		secondary = player.GetColor("secondary");
		playerName = player.GetPlayerName();
		transform.FindChild("Cursor").GetComponent<UISprite>().color = primary;
		transform.FindChild("Cursor").FindChild("Label").GetComponent<UILabel>().color = secondary;
		if (playerName.Length > 0){
			transform.FindChild("Cursor").FindChild("Label").GetComponent<UILabel>().text = playerName[0].ToString();
		}
	}
	
	public void SimTouch(){
		//Physics.Raycast(new Ray(transform.position), Vector3.forward);
	}
	
}