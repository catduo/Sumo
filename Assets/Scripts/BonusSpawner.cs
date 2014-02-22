using UnityEngine;
using System.Collections;

public enum BonusType{
	Speed,
	Immunity,
	Rampage,
	Strength,
	Range,
	None
}

public class BonusSpawner : MonoBehaviour {
	
	public BonusType bonusType = BonusType.None;
	public Transform bonusBox;
	
	// Use this for initialization
	void Start () {
		bonusBox = transform.FindChild("BonusBox");
	}
	
	// Update is called once per frame
	void Update () {
		if(MenuManager.gameState == GameState.SuddenDeath){
			bonusType = BonusType.None;
		}
		if(bonusType != BonusType.None){
			bonusBox.collider.enabled = true;
			bonusBox.localPosition = new Vector3(0, 0, Mathf.Sin(Time.time)/6 + 0.3F);
			bonusBox.Rotate(new Vector3(0, 0, 1));
			bonusBox.GetChild(0).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(1).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(2).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(3).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(4).GetChild(0).renderer.enabled = false;
			bonusBox.FindChild(bonusType.ToString()).GetChild(0).renderer.enabled = true;
		}
		else{
			bonusBox.collider.enabled = false;
			bonusBox.GetChild(0).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(1).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(2).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(3).GetChild(0).renderer.enabled = false;
			bonusBox.GetChild(4).GetChild(0).renderer.enabled = false;
		}
	}
}
