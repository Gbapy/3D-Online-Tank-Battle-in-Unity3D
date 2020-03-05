using UnityEngine;
using System.Collections;
using MagicBattle;

public class TSWindowSelectTeam1ButtonBehaviour : MonoBehaviour {
	public GUITexture backGround;
	public GUIStyle txtStyle;
	
	private Rect rect1;
	private Rect rect2;
	private Vector2 scrollPos1 = Vector2.zero;
	private Vector2 scrollPos2 = Vector2.zero;

	void Update () {
		if(GlobalInfo.teamSelWindowFlag){
			backGround.enabled = true;
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(-Screen.width * 0.18f,-Screen.height * 0.28f,Screen.width * 0.08f,Screen.height * 0.04f);			
			backGround.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
			rect1 = new Rect(Screen.width * 0.234375f,Screen.height * 0.246f,Screen.width * 0.25293f,Screen.height * 0.46434f);
			rect2 = new Rect(Screen.width * 0.50293f,Screen.height * 0.246f,Screen.width * 0.25293f,Screen.height * 0.46434f);
		}else{
			guiTexture.enabled = false;
			backGround.enabled = false;
			enabled = false;
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_1");
		GlobalInfo.userInfo.team = TeamKind.Lightening;
		GlobalInfo.userInfo.joined = true;
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
	
	void OnGUI() {
		if(GlobalInfo.teamSelWindowFlag){
			int i = 0;
			int j = 0;
			System.Collections.Generic.List<UserInfoClass> team1 = new System.Collections.Generic.List<UserInfoClass>();
			System.Collections.Generic.List<UserInfoClass> team2 = new System.Collections.Generic.List<UserInfoClass>();
			foreach(UserInfoClass uf in GlobalInfo.userInfoList){
				if(uf.joined){
					if(uf.team.Equals(TeamKind.Lightening)){
						team1.Add(uf);
						i++;
					}
					if(uf.team.Equals(TeamKind.Thunder)){
						team2.Add(uf);	
						j++;
					}
				}
			}
			scrollPos1 = GUI.BeginScrollView(rect1,scrollPos1,new Rect(0,0,Screen.width * 0.18f,Screen.height * 1.2f),false,true);
			i = 0;
			foreach(UserInfoClass uf in team1){
				GUI.DrawTexture(new Rect(0,Screen.height * 0.08f * i,Screen.width * 0.08f,Screen.height * 0.06f),(Texture)Resources.Load("GUI/Tank/Equip" + ((int)(uf.equipment + 1)).ToString()));
				GUI.Label(new Rect(Screen.width * 0.09f,Screen.height * 0.08f * i,Screen.width * 0.1f,Screen.height * 0.06f),uf.name,txtStyle);
				i++;
			}
			GUI.EndScrollView();
			i = 0;
			scrollPos2 = GUI.BeginScrollView(rect2,scrollPos2,new Rect(0,0,Screen.width * 0.18f,Screen.height * 1.2f),false,true);
			foreach(UserInfoClass uf in team2){
				GUI.DrawTexture(new Rect(0,Screen.height * 0.08f * i,Screen.width * 0.08f,Screen.height * 0.06f),(Texture)Resources.Load("GUI/Tank/Equip" + ((int)(uf.equipment + 1)).ToString()));
				GUI.Label(new Rect(Screen.width * 0.09f,Screen.height * 0.08f * i,Screen.width * 0.1f,Screen.height * 0.06f),uf.name);
				i++;
			}
			GUI.EndScrollView();
		}
	}
}
