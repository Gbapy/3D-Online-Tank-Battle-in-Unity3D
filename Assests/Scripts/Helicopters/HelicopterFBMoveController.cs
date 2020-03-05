using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterFBMoveController : MonoBehaviour {
	public float maxSpeed = 10.0f;
	public Transform heliBody;
	public Transform wing1;
	public Transform wing2;
	public Transform myPos;
	public GameObject panelPref;

	private float fbTime = 1.0f;
	private float fbSurgeAngle = 0.0f;
	private bool surgeDir = false;
	private float curSpeed = 0.0f;
	private float curFBAngle = 0.0f;
	private float speedBackTime = 0.0f;
	private float fbAngleBackTime = 0.0f;
	private bool smallSurgeFlag = false;
	private float smallSurgeTime = 0.0f;
	private float smallSurgeAngle = 0.0f;
	private float smallSurgeRate = 0.0f;
	private GUITexture panel;

	const float MAX_FRONT_SURGEANGLE = Mathf.PI / 10.0f;
	const float MAX_BACK_SURGEANGLE = -Mathf.PI / 10.0f;
	const float FRONT_SURGE_RPM = Mathf.PI / 300.0f;
	const float BACK_SURGE_RPM = Mathf.PI / 300.0f;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		if(!networkView.isMine){
			enabled = false;
			return;
		}
		GameObject per = GameObject.Find ("Performance");
		per.SendMessage ("OnSetEnabledFlag", true, SendMessageOptions.DontRequireReceiver);
		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
		rb.mass = 1.0f;
		rb.drag = 2.0f;
		rb.angularDrag = 2.0f;
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		smallSurgeRate = Random.Range(0.5f,2.0f);
		GlobalInfo.playerViewID = networkView.viewID;
		GlobalInfo.userInfo.playerViewID = networkView.viewID;
		GlobalInfo.myPlayer = gameObject;
		if(Network.isServer){
			GlobalInfo.userInfoList[0].equipment = GlobalInfo.userInfo.equipment;
			GlobalInfo.userInfoList[0].isReady = true;
			GlobalInfo.userInfoList[0].destroyed = false;
			GlobalInfo.userInfoList[0].playerViewID = GlobalInfo.playerViewID;
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
		}else{
			GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,GlobalInfo.userInfo.name,(int)GlobalInfo.userInfo.equipment,true,GlobalInfo.playerViewID);			
		}		
		Camera.main.gameObject.GetComponent<AudioListener>().enabled = false;
		GameObject go = (GameObject)GameObject.Instantiate (panelPref, new Vector3(0,0,-1), Quaternion.identity);
		panel = go.guiTexture;
		panel.enabled = false;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float my = 0.0f;
		bool moveFlag = true;
		float ms = 0.0f;

		if (!GlobalInfo.gameStarted) {
			if(Network.isServer){
				GlobalInfo.userInfoList[0].equipment = GlobalInfo.userInfo.equipment;
				GlobalInfo.userInfoList[0].isReady = true;
				GlobalInfo.userInfoList[0].destroyed = false;
				GlobalInfo.userInfoList[0].playerViewID = GlobalInfo.playerViewID;
				GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
			}else{
				GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,GlobalInfo.userInfo.name,(int)GlobalInfo.userInfo.equipment,true,GlobalInfo.playerViewID);			
			}
			return;
		}	
		GlobalInfo.curPlayer = myPos;

		if(Input.GetKey(KeyCode.LeftShift)) ms = maxSpeed * 0.5f; else ms = maxSpeed;
		if(!GlobalInfo.chatScreenFlag) {
			my = Input.GetAxis("Vertical");
			if(curSpeed == 0.0f){
				if(Input.GetKey(KeyCode.Q)){
					my = 1.0f;moveFlag = false;curSpeed = 0.0f;
				}
				if(Input.GetKey(KeyCode.E)){
					my = -1.0f;moveFlag = false;curSpeed = 0.0f;
				}
			}
		}
		panel.enabled = true;
		panel.pixelInset = new Rect(0,0,Screen.width,Screen.height);
		if(my == 0.0f){
			fbTime = 1.0f;
			fbAngleBackTime += Time.deltaTime * 4.0f;
			speedBackTime += Time.deltaTime * 0.3f;
			if(curFBAngle == 0.0f){
				smallSurgeFlag = true;
			}else{
				if(!smallSurgeFlag){
					if(fbSurgeAngle > 0.0f){
						my = Mathf.Lerp(0.005f,FRONT_SURGE_RPM,fbAngleBackTime);
						heliBody.RotateAroundLocal(Vector3.right,-my);
						fbSurgeAngle -= my;
						if(fbSurgeAngle <= 0.0f){
							smallSurgeFlag = true;
							smallSurgeTime = 0.0f;
							surgeDir = true;
							smallSurgeAngle = Random.Range(Mathf.PI / 20.0f,Mathf.PI / 10.0f);
							smallSurgeRate = Random.Range(0.3f,0.6f);
						}
					}else{
						my = Mathf.Lerp(0.005f,BACK_SURGE_RPM,fbAngleBackTime);
						heliBody.RotateAroundLocal(Vector3.right,my);
						fbSurgeAngle += my;
						if(fbSurgeAngle >= 0.0f){
							smallSurgeFlag = true;
							smallSurgeTime = 0.0f;
							surgeDir = false;
							smallSurgeAngle = Random.Range(Mathf.PI / 20.0f,Mathf.PI / 10.0f);
							smallSurgeRate = Random.Range(0.3f,0.6f);
						}
					}			
				}
			}
			if(curSpeed != 0.0f){
				my = Mathf.Lerp(curSpeed,0.0f,speedBackTime);
				if(speedBackTime > 1.0f) curSpeed = 0.0f;
				transform.Translate(0,0,my,Space.Self);
			}
		}else{
			smallSurgeFlag = false;
			fbAngleBackTime = 0.0f;
			curFBAngle = fbSurgeAngle;
			speedBackTime = 0.0f;
			if(my > 0.0f){
				if(!surgeDir) fbTime = 0.0f;
				surgeDir = true;
				fbTime += Time.deltaTime * 4.0f;
				if(moveFlag){
					curSpeed = Mathf.Lerp(0.0f,ms,fbTime / 5.0f) * Time.deltaTime;
					transform.Translate(0,0,curSpeed,Space.Self);
				}
				if(fbSurgeAngle < MAX_FRONT_SURGEANGLE){
					my = Mathf.Lerp(0.0f,FRONT_SURGE_RPM,1.0f / fbTime);
					heliBody.RotateAroundLocal(Vector3.right,my);
					fbSurgeAngle += my;
				}
			}else{
				if(surgeDir) fbTime = 0.0f;
				surgeDir = false;
				fbTime += Time.deltaTime * 4.0f;
				if(moveFlag){
					curSpeed = Mathf.Lerp(0.0f,-ms * 0.3f,fbTime / 5.0f) * Time.deltaTime;
					transform.Translate(0,0,curSpeed);
				}
				if(fbSurgeAngle > MAX_BACK_SURGEANGLE){
					my = Mathf.Lerp(0.0f,BACK_SURGE_RPM,1.0f / fbTime);
					heliBody.RotateAroundLocal(Vector3.right,-my);
					fbSurgeAngle -= my;
				}
			}
		}
		if(smallSurgeFlag){
			if(surgeDir){
				smallSurgeTime += Time.deltaTime * Mathf.PI * smallSurgeRate;
				my = smallSurgeAngle * Mathf.Cos(smallSurgeTime);
				my *= Mathf.PI / 180;
				heliBody.RotateAroundLocal(Vector3.right,-my);
				fbSurgeAngle -= my;
				if(smallSurgeTime >= Mathf.PI){
					float ang = Vector3.Angle(transform.up,heliBody.forward);
					ang = ang - 102.0f;
					ang = ang * Mathf.PI/ 180.0f;
					heliBody.RotateAroundLocal(Vector3.right,-my);
					surgeDir = !surgeDir;
					smallSurgeAngle = Random.Range(Mathf.PI / 80.0f,Mathf.PI / 50.0f);
					smallSurgeTime = 0.0f;
					smallSurgeRate = Random.Range(0.3f,0.6f);
				}
			}else{
				smallSurgeTime += Time.deltaTime * Mathf.PI * smallSurgeRate;
				my = smallSurgeAngle * Mathf.Cos(smallSurgeTime);
				my *= Mathf.PI / 180;
				heliBody.RotateAroundLocal(Vector3.right,my);
				fbSurgeAngle += my;
				if(smallSurgeTime >= Mathf.PI){
					float ang = Vector3.Angle(transform.up,heliBody.forward);
					ang = ang - 102.0f;
					ang = ang * Mathf.PI/ 180.0f;
					heliBody.RotateAroundLocal(Vector3.right,-my);
					surgeDir = !surgeDir;
					smallSurgeAngle = Random.Range(Mathf.PI / 80.0f,Mathf.PI / 50.0f);
					smallSurgeTime = 0.0f;
					smallSurgeRate = Random.Range(0.3f,0.6f);
				}
			}
		}
		wing1.Rotate(transform.up,Time.deltaTime * 180 / Mathf.PI);
		wing2.Rotate(transform.up,-Time.deltaTime * 540 / Mathf.PI);
	}
	
	void OnDestroy() {
		GameObject.Destroy (panel.gameObject);
	}
}
