using UnityEngine;
using System.Collections;
using MagicBattle;

public class TankSpecialCameraController : MonoBehaviour {
	public Transform h_Gyro;
	public Transform v_Gyro;
	public float zoomSpeed = 300.0f;
	public Texture2D specialCamera;

	private bool blurFlag = false;
	private float psTime = 0.0f;
	private float zoomTime = 0.0f;
	private MotionBlur mb;
	private float camLen = 0.0f;
	private float camMinDistance = 0.0f;
	private float camMaxDistance = 0.0f;
	private Vector3 targetDist;
	private float curCamLen = 0.0f;
	private DepthOfField34 df;
	
	const float MAX_DISTANCE = 300.0f;	
	// Use this for initialization
	void Start () {
		if(!networkView.isMine){
			enabled = false;
			return;
		}
		mb = Camera.main.GetComponent<MotionBlur>();
		df = Camera.main.GetComponent<DepthOfField34>();
	}
	
	// Update is called once per frame
	void Update () {
		float fv = 0.0f;
		float mx = 0.0f;
		float my = 0.0f;
		
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.chatScreenFlag) return;
		if(!GlobalInfo.specialCamState) return;
		if(Input.GetKey(KeyCode.LeftShift)) {
			mx = Input.GetAxis("Mouse X") * 0.2f / camLen;
			my = Input.GetAxis("Mouse Y") * 0.2f / camLen;
		}else{
			mx = Input.GetAxis("Mouse X") / camLen;
			my = Input.GetAxis("Mouse Y") / camLen;
		}
		if(mx != 0 || my != 0){
			h_Gyro.RotateAroundLocal(Vector3.up,mx);
			fv = Vector3.Angle(h_Gyro.forward,v_Gyro.forward);
//			if(Vector3.Angle(h_Gyro.up,v_Gyro.forward) > 90.0f)
				fv = 90 - Vector3.Angle(h_Gyro.up,v_Gyro.forward);
			my *= 180.0f / Mathf.PI;
			fv += my;
			if(fv > 40.0f || fv < -10.0f){
				if(fv > 40.0f)
					my -= fv - 40.0f;
				else
					my += -10.0f - fv;
			}
			my *= Mathf.PI / 180.0f;
			v_Gyro.RotateAroundLocal(Vector3.right,-my);
			Ray aimRay = new Ray(v_Gyro.position,v_Gyro.forward);
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(aimRay,out hit);
			if(hit.collider != null){
				GlobalInfo.realAimPos = hit.point;
			}else{
				GlobalInfo.realAimPos = aimRay.GetPoint(10000.0f);
			}
		}
		targetDist = GlobalInfo.realAimPos - v_Gyro.position; 
		if(targetDist.magnitude > MAX_DISTANCE) 
			camMaxDistance = MAX_DISTANCE - 1.0f;
		else
			camMaxDistance = targetDist.magnitude - 1.0f;
		if(camMaxDistance <= camMinDistance){
			SendMessage("DisableSpecialCamera",SendMessageOptions.DontRequireReceiver);
			return;
		}
		if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)){
			blurFlag = true;
			mb.enabled = true;
			mb.blurAmount = 0.8f;
			psTime = 0.0f;
			zoomTime = 0.0f;
		}
		if(Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E)){
			blurFlag = false;
		}
		if(Input.GetKey(KeyCode.Q)){
			zoomTime += Time.deltaTime * 0.5f;
			fv = Mathf.Lerp(0.0f,zoomSpeed,zoomTime);
			camLen += fv * Time.deltaTime;
			if(camLen > MAX_DISTANCE) {
				camLen = MAX_DISTANCE;
				blurFlag = false;
			}
		}
		if(Input.GetKey(KeyCode.E)){
			zoomTime += Time.deltaTime;
			fv = Mathf.Lerp(0.0f,zoomSpeed,zoomTime);
			camLen -= fv * Time.deltaTime;
			if(camLen < camMinDistance){
				camLen = camMinDistance;
				blurFlag = false;
			}
		}
		if(GlobalInfo.isMoving)
			camLen = camMinDistance;
//		else
		v_Gyro.LookAt(GlobalInfo.realAimPos,transform.up);
		curCamLen = Mathf.Clamp(camLen,camMinDistance,camMaxDistance);
		if(curCamLen < camLen){
			fv = 1000.0f - camLen;
			fv = fv * Mathf.Tan(Mathf.PI / 6.0f);
			fv = Mathf.Atan(fv / (1000.0f - curCamLen));
			fv = fv * 180.0f / Mathf.PI;
			Camera.main.fieldOfView = fv;
			df.enabled = true;
			df.focalPoint = camLen;
		}else if(curCamLen == camLen){
			df.enabled = false;
			Camera.main.fieldOfView = 60.0f;
		}
		Camera.main.transform.position = v_Gyro.position + v_Gyro.forward * curCamLen;
		Camera.main.transform.localRotation = Quaternion.identity;
		if(!blurFlag){
			if(mb.enabled){
				psTime += Time.deltaTime * 0.5f;
				fv = mb.blurAmount - psTime;
				if(fv < 0.0f){
					mb.enabled = false;
					fv = 0.0f;
				}
				mb.blurAmount = fv;
			}
		}
	}
	
	void OnSetCamMinDistance(float a) {
		camMinDistance = a;
		camLen = a;
	}
	
	void OnGUI () {
		if(!GlobalInfo.specialCamState) return;
		GUI.DrawTexture(new Rect(Screen.width / 2.0f - Screen.height * 12.0f / 9.0f,0.0f,Screen.height * 24.0f / 9.0f,Screen.height),specialCamera);
		int x;
		float y = 0.0f;
		x = Mathf.FloorToInt(targetDist.magnitude * 10.0f);
		y = x / 10.0f;
		string str = y.ToString();
		string tmp = "";
		for(int i = 0;i < str.Length;i++){
			tmp = str.Substring(i,1);
			if(tmp == ".")
				GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * i,Screen.height * 0.1f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/Dot_G"));
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * i,Screen.height * 0.1f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));
		}
		GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * str.Length,Screen.height * 0.12f,Screen.width * 0.02f,Screen.height * 0.03f),(Texture)Resources.Load("GUI/SpecialCamera/T_1"));
		x = Mathf.FloorToInt(camLen);
		y = x / 10.0f;
		str = y.ToString();
		x = 0;
		for(int i = 0;i < str.Length;i++){
			tmp = str.Substring(i,1);
			if(tmp == ".")
				GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * i,Screen.height * 0.23f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/Dot_G"));
			else	
				GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * i,Screen.height * 0.23f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));
			x++;
		}	
		GUI.DrawTexture(new Rect(Screen.width * 0.9f + Screen.width * 0.01f * str.Length,Screen.height * 0.25f,Screen.width * 0.02f,Screen.height * 0.03f),(Texture)Resources.Load("GUI/SpecialCamera/T_2"));
	}
}
