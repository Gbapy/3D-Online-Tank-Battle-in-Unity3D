using UnityEngine;
using System.Collections;
using MagicBattle;

public class ESWindowBAckButtonBehaviour : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.equipSelWindowFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.35f,-Screen.height * 0.46f,Screen.width * 0.08f,Screen.height * 0.04f);			
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}		
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Equipments/btn_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Equipments/btn_1");
		GlobalInfo.exhibitionFlag = false;
		if(GlobalInfo.curExhibitionEquip != null)
			DestroyImmediate(GlobalInfo.curExhibitionEquip);		
		GlobalInfo.equipSelWindowFlag = false;
		GlobalInfo.userInfo.joined = false;
		if(Network.isServer){
			GlobalInfo.userInfoList[0].joined = false;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);			
		}else
			GlobalInfo.rpcControl.RPC("OnUpdateUserJoinedRPC",RPCMode.Server,GlobalInfo.userInfo.name,false);
		GlobalInfo.teamSelWindowFlag = true;
		foreach(Transform tr in GlobalInfo.teamSelWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}
}
