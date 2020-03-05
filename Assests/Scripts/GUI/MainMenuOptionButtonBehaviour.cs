using UnityEngine;
using System.Collections;
using MagicBattle;

public class MainMenuOptionButtonBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.mainMenuFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.3f,Screen.height * 0.2f,Screen.width * 0.25f,Screen.height * 0.05f);
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Option_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Option_1");
		GlobalInfo.mainMenuFlag = false;
		GlobalInfo.optionWindowFlag = true;
		foreach(Transform tr in GlobalInfo.optionWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}	
}
