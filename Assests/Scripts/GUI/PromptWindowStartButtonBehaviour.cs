using UnityEngine;
using System.Collections;
using MagicBattle;

public class PromptWindowStartButtonBehaviour : MonoBehaviour {
	public GameObject mapPref;
	public GameObject[] equipments;

	// Update is called once per frame
	void Update () {
		if(GlobalInfo.promptFlag){
			if(Network.isServer){
				if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded){
					guiTexture.enabled = true;
					guiTexture.pixelInset = new Rect(Screen.width * 0.22f,-Screen.height * 0.35f,Screen.width * 0.08f,Screen.height * 0.04f);
					if(IsReady()){
						guiTexture.color = new Color(guiTexture.color.r,guiTexture.color.g,guiTexture.color.b,0.9f);
					}else{
						guiTexture.color = new Color(guiTexture.color.r,guiTexture.color.g,guiTexture.color.b,0.1f);
					}
				}else{
					guiTexture.enabled = false;
					enabled = false;
				}
			}
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded && IsReady())
			guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Start1_02");
	}
	
	void OnMouseUp() {
		if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded && IsReady()){
			guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Start1_01");
			Screen.lockCursor = true;
			int aCount = 0;
			int bCount = 0;
//			if(Network.isServer) {
//				foreach(UserInfoClass uInfo in GlobalInfo.userInfoList) {
//					if(uInfo.team == TeamKind.Lightening)
//						aCount++;
//					else
//						bCount++;
//				}
//				UserInfoClass uf;
//				GameObject[] go = GameObject.FindGameObjectsWithTag("InitPosA");
//				for(int i = aCount+1;i<6;i++){
//					int n = Random.Range(0,15);
//					GameObject a = (GameObject)Network.Instantiate(equipments[n],go[i].transform.position,go[i].transform.rotation,0);
//					uf = new UserInfoClass();
//					uf.name = "Tank" + i.ToString();
//					uf.playerViewID = a.networkView.viewID;
//					uf.isReady = true;
//					uf.joined = true;
//					uf.team = TeamKind.Lightening;
//					uf.aiFlag = true;
//					GlobalInfo.userInfoList.Add(uf);
//					a.SendMessage("OnSetName",uf.name,SendMessageOptions.DontRequireReceiver);
//					a.SendMessage("OnSetState",TankControlState.AutoDrive,SendMessageOptions.DontRequireReceiver);
//					a.GetComponent<AITankBehaviour>().enabled = true;
//				}
//				go = GameObject.FindGameObjectsWithTag("InitPosB");
//				for(int i = aCount+1;i<6;i++){
//					int n = Random.Range(0,15);
//					GameObject a = (GameObject)Network.Instantiate(equipments[n],go[i].transform.position,go[i].transform.rotation,0);
//					uf = new UserInfoClass();
//					uf.name = "Tank" + i.ToString();
//					uf.playerViewID = a.networkView.viewID;
//					uf.isReady = true;
//					uf.joined = true;
//					uf.team = TeamKind.Thunder;
//					uf.aiFlag = true;
//					GlobalInfo.userInfoList.Add(uf);
//					a.SendMessage("OnSetName",uf.name,SendMessageOptions.DontRequireReceiver);
//					a.SendMessage("OnSetState",TankControlState.AutoDrive,SendMessageOptions.DontRequireReceiver);
//					a.GetComponent<AITankBehaviour>().enabled = true;
//				}
//			}
			Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
			GlobalInfo.rpcControl.RPC("OnGameStartedRPC",RPCMode.All);
		}
	}
	
	bool IsReady() {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			if(!uf.isReady) return false;
		}
		return true;
	}
}
