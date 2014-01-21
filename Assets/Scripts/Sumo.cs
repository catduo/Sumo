using UnityEngine;
using System.Collections;



public class Sumo : MonoBehaviour {

	public float speed = 1;
	public float strength = 1;
	public float range = 1;
	public float defense = 1;
	public Vector2 movement;
	public Vector2 facing;
	public float attackPower = 0;
	public float attackMax = 120;
	public float attackTime;
	private float attackDuration = 0.2F;
	public bool is_attacking = false;
	
	private NetworkPlayer myPlayer;
	public int playerNumber;
	public string playerName;
	private Color primary;
	private Color secondary;
	
	public string selectedArena = "";
	public BonusType activeBoost = BonusType.None;
	public bool is_boostActive = false;
	public float boostStart;
	public float boostDuration = 5;
	public Color flashColor = Color.clear;
	
	private Transform body;
	private Transform robot;
	private Transform hand;
	public Transform crown;
	public GameObject projectile;
	private Transform modifiers;
	
	private float flashTime = 0.1F;
	private bool flash = false;
	
	private Jovios jovios;

	// Use this for initialization
	void Start () {
		jovios = GameManager.jovios;
		crown = transform.FindChild("Crown");
		body = transform.FindChild("Body");
		modifiers = transform.FindChild("Modifiers");
		GameObject newHand = (GameObject) GameObject.Instantiate(projectile, body.up + body.position, Quaternion.identity);
		hand = newHand.transform;
		hand.GetComponent<Projectile>().playerNumber = playerNumber;
		hand.name = "Hand";
		hand.parent = transform;
		hand.localScale = new Vector3(0.3F, 0.3F, 0.3F);
		speed = 1;
		robot = body.FindChild("Robot1");
		for(int i = 0; i < robot.childCount; i++){
			if(robot.GetChild(i).name == "Sphere_008"){
				robot.GetChild(i).GetChild(0).renderer.material.color = primary;
				robot.GetChild(i).GetChild(1).renderer.material.color = primary;
				robot.GetChild(i).GetChild(2).renderer.material.color = primary;
			}
			else if(robot.GetChild(i).name == "Sphere_009"){
				robot.GetChild(i).GetChild(0).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(1).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(2).renderer.material.color = secondary;
			}
			else{
				robot.GetChild(i).renderer.material.color = Color.grey;
			}
		}
		robot = robot.GetChild(13);
	}
	
	void Update () {
		crown.position = body.position + new Vector3(0,0.1F,-0.5F);
		if(MenuManager.gameState == GameState.ChooseArena){
			hand.collider.enabled = false;
			hand.renderer.enabled = false;
		}
		else{
			hand.collider.enabled = true;
			hand.renderer.enabled = true;
		}
		if(is_boostActive){
			switch(activeBoost){
			case BonusType.Speed:
				speed = 1.5F;
				range = 1;
				strength = 1;
				defense = 1;
				flashColor = Color.yellow;
				body.transform.name = "Body";
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				break;
			
			case BonusType.Immunity:
				defense = 100;
				range = 1;
				strength = 1;
				speed = 1;
				flashColor = Color.white;
				body.transform.name = "Body";
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				break;
			
			case BonusType.Rampage:
				defense = 3;
				speed = 1.2F;
				hand.collider.enabled = false;
				hand.renderer.enabled = false;
				body.transform.name = "Rampage";
				range = 1;
				flashColor = Color.red;
				break;
			
			case BonusType.Strength:
				strength = 2;
				range = 1;
				defense = 1;
				speed = 1;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				flashColor = Color.green;
				body.transform.name = "Body";
				break;
			
			case BonusType.Range:
				range = 2;
				strength = 1;
				defense = 1;
				speed = 1;
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				flashColor = Color.blue;
				body.transform.name = "Body";
				break;
			}
			if(flashTime + 0.2F < Time.time){
				flashTime = Time.time;
				flash = !flash;
			}
			if(flash){
				robot.renderer.material.color = primary;
			}
			else{
				robot.renderer.material.color = flashColor;
			}
			if(boostStart + boostDuration < Time.time){
				defense = 1;
				speed = 1;
				range = 1;
				strength = 1;
				is_boostActive = false;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				body.transform.name = "Body";
				robot.renderer.material.color = primary;
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!body.GetComponent<SumoCollision>().is_ringOut && (MenuManager.gameState == GameState.ChooseArena || MenuManager.gameState == GameState.GameOn)){	
			if(body.rigidbody.velocity.magnitude < 1){
				body.rigidbody.velocity = new Vector3(0, 0, body.rigidbody.velocity.z);
			}
			else{
				body.rigidbody.velocity *= 0.95F;
			}
			if(jovios.GetPlayer(playerNumber).GetInput("right").GetDirection() != Vector2.zero){
				if(!is_attacking){
					attackPower++;
				}
				if(jovios.GetPlayer(playerNumber).GetInput("right").GetDirection().y > 0){
					body.eulerAngles = new Vector3(body.eulerAngles.x, body.eulerAngles.y, - Vector2.Angle(new Vector2(1,0), jovios.GetPlayer(playerNumber).GetInput("right").GetDirection()));
				}
				else{
					body.eulerAngles = new Vector3(body.eulerAngles.x, body.eulerAngles.y, Vector2.Angle(new Vector2(1,0), jovios.GetPlayer(playerNumber).GetInput("right").GetDirection()));
				}
			}
			else if(attackPower > 0 && !is_attacking){
				Attack();
			}
			transform.Translate( new Vector3(speed / 10 * jovios.GetPlayer(playerNumber).GetInput("left").GetDirection().y, speed / 10 * jovios.GetPlayer(playerNumber).GetInput("left").GetDirection().x, 0));
			body.rigidbody.angularVelocity = Vector3.zero;
			float handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
			hand.localScale = new Vector3(handScale, handScale, handScale);
			if(is_attacking){
				hand.GetComponent<Projectile>().facing = body.up;
				hand.GetComponent<Projectile>().fireDuration = range;
				is_attacking = false;
				attackPower = 0;
				hand.rigidbody.useGravity = true;
				hand.parent = modifiers;
				attackPower = 0;
				handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
				GameObject newHand = (GameObject) GameObject.Instantiate(projectile, body.up + body.position, Quaternion.identity);
				hand = newHand.transform;
				hand.GetComponent<Projectile>().playerNumber = playerNumber;
				hand.name = "Hand";
				hand.parent = transform;
				hand.localScale = new Vector3(handScale, handScale, handScale);
			}
			else{
				hand.position = body.up  + body.position;
			}
		}
		else{
			hand.position = body.up + body.position;
		}
	}
	
	public void SetMyPlayer (JoviosPlayer player){
		myPlayer = player.GetNetworkPlayer();
		playerNumber = player.GetPlayerNumber();
		primary = player.GetColor("primary");
		secondary = player.GetColor("secondary");
		playerName = player.GetPlayerName();
		body = transform.FindChild("Body");
		body.GetComponent<SumoCollision>().startPosition = body.position;
		robot = body.FindChild("Robot1");
		for(int i = 0; i < robot.childCount; i++){
			if(robot.GetChild(i).name == "Sphere_008"){
				robot.GetChild(i).GetChild(0).renderer.material.color = primary;
				robot.GetChild(i).GetChild(1).renderer.material.color = primary;
				robot.GetChild(i).GetChild(2).renderer.material.color = primary;
			}
			else if(robot.GetChild(i).name == "Sphere_009"){
				robot.GetChild(i).GetChild(0).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(1).renderer.material.color = secondary;
				robot.GetChild(i).GetChild(2).renderer.material.color = secondary;
			}
			else{
				robot.GetChild(i).renderer.material.color = Color.grey;
			}
		}
		robot = robot.GetChild(13);
	}
	
	public void Attack(){
		is_attacking = true;
		attackTime = Time.time;
	}
}
