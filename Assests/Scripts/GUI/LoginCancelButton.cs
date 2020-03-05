using UnityEngine;
using System.Collections;
using MagicBattle;

public class LoginCancelButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!GlobalInfo.logged && GlobalInfo.serverFound){
			if(!GlobalInfo.warnFlag){
				guiTexture.enabled = true;
				guiTexture.pixelInset = new Rect(Screen.width * 0.04f,-Screen.height * 0.11f,Screen.width * 0.06f,Screen.height * 0.04f);
			}else
				guiTexture.enabled = false;
		}
	}

	void OnGUI () {
		Event e = Event.current;
		if(e.isKey)
			if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)OnMouseUp();
	}
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Login/Btn_4");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Login/Btn_3");
		GlobalInfo.disconnected = true;
		Application.Quit();
	}
}
