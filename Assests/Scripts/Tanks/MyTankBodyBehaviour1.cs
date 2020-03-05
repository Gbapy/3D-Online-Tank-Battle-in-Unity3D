using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankBodyBehaviour1 : MonoBehaviour {
	public Transform[] wheel;
	public Transform[] wheelColliderObject;
	public Renderer leftCrawler;
	public Renderer rightCrawler;
	public Material crawlerMat;
	public float maxSpeed;
	public float maxSteerAngle;
	public Transform CoM;
	public Transform surgeForcePos;
	public ParticleEmitter DepartSteamLeft;
	public ParticleEmitter DepartSteamRight;
	public ParticleEmitter RunningSteamLeft;
	public ParticleEmitter RunningSteamRight;
	public Transform leftSkidMarkPosition;
	public Transform rightSkidMarkPosition;
	public Transform myPos;
	public float forwadSurgeForce = -5.0f;
	public float backwardSurgeForce = 3.0f;
	public float surgeTime = 0.25f;
	public float mass;
	public float wheelMass = 1.0f;
	public float wheelRadius = 0.4f;
	public float wheelSuspensionDistance = 0.1f;
	public float wheelSpring = 100.0f;
	public float wheelDamper = 500.0f;
	public float wheelTargetPosition = 6.0f;
	public Transform head;
	public Transform waterFoam;
	
	private WheelCollider[] wheelCollider;
	private int leftLastIndex = -1;
	private int rightLastIndex = -1;
	private float psTime = 0.0f;
	private float surgeForce = 0.0f;
	private float surgeAnimTime = 0.0f;
	private bool surgeAnimFlag = false;
	private Transform skidMark;
	private Skidmarks skid;
	private bool movable = true;
	private Material myCrawlerMat;
	private bool floatFlag = false;
	
	const float skidMarkIntensity = 1.0f;
	const int WHEEL_COUNT = 16;
	const float TANK_WIDTH = 3.5f;
	const float TANK_HEIGHT = 7.0f;
	const float SURGE_TIME = 0.25f;
	const float BELTWIDH = 1.8f;
	const float SURGEANIMPERIOD = 1.0f;
	const float WATER_LEVEL = 30.5f;
	
	// Use this for initialization
	void Awake () {
		skidMark = GameObject.Find("Skidmarks").transform;
		skid = skidMark.GetComponent<Skidmarks>();
		myCrawlerMat = new Material(crawlerMat);
		leftCrawler.sharedMaterial = myCrawlerMat;
		rightCrawler.sharedMaterial = myCrawlerMat;
//		if(!networkView.isMine) return;
//		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
//		rb.mass = mass;
//		rb.drag = 0.0f;
//		rb.angularDrag = 0.0f;
//		rb.useGravity = true;
//		rb.isKinematic = false;
		rigidbody.centerOfMass = CoM.localPosition;
		JointSpring js;
		wheelCollider = new WheelCollider[WHEEL_COUNT];
		for(int i=0;i<WHEEL_COUNT;i++){
			wheelCollider[i] = wheelColliderObject[i].gameObject.AddComponent<WheelCollider>();
			wheelCollider[i].mass = wheelMass;
			wheelCollider[i].radius = wheelRadius;
			wheelCollider[i].suspensionDistance = wheelSuspensionDistance;
			js = new JointSpring();
			js.spring = wheelSpring;
			js.damper = wheelDamper;
			js.targetPosition = wheelTargetPosition;
			wheelCollider[i].suspensionSpring = js;
			wheelCollider[i].brakeTorque = Mathf.Infinity;
		}		
//		head.GetComponent<MyTankHeadBehaviour>().mTank = rb;
//		GlobalInfo.playerViewID = networkView.viewID;
		Camera.main.gameObject.GetComponent<AudioListener>().enabled = true;
//		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		bool keyUpFlag = false;
		
//		if(!networkView.isMine) return;
//		if(GlobalInfo.chatScreenFlag) return;
		if(!IsGrounded() && !floatFlag){
			return;
		}
		float spd = Input.GetAxis("Vertical");
//		float str = Input.GetAxis("Horizontal");
		
		if(leftSkidMarkPosition.position.y < WATER_LEVEL)//if(myPos.position.y < GlobalInfo.water_Level)
			floatFlag = true;
		else
			floatFlag = false;
//		print (leftSkidMarkPosition.position.y +":"+ WATER_LEVEL);
//		if(spd == 0.0f && str == 0.0f)
//			head.SendMessage("OnNotMove",SendMessageOptions.DontRequireReceiver);
//		else
//			head.SendMessage("OnMove",SendMessageOptions.DontRequireReceiver);	
//		if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow)){
//			psTime = 0.0f;
//			keyUpFlag = true;
//		}
//		if(!movable){
//			spd = 1.0f;
//			psTime = 0.0f;
//			movable = true;
//		}
//		GlobalInfo.curPlayerPosition = myPos.position;
//		if(surgeAnimFlag){
//			surgeAnimTime += Time.deltaTime;
//			if(surgeAnimTime > SURGEANIMPERIOD){
//				surgeAnimFlag = false;
//			}
//		}		

//		GlobalInfo.isMoving = false;
//		if(spd != 0){
			if(psTime == 0.0f){
//				if(spd > 0) surgeForce = forwadSurgeForce; else surgeForce = backwardSurgeForce;
//				if(surgeAnimFlag) return;
				rigidbody.AddForceAtPosition(new Vector3(0.0f,surgeForce,0.0f),surgeForcePos.position,ForceMode.Acceleration);
				surgeAnimFlag = true;
				surgeAnimTime = 0.0f;
				DepartSteamLeft.emit = true;
				DepartSteamRight.emit = true;
			}
			psTime += Time.deltaTime / 2;
//			if(psTime >= surgeTime){
				for(int i=0;i<WHEEL_COUNT;i++){
					wheel[i].RotateAroundLocal(Vector3.left, -maxSpeed * Time.deltaTime);
				}
//				if(spd != 0.0f)
					myCrawlerMat.mainTextureOffset = new Vector2(myCrawlerMat.mainTextureOffset.x + 0.5f * Time.deltaTime, myCrawlerMat.mainTextureOffset.y);
				float ang = Vector3.Angle(Vector3.up,transform.forward);
				if(spd > 0.0f){
					if(ang > 90.0f){
						ang = ang - 90.0f;
						ang = ang * 2.0f;
						ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
					}else{
						ang = 90.0f - ang;
						ang = ang * 2.0f;
						ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
					}
				}else{
					if(ang > 90.0f){
						ang = ang - 90.0f;
						ang = ang * 2.0f;
						ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
					}else{
						ang = 90.0f - ang;
						ang = ang * 2.0f;
						ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
					}				
				}
//				if(keyUpFlag)ang = 0.0f;//ang * 0.1f;
//				spd = spd + ang;
				transform.Translate(new Vector3(0,0, Mathf.Lerp(0 , maxSpeed , (psTime - SURGE_TIME)) * Time.deltaTime),Space.Self);
				RunningSteamLeft.emit = true;
				RunningSteamRight.emit = true;
				DepartSteamLeft.emit = false;
				DepartSteamRight.emit = false;				
//			}
//			GlobalInfo.isMoving = true;
//		}else{
//			if(psTime != 0){
//				if(surgeForce == forwadSurgeForce) surgeForce = backwardSurgeForce; else surgeForce = forwadSurgeForce;
//				if(surgeAnimFlag) return;
//				rigidbody.AddForceAtPosition(new Vector3(0.0f,surgeForce,0.0f),surgeForcePos.position,ForceMode.Acceleration);
//				surgeAnimFlag = true;
//				surgeAnimTime = 0.0f;
//				psTime = 0.0f;
//				RunningSteamLeft.emit = false;
//				RunningSteamRight.emit = false;	
//				DepartSteamLeft.emit = false;
//				DepartSteamRight.emit = false;						
//			}
//		}
//		if(str != 0.0f ){
//			transform.RotateAroundLocal(new Vector3(0,1,0),str * maxSteerAngle * Time.deltaTime);
//			if(spd == 0.0f)
//				myCrawlerMat.mainTextureOffset = new Vector2(myCrawlerMat.mainTextureOffset.x + str * Time.deltaTime, myCrawlerMat.mainTextureOffset.y);
////			GlobalInfo.isMoving = true;
//		}
	}

	void OnCollisionStay(Collision coll) {
		if(!floatFlag){
//			if(GlobalInfo.isMoving){
				Vector3 aaa = coll.contacts[0].normal;
//				aaa = new Vector3(aaa.x,aaa.y * -1, aaa.z);
				if(wheelCollider[5].isGrounded || wheelCollider[6].isGrounded){
					leftLastIndex = skid.AddSkidMark(leftSkidMarkPosition.position,aaa ,skidMarkIntensity,BELTWIDH,leftLastIndex);
				}else{
					leftLastIndex = -1;
				}
				if(wheelCollider[13].isGrounded || wheelCollider[14].isGrounded) {
					rightLastIndex = skid.AddSkidMark(rightSkidMarkPosition.position,aaa,skidMarkIntensity,BELTWIDH,rightLastIndex);
				}else{
					rightLastIndex = -1;
				}
//			}
			foreach(Transform a in waterFoam){
				a.particleEmitter.emit = false;
			}			
		}else{
			foreach(Transform a in waterFoam){
				a.particleEmitter.emit = true;
			}			
			leftLastIndex = -1;
			rightLastIndex = -1;
		}			
	}
	
	void OnCollisionEnter(Collision col){
		Vector3 tmp = col.contacts[0].point - myPos.position;
		tmp.Normalize();
		float ang =	Vector3.Angle(transform.up,tmp);
		if(ang < 100.0f){
			movable = false;
		}
	}
	
	bool IsGrounded(){
		bool flag = false;
		
		Ray mRay = new Ray(Vector3.zero,Vector3.down);
		RaycastHit hit = new RaycastHit();
		Vector3 tmp;
		for(int i=0;i<16;i++){
			mRay.origin = wheelCollider[i].transform.position;
			Physics.Raycast(mRay,out hit);
			if(hit.collider != null){
				tmp = hit.point - mRay.origin;
				if(tmp.magnitude < 3.0f){
					flag = true;
					break;
				}
			}
		}
		return flag;
	}
}