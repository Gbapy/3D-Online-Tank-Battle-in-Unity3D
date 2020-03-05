using UnityEngine;
using System.Collections;
using MagicBattle;

public class TSWindowBackButtonBehaviour : MonoBehaviour {
	public GUITexture bgImage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.teamSelWindowFlag){		
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.2217f,-Screen.height * 0.3937f,Screen.width * 0.0888f,Screen.height * 0.043668f);			
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/btn_2");
	}
	
	IEnumerator OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/btn_1");		
		GlobalInfo.teamSelWindowFlag = false;
		GlobalInfo.battleFieldSelected = false;
		AsyncOperation async = Application.LoadLevelAsync (0); 
		yield return async;
		if(Network.isClient){
			GlobalInfo.mainMenuFlag = true;
			foreach(Transform tr in GlobalInfo.mainMenu){
				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
			}
		}else{
			GlobalInfo.rpcControl.RPC("OnChangeBattleFieldRPC",RPCMode.Others);
			foreach(UserInfoClass uf in GlobalInfo.userInfoList){
				uf.destroyed = false;
				uf.isReady = false;
				uf.joined = false;
			}
			GlobalInfo.gameEnded = true;
			GlobalInfo.gameStarted = false;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
			GlobalInfo.battleFieldSelWindowFlag = true;
			foreach(Transform tr in GlobalInfo.battleFieldSelWindow){
				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
			}
		}
		bgImage.guiTexture.enabled = true;
	}
}
