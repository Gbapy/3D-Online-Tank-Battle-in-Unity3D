using UnityEngine;
using System.Collections;
using MagicBattle;
	
public class HelpWindowBackButtonBehaviour : MonoBehaviour {
	public GUITexture backGround;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.helpWindowFlag){
			guiTexture.enabled = true;
			backGround.enabled = true;
			backGround.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
			guiTexture.pixelInset = new Rect(Screen.width * 0.18f,-Screen.height * 0.275f,Screen.width * 0.08f,Screen.height * 0.04f);			
		}else{
			guiTexture.enabled = false;
			backGround.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/btn_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/btn_1");		
		GlobalInfo.helpWindowFlag = false;
		GlobalInfo.mainMenuFlag = true;
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}
}
