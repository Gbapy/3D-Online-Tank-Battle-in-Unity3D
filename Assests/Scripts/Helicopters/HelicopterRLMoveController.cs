using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterRLMoveController : MonoBehaviour {
	public Transform heliBody;
	public float maxSpeed = 8.0f;
	
	private float rlTime = 0.0f;
	private float rlAngleBackTime = 0.0f;
	private float speedBackTime = 0.0f;
	private float rlSurgeAngle = 0.0f;
	private bool smallSurgeFlag = true;
	private float smallSurgeTime = 0.0f;
	private bool surgeDir = true;
	private float smallSurgeAngle = 0.0f;
	private float curRLAngle = 0.0f;
	private float curSpeed = 0.0f;
	private float smallSurgeRate = 0.0f;
	
	const float MAX_SURGEANGLE = Mathf.PI / 4.0f;
	const float SURGE_RPM = Mathf.PI / 300.0f;
	
	// Use this for initialization
	void Start () {
		if(!networkView.isMine){
			enabled = false;
			return;
		}
		smallSurgeRate = Random.Range(0.5f,2.0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float mx = 0.0f;
		float ms = 0.0f;

		if(!GlobalInfo.gameStarted) return;
		if(!GlobalInfo.chatScreenFlag)mx = Input.GetAxis("Horizontal");
		if(Input.GetKey(KeyCode.LeftShift)) ms = maxSpeed * 0.5f; else ms = maxSpeed;
		if(mx == 0.0f){
			rlTime = 1.0f;
			rlAngleBackTime += Time.deltaTime * 4.0f;
			speedBackTime += Time.deltaTime * 0.3f;
			if(curRLAngle == 0.0f){
				smallSurgeFlag = true;
			}else{
				if(!smallSurgeFlag){
					if(rlSurgeAngle > 0.0f){
						mx = Mathf.Lerp(0.005f,SURGE_RPM,rlAngleBackTime);
						heliBody.RotateAroundLocal(Vector3.forward,-mx);
						rlSurgeAngle -= mx;
						if(rlSurgeAngle <= 0.0f){
							smallSurgeFlag = true;
							smallSurgeTime = 0.0f;
							surgeDir = true;
							smallSurgeAngle = Random.Range(Mathf.PI / 10.0f,Mathf.PI / 5.0f);
							smallSurgeRate = Random.Range(0.3f,0.6f);
						}
					}else{
						mx = Mathf.Lerp(0.005f,SURGE_RPM,rlAngleBackTime);
						heliBody.RotateAroundLocal(Vector3.forward,mx);
						rlSurgeAngle += mx;
						if(rlSurgeAngle >= 0.0f){
							smallSurgeFlag = true;
							smallSurgeTime = 0.0f;
							surgeDir = false;
							smallSurgeAngle = Random.Range(Mathf.PI / 10.0f,Mathf.PI / 5.0f);
							smallSurgeRate = Random.Range(0.3f,0.6f);
						}
					}			
				}
			}
			if(curSpeed != 0.0f){
				mx = Mathf.Lerp(curSpeed,0.0f,speedBackTime);
				if(speedBackTime > 1.0f) curSpeed = 0.0f;
				transform.Translate(mx,0,0,Space.Self);
			}
		}else{
			smallSurgeFlag = false;
			rlAngleBackTime = 0.0f;
			curRLAngle = rlSurgeAngle;
			speedBackTime = 0.0f;
			if(mx > 0.0f){
				if(!surgeDir) rlTime = 0.0f;
				surgeDir = true;
				rlTime += Time.deltaTime * 3.0f;
				curSpeed = Mathf.Lerp(0.0f,ms,rlTime / 5.0f) * Time.deltaTime;
				transform.Translate(curSpeed,0,0,Space.Self);
				if(Mathf.Abs(rlSurgeAngle) < MAX_SURGEANGLE){
					mx = Mathf.Lerp(0.0f,SURGE_RPM,1.0f / rlTime);
					heliBody.RotateAroundLocal(Vector3.forward,-mx);
					rlSurgeAngle -= mx;
				}
			}else{
				if(surgeDir) rlTime = 0.0f;
				surgeDir = false;
				rlTime += Time.deltaTime * 3.0f;
				curSpeed = Mathf.Lerp(0.0f,-ms,rlTime / 5.0f) * Time.deltaTime;
				transform.Translate(curSpeed,0,0);
				if(Mathf.Abs(rlSurgeAngle) < MAX_SURGEANGLE){
					mx = Mathf.Lerp(0.0f,SURGE_RPM,1.0f / rlTime);
					heliBody.RotateAroundLocal(Vector3.forward,mx);
					rlSurgeAngle += mx;
				}
			}
		}
		if(smallSurgeFlag){
			if(surgeDir){
				smallSurgeTime += Time.deltaTime * Mathf.PI * smallSurgeRate;
				mx = smallSurgeAngle * Mathf.Cos(smallSurgeTime);
				mx *= Mathf.PI / 180;
				heliBody.RotateAroundLocal(Vector3.forward,-mx);
				rlSurgeAngle -= mx;
				if(smallSurgeTime >= Mathf.PI){
					surgeDir = !surgeDir;
					smallSurgeAngle = Random.Range(Mathf.PI / 80.0f,Mathf.PI / 50.0f);
					smallSurgeTime = 0.0f;
					smallSurgeRate = Random.Range(0.3f,0.6f);
				}
			}else{
				smallSurgeTime += Time.deltaTime * Mathf.PI * smallSurgeRate;
				mx = smallSurgeAngle * Mathf.Cos(smallSurgeTime);
				mx *= Mathf.PI / 180;
				heliBody.RotateAroundLocal(Vector3.forward,mx);
				rlSurgeAngle += mx;
				if(smallSurgeTime >= Mathf.PI){
					surgeDir = !surgeDir;
					smallSurgeAngle = Random.Range(Mathf.PI / 80.0f,Mathf.PI / 50.0f);
					smallSurgeTime = 0.0f;
					smallSurgeRate = Random.Range(0.3f,0.6f);
				}
			}
		}
//		transform.localRotation = Quaternion.Euler (new Vector3 (transform.localRotation.x, 0.0f, transform.localRotation.z));
	}
}
