using UnityEngine;
using System.Collections;



public class Sumo : MonoBehaviour {

	public float scale = 1;
	public float speed = 0.7F;
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
	
	public JoviosUserID myPlayer;
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
	
	private bool is_strong = false;
	private bool is_range = false;
	private bool is_rampage = false;
	
	private Jovios jovios;

	// Use this for initialization
	void Start () {
		jovios = GameManager.jovios;
		scale = 0.7F + 0.5F / (jovios.GetPlayerCount());
		body = transform.FindChild("Body");
		crown = body.FindChild("Crown");
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
		if(MenuManager.gameState == GameState.ChooseArena){
			hand.collider.enabled = false;
			hand.FindChild("Sphere").renderer.enabled = false;
		}
		else if(!is_rampage){
			hand.collider.enabled = true;
			hand.FindChild("Sphere").renderer.enabled = true;
		}
		if(is_boostActive){
			is_boostActive = false;
			scale = 0.7F + 0.5F / (jovios.GetPlayerCount());
			speed = 0.7F;
			range = 1;
			strength = 1;
			defense = 1;
			body.transform.name = "Body";
			hand.collider.enabled = true;
			hand.FindChild("Sphere").renderer.enabled = true;
			hand.GetComponent<Projectile>().Normal();
			primary = new Color(primary.r, primary.g, primary.b, 1);
			secondary = new Color(secondary.r, secondary.g, secondary.b, 1);
			Shader robotShader = Shader.Find("Reflective/Specular");
			Color metal = Color.grey;
			is_strong = false;
			is_range = false;
			is_rampage = false;
			switch(activeBoost){
			case BonusType.Speed:
				speed = 1.2F;
				break;
			
			case BonusType.Immunity:
				robotShader = Shader.Find("Transparent/Diffuse");
				defense = 100;
				primary = new Color(primary.r, primary.g, primary.b, 0.5F);
				secondary = new Color(secondary.r, secondary.g, secondary.b, 0.5F);
				break;
			
			case BonusType.Rampage:
				scale = 1.5F * (0.7F + 0.5F / (jovios.GetPlayerCount()));
				defense = 3;
				speed = 0.8F;
				is_rampage = true;
				metal = Color.red;
				body.transform.name = "Rampage";
				break;
			
			case BonusType.Strength:
				is_strong = true;
				break;
			
			case BonusType.Range:
				is_range = true;
				range = 4;
				break;
			}
			robot = body.FindChild("Robot1");
			for(int i = 0; i < robot.childCount; i++){
				if(robot.GetChild(i).name == "Sphere_008"){
					robot.GetChild(i).GetChild(0).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(1).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(2).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(0).renderer.material.color = primary;
					robot.GetChild(i).GetChild(1).renderer.material.color = primary;
					robot.GetChild(i).GetChild(2).renderer.material.color = primary;
				}
				else if(robot.GetChild(i).name == "Sphere_009"){
					robot.GetChild(i).GetChild(0).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(1).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(2).renderer.material.shader = robotShader;
					robot.GetChild(i).GetChild(0).renderer.material.color = secondary;
					robot.GetChild(i).GetChild(1).renderer.material.color = secondary;
					robot.GetChild(i).GetChild(2).renderer.material.color = secondary;
				}
				else{
					robot.GetChild(i).renderer.material.shader = robotShader;
					robot.GetChild(i).renderer.material.color = metal;
				}
			}
			robot = robot.GetChild(13);
			body.localScale = Vector3.one * scale;
		}
		else if(boostStart + boostDuration < Time.time){
			primary = new Color(primary.r, primary.g, primary.b, 1);
			secondary = new Color(secondary.r, secondary.g, secondary.b, 1);
			scale = 0.7F + 0.5F / (jovios.GetPlayerCount());
			speed = 0.7F;
			range = 1;
			strength = 1;
			defense = 1;
			body.transform.name = "Body";
			hand.collider.enabled = true;
			hand.FindChild("Sphere").renderer.enabled = true;
			hand.GetComponent<Projectile>().Normal();
			body.localScale = Vector3.one * scale;
			is_strong = false;
			is_range = false;
			is_rampage = false;
			robot = body.FindChild("Robot1");
			for(int i = 0; i < robot.childCount; i++){
				if(robot.GetChild(i).name == "Sphere_008"){
					robot.GetChild(i).GetChild(0).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(1).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(2).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(0).renderer.material.color = primary;
					robot.GetChild(i).GetChild(1).renderer.material.color = primary;
					robot.GetChild(i).GetChild(2).renderer.material.color = primary;
				}
				else if(robot.GetChild(i).name == "Sphere_009"){
					robot.GetChild(i).GetChild(0).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(1).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(2).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).GetChild(0).renderer.material.color = secondary;
					robot.GetChild(i).GetChild(1).renderer.material.color = secondary;
					robot.GetChild(i).GetChild(2).renderer.material.color = secondary;
				}
				else{
					robot.GetChild(i).renderer.material.shader = Shader.Find("Reflective/Specular");
					robot.GetChild(i).renderer.material.color = Color.grey;
				}
			}
			robot = robot.GetChild(13);
			activeBoost = BonusType.None;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!body.GetComponent<SumoCollision>().is_ringOut && (MenuManager.gameState == GameState.ChooseArena || MenuManager.gameState == GameState.GameOn || MenuManager.gameState == GameState.SuddenDeath)){	
			if(MenuManager.gameState != GameState.ChooseArena){
				if(is_strong){
					hand.GetComponent<Projectile>().Strong();
				}
				else if(is_rampage){
					for(int i = 0; i < hand.childCount; i++){
						hand.GetChild(i).renderer.enabled = false;
					}
					hand.collider.enabled = false;
				}
				else{
					hand.GetComponent<Projectile>().Normal();
				}
				if(body.rigidbody.velocity.magnitude < 1){
					body.rigidbody.velocity = new Vector3(0, 0, body.rigidbody.velocity.z);
				}
				else{
					body.rigidbody.velocity *= 0.95F;
				}
				if(jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("right").GetDirection() != Vector2.zero){
					if(!is_attacking){
						attackPower++;
					}
					if(jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("right").GetDirection().y > 0){
						body.eulerAngles = new Vector3(body.eulerAngles.y, body.eulerAngles.x, Vector2.Angle(new Vector2(1,0), jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("right").GetDirection()) - 90);
					}
					else{
						body.eulerAngles = new Vector3(body.eulerAngles.y, body.eulerAngles.x, - Vector2.Angle(new Vector2(1,0), jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("right").GetDirection()) - 90);
					}
				}
			}
			transform.Translate( new Vector3(speed / 10 * jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("left").GetDirection().x, speed / 10 * jovios.GetPlayer(playerNumber).GetControllerStyle().GetDirection("left").GetDirection().y, 0));
			body.rigidbody.angularVelocity = Vector3.zero;
			float handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
			hand.localScale = new Vector3(handScale, handScale, handScale);
			if(is_attacking){
				if(is_range){
					hand.GetComponent<Projectile>().Range();
				}
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
		myPlayer = player.GetUserID();
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

	void OnDisable(){
		if(jovios.GetPlayer(playerNumber) != null){
			jovios.GetPlayer(playerNumber).RemovePlayerObject(gameObject);
			if(jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left") != null){
				jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("left").SetDirection(Vector2.zero);
			}
			if(jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("right") != null){
			jovios.GetPlayer(myPlayer).GetControllerStyle().GetDirection("right").SetDirection(Vector2.zero);
			}
		}
	}
}
