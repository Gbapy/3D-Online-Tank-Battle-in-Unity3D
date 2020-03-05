using UnityEngine;
using System.Collections;
using MagicBattle;
	
public class PromptWindowBackButtonBehaviour : MonoBehaviour {
	void Update () {
		if(GlobalInfo.promptFlag){
			if(GlobalInfo.gameStarted && GlobalInfo.gameEnded){
				guiTexture.enabled = true;
				guiTexture.pixelInset = new Rect(Screen.width * 0.22f,-Screen.height * 0.35f,Screen.width * 0.08f,Screen.height * 0.04f);
				Camera.mainCamera.GetComponent<UltraRedRayCamera>().enabled = false;
				Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
			}
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		if(GlobalInfo.gameStarted && GlobalInfo.gameEnded)
			guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/btn_2");
	}
	
	void OnMouseUp() {
		if(GlobalInfo.gameStarted && GlobalInfo.gameEnded){
			GameObject[] go = GameObject.FindGameObjectsWithTag("map");
			foreach(GameObject a in go){
				DestroyImmediate(a);
			}	
			go = GameObject.FindGameObjectsWithTag("Broken");
			foreach(GameObject a in go){
				DestroyImmediate(a);
			}			
			if(GlobalInfo.myPlayer != null) {
				Camera.mainCamera.transform.parent = null;
				Network.RemoveRPCs(GlobalInfo.playerViewID);
				Network.Destroy(GlobalInfo.playerViewID);

				GlobalInfo.myPlayer = null;
				GlobalInfo.destroyed = true;
			}			
			guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/btn_1");
			GlobalInfo.promptFlag = false;
			GlobalInfo.gameStarted = false;
			GlobalInfo.equipSelWindowFlag = true;
			foreach(Transform tr in GlobalInfo.equipSelWindow){
				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
			}
			GlobalInfo.exhibitionFlag = true;
			foreach(Transform tr in GlobalInfo.exhibitionWindow){
				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
			}
			GlobalInfo.playerLoaded = false;
		}
	}
}
