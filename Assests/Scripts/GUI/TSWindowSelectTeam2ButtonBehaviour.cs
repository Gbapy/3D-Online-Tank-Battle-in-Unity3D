using UnityEngine;
using System.Collections;
using MagicBattle;

public class TSWindowSelectTeam2ButtonBehaviour : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.teamSelWindowFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.09f,-Screen.height * 0.28f,Screen.width * 0.08f,Screen.height * 0.04f);			
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_1");
		GlobalInfo.userInfo.team = TeamKind.Thunder;
		GlobalInfo.userInfo.joined = true;
		GlobalInfo.userInfo.isReady = false;
		if(Network.isServer){
			GlobalInfo.userInfoList[0].team = GlobalInfo.userInfo.team;
			GlobalInfo.userInfoList[0].score = 0;
			GlobalInfo.userInfoList[0].hitCount = 0;
			GlobalInfo.userInfoList[0].shootCount = 0;
			GlobalInfo.userInfoList[0].joined = true;
			GlobalInfo.userInfoList[0].destroyed = false;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);			
		}else
			GlobalInfo.rpcControl.RPC("OnUpdateUserTeamRPC",RPCMode.Server,GlobalInfo.userInfo.name,(int)GlobalInfo.userInfo.team);		
		GlobalInfo.teamSelWindowFlag = false;
		GlobalInfo.equipSelWindowFlag = true;
		foreach(Transform tr in GlobalInfo.equipSelWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
		GlobalInfo.exhibitionFlag = true;	
		foreach(Transform tr in GlobalInfo.exhibitionWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}		
	}
}
