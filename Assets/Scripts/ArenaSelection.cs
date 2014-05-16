using UnityEngine;
using System.Collections;

public class ArenaSelection : MonoBehaviour {
	public static bool is_label = false;
	void Update(){
		GameObject.Find("ArenaSelectionLabel").GetComponent<UILabel>().enabled = is_label;
	}
	public void ChooseArena1(){
		GameObject.Find("Logo").GetComponent<UIPanel>().enabled = false;
		GameManager.ChooseArena(1);
	}
	public void ChooseArena2(){
		GameObject.Find("Logo").GetComponent<UIPanel>().enabled = false;
		GameManager.ChooseArena(2);
	}
	public void ChooseArena3(){
		GameObject.Find("Logo").GetComponent<UIPanel>().enabled = false;
		GameManager.ChooseArena(3);
	}
	public void ChooseArena4(){
		GameObject.Find("Logo").GetComponent<UIPanel>().enabled = false;
		GameManager.ChooseArena(4);
	}
}
