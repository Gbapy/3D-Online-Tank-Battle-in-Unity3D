using UnityEngine;
using System.Collections;
using MagicBattle;

public class BFSelWindowBAckButtonBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.battleFieldSelWindowFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.18f,-Screen.height * 0.3f,Screen.width * 0.08f,Screen.height * 0.04f);			
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}	
	}

	void OnGUI() {
		Event e = Event.current;
		if(e.isKey)
			if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)OnMouseUp();
	}
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/BattleFields/btn_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/BattleFields/btn_1");		
		GlobalInfo.battleFieldSelWindowFlag = false;
		GlobalInfo.mainMenuFlag = true;
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}
}
