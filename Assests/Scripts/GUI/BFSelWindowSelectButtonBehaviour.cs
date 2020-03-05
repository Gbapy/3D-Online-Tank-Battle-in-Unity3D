using UnityEngine;
using System.Collections;
using MagicBattle;

public class BFSelWindowSelectButtonBehaviour : MonoBehaviour {
	public GUITexture backGround;
	public GUITexture bgImage;
	public GUITexture preView;
	public GUIStyle btn1;
	public GUIStyle btn2;
	public GUIStyle btn3;
	public GUIStyle btn4;
	public GUIStyle btn5;
	public GUIStyle btn6;
	public GUIStyle btn7;
	public GUIStyle btn8;
	public GUIStyle btn9;
	public GUIStyle btn10;
	public GUIStyle night;
	public GUIStyle noon;
	public GUIStyle fog;
	public GUIStyle scrollStyle;
	
	private Vector2 scrollPos = Vector2.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.battleFieldSelWindowFlag){
			guiTexture.enabled = true;
			backGround.enabled = true;
			preView.enabled = true;
			backGround.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
			guiTexture.pixelInset = new Rect(-Screen.width * 0.2f,-Screen.height * 0.3f,Screen.width * 0.08f,Screen.height * 0.04f);			
			preView.pixelInset = new Rect(Screen.width * -0.0205f,Screen.height * -0.23653566f,Screen.width * 0.31641f,Screen.height * 0.459970888f);
		}else{
			backGround.enabled = false;
			guiTexture.enabled = false;
			preView.enabled = false;
			enabled = false;
		}
	}
	
	void OnGUI() {
		bgImage.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
		if(GlobalInfo.battleFieldSelWindowFlag){
			Event e = Event.current;
			if(e.isKey)
				if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)StartCoroutine("OnMouseUp");
			if(GUI.Button(new Rect(Screen.width * 0.556328125f,Screen.height * 0.23f,Screen.width * 0.05f,Screen.height * 0.04f),"",noon)){
				GlobalInfo.nightOrNoonFlag = 1;
			}
			if(GUI.Button(new Rect(Screen.width * 0.6088125f,Screen.height * 0.23f,Screen.width * 0.05f,Screen.height * 0.04f),"",night)){
				GlobalInfo.nightOrNoonFlag = 0;
			}
			if(GlobalInfo.nightOrNoonFlag == 1){
				night.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Night1");
				noon.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Noon2");
			}else if(GlobalInfo.nightOrNoonFlag == 0){
				night.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Night2");
				noon.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Noon1");
			}
			if(GUI.Button(new Rect(Screen.width * 0.7f,Screen.height * 0.23f,Screen.width * 0.05f,Screen.height * 0.04f),"",fog)){
				GlobalInfo.fogFlag = !GlobalInfo.fogFlag;
			}
			if(GlobalInfo.fogFlag) 
				fog.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Fog_2");
			else
				fog.normal.background = (Texture2D)Resources.Load("GUI/BattleFields/Fog_1");
//			scrollPos = GUI.BeginScrollView(new Rect(Screen.width * 0.212f,Screen.height * 0.280932f,Screen.width * 0.250469f,Screen.height * 0.4626928675f)
//			,scrollPos,new Rect(0,0,Screen.width * 0.22f,Screen.height * 0.45f));
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn1)){
				GlobalInfo.curBattleField = BattleFieldKind.TrainPlace;
				preView.texture = (Texture)Resources.Load("GUI/Map/Train");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.045f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn2)){
				GlobalInfo.curBattleField = BattleFieldKind.Depod;
				preView.texture = (Texture)Resources.Load("GUI/Map/Depot");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.09f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn3)){
				GlobalInfo.curBattleField = BattleFieldKind.Village;
				preView.texture = (Texture)Resources.Load("GUI/Map/Village");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.135f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn4)){
				GlobalInfo.curBattleField = BattleFieldKind.Freedom;
				preView.texture = (Texture)Resources.Load("GUI/Map/Freedom");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.18f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn5)){
				GlobalInfo.curBattleField = BattleFieldKind.Port;
				preView.texture = (Texture)Resources.Load("GUI/Map/Port");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.225f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn6)){
				GlobalInfo.curBattleField = BattleFieldKind.RadarBase;
				preView.texture = (Texture)Resources.Load("GUI/Map/Radar");
			}	
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.27f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn7)){
				GlobalInfo.curBattleField = BattleFieldKind.Trench;
				preView.texture = (Texture)Resources.Load("GUI/Map/Trench");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.315f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn8)){
				GlobalInfo.curBattleField = BattleFieldKind.Green;
				preView.texture = (Texture)Resources.Load("GUI/Map/Green");
			}
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.36f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn9)){
				GlobalInfo.curBattleField = BattleFieldKind.Cliff;
				preView.texture = (Texture)Resources.Load("GUI/Map/Cliff");
			}	
			if(GUI.Button(new Rect(Screen.width * 0.21f,Screen.height * 0.2781f + Screen.height * 0.405f,Screen.width * 0.2509f,Screen.height * 0.045f),"",btn10)){
				GlobalInfo.curBattleField = BattleFieldKind.Snow;
				preView.texture = (Texture)Resources.Load("GUI/Map/Snow");
			}	
//			GUI.EndScrollView();
		}
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_2");
	}
	
	IEnumerator OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Team/Select_1");
		GlobalInfo.battleFieldSelWindowFlag = false;
		GlobalInfo.battleFieldSelected = true;
		if (GlobalInfo.curBattleField == BattleFieldKind.TrainPlace && GlobalInfo.nightOrNoonFlag == 0)
			GlobalInfo.curBattleField = BattleFieldKind.TrainPaceNight;
		else if(GlobalInfo.curBattleField == BattleFieldKind.TrainPaceNight && GlobalInfo.nightOrNoonFlag == 1)
			GlobalInfo.curBattleField = BattleFieldKind.TrainPlace;
		GlobalInfo.rpcControl.RPC("OnResponseBattleFieldRPC",RPCMode.Others,(int)GlobalInfo.curBattleField,GlobalInfo.battleFieldSelected);
		AsyncOperation async = Application.LoadLevelAsync ((int)GlobalInfo.curBattleField + 1); 
		yield return async;
//		if(GlobalInfo.curBattleField == BattleFieldKind.TrainPlace) {
//			GlobalInfo.equipSelWindowFlag = true;
//			foreach(Transform tr in GlobalInfo.equipSelWindow){
//				tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
//			}	
//		}else{
		GlobalInfo.teamSelWindowFlag = true;
		foreach(Transform tr in GlobalInfo.teamSelWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}	
//		}
		bgImage.guiTexture.enabled = false;
	}
}
