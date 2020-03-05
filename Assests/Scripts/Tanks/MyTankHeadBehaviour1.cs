using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankHeadBehaviour1 : MonoBehaviour {
//	public GUITexture aimCross;
//	public GameObject aimCrossPrefab;
	public Transform canon;
//	public Transform camPos;
	public Transform gyro;
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
//	public Transform secondaryCamPos;
	public Transform misileLauncherPos;
	public float misileFireRate = 5.0f;	
	public GameObject shell;
	
	private Vector3 hitPos;
	private bool Cannonflag=false;
//	private float nxtFireTime = 0.0f;
//	private float psTime = 0.0f;
	private float CannonMoveTime;
	private Vector3 prepos;
	private Vector3 oldpos;
	private Vector3 targetNormal;
	private Vector3 realAimPos;
//	private bool aimCrossControlFlag = true;
	private	Ray aimRay;
	private	RaycastHit hit;
//	private bool rotatable = true;
//	private bool canonFixed = false;
//	private float h_Rot = 0.0f;
//	private float v_Rot = 0.0f;
//	private float misileTime = 0.0f;
	
	const float AIMCROSS_WIDTH = 30.0f;
	const float AIMCROSS_HEIGHT = 30.0f;
	const int TRIGGERLAYER = 8;
	const float MAX_DISTANCE = 1000.0f;
	const float EP = 0.0001f;
	
	private float duringtime = 0;
		
	void Start() {
//		if(!networkView.isMine) return;
//		GameObject g = (GameObject)Instantiate(aimCrossPrefab,new Vector3(0.5f,0.5f,0.0f),Quaternion.identity);
//		aimCross = g.guiTexture;
//		aimCross.pixelInset = new Rect(-AIMCROSS_WIDTH / 2.0f,-AIMCROSS_HEIGHT / 2.0f,AIMCROSS_WIDTH,AIMCROSS_HEIGHT);
//		camPos = Camera.main.transform;
//		SmoothFollow sm = camPos.GetComponent<SmoothFollow>();
//		sm.enabled = true;
//		sm.target = canonSP;
//		sm.distance = camDistance;
//		sm.height = camHeight;	
		duringtime = Random.Range(2.5f,7.7f);
		Invoke("_SelfMove",duringtime);
	}
	
	void _SelfMove(){
		if(!Cannonflag){
	 		CannonMoveTime=0.0f;
	 		oldpos=canon.localPosition;
	 		Cannonflag=true;
		}
//		canon.SendMessage("OnCameraVibrate",false,SendMessageOptions.DontRequireReceiver);
//					if(GlobalInfo.isMoving)
		mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce ,canonSP.position);
//					else
//						mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce,canonSP.position);
//					nxtFireTime = psTime + fireTime;
//		GlobalInfo.rpcControl.networkView.RPC("OnShootRPC",RPCMode.All,(int)ShellKind.HeavyShell,canonSP.position,canonSP.forward,GlobalInfo.playerViewID);
		Instantiate(shootFlame,canonSP.position,canonSP.rotation);
		GameObject go = (GameObject)Instantiate(shell,canonSP.position,mTank.rotation);//new Quaternion(0,342.67f,0,0));
//		go.transform.Translate(Vector3.forward * 20f);
		go.rigidbody.AddForce(new Vector3(-2f,0.5f,3) * 700f);
//		go.rigidbody.AddForce(canonSP.position.normalized * 100);
//		Instantiate(bombedSite[(int)p.attackedShellKind],p.attackedPoint + new Vector3(0,0.05f,0),Quaternion.FromToRotation(Vector3.up,p.normal));
//		GameObject go = (GameObject)Instantiate(shell[0],this.transform.position,this.transform.rotation);
//		go.SendMessage("OnSetShellInfo",param,SendMessageOptions.DontRequireReceiver);
		duringtime = Random.Range(0.5f,3f);
		Invoke("_SelfMove",duringtime);
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
		
//		if(departFlag && !destroyedFlag){
//			psTime += Time.deltaTime;			
//			speed += accel * Time.deltaTime;
//			lastPos = transform.position;
//			transform.position = transform.position + speed * dir * Time.fixedDeltaTime - new Vector3(0,9.8f * Time.fixedDeltaTime,0);	
//			transform.LookAt(transform.position + dir);
//			Physics.Raycast(new Ray(lastPos,dir),out hit);
//			if(hit.collider != null){
//				Vector3 tmp = hit.point - transform.position;
//				if(tmp.magnitude <= speed * Time.fixedDeltaTime){
//					destroyedFlag = true;
//					if(viewID.Equals(GlobalInfo.playerViewID))
//						GlobalInfo.rpcControl.RPC("OnShellAttackedRPC",RPCMode.All,hit.point,hit.normal,(int)shellKind,viewID);
//					Destroy(this.gameObject);
//				}
//			}
//			if(psTime > GlobalInfo.shellProperty[(int)shellKind].lifeCycle){
//				Destroy(this.gameObject);
//				destroyedFlag = true;
//			}	
//		}
		
		
	}
	
	void Update() {
//		float mx,my;
//		float sx,sy;
		
//		if(!networkView.isMine) return;
//		if(Input.GetKeyDown(KeyCode.C)){
//			SmoothFollow sm = camPos.GetComponent<SmoothFollow>();
//			if(camPosState){
//				camPos.parent = null;
//				sm.enabled = true;
//				sm.target = canonSP;
//				sm.distance = camDistance;
//				sm.height = camHeight;
//			}else{
//				sm.enabled = false;
//				camPos.position = secondaryCamPos.position;
//				camPos.rotation = secondaryCamPos.rotation;
//				camPos.parent = secondaryCamPos;
//				aimCross.pixelInset = new Rect(- AIMCROSS_WIDTH / 2.0f,- AIMCROSS_HEIGHT /2.0f ,AIMCROSS_WIDTH,AIMCROSS_HEIGHT);
//				h_Rot = 0.0f;		
//				v_Rot = 0.0f;
//				misileTime = misileFireRate;
//			}
//			camPosState = !camPosState;
//		}		
//		if(Input.GetKeyDown(KeyCode.Space))	canonFixed = !canonFixed;
//		if(!camPosState){
//			mx = aimCross.pixelInset.x;
//			my = aimCross.pixelInset.y;
//			sx = Input.GetAxis("Mouse X") * 15;
//			sy = Input.GetAxis("Mouse Y") * 15;			
//			if(sx != 0.0f || sy != 0.0f){
//				rotatable = true;
//				mx += sx;my += sy;
//				mx = Mathf.Clamp(mx , -Screen.width / 2 , Screen.width / 2 - AIMCROSS_WIDTH);
//				my = Mathf.Clamp(my , -Screen.height / 2 , Screen.height / 2 - AIMCROSS_HEIGHT);
//				aimCross.pixelInset = new Rect(mx,my ,AIMCROSS_WIDTH,AIMCROSS_HEIGHT);
//				sx = Screen.width / 2 + aimCross.pixelInset.x + AIMCROSS_WIDTH / 2;
//				sy = Screen.height / 2 + aimCross.pixelInset.y + AIMCROSS_HEIGHT / 2;
////				aimRay = camPos.camera.ScreenPointToRay(new Vector3(sx,sy,camPos.camera.near));
//				Physics.Raycast(aimRay,out hit);
//				if(hit.collider != null){
//					realAimPos = hit.point;
//				}else{
//					realAimPos = aimRay.GetPoint(MAX_DISTANCE);
//				}		
//			}else if(realAimPos != Vector3.zero){
//				if(aimCrossControlFlag){
////					Vector3 aimCrossPos = camPos.camera.WorldToScreenPoint(realAimPos);
//					aimCross.pixelInset = new Rect(aimCrossPos.x - AIMCROSS_WIDTH / 2 - Screen.width / 2
//						,aimCrossPos.y - AIMCROSS_HEIGHT / 2 - Screen.height / 2,AIMCROSS_WIDTH,AIMCROSS_HEIGHT);
//				}
//			}
//			if(!canonFixed) rotatable = true;
//			Vector3 tmp = realAimPos - transform.position;
//			tmp.Normalize();
//			float ang = Vector3.Angle(transform.up,tmp);
//			float ang1 = Vector3.Angle(transform.up,canonSP.forward);
//			ang = Mathf.Clamp(ang,50.0f,95.0f);
//			ang1 = (ang - ang1) * Mathf.PI / 180;
//			float tmp1 = canon_Rpm_V * Time.deltaTime;
//			if(Mathf.Abs(ang1) > tmp1) ang1 = Mathf.Sign(ang1) * tmp1;
//			if(rotatable)
//				canon.RotateAroundLocal(new Vector3(1,0,0),ang1);
//			tmp = realAimPos - transform.position;
//			ang = Vector3.Angle(transform.up,tmp);
//			ang = tmp.magnitude * Mathf.Cos (ang * Mathf.PI / 180);
//			tmp = transform.up * ang;
//			gyro.position = transform.position + tmp;
//			gyro.LookAt(realAimPos);
//			ang = Vector3.Angle(transform.forward,gyro.forward);
//			ang = ang * Mathf.PI / 180;
//			ang1 = canon_Rpm_H * Time.deltaTime;
//			if(ang > ang1 ) ang = ang1;
//			if(Vector3.Angle(transform.right,gyro.forward) > 90.0f){
//				ang = -ang;
//			}
//			if(rotatable)
//				transform.RotateAroundLocal(new Vector3(0,1,0),ang);
////			psTime += Time.deltaTime;
//			if(Input.GetMouseButtonDown(0)){
////				if(psTime >= nxtFireTime){
//					if(!Cannonflag){
//				 		CannonMoveTime=0.0f;
//				 		oldpos=canon.localPosition;
//				 		Cannonflag=true;
//					}
//					canon.SendMessage("OnCameraVibrate",false,SendMessageOptions.DontRequireReceiver);
////					if(GlobalInfo.isMoving)
//						mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce * 0.5f,canonSP.position);
////					else
////						mTank.AddForceAtPosition(-canonSP.forward * maxRecoilForce,canonSP.position);
////					nxtFireTime = psTime + fireTime;
////					GlobalInfo.rpcControl.networkView.RPC("OnShootRPC",RPCMode.All,(int)ShellKind.HeavyShell,canonSP.position,canonSP.forward,GlobalInfo.playerViewID);
////				}
//			}
//			if(!rotatable && canonFixed){
//				Ray mRay = new Ray(canonSP.position,canonSP.forward);
//				RaycastHit hit = new RaycastHit();
//				Physics.Raycast(new Ray(canonSP.position,canonSP.forward),out hit);
//				if(hit.collider != null)
//					realAimPos = hit.point;
//				else
//					realAimPos = mRay.GetPoint(1000.0f);
//			}
//		}else{
//			h_Rot += Input.GetAxis("Mouse X") * 0.01f;
//			v_Rot = Input.GetAxis("Mouse Y") * 0.01f;
//			float ang = 0.0f;
//			if(Mathf.Abs(h_Rot) > canon_Rpm_H * Time.deltaTime) 
//				ang = Mathf.Sign(h_Rot) * canon_Rpm_H * Time.deltaTime;
//			else
//				ang = h_Rot * Time.deltaTime;
//			transform.RotateAroundLocal(new Vector3(0,1,0),ang);
//			camPos.RotateAroundLocal(Vector3.right,-v_Rot);
//			h_Rot -= ang;
//			misileTime += Time.deltaTime;
//			if(Input.GetMouseButtonDown(0)){
//				if(misileTime >= misileFireRate){
//					Vector3 tv;
//					int ti = -1;
//					float tf = 0.0f;
//					float tmp = 0.0f;
//					int j = 0;
//					
//					misileTime = 0.0f;
//					GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerHeli");
//					for(int i=0;i<go.Length;i++){
//						tv = go[i].transform.position - misileLauncherPos.position;
//						tv.Normalize();
//						tmp = Vector3.Angle(tv,misileLauncherPos.forward);
//						if(tmp <= 30.0f){
//							j++;
//							tv = go[i].transform.position - misileLauncherPos.position;
//							if(j == 1){
//								tf = tv.magnitude;
//								ti = i;
//							}else{
//								if(tv.magnitude < tf){
//									tf = tv.magnitude;
//									ti = i;
//								}
//							}
//						}
//					}
////					if(ti != -1){
//////						GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,misileLauncherPos.position,misileLauncherPos.forward,GlobalInfo.playerViewID,go[ti].networkView.viewID);
////					}else{
////						GlobalInfo.rpcControl.RPC("OnLaunchRPC",RPCMode.All,(int)ShellKind.GuidedAirMisile,misileLauncherPos.position,misileLauncherPos.forward,GlobalInfo.playerViewID,GlobalInfo.playerViewID);
////					}
//				}			
//			}
//		}
	}
	
	void SetEnabled(bool ena){
		this.enabled = ena;
	}

//	void SetAimCrossControlFlag(bool f){
////		aimCrossControlFlag = f;
//	}
	
//	void OnMove() {
//		rotatable = false;		
//	}
//	
//	void OnNotMove() {
//		rotatable = true;
//	}
}