using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankCanonBehaviour1 : MonoBehaviour {
	public float camVibrateForce = 0.6f;
	public float camVibratePeriod = 1.0f;
	
	private bool camAnimFlag = false;
	private float camAnimTime = 0.0f;
	private Vector3 camPos;
	private Transform cam;
	private bool attackedFlag = false;
	private int vibDir = -1;
	// Use this for initialization
	void Start () {
//		if(!networkView.isMine)return;
		cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
//		if(!networkView.isMine) return;
		if(camAnimFlag == true){
			if(camAnimTime == 0.0f)
				SendMessageUpwards("SetAimCrossControlFlag",false,SendMessageOptions.DontRequireReceiver);
			camAnimTime += Time.deltaTime;
			float tmp = Mathf.Lerp(camVibrateForce,0.0f,camAnimTime / camVibratePeriod);//
			if(attackedFlag) tmp = tmp * 5.0f;
			tmp = tmp * vibDir;
			vibDir = -vibDir;
			cam.position = camPos + new Vector3(Random.value * tmp,Random.value * tmp,Random.value * tmp);
			if(camAnimTime > camVibratePeriod) {
				camAnimFlag = false;
				cam.position = camPos;
				camAnimTime = 0.0f;
				this.SendMessageUpwards("SetAimCrossControlFlag",true,SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void SetEnabled(bool ena){
		this.enabled = ena;
	}
	
	void OnCameraVibrate(bool flag){
		attackedFlag = flag;
//		if(!networkView.isMine)return;
		camAnimFlag = true;
		camAnimTime = 0.0f;
		camPos = cam.position;		
	}
}
