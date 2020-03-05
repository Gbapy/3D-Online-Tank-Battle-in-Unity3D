using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankHeadBehaviour : MonoBehaviour {
	public Transform body;
	public Transform canon;
	public Transform camPos;
	public Transform gyro;
	public Transform h_Gyro;
	public Transform v_Gyro;
	public GameObject shootFlame;
	public Transform canonSP;
	public Rigidbody mTank;
	public float fireTime = 1.0f;
	public float maxRecoilForce = 3000.0f;
	public ShellKind shellKind = ShellKind.HeavyShell;
	public float canon_Rpm_H = 0.628f;
	public float canon_Rpm_V = 0.2f;
	public float camDistance = 30.0f;
	public float camHeight = 5.0f;
	public Transform secondaryCamPos;
	public Transform thirdCamPos;
//	public Transform misileLauncherPos;
//	public float misileFireRate = 5.0f;	
	public int maxShellCount = 50;
//	public int maxMisileCount = 20;
	public Texture shellMark;
//	public Texture misileMark;
	public Texture exhaustedShellMark;
//	public Texture exhaustedMisileMark;
	public GUIStyle txtStyle;
	public Transform gyroBase;
	
	private Vector3 hitPos;
	private bool Cannonflag=false;
	private float nxtFireTime = 0.0f;
	private float psTime = 0.0f;
	private float CannonMoveTime;
	private Vector3 prepos;
	private Vector3 oldpos;
	private Vector3 targetNormal;
	private bool targeted = false;
	private bool aimCrossControlFlag = true;
	private	Ray aimRay;
	private	RaycastHit hit;
	private bool rotatable = true;
	private bool canonFixed = true;
	private float v_Rot = 0.0f;
//	private float misileTime = 0.0f;
	private DepthOfField34 df;
	private UltraRedRayCamera urc;
	private SmoothFollow sm;
	private bool h_Aimed = false;
	private bool v_Aimed = false;
	private int lastCamPosState = 0;
	private Texture aimCross;
	private Vector2 aimCrossPos;
	private GlobalFog gf;
	private bool fireFlag = false;
	private TankControlState tankControlState = TankControlState.Manual;
	private Vector3 aiAimPos;
	private string nam;

	const float AIMCROSS_WIDTH = 40.0f;
	const float AIMCROSS_HEIGHT = 40.0f;
	const int TRIGGERLAYER = 8;
	const float MAX_DISTANCE = 1000.0f;
	const float EP = 0.0001f;
		
	void Start() {
		if(!networkView.isMine) {
			enabled = false;
			return;
		}
	}
	
	void Update() {
		float mx,my;
		float sx,sy;
		float ang = 0.0f;
		float ang1 = 0.0f;
		
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.chatScreenFlag) return;
		if(tankControlState == TankControlState.Manual){
			sm.target = transform;
			if(Input.GetKeyDown(KeyCode.C)){
				if(!GlobalInfo.camAnimFlag){
					if(GlobalInfo.specialCamState){
						DisableSpecialCamera();
						GlobalInfo.specialCamState = false;
					}
					SwitchCamera(false);
				}
			}	
			if(Input.GetKeyDown(KeyCode.X)){
				if(!GlobalInfo.camAnimFlag){
					if(!GlobalInfo.specialCamState){
						if(IsAvaluableSP() == true)
							EnableSpecialCamera();
					}else{
						DisableSpecialCamera();
					}
				}
			}
			sx = Input.GetAxis("Mouse X") * 15;
			sy = Input.GetAxis("Mouse Y") * 15;	
			if(!GlobalInfo.specialCamState){
				if(GlobalInfo.camPosState == 2){
					if(sy != 0.0f){
						my = aimCrossPos.y;
						my -= sy;
						my = Mathf.Clamp(my , AIMCROSS_HEIGHT / 2.0f , Screen.height - AIMCROSS_HEIGHT / 2.0f);
						aimCrossPos = new Vector2(Screen.width / 2.0f,my);
						aimRay = camPos.camera.ScreenPointToRay(new Vector3(aimCrossPos.x,Screen.height - aimCrossPos.y,camPos.camera.near));
						Physics.Raycast(aimRay,out hit);
						if(hit.collider != null){
							GlobalInfo.realAimPos = hit.point;
							targeted = true;
						}else{
							GlobalInfo.realAimPos = aimRay.GetPoint(MAX_DISTANCE);
							targeted = false;
						}
					}else if(GlobalInfo.realAimPos != Vector3.zero){
						if(aimCrossControlFlag){
							Vector2 ac = camPos.camera.WorldToScreenPoint(GlobalInfo.realAimPos);
							aimCrossPos = new Vector2(ac.x,Screen.height - ac.y);
						}
					}
				}else{
					if(Input.GetKeyDown(KeyCode.Space))	canonFixed = !canonFixed;
					mx = aimCrossPos.x;
					my = aimCrossPos.y;
			
					if(sx != 0.0f || sy != 0.0f){
						rotatable = true;
						mx += sx;my -= sy;
						mx = Mathf.Clamp(mx , AIMCROSS_WIDTH / 2.0f , Screen.width - AIMCROSS_WIDTH / 2.0f);
						my = Mathf.Clamp(my , AIMCROSS_HEIGHT / 2.0f , Screen.height - AIMCROSS_HEIGHT / 2.0f);
						aimCrossPos = new Vector2(mx,my);
						aimRay = camPos.camera.ScreenPointToRay(new Vector3(aimCrossPos.x,Screen.height - aimCrossPos.y,camPos.camera.near));
						Physics.Raycast(aimRay,out hit);
						if(hit.collider != null){
							GlobalInfo.realAimPos = hit.point;
							targeted = true;
						}else{
							GlobalInfo.realAimPos = aimRay.GetPoint(MAX_DISTANCE);
							targeted = false;
						}		
					}else if(GlobalInfo.realAimPos != Vector3.zero){
						if(aimCrossControlFlag){
							Vector2 ac = camPos.camera.WorldToScreenPoint(GlobalInfo.realAimPos);
							aimCrossPos = new Vector2(ac.x,Screen.height - ac.y);
						}
					}
				}
			}
			if(!rotatable && canonFixed){
				Ray mRay = new Ray(canonSP.position,canonSP.forward);
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(new Ray(canonSP.position,canonSP.forward),out hit);
				if(hit.collider != null){
					GlobalInfo.realAimPos = hit.point;
					targeted = true;
				}else{
					GlobalInfo.realAimPos = mRay.GetPoint(1000.0f);
					targeted = false;
				}
			}
		}
		h_Aimed = false;v_Aimed = false;
		if(!canonFixed) rotatable = true;
		Vector3 tmp = Vector3.zero;
		if(tankControlState == TankControlState.Manual){
			tmp = GlobalInfo.realAimPos - transform.position;
		}else{
			if(aiAimPos != Vector3.zero) tmp = aiAimPos - transform.position;
		}
		tmp.Normalize();
		ang = Vector3.Angle(transform.up,tmp);
		ang1 = Vector3.Angle(transform.up,canonSP.forward);
		ang = Mathf.Clamp(ang,70.0f,100.0f);
		ang1 = (ang - ang1) * Mathf.PI / 180;
		float tmp1 = canon_Rpm_V * Time.deltaTime;
		if(Mathf.Abs(ang1) > tmp1) ang1 = Mathf.Sign(ang1) * tmp1; else v_Aimed = true;
		if(rotatable)
			canon.RotateAroundLocal(new Vector3(1,0,0),ang1);
		if(tankControlState == TankControlState.Manual){
			tmp = GlobalInfo.realAimPos - gyroBase.position;
		}else{
			if(aiAimPos != Vector3.zero) tmp = aiAimPos - gyroBase.position;
		}
		ang = Vector3.Angle(transform.up,tmp);
		ang = tmp.magnitude * Mathf.Cos (ang * Mathf.PI / 180);
		tmp = transform.up * ang;
		gyro.position = gyroBase.position + tmp;
		if(tankControlState == TankControlState.Manual){
			gyro.LookAt(GlobalInfo.realAimPos,transform.up);
		}else{
			if(aiAimPos != Vector3.zero) gyro.LookAt(aiAimPos,transform.up);
		}
		if(!GlobalInfo.specialCamState){
			h_Gyro.position = gyroBase.position;
			h_Gyro.rotation = gyro.rotation;
		}
		if(GlobalInfo.camPosState == 2)
			ang = Vector3.Angle(transform.forward,body.forward);
		else
			ang = Vector3.Angle(transform.forward,gyro.forward);
		ang = ang * Mathf.PI / 180;
		ang1 = canon_Rpm_H * Time.deltaTime;
		if(ang > ang1) ang = ang1; else h_Aimed = true;
		if(h_Aimed && v_Aimed) GlobalInfo.targetAimed = true; else GlobalInfo.targetAimed = false;
		if(GlobalInfo.camPosState == 2)
			ang1 = Vector3.Angle(-body.right,transform.forward);
		else
			ang1 = Vector3.Angle(transform.right,gyro.forward);
		if(ang1 != 90.0f) {
			if(ang1 > 90.0f)ang = -ang;
			if(rotatable)
				transform.RotateAroundLocal(new Vector3(0,1,0),ang);
		}

//		if(GlobalInfo.camPosState == 1)
//			tmp = camPos.forward;
//		else if(GlobalInfo.camPosState == 0)
//			tmp = misileLauncherPos.forward;
//		if(GlobalInfo.camPosState == 1){
//			v_Rot = Input.GetAxis("Mouse Y") * 0.05f;
//			camPos.RotateAroundLocal(Vector3.right,-v_Rot);
//			Vector3 realAimRay = GlobalInfo.realAimPos - camPos.position;
//			df.focalPoint = realAimRay.magnitude;
//			if(targeted)
//				df.enabled = false;
//			else
//				df.enabled = false;
//		}
		psTime += Time.deltaTime;
		if(tankControlState == TankControlState.Manual){
			if(Input.GetMouseButtonDown(0)){
				if(psTime >= nxtFireTime && maxShellCount > 0){
					if(!Cannonflag){
				 		CannonMoveTime=0.0f;
				 		oldpos=canon.localPosition;
				 		Cannonflag=true;
					}
					canon.SendMessage("OnShootVibrate",SendMessageOptions.DontRequireReceiver);
					if(GlobalInfo.isMoving)
						mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce * 0.5f,canonSP.position);
					else
						mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce,canonSP.position);
					nxtFireTime = psTime + fireTime;
					maxShellCount--;
					int useGravity = 0;
					if(GlobalInfo.specialCamState)
						useGravity = 0;
					else
						useGravity = 1;
					UpdateShootCount();
					if(tankControlState == TankControlState.AutoDrive) useGravity = 0;
					GlobalInfo.rpcControl.RPC("OnShootRPC",RPCMode.All,(int)shellKind,canonSP.position,canonSP.forward,useGravity,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
					fireFlag = false;
				}
			}
		}else{
			if(fireFlag){
				if(h_Aimed && v_Aimed){
					if(psTime >= nxtFireTime && maxShellCount > 0){
						if(!Cannonflag){
							CannonMoveTime=0.0f;
							oldpos=canon.localPosition;
							Cannonflag=true;
						}
						canon.SendMessage("OnShootVibrate",SendMessageOptions.DontRequireReceiver);
						if(GlobalInfo.isMoving)
							mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce * 0.5f,canonSP.position);
						else
							mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce,canonSP.position);
						nxtFireTime = psTime + fireTime;
						maxShellCount--;
						UpdateShootCount();
						GlobalInfo.rpcControl.RPC("OnShootRPC",RPCMode.All,(int)shellKind,canonSP.position,canonSP.forward,0,networkView.viewID,nam);
						fireFlag = false;
					}
				}
			}
		}
//		misileTime += Time.deltaTime;
//		if(Input.GetKeyDown(KeyCode.M) && maxMisileCount > 0){
//			if(misileTime >= misileFireRate){
//				Vector3 tv;
//				int ti = -1;
//				float tf = 0.0f;
//				int j = 0;
//				
//				misileTime = 0.0f;
//				GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerHeli");
//				for(int i=0;i<go.Length;i++){
//					tv = go[i].transform.position - misileLauncherPos.position;
//					tv.Normalize();
//					tmp1 = Vector3.Angle(tv,tmp);
//					if(tmp1 <= 30.0f){
//						j++;
//						tv = go[i].transform.position - misileLauncherPos.position;
//						if(j == 1){
//							tf = tv.magnitude;
//							ti = i;
//						}else{
//							if(tv.magnitude < tf){
//								tf = tv.magnitude;
//								ti = i;
//							}
//						}
//					}
//				}
//				maxMisileCount--;
//				UpdateShootCount();
//				if(ti != -1){
//					GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,misileLauncherPos.position,tmp,GlobalInfo.playerViewID,go[ti].networkView.viewID,GlobalInfo.userInfo.name);
//				}else{
//					GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,misileLauncherPos.position,tmp,GlobalInfo.playerViewID,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
//				}
//			}			
//		}
	}

	void OnFire() {
		fireFlag = true;
	}

	void OnSetState(TankControlState ts) {
		tankControlState = ts;
		if(tankControlState == TankControlState.AutoDrive) {
			GetComponent<TankSpecialCameraController>().enabled = false;
			GetComponent<TankCanonBehaviour>().enabled = false;
			foreach(UserInfoClass a in GlobalInfo.userInfoList) {
				if(a.playerViewID.Equals(networkView.viewID)) nam = a.name;
			}
		}else{
			aimCross = (Texture)Resources.Load("GUI/GunSights/TankGunSight");
			aimCrossPos = new Vector2(Screen.width  / 2.0f,Screen.height / 2.0f);
			camPos = Camera.main.transform;
			GlobalInfo.playerCamera = Camera.main;
			sm = camPos.GetComponent<SmoothFollow>();
			sm.enabled = true;
			sm.target = transform;
			sm.distance = camDistance;
			sm.height = camHeight;	
			df = camPos.GetComponent<DepthOfField34>();
			df.enabled = false;
			urc = camPos.GetComponent<UltraRedRayCamera>();
			urc.enabled = false;
			Camera.main.SendMessage ("OnSetWather", SendMessageOptions.DontRequireReceiver);
			gf = camPos.GetComponent<GlobalFog>();
			GameObject go = GameObject.Find("Snow");
			if(go != null) {
				for(int i=0;i<4;i++){
					camPos.GetChild(i).particleEmitter.emit = true;
				}
			}
			go = GameObject.Find("Rain");
			if(go != null) {
				for(int i=0;i<4;i++){
					camPos.GetChild(i).particleEmitter.emit = true;
				}
			}
		}
	}

	void OnSetAimPos(Vector3 pos) {
		aiAimPos = pos;
	}

	void FixedUpdate () {
	  if(Cannonflag){
	    if(CannonMoveTime<0.3f){
		  if(CannonMoveTime<0.1f)
			canon.position = canon.position - canonSP.forward * 0.06f;
		  else
			canon.position = canon.position + canonSP.forward * 0.03f;
		  CannonMoveTime=CannonMoveTime+Time.fixedDeltaTime;
		}else{
			canon.localPosition = oldpos;
			Cannonflag=false;
		}
	  }
	}
	
	void OnGUI () {
		if(tankControlState != TankControlState.Manual) return;
		if(!GlobalInfo.chatScreenFlag && GlobalInfo.gameStarted){
			if(!GlobalInfo.specialCamState){
				GUI.DrawTexture(new Rect(aimCrossPos.x - AIMCROSS_WIDTH / 2.0f,aimCrossPos.y - AIMCROSS_HEIGHT / 2.0f,AIMCROSS_WIDTH,AIMCROSS_HEIGHT),aimCross);
			}
			if(maxShellCount > 0)
				GUI.DrawTexture(new Rect(Screen.width * 0.85f,Screen.height * 0.7f,Screen.width * 0.05f,Screen.height * 0.2f),shellMark);
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.85f,Screen.height * 0.7f,Screen.width * 0.05f,Screen.height * 0.2f),exhaustedShellMark);
//			if(maxMisileCount > 0)
//				GUI.DrawTexture(new Rect(Screen.width * 0.92f,Screen.height * 0.7f,Screen.width * 0.05f,Screen.height * 0.2f),misileMark);
//			else
//				GUI.DrawTexture(new Rect(Screen.width * 0.92f,Screen.height * 0.7f,Screen.width * 0.05f,Screen.height * 0.2f),exhaustedMisileMark);
			if(GlobalInfo.specialCamState){
				txtStyle.normal.textColor = new Color(1,1,1);
			}else{
				txtStyle.normal.textColor = new Color(0,0,0);
			}
			GUI.Label(new Rect(Screen.width * 0.85f,Screen.height * 0.93f,Screen.width * 0.05f,Screen.height * 0.05f),maxShellCount.ToString(),txtStyle);
//			GUI.Label(new Rect(Screen.width * 0.92f,Screen.height * 0.93f,Screen.width * 0.05f,Screen.height * 0.05f),maxMisileCount.ToString(),txtStyle);
		}
	}
	
	void SetEnabled(bool ena){
		this.enabled = ena;
	}

	void SetAimCrossControlFlag(bool f){
		aimCrossControlFlag = f;
	}
	
	void OnMove() {
		rotatable = false;		
	}
	
	void OnNotMove() {
		rotatable = true;
	}
	
	void SwitchCamera(bool flag) {
		if(flag){
			GlobalInfo.camPosState--;
			if(GlobalInfo.camPosState < 0)GlobalInfo.camPosState = 2;
		}
		switch(GlobalInfo.camPosState){
		case 0:
			sm.enabled = false;
			camPos.position = secondaryCamPos.position;
			camPos.rotation = secondaryCamPos.rotation;
			camPos.parent = secondaryCamPos;
			canonFixed = false;
			aimCrossPos = new Vector2(Screen.width / 2.0f,Screen.height / 2.0f);
			v_Rot = 0.0f;
			//df.enabled = true;
			canonFixed = true;
//			misileTime = misileFireRate;	
			GlobalInfo.camPosState = 1;
			break;								
		case 1:
			aimCrossPos = new Vector2(Screen.width / 2.0f,Screen.height / 2.0f);
			sm.enabled = false;
//			df.enabled = false;
			canonFixed = false;
			camPos.parent = body;
			camPos.localPosition = thirdCamPos.localPosition;
			camPos.localRotation = thirdCamPos.localRotation;
			GlobalInfo.camPosState = 2;
			break;
		case 2:
			camPos.parent = null;
			sm.enabled = true;
			sm.target = transform;
			sm.distance = camDistance;
			sm.height = camHeight;
			canonFixed = true;
//			df.enabled = false;
			GlobalInfo.camPosState = 0;
			break;
		default:
			break;
		}		
	}
	
	void EnableSpecialCamera() {
		GlobalInfo.specialCamState = true;
		canonFixed = false;
		lastCamPosState = GlobalInfo.camPosState;
		GlobalInfo.camPosState = 0;
		sm.enabled = false;
		df.enabled = false;
		if (GlobalInfo.nightOrNoonFlag == 0) {
			urc.enabled = true;
			RenderSettings.ambientLight = Color.white;
		}
		v_Gyro.localRotation = Quaternion.identity;
		camPos.parent = v_Gyro;
		camPos.localRotation = Quaternion.identity;
		Vector3 tmp = canonSP.position - v_Gyro.position;
		camPos.position = v_Gyro.position + v_Gyro.forward * tmp.magnitude;
		SendMessage("OnSetCamMinDistance",tmp.magnitude,SendMessageOptions.DontRequireReceiver);
		//gf.enabled = false;
	}
	
	void DisableSpecialCamera() {
		GlobalInfo.specialCamState = false;
		urc.enabled = false;
		camPos.GetComponent<MotionBlur>().enabled = false;
		df.enabled = false;
		Camera.main.SendMessage ("OnSetWather", SendMessageOptions.DontRequireReceiver);
		camPos.parent = null;
		camPos.camera.fieldOfView = 60.0f;
		GlobalInfo.camPosState = lastCamPosState;
		camPos.position = v_Gyro.position;
		if(GlobalInfo.fogFlag)gf.enabled = true;
		SwitchCamera(true);	
	}
	
	bool IsAvaluableSP() {
		Vector3 v1,v2;
		
		v_Gyro.localRotation = Quaternion.identity;
		Ray mRay = new Ray(v_Gyro.position,v_Gyro.forward);
		RaycastHit mHit = new RaycastHit();
		Physics.Raycast(mRay,out mHit);
		if(mHit.collider != null){
			v1 = mHit.point	 - transform.position;	
			v2 = canonSP.position - transform.position;
			if(v1.magnitude < v2.magnitude) return false; else return true;
		}
		return true;
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