using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

	float scalingFactor;
	bool is_scaling;
	Vector3 baseScale;

	void Start(){
		baseScale = transform.localScale;
	}

	void Update(){
		if(is_scaling){
			transform.localScale = new Vector3(scalingFactor * transform.localScale.x, transform.localScale.y / scalingFactor, scalingFactor * transform.localScale.z);
			if(transform.localScale.x <= baseScale.x){
				transform.localScale = baseScale;
				is_scaling = false;
			}
			if(transform.localScale.x >= baseScale.x * 1.5F){
				StartCoroutine("AnimateBack");
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if(TutorialArena.tutorialStep == 3){
			if(col.name == "Hand"){
				Destroy(gameObject);
			}
		}
		else{
			switch(col.name){
			case "Hand":
				break;
			case "Body":
				StartCoroutine("Animate");
				break;
			case "Rampage":
				StartCoroutine("Animate");
				break;
			}
		}
	}
	public IEnumerator Animate(){
		is_scaling = true;	
		scalingFactor = 1.1F;
		yield return new WaitForSeconds(0.05F);
		StartCoroutine("AnimateBack");
		
	}
	public IEnumerator AnimateBack(){
		scalingFactor = 0.95F;
		yield return new WaitForSeconds(0.2F);
	}
}
