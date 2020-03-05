using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterGUIController : MonoBehaviour {
	public Transform GUIPos;
	public GUIStyle labelStyle;
	
	private bool flag = true;
	// Use this for initialization
	
	void Update() {
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.playerCamera != null){
			Vector3 tmp = GUIPos.position - GlobalInfo.playerCamera.transform.position;
			tmp.Normalize();
			float ang = Vector3.Angle(GlobalInfo.playerCamera.transform.forward,tmp);
			if(ang < 60)
				flag = true;
			else
				flag = false;
		}
	}
	void OnGUI() {
		string nam = "";
		if(flag){
			if(!networkView.isMine){
				if(GlobalInfo.playerCamera != null){
					foreach(UserInfoClass uf in GlobalInfo.userInfoList){
						if(uf.playerViewID.Equals(networkView.viewID)){
							nam = uf.name;
							if(uf.team == TeamKind.Lightening){
								labelStyle.normal.textColor = Color.red;
							}else{ 
								labelStyle.normal.textColor = Color.blue;
							}
						}
					}
					Vector3 pos = GlobalInfo.playerCamera.WorldToScreenPoint(GUIPos.position);
					GUI.Label(new Rect(pos.x - Screen.width * 0.05f,Screen.height - pos.y - Screen.height * 0.1f,Screen.width * 0.1f,Screen.height * 0.1f),nam,labelStyle);
				}
			}
		}
	}
}
