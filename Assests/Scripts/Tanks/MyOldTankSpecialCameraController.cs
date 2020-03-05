using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyOldTankSpecialCameraController : MonoBehaviour {
	public Transform h_Gyro;
	public Transform v_Gyro;
	public Texture2D[] specialCamera;
	public Texture2D texture1;
	public Texture2D texture2;
	public float zoomSpeed = 5.0f;

	private int specialCamIndex = 0;
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
	private int targetZoomScale = 2;
	private float curZoomScale = 0.0f;
	private int zoomWay = 0;
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
		if(!GlobalInfo.specialCamState) {
			return;
		}
		mx = Input.GetAxis("Mouse X") / (curZoomScale * 3.0f);
		my = Input.GetAxis("Mouse Y") / (curZoomScale * 3.0f);
		h_Gyro.RotateAroundLocal(Vector3.up,mx);
		fv = Vector3.Angle(h_Gyro.forward,v_Gyro.forward);
		if(Vector3.Angle(h_Gyro.up,v_Gyro.forward) > 90.0f)
			fv = -fv;
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
		if(mx != 0 || my != 0){
			Ray aimRay = new Ray(v_Gyro.position,v_Gyro.forward);
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(aimRay,out hit);
			if(hit.collider != null){
				GlobalInfo.realAimPos = hit.point;
			}else{
				GlobalInfo.realAimPos = aimRay.GetPoint(MAX_DISTANCE);
			}
		}
		targetDist = GlobalInfo.realAimPos - v_Gyro.position; 
		if(targetDist.magnitude <= camMinDistance){
			SendMessage("DisableSpecialCamera",SendMessageOptions.DontRequireReceiver);
			return;
		}
		if(Input.GetKeyDown(KeyCode.Q)){
			blurFlag = true;
			mb.enabled = true;
			mb.blurAmount = 0.8f;
			psTime = 0.0f;
			curZoomScale += Time.deltaTime * zoomSpeed;
			if(curZoomScale > 30.0f) curZoomScale = 30.0f;
			zoomWay = 1;
		}
		if(Input.GetKeyDown(KeyCode.E)){
			blurFlag = true;
			mb.enabled = true;
			mb.blurAmount = 0.8f;
			psTime = 0.0f;
			curZoomScale -= Time.deltaTime * zoomSpeed;
			if(curZoomScale < 2.0f) curZoomScale = 2.0f;
			zoomWay = -1;
		}
		v_Gyro.LookAt(GlobalInfo.realAimPos,transform.up);
		curCamLen = Mathf.Clamp(camLen,camMinDistance,camMaxDistance);
		Camera.main.transform.localRotation = Quaternion.identity;

		float delta = targetZoomScale - curZoomScale;
		if(delta != 0.0f){
//			curZoomScale += Mathf.Sign(delta) * Time.deltaTime * zoomSpeed;
//			if(zoomWay != Mathf.Sign(delta)) curZoomScale = targetZoomScale;
			fv = Mathf.Tan(Mathf.PI / 6.0f) * 1000.0f;
			fv = fv / curZoomScale;
			fv = fv / 2.0f;
			fv = Mathf.Atan(fv / 1000.0f) * 2.0f;
			Camera.main.fieldOfView = fv * 180.0f / Mathf.PI;
		}else{
			blurFlag = false;
		}
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
		if(Input.GetKeyDown(KeyCode.Z)){
			specialCamIndex++;
			if(specialCamIndex == 4) specialCamIndex = 0;
		}
	}
	
	void OnSetCamMinDistance(float a) {
		camMinDistance = a;
		targetZoomScale = 2;
		curZoomScale = 2.0f;
	}
	
	void OnGUI () {
		if(!GlobalInfo.specialCamState) return;
		float w = Screen.height * 12.0f / 9.0f;
		GUI.DrawTexture(new Rect(Screen.width / 2.0f - w,0.0f,w * 2.0f,Screen.height),specialCamera[specialCamIndex]);
		GUI.DrawTexture(new Rect(Screen.width * 0.8f,Screen.height * 0.11f,Screen.width * 0.12f,Screen.height * 0.03f),texture1);
		int x;
		float y = 0.0f;
		x = Mathf.FloorToInt(targetDist.magnitude * 10.0f);
		y = x / 10.0f;
		string str = y.ToString();
		string tmp = "";
		for(int i = 0;i < str.Length;i++){
			tmp = str.Substring(i,1);
			if(tmp == ".")
				GUI.DrawTexture(new Rect(Screen.width * 0.925f + Screen.width * 0.01f * i,Screen.height * 0.1f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/Dot_G"));
			else
				GUI.DrawTexture(new Rect(Screen.width * 0.925f + Screen.width * 0.01f * i,Screen.height * 0.1f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));
		}
		GUI.DrawTexture(new Rect(Screen.width * 0.8f,Screen.height * 0.24f,Screen.width * 0.12f,Screen.height * 0.03f),texture2);
		x = Mathf.FloorToInt(curZoomScale * 10.0f);
		y = x / 10.0f;
		str = y.ToString();
		x = 0;
		for(int i = 0;i < str.Length;i++){
			tmp = str.Substring(i,1);
			if(tmp == ".")
				GUI.DrawTexture(new Rect(Screen.width * 0.925f + Screen.width * 0.01f * i,Screen.height * 0.23f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/Dot_G"));
			else	
				GUI.DrawTexture(new Rect(Screen.width * 0.925f + Screen.width * 0.01f * i,Screen.height * 0.23f,Screen.width * 0.01f,Screen.height * 0.05f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));
			x++;
		}		
	}
}
