using UnityEngine;
using System.Collections;
using MagicBattle;

public class OptionWindowBackButtonBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.optionWindowFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.11f,-Screen.height * 0.2f,Screen.width * 0.08f,Screen.height * 0.045f);	
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/btn_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/btn_1");
		GlobalInfo.optionWindowFlag = false;
		GlobalInfo.mainMenuFlag = true;
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}	
}
