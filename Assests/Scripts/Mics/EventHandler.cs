using UnityEngine;
using System;
using System.Collections;
using MagicBattle;

public class EventHandler : MonoBehaviour {
	public GUIStyle chatTextStyle;
	public GUIStyle iconTextStyle;
	public GameObject[] explosion;	
	public GameObject[] destroyedObject;
	public GameObject[] shell;
	public GameObject[] shootFlame;
	public float startDelay = 10.0f;
	public float exitDelay = 3.0f;	
	public float chatDelay = 9.0f;
	public Material noonMat;
	public Material nightMat;
	public GameObject mapPref;
	private string chatString = "";
	private bool helpFlag = false;
	private System.Collections.Generic.List<ChatClass> chatMsg = new System.Collections.Generic.List<ChatClass>();	
	private Vector2[] battleFieldSize = new Vector2[9];
	private float psTime = 0.0f;
	private bool captureFlag = false;
	private int captureIndex = 0;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		GlobalInfo.eventHandler = this.gameObject;
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Exponential;
		RenderSettings.fogDensity = 0.003f;
		RenderSettings.fogEndDistance = 300.0f;
		RenderSettings.fogStartDistance = 0.0f;
		RenderSettings.fogColor = new Color(0.243f,0.298f,0.349f,1.0f);
		battleFieldSize[0] = new Vector2(1000.0f,1000.0f);
		battleFieldSize[1] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[2] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[3] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[4] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[5] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[6] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[7] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[8] = new Vector2(1000.0f,1500.0f);
		Time.captureFramerate = 0;
	}
	
	void Update() {
		if(GlobalInfo.gameStarted && !GlobalInfo.gameEnded){
			if(Input.GetKey(KeyCode.Slash))helpFlag = true;else helpFlag = false;
		}
		if(chatMsg.Count > 0){
			foreach(ChatClass a in chatMsg){
				a.psTime += Time.deltaTime;
				if(a.psTime >= chatDelay){
					chatMsg.Remove(a);
				}
			}
		}
//		if(!Screen.fullScreen) Screen.fullScreen = true;
//		if (Network.isClient) {
//			psTime += Time.deltaTime;
//			if(psTime > 1.0f){
//				GlobalInfo.rpcControl.RPC("OnReportClientStateRPC",RPCMode.Server);
//				psTime = 0.0f;
//			}
//		}
		if(Input.GetKeyDown(KeyCode.F5)) {
			captureIndex = 0;
			captureFlag = true;
			QualitySettings.SetQualityLevel(4);
			GlobalInfo.rpcControl.RPC("SetTimeScaleRPC",RPCMode.All,0.2f);
		}
		if(Input.GetKeyDown(KeyCode.Escape)) {
			captureFlag = false;
			QualitySettings.SetQualityLevel(GlobalInfo.curDetail);
			GlobalInfo.rpcControl.RPC("SetTimeScaleRPC",RPCMode.All,1.0f);
		}
		if(!captureFlag){
			if(Input.GetKeyDown(KeyCode.F6)) {
				captureIndex = 0;
				captureFlag = true;
				QualitySettings.SetQualityLevel(3);
				GlobalInfo.rpcControl.RPC("SetTimeScaleRPC",RPCMode.All,0.1f);
			}
		}
		if(captureFlag) {
			captureIndex++;
			Application.CaptureScreenshot(Application.dataPath + "/ScreenShot/" + captureIndex.ToString() + ".png");
		}
	}

	void OnSetTimeScale(float timeScale) {
		Time.timeScale = timeScale;
	}

	void OnReportClientStateRPC(NetworkViewID nViewID) {
		foreach (ReportClass rp in GlobalInfo.report) {
			if(rp.nViewID.Equals(nViewID)){
				rp.reportedTime  = Time.realtimeSinceStartup;
			}
		}
	}	

	void OnGUI(){
		if(GlobalInfo.gameStarted && !GlobalInfo.gameEnded){
			Event e = Event.current;
			if(e.isKey){
				if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Return){
					if(GlobalInfo.chatScreenFlag){
						if(chatString != ""){
							chatString = GlobalInfo.userInfo.name + " : " + chatString;
							GlobalInfo.rpcControl.RPC("OnChatMessageRPC",RPCMode.All,chatString);
							chatString = "";
							Screen.lockCursor = true;
							GlobalInfo.chatScreenFlag = false;							
						}
					}else{
						Screen.lockCursor = false;
						GlobalInfo.chatScreenFlag = true;
					}
				}
			}
			if(!captureFlag) {
				GUI.Label(new Rect(0,0,Screen.width,20.0f),"Ready to ScreenShot",chatTextStyle);
			}
			if(GlobalInfo.chatScreenFlag){
				chatString = GUI.TextField(new Rect(Screen.width * 0.2354f,Screen.height * 0.96f,Screen.width * 0.51074f,Screen.height * 0.03639f),chatString);
			}	
			if(GlobalInfo.userInfo.equipment != EquipmentKind.Ghost){
				if(!GlobalInfo.specialCamState){
					int n = 0;
					int m = 0;
						
					foreach(UserInfoClass uf in GlobalInfo.userInfoList) {
						if(uf.team.Equals(TeamKind.Lightening)){
							n++;
							if(uf.destroyed){
								GUI.DrawTexture(new Rect(0,Screen.height * 0.08f * n,Screen.width * 0.06f,Screen.height * 0.05f)
									,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString() + "_1"));
							}else{
								GUI.DrawTexture(new Rect(0,Screen.height * 0.08f * n,Screen.width * 0.06f,Screen.height * 0.05f)
									,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
							}
							GUI.Label(new Rect(0,Screen.height * 0.08f * n + Screen.height * 0.05f,Screen.width * 0.06f,Screen.height * 0.03f),uf.name,iconTextStyle);
						}else{
							m++;
							if(uf.destroyed){
								GUI.DrawTexture(new Rect(Screen.width * 0.94f,Screen.height * 0.08f * m,Screen.width * 0.06f,Screen.height * 0.05f)
									,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString() + "_1"));
							}else{
								GUI.DrawTexture(new Rect(Screen.width * 0.94f,Screen.height * 0.08f * m,Screen.width * 0.06f,Screen.height * 0.05f)
									,(Texture)Resources.Load("GUI/Tank/Equip" + ((int)uf.equipment + 1).ToString()));
							}
							GUI.Label(new Rect(Screen.width * 0.94f,Screen.height * 0.08f * m + Screen.height * 0.05f,Screen.width * 0.06f,Screen.height * 0.03f),uf.name,iconTextStyle);
						}
					}
				}
			}
			if(helpFlag){
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),(Texture)Resources.Load("GUI/Help/Help"));
			}
			if(chatMsg.Count > 0){
				GUI.color = new Color(0,0,0,1);
				int i = 0;
				chatTextStyle.normal.textColor = Color.white;
				foreach(ChatClass a in chatMsg){
					GUI.Label(new Rect(0,Screen.height - 50.0f - 21.0f * i,Screen.width,20.0f),a.msg,chatTextStyle);
					i++;
				}
			}
		}
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param){
		RaycastHit[] hits;
		Transform tr;
		Vector3 sweepSpartPoint;
		bool flag = false;
		System.Collections.Generic.List<GameObject> hitList = new System.Collections.Generic.List<GameObject>();
		
		sweepSpartPoint = param.attackedPoint - param.direction;
		hits = Physics.SphereCastAll(new Ray(sweepSpartPoint,param.direction),0.5f,param.direction.magnitude * 2.0f);
		if(hits.Length > 0){
			foreach(RaycastHit h in hits){
				if(h.collider != null){
					tr = h.collider.transform;
					while(tr.parent != null){
						flag = false;
						foreach(GameObject go in hitList){
							if(go.Equals(tr.gameObject)){
								flag = true;
								break;
							}
						}
						if(!flag)hitList.Add(tr.gameObject);
						tr = tr.parent;
					}
					flag = false;
					foreach(GameObject go in hitList){
						if(go.Equals(tr.gameObject)){
							flag = true;
							break;
						}
					}
					if(!flag)hitList.Add(tr.gameObject);
				}
			}
			foreach(GameObject go in hitList){
				go.SendMessage("OnShellAttacked",param,SendMessageOptions.DontRequireReceiver);
			}
		}
		if(GlobalInfo.canon != null)
			GlobalInfo.canon.SendMessage("OnAttackVibrate",param,SendMessageOptions.DontRequireReceiver);
		Instantiate(explosion[(int)param.attackedShellKind],param.attackedPoint,Quaternion.identity);
		if(param.attackedPoint.y < GlobalInfo.water_Level)
			StartCoroutine("RaiseWaterFoam",param);
	}
	
	IEnumerator RaiseWaterFoam(ShellAttackedSendMsgParam param){
		yield return new WaitForSeconds(0.3f);
		if(param.attackedShellKind.Equals(ShellKind.LightShell))
			Instantiate(explosion[7],param.attackedPoint,Quaternion.identity);
		else
			Instantiate(explosion[8],param.attackedPoint,Quaternion.identity);
	}
		
	void OnDestroyed(DestroyedSendMsgParam param) {
		Instantiate(destroyedObject[param.kind],param.position,param.rotation);
		if(GlobalInfo.playerViewID.Equals(param.viewID)){
			Camera.main.GetComponent<AudioListener>().enabled = true;
		}
	}
	
	void OnShoot(ShootSendMsgParam param) {
		Instantiate(shootFlame[(int)param.kind],param.position,Quaternion.LookRotation(param.dir));
		GameObject go = (GameObject)Instantiate(shell[(int)param.kind],param.position,Quaternion.LookRotation(param.dir));
		go.SendMessage("OnSetShellInfo",param,SendMessageOptions.DontRequireReceiver);
	}
	
	void OnLaunch(ShootSendMsgParam param) {
		Instantiate(shootFlame[(int)param.kind],param.position,Quaternion.LookRotation(param.dir));
		GameObject go = (GameObject)Instantiate(shell[(int)param.kind],param.position,Quaternion.LookRotation(param.dir));
		go.SendMessage("OnSetMisileInfo",param,SendMessageOptions.DontRequireReceiver);
	}
	
	void OnChatMessage(string msg){
		ChatClass chat = new ChatClass();
		chat.msg = msg;
		chat.psTime = 0.0f;
		chatMsg.Add(chat);
	}
	
	void OnChangeBattleField() {
		if(GlobalInfo.myPlayer != null) {
			Camera.mainCamera.transform.parent = null;
			Network.RemoveRPCs(GlobalInfo.playerViewID);
			Network.Destroy(GlobalInfo.playerViewID);
//			Camera.mainCamera.GetComponent<ColorCorrectionCurves>().enabled = false;
			GlobalInfo.myPlayer = null;
			GlobalInfo.destroyed = true;
		}
		GlobalInfo.playerLoaded = false;
		GlobalInfo.userInfo.destroyed = false;
		GlobalInfo.userInfo.isReady = false;
		GlobalInfo.userInfo.joined = false;
		GlobalInfo.gameStarted = false;
		GlobalInfo.gameEnded = true;
		GlobalInfo.battleFieldSelected = false;
		GlobalInfo.equipSelWindowFlag = false;
		GlobalInfo.exhibitionFlag = false;
		GlobalInfo.mainMenuFlag = true;
		GlobalInfo.promptFlag = false;
		GlobalInfo.teamSelWindowFlag = false;
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnUpdateUserList() {
		int i = 0;
		
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			i++;
			if(i == 1)
				GlobalInfo.rpcControl.RPC("OnDownLoadUserListRPC",RPCMode.Others,true,uf.name,uf.password,(int)uf.team,(int)uf.equipment,uf.score,uf.hitCount,uf.shootCount,uf.joined,uf.destroyed,uf.isReady,uf.playerViewID);
			else
				GlobalInfo.rpcControl.RPC("OnDownLoadUserListRPC",RPCMode.Others,false,uf.name,uf.password,(int)uf.team,(int)uf.equipment,uf.score,uf.hitCount,uf.shootCount,uf.joined,uf.destroyed,uf.isReady,uf.playerViewID);
		}	
	}
	
	void OnUpdateUserScore() {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			GlobalInfo.rpcControl.RPC("OnDownLoadUserScoreRPC",RPCMode.Others,uf.name,uf.score);
		}
	}
	
	void OnUpdateUserHitCount() {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			GlobalInfo.rpcControl.RPC("OnDownLoadUserHitCountRPC",RPCMode.Others,uf.name,uf.hitCount);
		}		
	}
	
	void OnUpdateUserShootCount() {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			GlobalInfo.rpcControl.RPC("OnDownLoadUserShootCountRPC",RPCMode.Others,uf.name,uf.hitCount);
		}				
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
 		if (Network.isServer) {
			Debug.Log("Local server connection disconnected");
		}else{
			if (info == NetworkDisconnection.LostConnection)
				Debug.Log("Lost connection to the server");
			else{
				GlobalInfo.disconnected = true;
				Application.Quit();
			}
		}		
	} 
	
	void OnGameStarted() {
		if(GlobalInfo.userInfo.equipment != EquipmentKind.Ghost)
			Instantiate(mapPref,Vector3.zero,Quaternion.identity);
		StartCoroutine("StartDelay");
	}
	
	void OnGameEnded() {
		Screen.lockCursor = false;
		GlobalInfo.gameEnded = true;
		GlobalInfo.promptFlag = true;
		foreach(Transform tr in GlobalInfo.promptWindow){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}		
		GlobalInfo.rpcControl.SendMessage("OnSetEnabled",false,SendMessageOptions.DontRequireReceiver);	
	}
	
	IEnumerator StartDelay() {
		GlobalInfo.promptFlag = false;
		yield return new WaitForSeconds(startDelay);
		GlobalInfo.rpcControl.SendMessage("OnSetEnabled",true,SendMessageOptions.DontRequireReceiver);	
	}
	
	void OnApplicationQuit() {
		if(!GlobalInfo.disconnected)
			Application.CancelQuit();
		else
			Application.Quit();
	}
	
	void OnSetWather() {
		Light globalLight = GameObject.FindGameObjectWithTag("GlobalLight").light;
		RenderSettings.fog = true;
		if(GlobalInfo.nightOrNoonFlag == 1){
			RenderSettings.skybox = noonMat;
			RenderSettings.fogColor = new Color(0.13333f,0.13333f,0.13333f,1.0f);
			RenderSettings.ambientLight = new Color(0.5f,0.5f,0.5f,0.5f);
			globalLight.color = new Color(1,1,1,1);
			globalLight.shadowStrength = 1.0f;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;
			Camera.main.GetComponent<GlobalFog>().globalFogColor = new Color(0.5f,0.5f,0.5f);
		}else if(GlobalInfo.nightOrNoonFlag == 0){
			RenderSettings.skybox = nightMat;
			RenderSettings.fogColor = new Color(0,0,0,1.0f);
			globalLight.color = Color.black;
			globalLight.intensity = 1.0f;
			RenderSettings.ambientLight = new Color(0.039f,0.0471f,0.0824f,1);
			globalLight.shadowStrength = 1.0f;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = false;
			Camera.main.GetComponent<GlobalFog>().globalFogColor = new Color(0.123f,0.0157f,0.1f);
		}
		Camera.main.GetComponent<UltraRedRayCamera> ().enabled = false;
		Camera.main.GetComponent<GlobalFog> ().enabled = true;
		if(GlobalInfo.fogFlag){
			Camera.main.GetComponent<GlobalFog>().globalDensity = 0.1f;
			Camera.main.GetComponent<GlobalFog>().startDistance = 20.0f;
			Camera.main.GetComponent<GlobalFog>().heightScale = 600.0f;
		}else{
			Camera.main.GetComponent<GlobalFog>().globalDensity = 0.0f;
			Camera.main.GetComponent<GlobalFog>().startDistance = 30.0f;
			Camera.main.GetComponent<GlobalFog>().heightScale = 100.0f;
		}
	}
}
