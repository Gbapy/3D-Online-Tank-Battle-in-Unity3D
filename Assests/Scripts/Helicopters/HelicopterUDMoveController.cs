using UnityEngine;
using System.Collections;
using MagicBattle;
public class HelicopterUDMoveController : MonoBehaviour {
	public float maxUDSpeed = 10.0f;
	private int moveDir = 0;
	private float curSpeed = 0.0f;
	private float psTime = 0.0f;
	private bool keyUpFlag = false;
	
	const float MAX_HEIGHT = 40.0f;
	const float MIN_HEIGHT = 12.0f;
	// Use this for initialization

	// Update is called once per frame
	void Start() {
		if(!networkView.isMine){
			enabled = false;
		}	
	}
	
	void Update () {
		
		if(!GlobalInfo.gameStarted) return;
		if(GlobalInfo.chatScreenFlag) return;
		psTime += Time.deltaTime * 0.3f;
		if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.F)){
			psTime = 0.0f;
			curSpeed = 0.0f;
		}
		if(Input.GetKey(KeyCode.R)){
			moveDir = 1;
			curSpeed = moveDir * Mathf.Lerp(0.0f,maxUDSpeed,psTime) * Time.deltaTime;
			if(GetHeight(transform.position) < MAX_HEIGHT)
				transform.Translate(0,curSpeed,0);
			else
				curSpeed = 0.0f;
			keyUpFlag  = false;
		}else if(Input.GetKey(KeyCode.F)){
			moveDir = -1;
			curSpeed = moveDir * Mathf.Lerp(0.0f,maxUDSpeed,psTime) * Time.deltaTime;
			if(GetHeight(transform.position) > MIN_HEIGHT)
				transform.Translate(0,curSpeed,0);
			else
				curSpeed = 0.0f;
			keyUpFlag = false;
		}
		if(Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.F)){
			psTime = 0.0f;
			keyUpFlag = true;
		}
		if(keyUpFlag && moveDir != 0){
			if(curSpeed != 0.0f){
				float t = Mathf.Lerp(curSpeed,0.0f,psTime);
				if(curSpeed > 0.0f){
					if(GetHeight(transform.position) < MAX_HEIGHT)
						transform.Translate(0,t,0);	
					else{
						moveDir = 0;curSpeed = 0.0f;
					}
				}
				if(curSpeed < 0.0f){
					if(GetHeight(transform.position) > MIN_HEIGHT)
						transform.Translate(0,t,0);
					else{
						moveDir = 0;curSpeed = 0.0f;
					}
				}
				if(psTime > 1.0f){
					moveDir = 0;curSpeed = 0.0f;
				}
			}else{
				moveDir = 0;
			}
		}
	}
	
	float GetHeight(Vector3 p){
		Ray r = new Ray(p,Vector3.down);
		RaycastHit h = new RaycastHit();
		
		Physics.Raycast(r,out h);
		if(h.collider != null){
			Vector3 t = p - h.point;
			return t.magnitude;
		}
		return 0.0f;
	}	
}
