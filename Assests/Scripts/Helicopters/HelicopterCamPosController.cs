using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterCamPosController : MonoBehaviour {
	public Transform cam;
	public Transform primaryCamPos;
	public Transform secondaryCamPos;
	public GameObject outsideLight;
	public GameObject insideLight;
	private bool animFlag = false;
	private float psTime = 0.0f;
	private Vector3 dir;
	// Use this for initialization
	void Start () {
		if(!networkView.isMine)
			enabled = false;
		if(GlobalInfo.nightOrNoonFlag == 0){
			outsideLight.SetActive(true);
			insideLight.SetActive(true);
		}else{
			outsideLight.SetActive(false);
			insideLight.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!GlobalInfo.gameStarted) return;
		if(!GlobalInfo.chatScreenFlag){
			if(Input.GetKeyDown(KeyCode.C)){
				if(GlobalInfo.camPosState == 1){
					dir = primaryCamPos.position - cam.position;
				}else if(GlobalInfo.camPosState == 0){
					dir = secondaryCamPos.position - cam.position;
				}
				animFlag = true;
				psTime= 0.0f;
				dir = dir / 0.3f;
				GlobalInfo.camPosState = 1 - GlobalInfo.camPosState;
			}
		}
		if(animFlag){
			psTime += Time.deltaTime;
			cam.position = cam.position + dir * Time.deltaTime;
			if(psTime >= 0.3f){
				if(GlobalInfo.camPosState == 0){
					cam.position = primaryCamPos.position;
					if(GlobalInfo.nightOrNoonFlag == 0){
						cam.GetComponent<UltraRedRayCamera>().enabled = false;
						Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
					}
				}else{
					cam.position = secondaryCamPos.position;
					if(GlobalInfo.nightOrNoonFlag == 0){
						cam.GetComponent<UltraRedRayCamera>().enabled = true;
						RenderSettings.ambientLight = Color.white;
					}
				}
				animFlag = false;
			}
		}
	}
}
