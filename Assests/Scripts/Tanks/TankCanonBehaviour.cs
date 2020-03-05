using UnityEngine;
using System.Collections;
using MagicBattle;

public class TankCanonBehaviour : MonoBehaviour {
	public float camVibrateForce = 0.6f;
	public float camVibratePeriod = 1.0f;
	public Transform secondaryCamPos;
	public Transform thirdCamPos;
	
	private float camAnimTime = 0.0f;
	private Vector3 camPos;
	private Transform cam;
	private int vibDir = -1;
	private float vibrateForce = 0.0f;
	// Use this for initialization
	void Start () {
		if(!networkView.isMine){
			enabled = false;
			return;
		}
		cam = Camera.main.transform;
		GlobalInfo.canon = gameObject;
	}
	
	// Update is called once per frame
	void Update () {	
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.camAnimFlag == true){

			if(camAnimTime == 0.0f)
				SendMessageUpwards("SetAimCrossControlFlag",false,SendMessageOptions.DontRequireReceiver);
			camAnimTime += Time.deltaTime;
			float tmp = Mathf.Lerp(vibrateForce,0.0f,camAnimTime / camVibratePeriod);//
			tmp = tmp * vibDir;
			vibDir = -vibDir;
			if(GlobalInfo.specialCamState) {
				cam.localPosition = camPos + new Vector3(Random.value * tmp,Random.value * tmp,Random.value * tmp);
				if(camAnimTime > camVibratePeriod) {
					cam.localPosition = camPos;
					GlobalInfo.camAnimFlag = false;
					camAnimTime = 0.0f;
					this.SendMessageUpwards("SetAimCrossControlFlag",true,SendMessageOptions.DontRequireReceiver);
				}
			}else{
				switch(GlobalInfo.camPosState){
				case 0:
					cam.position = cam.position + new Vector3(Random.value * tmp * 5.0f,Random.value * tmp * 5.0f,Random.value * tmp * 5.0f);
					break;
				case 1:
					cam.position = secondaryCamPos.position + new Vector3(Random.value * tmp / 2.0f,Random.value * tmp / 2.0f,Random.value * tmp / 2.0f);
					break;
				case 2:
					//cam.position = thirdCamPos.position + new Vector3(Random.value * tmp / 5.0f,Random.value * tmp / 5.0f,Random.value * tmp / 5.0f); 
					break;
				default:
					break;
				}
				if(camAnimTime > camVibratePeriod) {
					Camera.mainCamera.GetComponent<MotionBlur>().enabled = false;
					GlobalInfo.camAnimFlag = false;
					camAnimTime = 0.0f;
					switch(GlobalInfo.camPosState){
					case 0:
						//cam.localPosition = camPos;
						break;
					case 1:
						cam.position = secondaryCamPos.position;
						break;
					case 2:
						cam.position = thirdCamPos.position;
						break;
					default:
						break;
					}				
					this.SendMessageUpwards("SetAimCrossControlFlag",true,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	void SetEnabled(bool ena){
		this.enabled = ena;
	}
	
	void OnAttackVibrate(ShellAttackedSendMsgParam param){
		if(!networkView.isMine)return;
		if (param.attackedShellKind == ShellKind.Bullet)return;
		Vector3 tmp = param.attackedPoint - transform.position;
		if(tmp.magnitude < GlobalInfo.shellProperty[(int)param.attackedShellKind].explosionRadius){
			if(GlobalInfo.specialCamState){
				camPos = cam.localPosition;
			}else{
				switch(GlobalInfo.camPosState){
				case 0:
					break;
				case 1:
					camPos = secondaryCamPos.position;
					break;
				case 2:
					camPos = thirdCamPos.position;
					break;
				default:
					break;
				}							
			}
			vibrateForce = camVibrateForce;
			GlobalInfo.camAnimFlag = true;
			camAnimTime = 0.0f;
//			Camera.mainCamera.GetComponent<MotionBlur>().enabled = true;
			Camera.mainCamera.GetComponent<MotionBlur>().blurAmount = 1.0f;
		}
	}
	
	void OnShootVibrate(){
		if(!networkView.isMine)return;
		if(GlobalInfo.specialCamState){
			camPos = cam.localPosition;
		}else{
			switch(GlobalInfo.camPosState){
			case 0:
				break;
			case 1:
				camPos = secondaryCamPos.position;
				break;
			case 2:
				camPos = thirdCamPos.position;
				break;
			default:
				break;
			}					
		}
		vibrateForce = camVibrateForce / 5.0f;
		GlobalInfo.camAnimFlag = true;
		camAnimTime = 0.0f;
//		Camera.mainCamera.GetComponent<MotionBlur>().enabled = true;
		Camera.mainCamera.GetComponent<MotionBlur>().blurAmount = 1.0f;
	}
}
