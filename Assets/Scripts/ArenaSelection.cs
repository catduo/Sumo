using UnityEngine;
using System.Collections;

public class ArenaSelection : MonoBehaviour {
	
	public void ChooseArena1(){
		GameManager.ChooseArena(1);
	}
	public void ChooseArena2(){
		GameManager.ChooseArena(2);
	}
	public void ChooseArena3(){
		GameManager.ChooseArena(3);
	}
	public void ChooseArena4(){
		GameManager.ChooseArena(4);
	}
}
