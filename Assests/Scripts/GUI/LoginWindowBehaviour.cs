using UnityEngine;
using System.Collections;
using System.IO;
using MagicBattle;

public class LoginWindowBehaviour : MonoBehaviour {
	public GUITexture loginWnd;
	public GUITexture bgImage;
	public GameObject rpc;
	public GUIStyle txtStyle;

	private string usrName = "";
	private string password = "";
	private float psTime = 0.0f;

	// Use this for initialization
	void Start () {
		SetInitData ();
		string ipAdrStr = "";
		ipAdrStr = GetMasterServerAddress();
		if(ipAdrStr != null)
			MasterServer.ipAddress = ipAdrStr;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("MagicBattle");
		Resolution[] resol = Screen.resolutions;
		GlobalInfo.curDetail = PlayerPrefs.GetInt("CurDetail");
		GlobalInfo.curDetail = Mathf.Clamp(GlobalInfo.curDetail,0,5);
		QualitySettings.SetQualityLevel(GlobalInfo.curDetail);
		int curResolusion = 0;
		curResolusion = PlayerPrefs.GetInt("CurResolusion");
		curResolusion = Mathf.Clamp(curResolusion,0,Screen.resolutions.Length);
		Screen.SetResolution(resol[curResolusion].width,resol[curResolusion].height,true,60);
	}
	
	void Update() {
		if(GlobalInfo.logoPlayed) {
			bgImage.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
		}
		if(!GlobalInfo.serverFound){
			if(!GlobalInfo.isConnecting){
				HostData[] hostList = MasterServer.PollHostList();
				if(hostList.Length > 0){
					GlobalInfo.isConnecting = true;
					Network.Connect(hostList[0]);
				}else{
					psTime += Time.deltaTime;
					if(psTime > 3.0f){
						Network.InitializeServer(25,4040,false);
						MasterServer.RegisterHost("MagicBattle","rrdMagicBattle");	
						GameObject go = (GameObject)Network.Instantiate(rpc,Vector3.zero,Quaternion.identity,0);
						DontDestroyOnLoad(go);
					}
				}
			}
		}else{
			if(!GlobalInfo.logged){
				if(!GlobalInfo.rpcFound){
					GameObject go = GameObject.FindGameObjectWithTag("RPC");
					DontDestroyOnLoad(go);
					if(go != null){
						GlobalInfo.rpcControl = go.networkView;
						GlobalInfo.rpcFound = true;
					}
				}else{
					if(!GlobalInfo.warnFlag){
						if(GlobalInfo.logoPlayed){
							Screen.showCursor = true;
							loginWnd.guiTexture.enabled = true;
							guiTexture.enabled = true;
							loginWnd.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
							guiTexture.pixelInset = new Rect(-Screen.width * 0.1f,-Screen.height * 0.11f,Screen.width * 0.06f,Screen.height * 0.04f);
						}
					}else{
						loginWnd.guiTexture.enabled = false;
						guiTexture.enabled = false;
					}
				}
			}
		}
	}
	
	void OnGUI() {
		if (!GlobalInfo.logoPlayed)return;
		Event e = Event.current;
		if(e.isKey)
			if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)OnMouseUp();
		if(GlobalInfo.warnFlag){
			GUI.TextArea(new Rect(Screen.width * 0.3f,Screen.height * 0.5f - 20.0f,Screen.width * 0.4f,40.0f),GlobalInfo.warnText);
			if(GUI.Button(new Rect(Screen.width * 0.45f,Screen.height * 0.5f + 25.0f,Screen.width * 0.1f,Screen.height * 0.05f),"OK")){
				GlobalInfo.warnFlag = false;
				GlobalInfo.logged = false;
			}
			return;
		}						
		if(GlobalInfo.serverFound && !GlobalInfo.logged){
			usrName = GUI.TextField(new Rect(Screen.width * 0.4483f,Screen.height * 0.3581f,Screen.width * 0.1836f,Screen.height * 0.03785f),usrName,txtStyle);
			password = GUI.TextField(new Rect(Screen.width * 0.4483f,Screen.height * 0.4484f,Screen.width *  0.1836f,Screen.height * 0.03785f),password,txtStyle);
		}
	}
	
	void OnMouseUp(){
		if (!GlobalInfo.logoPlayed)return;
		guiTexture.texture = (Texture)Resources.Load("GUI/Login/Btn_1");
		if(usrName != "" && password != ""){
			if(Network.isServer)
				LogInPassed();
			else
				if(!GlobalInfo.certificated)
					GlobalInfo.rpcControl.RPC("OnCertificateUserRPC",RPCMode.Server,usrName,password);
			loginWnd.guiTexture.enabled = false;
		}
	}
	
	void OnMouseDown() {
		if (!GlobalInfo.logoPlayed)return;
		guiTexture.texture = (Texture)Resources.Load("GUI/Login/Btn_2");
	}
	
	void OnServerInitialized() {
		UserInfoClass uf = new UserInfoClass();
		uf.name = "";
		uf.password = "";
		GlobalInfo.userInfoList.Add(uf);
		GlobalInfo.serverFound = true;
	}
	
	void OnConnectedToServer() {
		GlobalInfo.isConnecting = false;
		GlobalInfo.serverFound = true;
	}	

	void OnCertficateResult(CertResultClass cr) {
		if(cr.certResult){
			GlobalInfo.certificated = true;
			LogInPassed();
		}else{
			GlobalInfo.certificated = false;
			GlobalInfo.warnFlag = true;
			GlobalInfo.warnText = cr.warning;
		}
	}
	
	void SetInitData() {
		txtStyle.normal.textColor = Color.white;
		SetShellProperties();
		GlobalInfo.serverFound = false;		
		GlobalInfo.LoginWindow = gameObject;
		GlobalInfo.mainMenu = GameObject.Find("MainMenu").transform;
		GlobalInfo.optionWindow = GameObject.Find("OptionWindow").transform;
		GlobalInfo.helpWindow = GameObject.Find("HelpWindow").transform;
		GlobalInfo.battleFieldSelWindow = GameObject.Find("BattleFieldSelectWindow").transform;
		GlobalInfo.teamSelWindow = GameObject.Find("TeamSelectWindow").transform;
		GlobalInfo.equipSelWindow = GameObject.Find("EquipmentSelectWindow").transform;
		GlobalInfo.exhibitionWindow = GameObject.Find("Exhibition").transform;
		GlobalInfo.promptWindow = GameObject.Find("PromptWindow").transform;				
		GlobalInfo.userInfo = new UserInfoClass();
		GlobalInfo.treeDataFileName = new string[GlobalInfo.terrainCount];
		GlobalInfo.treeDataFileName [9] = "/TrainPlaceTreeData.bin";
	}
	
	void SetShellProperties() {
		GlobalInfo.shellProperty = new ShellClass[GlobalInfo.kindsOfShell];
		GlobalInfo.shellProperty[0] = new ShellClass();
		GlobalInfo.shellProperty[0].explosionPower = 100000.0f;
		GlobalInfo.shellProperty[0].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[0].destructionPower = 200.0f;
		GlobalInfo.shellProperty[0].speed = 300.0f;
		GlobalInfo.shellProperty[0].lifeCycle = 15.0f;
		GlobalInfo.shellProperty[1] = new ShellClass();
		GlobalInfo.shellProperty[1].explosionPower = 50000.0f;
		GlobalInfo.shellProperty[1].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[1].destructionPower = 1200.0f;
		GlobalInfo.shellProperty[1].speed = 400.0f;
		GlobalInfo.shellProperty[1].lifeCycle = 15.0f;
		GlobalInfo.shellProperty[2] = new ShellClass();
		GlobalInfo.shellProperty[2].explosionPower = 10000.0f;
		GlobalInfo.shellProperty[2].explosionRadius = 3.0f;
		GlobalInfo.shellProperty[2].destructionPower = 600.0f;
		GlobalInfo.shellProperty[2].speed = 180.0f;
		GlobalInfo.shellProperty[2].lifeCycle = 15.0f;
		GlobalInfo.shellProperty[3] = new ShellClass();
		GlobalInfo.shellProperty[3].explosionPower = 10000.0f;
		GlobalInfo.shellProperty[3].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[3].destructionPower = 600.0f;
		GlobalInfo.shellProperty[3].speed = 400.0f;
		GlobalInfo.shellProperty[3].lifeCycle = 15.0f;
		GlobalInfo.shellProperty[4] = new ShellClass();
		GlobalInfo.shellProperty[4].explosionPower = 200000.0f;
		GlobalInfo.shellProperty[4].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[4].destructionPower = 900.0f;
		GlobalInfo.shellProperty[4].speed = 300.0f;
		GlobalInfo.shellProperty[4].lifeCycle = 15.0f;		
		GlobalInfo.shellProperty[5] = new ShellClass();
		GlobalInfo.shellProperty[5].explosionPower = 50000.0f;
		GlobalInfo.shellProperty[5].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[5].destructionPower = 100.0f;
		GlobalInfo.shellProperty[5].speed = 300.0f;
		GlobalInfo.shellProperty[5].lifeCycle = 15.0f;			
		GlobalInfo.shellProperty[6] = new ShellClass();
		GlobalInfo.shellProperty[6].explosionPower = 50000.0f;
		GlobalInfo.shellProperty[6].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[6].destructionPower = 1200.0f;
		GlobalInfo.shellProperty[6].speed = 300.0f;
		GlobalInfo.shellProperty[6].lifeCycle = 15.0f;	
		GlobalInfo.shellProperty[7] = new ShellClass();
		GlobalInfo.shellProperty[7].explosionPower = 200.0f;
		GlobalInfo.shellProperty[7].explosionRadius = 20.0f;
		GlobalInfo.shellProperty[7].destructionPower = 25.0f;
		GlobalInfo.shellProperty[7].speed = 200.0f;
		GlobalInfo.shellProperty[7].lifeCycle = 15.0f;	
	}
	
	string GetMasterServerAddress() {
		char[] ipAdrChar = new char[255];
		string ipAdrStr = "";
		try{
			BinaryReader br = new BinaryReader(File.Open(Application.dataPath + "/ServerInfo.txt",FileMode.Open));
			ipAdrChar = br.ReadChars(255);
			br.Close();
			for(int i = 0;i<ipAdrChar.Length;i++){
				ipAdrStr += ipAdrChar[i];
			}
		}catch(IOException e){
			ipAdrStr = "";
		}
		return ipAdrStr;
	}

	void LogInPassed() {
		GlobalInfo.userInfo.name = usrName;		
		GlobalInfo.userInfo.password = password;
		GlobalInfo.logged = true;	
		if(Network.isServer){
			GlobalInfo.userInfoList[0].name = usrName;
			GlobalInfo.userInfoList[0].password = password;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
		}
		GlobalInfo.mainMenuFlag = true;
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
		Destroy(gameObject);
	}
}
