using UnityEngine;
using System.Collections;
using MagicBattle;

public class MainMenuStartButtonBehaviour : MonoBehaviour {
	public GUITexture bgImage;
	
	// Use this for initialization
	void Start () {
		if(Network.isClient){
			GlobalInfo.rpcControl.RPC("OnRequestBattleFieldRPC",RPCMode.Server);
		}
		if(GlobalInfo.myPlayer != null) {
			Camera.mainCamera.transform.parent = null;
			Network.RemoveRPCs(GlobalInfo.playerViewID);
			Network.Destroy(GlobalInfo.playerViewID);
			GlobalInfo.myPlayer = null;
			GlobalInfo.destroyed = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.mainMenuFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.3f,Screen.height * 0.26f,Screen.width * 0.25f,Screen.height * 0.05f);
			bgImage.enabled = true;
			bgImage.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
			if(Network.isServer){
				guiTexture.color = new Color(guiTexture.color.r,guiTexture.color.g,guiTexture.color.b,0.9f);				
			}else{
				if(GlobalInfo.battleFieldSelected){
					guiTexture.color = new Color(guiTexture.color.r,guiTexture.color.g,guiTexture.color.b,0.9f);				
				}else{
					guiTexture.color = new Color(guiTexture.color.r,guiTexture.color.g,guiTexture.color.b,0.1f);				
				}
			}
			Screen.showCursor = true;
			Screen.lockCursor = false;
			GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerTank");
			foreach(GameObject a in go){
				if(a.networkView.isMine){
					Network.RemoveRPCs(a.networkView.viewID);
					Network.Destroy(a.networkView.viewID);
				}
			}
			GlobalInfo.myPlayer = null;
			GlobalInfo.destroyed = true;
			GlobalInfo.battleFieldSelWindowFlag = false;
			GlobalInfo.chatScreenFlag = false;
			GlobalInfo.equipSelWindowFlag = false;
			GlobalInfo.exhibitionFlag = false;
			GlobalInfo.helpWindowFlag = false;
			GlobalInfo.optionWindowFlag = false;
			GlobalInfo.promptFlag = false;
			GlobalInfo.teamSelWindowFlag = false;
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		if(Network.isServer){
			guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/start_2");
		}else{
			if(GlobalInfo.battleFieldSelected){
				guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/start_2");
			}
		}
	}
	
	IEnumerator OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/start_1");
		if(Network.isClient){
			if(GlobalInfo.battleFieldSelected) {
				GlobalInfo.mainMenuFlag = false;
				AsyncOperation async = Application.LoadLevelAsync ((int)GlobalInfo.curBattleField + 1); 
				yield return async;			
				bgImage.guiTexture.enabled = false;
				GlobalInfo.teamSelWindowFlag = true;
				foreach(Transform tr in GlobalInfo.teamSelWindow){
					tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
				}				
			}
		}else{
			GlobalInfo.mainMenuFlag = false;
			GlobalInfo.battleFieldSelWindowFlag = true;
			foreach(Transform tr in GlobalInfo.battleFieldSelWindow){
				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
			}			
		}
	}
	
	void OnEnable() {
		Screen.lockCursor = false;
		if(Network.isClient){
			GlobalInfo.rpcControl.RPC("OnRequestBattleFieldRPC",RPCMode.Server);	
		}
	}
}
