using UnityEngine;
using System.Collections;
using MagicBattle;

public class MainMenuHelpButtonBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.mainMenuFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.3f,Screen.height * 0.14f,Screen.width * 0.25f,Screen.height * 0.05f);
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Help_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Help_1");
		GlobalInfo.mainMenuFlag = false;
		GlobalInfo.helpWindowFlag = true;
		foreach(Transform tr in GlobalInfo.helpWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}		
}
