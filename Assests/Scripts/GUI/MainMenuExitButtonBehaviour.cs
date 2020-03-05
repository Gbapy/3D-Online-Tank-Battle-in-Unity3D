using UnityEngine;
using System.Collections;
using MagicBattle;

public class MainMenuExitButtonBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.mainMenuFlag){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * 0.3f,Screen.height * 0.08f,Screen.width * 0.25f,Screen.height * 0.05f);
		}else{
			guiTexture.enabled = false;	
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Exit_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Buttons/Exit_1");
		if(Network.isServer){
			MasterServer.UnregisterHost();
			GlobalInfo.disconnected = true;
			Application.Quit();
		}else{
			GlobalInfo.rpcControl.RPC("OnDisconnectPlayerRPC",RPCMode.Server,GlobalInfo.playerViewID,GlobalInfo.userInfo.name,true);	
			Network.Disconnect();
		}	
	}			
}
