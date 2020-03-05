using UnityEngine;
using System.Collections;
using MagicBattle;

public class ESWindowSelectButtonBehaviour : MonoBehaviour {
	public GUITexture equipmentBar;
	public GUITexture detailWindow;
	public GUITexture backImage;
	public GUIStyle selButtonStyle;
	public GUIContent selButtonContent;
	public GUIStyle prevButtonStyle;
	public GUIStyle nextButtonStyle;
	public Vector2 scrollPos = Vector2.zero;
	public Transform exhibitionPos;
	public GameObject[] exhibitionEquips;
	public GameObject exhibitionCam;
	public float[] distances;
	public float[] heights;
	public GameObject[] equipments;
	
	private GUIStyle scrollStyle = new GUIStyle();
	
	void Start() {

	}
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.equipSelWindowFlag){
			equipmentBar.enabled = true;
			detailWindow.enabled = true;
			guiTexture.enabled = true;
			backImage.enabled = true;
			equipmentBar.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height * 0.18650089f);
			detailWindow.pixelInset = new Rect(Screen.width * 0.17f,-Screen.height * 0.27f,Screen.width * 0.23f,Screen.height * 0.64f);
			guiTexture.pixelInset = new Rect(Screen.width * 0.35f,-Screen.height * 0.4f,Screen.width * 0.08f,Screen.height * 0.04f);			
			backImage.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
		}else{
			equipmentBar.enabled = false;
			detailWindow.enabled = false;
			guiTexture.enabled = false;
			backImage.enabled = false;
			enabled = false;
		}	
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Equipments/Select_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Equipments/Select_1");
		if(Network.isServer)
			RecieveGameStartedState(false);
		else
			GlobalInfo.rpcControl.RPC("RequestGameStartedStateRPC",RPCMode.Server);
	}
	
	void OnEnable() {
		GlobalInfo.userInfo.equipment = EquipmentKind.KV1;
		GlobalInfo.curExhibitionEquip = (GameObject)Instantiate(exhibitionEquips[3],exhibitionPos.position,exhibitionPos.rotation);
		exhibitionCam.SendMessage("OnSetCamDistance",distances[3],SendMessageOptions.DontRequireReceiver);
		exhibitionCam.SendMessage("OnSetCamHeight",heights[3],SendMessageOptions.DontRequireReceiver);			
	}
	
	void OnGUI() {
		if(GlobalInfo.equipSelWindowFlag){
			GUI.skin.horizontalScrollbar = scrollStyle;
			GUI.skin.horizontalScrollbarLeftButton = scrollStyle;
			GUI.skin.horizontalScrollbarRightButton = scrollStyle;
			GUI.skin.horizontalScrollbarThumb = scrollStyle;
			GUI.skin.verticalScrollbar = scrollStyle;
			GUI.skin.verticalScrollbarDownButton = scrollStyle;
			GUI.skin.verticalScrollbarUpButton = scrollStyle;
			GUI.skin.verticalScrollbarThumb = scrollStyle;
			if(GUI.RepeatButton(new Rect(Screen.width * 0.01f,Screen.height * 0.85f,Screen.width * 0.03f,Screen.height * 0.11f),selButtonContent,prevButtonStyle)){
				scrollPos = new Vector2(Mathf.Clamp(scrollPos.x - Screen.width * 0.02f,0,Screen.width * 2f),scrollPos.y);
			}
			if(GUI.RepeatButton(new Rect(Screen.width * 0.76f,Screen.height * 0.85f,Screen.width * 0.03f,Screen.height * 0.11f),selButtonContent,nextButtonStyle)){
				scrollPos = new Vector2(Mathf.Clamp(scrollPos.x + Screen.width * 0.02f,0,Screen.width * 2f),scrollPos.y);
			}
			GUI.Box(new Rect(Screen.width * 0.05f,Screen.height * 0.825f,Screen.width * 0.7f,Screen.height * 0.16f) , "");
			GUI.BeginScrollView(new Rect(Screen.width * 0.05f,Screen.height * 0.82f,Screen.width * 0.7f,Screen.height * 0.1735f),scrollPos
				,new Rect(0,Screen.height * 0.005f,Screen.width * 2.74f,Screen.height * 0.16f));
//			if(GlobalInfo.curBattleField == BattleFieldKind.TrainPlace) {
//				for(int i=3;i<19;i++){
//					if(GUI.Button(new Rect(Screen.width * 0.13f * (i - 3) + Screen.width * 0.01f,Screen.height * 0.015f,Screen.width * 0.12f,Screen.height * 0.15f),selButtonContent,selButtonStyle)){
//						if(GlobalInfo.curExhibitionEquip != null)
//							DestroyImmediate(GlobalInfo.curExhibitionEquip);
//						GlobalInfo.userInfo.equipment = (EquipmentKind)i;	
//						GlobalInfo.curExhibitionEquip = (GameObject)Instantiate(exhibitionEquips[i],exhibitionPos.position,exhibitionPos.rotation);
//						exhibitionCam.SendMessage("OnSetCamDistance",distances[i],SendMessageOptions.DontRequireReceiver);
//						exhibitionCam.SendMessage("OnSetCamHeight",heights[i],SendMessageOptions.DontRequireReceiver);
//						if(Network.isServer){
//							GlobalInfo.userInfoList[0].equipment = (EquipmentKind)i;
//							GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
//						}else{
//							GlobalInfo.rpcControl.RPC("OnSelectUserEquipRPC",RPCMode.Server,GlobalInfo.userInfo.name,i);
//						}
//						detailWindow.texture = (Texture2D)Resources.Load("GUI/EquipSelect/P_" + (i + 1).ToString());
//					}
//					GUI.DrawTexture(new Rect(Screen.width * 0.13f * i + Screen.width * 0.02f ,Screen.height * 0.02175f,Screen.width * 0.1f,Screen.height * 0.13f)
//					                ,(Texture)Resources.Load("GUI/Tank/Equip" + (i + 1).ToString()));
//				}
//			}else{
				for(int i=0;i<21;i++){
					if(GUI.Button(new Rect(Screen.width * 0.13f * i + Screen.width * 0.01f,Screen.height * 0.015f,Screen.width * 0.12f,Screen.height * 0.15f),selButtonContent,selButtonStyle)){
						if(GlobalInfo.curExhibitionEquip != null)
							DestroyImmediate(GlobalInfo.curExhibitionEquip);
						GlobalInfo.userInfo.equipment = (EquipmentKind)i;	
						GlobalInfo.curExhibitionEquip = (GameObject)Instantiate(exhibitionEquips[i],exhibitionPos.position,exhibitionPos.rotation);
						exhibitionCam.SendMessage("OnSetCamDistance",distances[i],SendMessageOptions.DontRequireReceiver);
						exhibitionCam.SendMessage("OnSetCamHeight",heights[i],SendMessageOptions.DontRequireReceiver);
						if(Network.isServer){
							GlobalInfo.userInfoList[0].equipment = (EquipmentKind)i;
							GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
						}else{
							GlobalInfo.rpcControl.RPC("OnSelectUserEquipRPC",RPCMode.Server,GlobalInfo.userInfo.name,i);
						}
						detailWindow.texture = (Texture2D)Resources.Load("GUI/EquipSelect/P_" + (i + 1).ToString());
					}
					GUI.DrawTexture(new Rect(Screen.width * 0.13f * i + Screen.width * 0.02f ,Screen.height * 0.02175f,Screen.width * 0.1f,Screen.height * 0.13f)
						,(Texture)Resources.Load("GUI/Tank/Equip" + (i + 1).ToString()));
				}
//			}
			GUI.EndScrollView();
		}
	}
	
	void RecieveGameStartedState(bool isGameStarted) {
		if(!isGameStarted && !GlobalInfo.playerLoaded){
			GlobalInfo.playerLoaded = true;
			int i = 0;
			Ray mRay;
			RaycastHit mHit = new RaycastHit();
			GameObject[] go;
			
			if(GlobalInfo.curExhibitionEquip != null)
				DestroyImmediate(GlobalInfo.curExhibitionEquip);
			guiTexture.texture = (Texture)Resources.Load("GUI/Equipments/Select_1");

//			if(GlobalInfo.curBattleField == BattleFieldKind.TrainPlace){
//				go = GameObject.FindGameObjectsWithTag("InitPosA");
//			}else{
				if(GlobalInfo.userInfo.team.Equals(TeamKind.Lightening))
					go = GameObject.FindGameObjectsWithTag("InitPosA");
				else
					go = GameObject.FindGameObjectsWithTag("InitPosB");
//			}
			foreach(UserInfoClass uf in GlobalInfo.userInfoList){
				if(uf.name.Equals(GlobalInfo.userInfo.name)) break;
				i++;
			}
			if(i < go.Length){
				GlobalInfo.camPosState = 0;
				GlobalInfo.exhibitionFlag = false;
				GlobalInfo.userInfo.isReady = true;
				GlobalInfo.destroyed = false;
				GlobalInfo.promptFlag = true;
				foreach(Transform tr in GlobalInfo.promptWindow){
					tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
				}
				if(Network.isClient) Screen.lockCursor = true;
				mRay = new Ray(go[i].transform.position,Vector3.down);
				Physics.Raycast(mRay,out mHit);
				GameObject a;
				if((int)GlobalInfo.userInfo.equipment < 18){
					a = (GameObject)Network.Instantiate(equipments[(int)GlobalInfo.userInfo.equipment],mHit.point + new Vector3(0.0f,2.0f,0.0f),go[i].transform.rotation,0);
				}else{
					a = (GameObject)Network.Instantiate(equipments[(int)GlobalInfo.userInfo.equipment],mHit.point + new Vector3(0.0f,30.0f,0.0f),go[i].transform.rotation,0);
				}
				a.SendMessage("OnSetState",TankControlState.Manual,SendMessageOptions.DontRequireReceiver);
				GlobalInfo.equipSelWindowFlag = false;
			}
		}
	}
}
