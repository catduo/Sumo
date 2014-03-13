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

	void Update(){
		transform.Translate( new Vector3(speed / 10 * jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").GetDirection().x, speed / 10 * jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").GetDirection().y, 0));
	}
	
	public void SetMyPlayer (JoviosPlayer player){
		jovios = GameManager.jovios;
		myPlayer = player.GetUserID();
		playerNumber = player.GetPlayerNumber();
		primary = player.GetColor("primary");
		secondary = player.GetColor("secondary");
		playerName = player.GetPlayerName();
	}

	public void SimTouch(){
		//Physics.Raycast(new Ray(transform.position), Vector3.forward);
	}

}