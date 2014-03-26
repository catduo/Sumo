using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialArena : MonoBehaviour {

	public static List<int> playersInsideGreenArea = new List<int>();
	public GameObject bonusSpawner;
	public GameObject bumper;
	public float bonusSpawnTimer;
	public int bonusSpawnNum = 0;
	public static int tutorialStep = 0;
	public static int tutorialTime = 0;

	// Use this for initialization
	void Start () {
		GameObject.Find("CountdownCorner").GetComponent<Countdown>().countTime = 10;
		StartCoroutine("ToStepOne");
		GameObject.Find("Tutorial").GetComponent<UIPanel>().enabled = true;
		GameObject.Find("Tutorial").transform.FindChild("Container").FindChild("Label").GetComponent<UILabel>().text = "Move your robot to the green area in the arena by using the left joystick.";
	}

	// Update is called once per frame
	void Update () {
		if(playersInsideGreenArea.Count >= GameObject.Find("PlayerObjects").transform.childCount && tutorialStep == 1){
			StepTwo();
		}
		if(tutorialStep == 3 && transform.FindChild("Bumpers").childCount == 0){
			StepFour();
		}
		if(bonusSpawnTimer + 0.5F < Time.time && MenuManager.gameState == GameState.GameOn && tutorialStep == 4){
			bonusSpawnTimer = Time.time;
			GameObject.Find("BonusSpawners").transform.GetChild(bonusSpawnNum).FindChild("BonusSpawner").GetComponent<BonusSpawner>().bonusType = (BonusType)bonusSpawnNum;
			bonusSpawnNum++;
			if(bonusSpawnNum > 4){
				bonusSpawnNum = 0;
			}
		}
	}

	public IEnumerator ToStepOne(){
		yield return new WaitForSeconds(1);
		tutorialStep = 1;
	}

	void StepTwo(){
		tutorialStep = 2;
		GameObject.Find("Tutorial").transform.FindChild("Container").FindChild("Label").GetComponent<UILabel>().text = "Bumpers have spawned at the places where you started.  Run into them to see how they will throw you around.  The next part of the tutorial starts in 10 seconds.";
		for(int i = 0; i < GameObject.Find("PlayerSpawners").transform.childCount; i++){
			GameObject newBumper = (GameObject) GameObject.Instantiate(bumper, Vector3.zero, Quaternion.identity);
			newBumper.transform.eulerAngles = new Vector3(90,0,0);
			newBumper.transform.parent = transform.FindChild("Bumpers");
			newBumper.transform.localPosition = GameObject.Find("PlayerSpawners").transform.GetChild(i).position;
			newBumper.transform.localScale = new Vector3(1,0.5F,1);
		}
		StartCoroutine("ToStepThree");
	}

	public IEnumerator ToStepThree(){
		GameObject.Find("CountdownTutorial").GetComponent<Countdown>().StartCountdown(10);
		yield return new WaitForSeconds(10);
		StepThree();
	}

	void StepThree(){
		tutorialStep = 3;
		GameObject.Find("Tutorial").transform.FindChild("Container").FindChild("Label").GetComponent<UILabel>().text = "Now shoot at the bumpers using the right joystick.  Hold to charge, move to aim, release to fire.  You can tap quickly to fire quickly.  Destroy all of them to continue";
	}

	void StepFour(){
		tutorialStep = 4;
		for(int i = 0; i < GameObject.Find("PlayerSpawners").transform.childCount; i+=2){
			GameObject newBonusSpawner = (GameObject) GameObject.Instantiate(bonusSpawner, Vector3.zero, Quaternion.identity);
			newBonusSpawner.transform.parent = transform.FindChild("BonusSpawners").transform;
			newBonusSpawner.transform.localPosition = GameObject.Find("PlayerSpawners").transform.GetChild(i).position;
			newBonusSpawner.transform.localScale = new Vector3(1,1,1);
		}
		StartCoroutine("ToStepFive");
		GameObject.Find("Tutorial").transform.FindChild("Container").FindChild("Label").GetComponent<UILabel>().text = "Try out the different powerups.  They will be spawning where the bumpers were.  The next part of the turorial will start in 30 seconds.";
	}
	
	public IEnumerator ToStepFive(){
		GameObject.Find("CountdownTutorial").GetComponent<Countdown>().StartCountdown(30);
		yield return new WaitForSeconds(30);
		StepFive();
	}

	void StepFive(){
		GameObject.Find("CountdownCorner").GetComponent<Countdown>().StartCountdown(30);
		Destroy(GameObject.Find ("Walls"));
		GameObject.Find("Tutorial").transform.FindChild("Container").FindChild("Label").GetComponent<UILabel>().text = "The walls have been removed from this arena, use the powerups to knock each other off.  You get a point if you were the last to touch someone before they get knowcked off.";
	}
	
	void OnDisable(){
		if(GameObject.Find("Tutorial") != null){
			GameObject.Find("Tutorial").GetComponent<UIPanel>().enabled = false;
		}
		tutorialStep = 0;
		playersInsideGreenArea = new List<int>();
	}
}
