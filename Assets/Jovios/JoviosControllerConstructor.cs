using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JoviosControllerConstructorType{
	Joystick,
	Button,
	Label
}

public class JoviosControllerConstructor : MonoBehaviour {
	public JoviosControllerConstructorType jcct;
	public void AddControllerComponent(JoviosControllerStyle jcs){
		switch(jcct){
		case JoviosControllerConstructorType.Button:
			jcs.AddButton1(new Vector2(transform.localPosition.x, transform.localPosition.y), new Vector2(transform.GetComponent<UIWidget>().width, transform.GetComponent<UIWidget>().height), "mc", transform.FindChild("Label").GetComponent<UILabel>().text, transform.name, color: transform.FindChild("Button1").GetComponent<UITexture>().color.ToString(), depth: transform.GetComponent<UIWidget>().depth);
			break;

		case JoviosControllerConstructorType.Joystick:
			jcs.AddJoystick(new Vector2(transform.localPosition.x, transform.localPosition.y), new Vector2(transform.GetComponent<UIWidget>().width, transform.GetComponent<UIWidget>().height), "mc", transform.name, depth: transform.GetComponent<UIWidget>().depth);
			break;

		case JoviosControllerConstructorType.Label:
			jcs.AddLabel(new Vector2(transform.localPosition.x, transform.localPosition.y), new Vector2(transform.GetComponent<UILabel>().width, transform.GetComponent<UILabel>().height), "mc", transform.GetComponent<UILabel>().text, color: transform.GetComponent<UILabel>().color.ToString(), depth: transform.GetComponent<UILabel>().depth, fontSize: transform.GetComponent<UILabel>().fontSize);
			break;
		}
	}
}