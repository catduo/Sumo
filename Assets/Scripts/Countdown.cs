using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {

	public int countTime;

	IEnumerator DecrementSecond(){
		yield return new WaitForSeconds(1);
		if(name != "CountdownCorner" || GameObject.Find ("Countdown").GetComponent<Countdown>().countTime < 1){
			countTime --;
			transform.FindChild("Label").GetComponent<UILabel>().text = countTime.ToString();
			if(countTime <= 0){
				GetComponent<UIPanel>().enabled = false;
			}
			else{
				StartCoroutine("DecrementSecond");
			}
		}
		else{
			StartCoroutine("DecrementSecond");
		}
	}

	public void StartCountdown(int seconds){
		countTime = seconds;
		transform.FindChild("Label").GetComponent<UILabel>().text = countTime.ToString();
		GetComponent<UIPanel>().enabled = true;
		StartCoroutine("DecrementSecond");
	}
}
