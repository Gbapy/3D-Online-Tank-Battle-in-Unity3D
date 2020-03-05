using UnityEngine;
using System.Collections;
using MagicBattle;

public class PromptWindowBehaviour : MonoBehaviour {
	public GUIStyle txtStyle;
	private Rect rect1;
	private Rect rect2;
	private Vector2 scrollPos1 = Vector2.zero;
	private Vector2 scrollPos2 = Vector2.zero;
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.promptFlag){
			if(GlobalInfo.gameStarted && GlobalInfo.gameEnded){
				guiTexture.texture = (Texture2D)Resources.Load("GUI/Equipments/BG_8");
			}else if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded){
				guiTexture.texture = (Texture2D)Resources.Load("GUI/Equipments/BG_7");
			}
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);			
			rect1 = new Rect(Screen.width * 0.197266f,Screen.height * 0.2402f,Screen.width * 0.29785f,Screen.height * 0.5313f);
			rect2 = new Rect(Screen.width * 0.50391f,Screen.height * 0.2402f,Screen.width * 0.29785f,Screen.height * 0.5313f);
		}else{
			guiTexture.enabled = false;
			enabled = false;
		}
	}
	
	void OnGUI() {
		if(GlobalInfo.promptFlag){
			int i = 0;
			int j = 0;
			float shootRate = 0.0f;
			
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
				if(GlobalInfo.gameStarted && GlobalInfo.gameEnded){
					GUI.DrawTexture(new Rect(Screen.width * 0.009f,Screen.height * 0.06f * i,Screen.width * 0.0762f,Screen.height * 0.06f)
					                ,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
					GUI.Label(new Rect(Screen.width * 0.09375f,Screen.height * 0.06f * i,Screen.width * 0.042f,Screen.height * 0.06f),uf.name,txtStyle);
					GUI.Label(new Rect(Screen.width * 0.1484f,Screen.height * 0.06f * i,Screen.width * 0.042f,Screen.height * 0.06f),Mathf.FloorToInt(uf.score).ToString(),txtStyle);
					GUI.Label(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i,Screen.width * 0.0723f,Screen.height * 0.06f),GetShootRate(uf).ToString(),txtStyle);
				}else if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded){
					GUI.DrawTexture(new Rect(Screen.width * 0.03125f,Screen.height * 0.06f * i,Screen.width * 0.0752f,Screen.height * 0.06f)
					                ,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
					GUI.Label(new Rect(Screen.width * 0.1221f,Screen.height * 0.06f * i,Screen.width * 0.07129f,Screen.height * 0.06f),uf.name,txtStyle);
					if(uf.isReady){
						GUI.DrawTexture(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i + Screen.height * 0.01f,Screen.width * 0.06641f,Screen.height * 0.04f),(Texture)Resources.Load("GUI/Score/Ready"));
					}else{
						GUI.DrawTexture(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i + Screen.height * 0.01f,Screen.width * 0.06641f,Screen.height * 0.04f),(Texture)Resources.Load("GUI/Score/NotReady"));
					}
				}
				i++;
			}
			GUI.EndScrollView();
			i = 0;
			scrollPos2 = GUI.BeginScrollView(rect2,scrollPos2,new Rect(0,0,Screen.width * 0.18f,Screen.height * 1.2f),false,true);
			foreach(UserInfoClass uf in team2){
				if(GlobalInfo.gameStarted && GlobalInfo.gameEnded){
					GUI.DrawTexture(new Rect(Screen.width * 0.009f,Screen.height * 0.06f * i,Screen.width * 0.0762f,Screen.height * 0.06f)
					                ,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
					GUI.Label(new Rect(Screen.width * 0.09375f,Screen.height * 0.06f * i,Screen.width * 0.042f,Screen.height * 0.06f),uf.name,txtStyle);
					GUI.Label(new Rect(Screen.width * 0.1484f,Screen.height * 0.06f * i,Screen.width * 0.042f,Screen.height * 0.06f),Mathf.FloorToInt(uf.score).ToString(),txtStyle);
					GUI.Label(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i,Screen.width * 0.0723f,Screen.height * 0.06f),GetShootRate(uf).ToString(),txtStyle);
				}else if(!GlobalInfo.gameStarted && GlobalInfo.gameEnded){
					GUI.DrawTexture(new Rect(Screen.width * 0.03125f,Screen.height * 0.06f * i,Screen.width * 0.0752f,Screen.height * 0.06f)
					                ,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
					GUI.Label(new Rect(Screen.width * 0.1221f,Screen.height * 0.06f * i,Screen.width * 0.07129f,Screen.height * 0.06f),uf.name,txtStyle);
					if(uf.isReady){
						GUI.DrawTexture(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i + Screen.height * 0.01f,Screen.width * 0.06641f,Screen.height * 0.04f),(Texture)Resources.Load("GUI/Score/Ready"));
					}else{
						GUI.DrawTexture(new Rect(Screen.width * 0.1973f,Screen.height * 0.06f * i + Screen.height * 0.01f,Screen.width * 0.06641f,Screen.height * 0.04f),(Texture)Resources.Load("GUI/Score/NotReady"));				
					}
				}
				i++;
			}
			GUI.EndScrollView();
		}
	}
	
	int GetShootRate(UserInfoClass uf) {
		if(uf.shootCount == 0){
			return 0;
		}else{
			float a = uf.hitCount * 100 / uf.shootCount;
			return Mathf.FloorToInt(a);
		}
	}
}
