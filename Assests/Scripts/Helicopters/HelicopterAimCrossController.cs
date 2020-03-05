using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterAimCrossController : MonoBehaviour {
	public Transform cam;
	public float springForce = 0.00003f;
	public float shellFireRate = 0.1f;
	public float misileFireRate = 0.5f;
	public Transform[] misileLauncherPos;
	public GameObject snowParticle;
	public GameObject rainParticle;
	public Transform gunPos;
	public ShellKind portableGun = ShellKind.LightShell;
	public ShellKind portableMisile = ShellKind.GeneralMisile;
	public GameObject rpcControl;
	public Texture bulletMark;
	public Texture exhaustedBulletMark;
	public Texture gMisileMark;
	public Texture exhaustedGMisileMark;
	public Texture aMisileMark;
	public Texture exhaustedAMisileMark;
	public int maxBulletCount = 400;
	public int maxGMisileCount = 50;
	public int maxAMisileCount = 20;	
	public GUIStyle txtStyle;
	public GUIStyle labelStyle;
	
	private float shellFireTime = 0.0f;
	private float misileFireTime = 0.0f;
	private float guidedAirMisileTime = 0.0f;
	private float guidedAirMisileFireRate = 3.0f;
	private float h_Rot = 0.0f;
	private float curH_Rot = 0.0f;
	private bool misileLauncher = false;
	private Texture aimCross;
	private Vector2 aimCrossPos;
	
	const float AIMCROSS_WIDTH = 30.0f;
	const float AIMCROSS_HEIGHT = 30.0f;
	
	// Use this for initialization
	void Start () {
		if(networkView.isMine){
			GlobalInfo.playerCamera = cam.camera;
			aimCross = (Texture)Resources.Load("GUI/GunSights/HeliGunSight");
			aimCrossPos = new Vector2(Screen.width / 2.0f,Screen.height / 2.0f);
			GlobalFog gf = Camera.main.GetComponent<GlobalFog>();
			GlobalFog gf1 = cam.gameObject.AddComponent<GlobalFog>();
			gf1.globalDensity = 0.05f;
			if(GlobalInfo.fogFlag){
				gf1.fogMode = gf.fogMode;
				gf1.fogShader = gf.fogShader;
				gf1.globalDensity = 0.15f;
				gf1.globalFogColor = gf.globalFogColor;
				gf1.height = gf.height;
				gf1.heightScale = gf.heightScale;
				gf1.startDistance = gf.startDistance;
			}
			GameObject go = GameObject.Find("Snow");
			if(go != null) {
				for(int i = 0;i < 4;i++) {
					go = (GameObject)GameObject.Instantiate(snowParticle,Vector3.zero,Quaternion.identity);
					go.transform.parent = cam;
					go.transform.localPosition = Camera.main.transform.GetChild(i).localPosition;
					Camera.main.transform.GetChild(i).particleEmitter.emit = false;
				}
			}
			go = GameObject.Find("Rain");
			if(go != null) {
				for(int i = 0;i < 4;i++) {
					go = (GameObject)GameObject.Instantiate(rainParticle,Vector3.zero,Quaternion.identity);
					go.transform.parent = cam;
					go.transform.localPosition = Camera.main.transform.GetChild(i).localPosition;
					Camera.main.transform.GetChild(i).particleEmitter.emit = false;
				}			
			}
			cam.GetComponent<ColorCorrectionCurves> ().saturation = Camera.main.GetComponent<ColorCorrectionCurves> ().saturation;
		}else{
			cam.gameObject.SetActive(false);
			enabled = false;
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.chatScreenFlag) return;
		float my = Input.GetAxis("Mouse Y") * 3.0f;
		float mx = Input.GetAxis("Mouse X") * 0.001f;
		float cx = aimCrossPos.x;
		float cy = aimCrossPos.y;
		
		cy -= my;
		if(GlobalInfo.camPosState == 0)
			cy = Mathf.Clamp(cy,Screen.height * 2.0f / 8.0f,Screen.height * 5.0f / 8.0f);
		else
			cy = Mathf.Clamp(cy,AIMCROSS_HEIGHT / 2.0f,Screen.height - AIMCROSS_HEIGHT / 2.0f);
		aimCrossPos = new Vector2(Screen.width / 2.0f,cy);
		h_Rot += mx;
		cam.RotateAroundLocal(Vector3.up,mx);
		float t = h_Rot - curH_Rot;
		t = t * springForce * Time.deltaTime;
		transform.RotateAroundLocal(Vector3.up,t);
		curH_Rot += t / 2.0f;
		cam.RotateAroundLocal(Vector3.up,-t / 2.0f);
		if(Input.GetMouseButtonDown(0)){
			shellFireTime = 0.0f;
			misileFireTime = 0.0f;
		}
		if(Input.GetMouseButton(0)){
			shellFireTime += Time.deltaTime;
			if(shellFireTime >= shellFireRate && maxBulletCount > 0){
				shellFireTime = 0.0f;
				Ray aimRay = cam.camera.ScreenPointToRay(new Vector3(aimCrossPos.x,Screen.height - aimCrossPos.y,cam.camera.near));
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(aimRay,out hit);
				Vector3 hitPoint;
				if(hit.collider != null){
					hitPoint = hit.point;
				}else{
					hitPoint = aimRay.GetPoint(1000.0f);
				}
				maxBulletCount--;
				gunPos.LookAt(hitPoint);
				UpdateShootCount();
				GlobalInfo.rpcControl.networkView.RPC("OnShootRPC",RPCMode.All,(int)portableGun,gunPos.position,gunPos.forward,0,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
			}
		}
		if(Input.GetMouseButton(1)){
			misileFireTime += Time.deltaTime;
			if(misileFireTime >= misileFireRate && maxGMisileCount > 0) {
				misileFireTime = 0.0f;
				Ray aimRay = cam.camera.ScreenPointToRay(new Vector3(aimCrossPos.x,Screen.height - aimCrossPos.y,cam.camera.near));
				RaycastHit hit = new RaycastHit();
				misileLauncher = !misileLauncher;
				Physics.Raycast(aimRay,out hit);
				Vector3 hitPoint;
				if(hit.collider != null){
					hitPoint = hit.point;
				}else{
					hitPoint = aimRay.GetPoint(1000.0f);
				}
				maxGMisileCount--;
				UpdateShootCount();
				if(misileLauncher){
					misileLauncherPos[0].LookAt(hitPoint);
					GlobalInfo.rpcControl.networkView.RPC("OnShootRPC",RPCMode.All,(int)portableMisile,misileLauncherPos[0].position,misileLauncherPos[0].forward,1,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
				}else{
					misileLauncherPos[1].LookAt(hitPoint);
					GlobalInfo.rpcControl.networkView.RPC("OnShootRPC",RPCMode.All,(int)portableMisile,misileLauncherPos[1].position,misileLauncherPos[1].forward,1,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
				}
			}
		}
		guidedAirMisileTime += Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.M)){
			if(guidedAirMisileTime >= guidedAirMisileFireRate && maxAMisileCount > 0){
				Vector3 tv;
				int ti = -1;
				float tf = 0.0f;
				float tmp = 0.0f;
				int j = 0;
				
				guidedAirMisileTime = 0.0f;
				GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerHeli");
				for(int i=0;i<go.Length;i++){
					tv = go[i].transform.position - gunPos.position;
					tv.Normalize();
					tmp = Vector3.Angle(tv,gunPos.forward);
					if(tmp <= 30.0f){
						j++;
						tv = go[i].transform.position - gunPos.position;
						if(j == 1){
							tf = tv.magnitude;
							ti = i;
						}else{
							if(tv.magnitude < tf){
								tf = tv.magnitude;
								ti = i;
							}
						}
					}
				}
				maxAMisileCount--;
				UpdateShootCount();
				if(ti != -1){
					GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,gunPos.position,gunPos.forward,GlobalInfo.playerViewID,go[ti].networkView.viewID,GlobalInfo.userInfo.name);
				}else{
					GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,gunPos.position,gunPos.forward,GlobalInfo.playerViewID,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
				}
			}					
		}
	}

	void OnGUI() {
		if(!GlobalInfo.chatScreenFlag && GlobalInfo.gameStarted){
			if(maxBulletCount > 0)
				GUI.DrawTexture(new Rect(Screen.width * 0.85f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),bulletMark);
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.85f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),exhaustedBulletMark);
			if(maxGMisileCount > 0)
				GUI.DrawTexture(new Rect(Screen.width * 0.89f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),gMisileMark);
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.89f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),exhaustedGMisileMark);
			if(maxAMisileCount > 0)
				GUI.DrawTexture(new Rect(Screen.width * 0.93f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),aMisileMark);
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.93f,Screen.height * 0.86f,Screen.width * 0.03f,Screen.height * 0.08f),exhaustedAMisileMark);
			txtStyle.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width * 0.85f,Screen.height * 0.95f,Screen.width * 0.03f,Screen.height * 0.01f),maxBulletCount.ToString(),txtStyle);
			GUI.Label(new Rect(Screen.width * 0.89f,Screen.height * 0.95f,Screen.width * 0.03f,Screen.height * 0.01f),maxGMisileCount.ToString(),txtStyle);
			GUI.Label(new Rect(Screen.width * 0.93f,Screen.height * 0.95f,Screen.width * 0.03f,Screen.height * 0.01f),maxAMisileCount.ToString(),txtStyle);		
			GUI.DrawTexture(new Rect(aimCrossPos.x - AIMCROSS_WIDTH / 2.0f,aimCrossPos.y - AIMCROSS_HEIGHT / 2.0f,AIMCROSS_WIDTH,AIMCROSS_HEIGHT),aimCross);
		}
	}
	
	void UpdateShootCount() {
		if(Network.isServer){
			GlobalInfo.userInfoList[0].shootCount++;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserShootCount",SendMessageOptions.DontRequireReceiver);
		}else{
			GlobalInfo.rpcControl.RPC("OnUpdateUserShootCountRPC",RPCMode.Server,GlobalInfo.userInfo.name);
		}
	}
}
