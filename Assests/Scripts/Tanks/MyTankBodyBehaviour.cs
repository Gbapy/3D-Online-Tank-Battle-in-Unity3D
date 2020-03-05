using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankBodyBehaviour : MonoBehaviour {
	public TankControlState tankControlState = TankControlState.Manual;
	public int wheelCount = 16;
	public Transform[] wheelColliderObject;
	public Renderer leftCrawler;
	public Renderer rightCrawler;
	public Texture[] crawlerTex;
	public float maxSpeed;
	public float maxSteerAngle;
	public Transform CoM;
	public Transform surgeForcePos;
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
	public float crawlerWidth = 1.08f;
	public Material crawlerMat;
	public GameObject lighter;
	public bool isKinamatics = false;

	private Material leftCrawlerMat;
	private Material rightCrawlerMat;
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
	private bool floatFlag = false;
	private float spd = 0.0f;
	private float str = 0.0f;
	private bool escaped = false;
	private float curLeftCrawlerTexIndex = 0;
	private float curRightCrawlerTexIndex = 0;
	private int halfWheelCount = 0;
	private bool lightUpFlag = false;
	private float mSpd = 0.0f;
	private float mStr = 0.0f;
	private bool forwardHitFlag = false;
	private bool backwardHitFlag = false;
	private bool forwardRightHitFlag = false;
	private bool backwardRightHitFlag = false;
	private bool forwardLeftHitFlag = false;
	private bool backwardLeftHitFlag = false;
	private bool stateSetFlag = false;
	private string nam = "";

	const float skidMarkIntensity = 1.0f;
	const float TANK_WIDTH = 3.5f;
	const float TANK_HEIGHT = 7.0f;
	const float SURGE_TIME = 0.25f;
	const float SURGEANIMPERIOD = 1.0f;
	
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		if(!networkView.isMine) {
			enabled = false;
			return;
		}
		skidMark = GameObject.Find("Skidmarks").transform;
		skid = skidMark.GetComponent<Skidmarks>();
		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = 0.0f;
		rb.angularDrag = 0.0f;
		if(isKinamatics){
			rb.useGravity = false;
			rb.isKinematic = true;
		}else{
			rb.useGravity = true;
			rb.isKinematic = false;
		}
		rigidbody.centerOfMass = CoM.localPosition;
		ConstantForce cf = gameObject.AddComponent<ConstantForce> ();
		cf.force = new Vector3 (0.0f, -100000.0f, 0.0f);
		JointSpring js;
		wheelCollider = new WheelCollider[wheelCount];
		for(int i=0;i<wheelCount;i++){
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
		head.GetComponent<MyTankHeadBehaviour>().mTank = rb;
		leftCrawlerMat = new Material(crawlerMat);
		leftCrawler.sharedMaterial = leftCrawlerMat;
		rightCrawlerMat = new Material(crawlerMat);
		rightCrawler.sharedMaterial = rightCrawlerMat;
		halfWheelCount = wheelCount / 2;
		if (GlobalInfo.nightOrNoonFlag == 1) {
			lightUpFlag = false;
			lighter.SetActive(false);
		} else if (GlobalInfo.nightOrNoonFlag == 0){
			lightUpFlag = true;
			lighter.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float curSpeed = 0.0f;
		float curSteer = 0.0f;

		if(GlobalInfo.gameStarted) {
			if(GlobalInfo.chatScreenFlag) return;

			if(GlobalInfo.nightOrNoonFlag == 0) {
				if (Input.GetKeyDown (KeyCode.V)) {
					lightUpFlag = !lightUpFlag;
					if (lightUpFlag) {
						lighter.SetActive (true);
					} else {
						lighter.SetActive (false);
					}
				}
			}
			if(!IsGrounded() && !floatFlag){
				return;
			}
			if(Input.GetKey(KeyCode.LeftShift)) {
				curSpeed = maxSpeed * 0.3f;
				curSteer = maxSteerAngle * 0.3f;
			}else{
				curSpeed = maxSpeed;
				curSteer = maxSteerAngle;
			}
			if(tankControlState == TankControlState.Manual){
				if(Input.GetKeyDown(KeyCode.L) && !GlobalInfo.chatScreenFlag){
					GlobalInfo.rpcControl.RPC("OnDisconnectPlayerRPC",RPCMode.All,networkView.viewID,GlobalInfo.userInfo.name,false);
				}
				spd = Input.GetAxis("Vertical");
				str = Input.GetAxis("Horizontal");
				if(forwardHitFlag && spd > 0.0f) {spd = 0.0f;psTime = 0.0f;}
				if(backwardHitFlag && spd < 0.0f) {spd = 0.0f;psTime = 0.0f;}
				if(forwardRightHitFlag && str > 0.0f) {str = 0.0f;psTime = 0.0f;}
				if(forwardLeftHitFlag && str < 0.0f) {str = 0.0f;psTime = 0.0f;}
				if(backwardRightHitFlag && str < 0.0f) {str = 0.0f;psTime = 0.0f;}
				if(backwardLeftHitFlag && str > 0.0f) {str = 0.0f;psTime = 0.0f;}		
			}else{
				SendMessage("OnForwardJammed",forwardHitFlag,SendMessageOptions.DontRequireReceiver);
				SendMessage("OnBackwardJammed",backwardHitFlag,SendMessageOptions.DontRequireReceiver);
				SendMessage("OnForwardRightJammed",forwardRightHitFlag,SendMessageOptions.DontRequireReceiver);
				SendMessage("OnForwardLeftJammed",forwardLeftHitFlag,SendMessageOptions.DontRequireReceiver);
				SendMessage("OnBackwardRightJammed",backwardRightHitFlag,SendMessageOptions.DontRequireReceiver);
				SendMessage("OnBackwardLeftJammed",backwardLeftHitFlag,SendMessageOptions.DontRequireReceiver);
				spd = mSpd;//Mathf.Lerp(0.0f,mSpd,aiTime);
				str = mStr;//Mathf.Lerp(0.0f,mStr,aiTime);
			}
			if(spd < 0) str = -str;
			if(myPos.position.y < GlobalInfo.water_Level){
				floatFlag = true;
				foreach(Transform a in waterFoam){
					a.particleEmitter.emit = true;
				}	
				if(GlobalInfo.camPosState == 2)
					head.SendMessage("SwitchCamera",false,SendMessageOptions.DontRequireReceiver);
			}else{
				floatFlag = false;
				foreach(Transform a in waterFoam){
					a.particleEmitter.emit = false;
				}	
			}

			if(spd == 0.0f && str == 0.0f){
				head.SendMessage("OnNotMove",SendMessageOptions.DontRequireReceiver);
				GlobalInfo.isMoving = false;
			}else{
				GlobalInfo.isMoving = true;
				head.SendMessage("OnMove",SendMessageOptions.DontRequireReceiver);	
			}
			GlobalInfo.curPlayer = myPos;
			if(surgeAnimFlag){
				surgeAnimTime += Time.deltaTime;
				if(surgeAnimTime > SURGEANIMPERIOD){
					surgeAnimFlag = false;
				}
			}		
			
			if(spd != 0){
				if(psTime == 0.0f){
					if(spd > 0) surgeForce = forwadSurgeForce; else surgeForce = backwardSurgeForce;
					if(surgeAnimFlag) return;
					rigidbody.AddForceAtPosition(new Vector3(0.0f,surgeForce,0.0f),surgeForcePos.position,ForceMode.Acceleration);
					surgeAnimFlag = true;
					surgeAnimTime = 0.0f;
					StartCoroutine("RaiseDepartDust");
					networkView.RPC("SetRunningDust",RPCMode.All,true);
				}
				psTime += Time.deltaTime / 2;
				if(psTime >= surgeTime){
					if(spd != 0.0f){
						curLeftCrawlerTexIndex += Mathf.Sign(spd) * Time.deltaTime * maxSpeed * 4.0f;
						if(curLeftCrawlerTexIndex < 0.0f) curLeftCrawlerTexIndex = 3.0f;
						curRightCrawlerTexIndex += Mathf.Sign(spd) * Time.deltaTime * maxSpeed * 4.0f;
						if(curRightCrawlerTexIndex < 0.0f) curRightCrawlerTexIndex = 3.0f;
						leftCrawler.sharedMaterial.mainTexture = crawlerTex[Mathf.FloorToInt(curLeftCrawlerTexIndex) % 4];
						rightCrawler.sharedMaterial.mainTexture = crawlerTex[Mathf.FloorToInt(curRightCrawlerTexIndex) % 4];
					}
					float ang = Vector3.Angle(Vector3.up,transform.forward);
					if(spd > 0.0f){
						if(ang > 90.0f){
							ang = ang - 90.0f;
//							ang = ang * 2.0f;
							ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
						}else{
							ang = 90.0f - ang;
//							ang = ang * 2.0f;
							ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
						}
						if(ang < 0)ang += spd;else ang = spd;
					}else{
						if(ang > 90.0f){
							ang = ang - 90.0f;
//							ang = ang * 2.0f;
							ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
						}else{
							ang = 90.0f - ang;
//							ang = ang * 2.0f;
							ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
						}	
						if(ang > 0)ang = spd + ang;else ang = spd;
					}
//					ang += Mathf.Sign(spd) * 0.3f;
					transform.Translate(new Vector3(0,0, Mathf.Lerp(0 , curSpeed * ang , (psTime - SURGE_TIME)) * Time.deltaTime),Space.Self);
				}
			}else{
				if(psTime != 0){
					if(surgeForce == forwadSurgeForce) surgeForce = backwardSurgeForce; else surgeForce = forwadSurgeForce;
					if(surgeAnimFlag) return;
					rigidbody.AddForceAtPosition(new Vector3(0.0f,surgeForce,0.0f),surgeForcePos.position,ForceMode.Acceleration);
					surgeAnimFlag = true;
					surgeAnimTime = 0.0f;
					networkView.RPC("SetRunningDust",RPCMode.All,false);
					psTime = 0.0f;
				}
			}
			if(str != 0.0f ){
				transform.RotateAroundLocal(new Vector3(0,1,0),str * curSteer * Time.deltaTime);
				if(spd == 0.0f){
					curLeftCrawlerTexIndex += Mathf.Sign(str) * Time.deltaTime * maxSpeed * 2.0f;
					if(curLeftCrawlerTexIndex < 0.0f) curLeftCrawlerTexIndex = 3.0f;
					curRightCrawlerTexIndex -= Mathf.Sign(str) * Time.deltaTime * maxSpeed * 2.0f;
					if(curRightCrawlerTexIndex < 0.0f) curRightCrawlerTexIndex = 3.0f;
					leftCrawler.sharedMaterial.mainTexture = crawlerTex[Mathf.FloorToInt(curLeftCrawlerTexIndex) % 4];
					rightCrawler.sharedMaterial.mainTexture = crawlerTex[Mathf.FloorToInt(curRightCrawlerTexIndex) % 4];
				}
			}
		}else if(stateSetFlag){
			if(tankControlState == TankControlState.Manual){
				if(Network.isServer){
					GlobalInfo.userInfoList[0].equipment = GlobalInfo.userInfo.equipment;
					GlobalInfo.userInfoList[0].isReady = true;
					GlobalInfo.userInfoList[0].destroyed = false;
					GlobalInfo.userInfoList[0].playerViewID = GlobalInfo.playerViewID;
					GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
				}else{
					GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,GlobalInfo.userInfo.name,(int)GlobalInfo.userInfo.equipment,true,GlobalInfo.playerViewID);			
				}
			}else{
				GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,nam,(int)GetComponent<MyTankShellAttackedBehaviour>().equipKind,true,networkView.viewID);			
			}
		}
		networkView.RPC("SetWaterFoam",RPCMode.All,floatFlag);
		networkView.RPC ("SetCrawlerTexIndex", RPCMode.All, Mathf.FloorToInt(curLeftCrawlerTexIndex) % 4, Mathf.FloorToInt(curRightCrawlerTexIndex) % 4);
	}

	void OnCollisionStay(Collision col) {
		float ang = 0.0f;
		bool flag = false;
		
		for(int i = 0;i < col.contacts.Length;i++) {
			if(col.collider.gameObject.layer == 12 || col.collider.gameObject.layer == 14){
				flag = true;
				ang = Vector3.Angle(transform.right,col.contacts[i].point - transform.position);
				if (ang < 60) {
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardRightHitFlag = true; else backwardRightHitFlag = true;
				}else if(ang > 120){
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardLeftHitFlag = true; else backwardLeftHitFlag = true;
				}
			}			
		}
		if(!flag){
			forwardLeftHitFlag = false;
			forwardRightHitFlag = false;
			backwardLeftHitFlag = false;
			backwardRightHitFlag = false;
		}
		if(!floatFlag){
			if(GlobalInfo.isMoving){
				if(wheelCollider[wheelCount / 2 - 2].isGrounded || wheelCollider[wheelCount / 2 - 1].isGrounded){
					leftLastIndex = skid.AddSkidMark(leftSkidMarkPosition.position,col.contacts[0].normal,skidMarkIntensity,crawlerWidth,leftLastIndex);
				}else{
					leftLastIndex = -1;
				}
				if(wheelCollider[wheelCount - 1].isGrounded || wheelCollider[wheelCount - 2].isGrounded) {
					rightLastIndex = skid.AddSkidMark(rightSkidMarkPosition.position,col.contacts[0].normal,skidMarkIntensity,crawlerWidth,rightLastIndex);
				}else{
					rightLastIndex = -1;
				}
			}
		}else{		
			leftLastIndex = -1;
			rightLastIndex = -1;
		}
	}
	
	void OnCollisionEnter(Collision col){
		float ang = 0.0f;
		for(int i = 0;i < col.contacts.Length;i++) {
			if(col.collider.gameObject.layer == 12 || col.collider.gameObject.layer == 14){
				ang = Vector3.Angle(transform.right,col.contacts[i].point - transform.position);
				if (ang < 60) {
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardRightHitFlag = true; else backwardRightHitFlag = true;
				}else if(ang > 120){
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardLeftHitFlag = true; else backwardLeftHitFlag = true;
				}
			}			
		}
		HouseActivityParam param = new HouseActivityParam();
		param.hitpoint = col.contacts[0].point;
		param.globalHitPoint = param.hitpoint;
		param.origin = myPos.position;
		col.collider.gameObject.SendMessage("OnHit",param,SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionExit(Collision col) {
		float ang = 0.0f;
		for(int i = 0;i < col.contacts.Length;i++) {
			if(col.collider.gameObject.layer == 12 || col.collider.gameObject.layer == 14){
				ang = Vector3.Angle(transform.right,col.contacts[i].point - transform.position);
				if (ang < 60) {
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardRightHitFlag = true; else backwardRightHitFlag = true;
				}else if(ang > 120){
					ang = Vector3.Angle(transform.forward,col.contacts[i].point - transform.position);
					if(ang < 90) forwardLeftHitFlag = true; else backwardLeftHitFlag = true;
				}
			}			
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.layer == 12){
			float ang = Vector3.Angle(transform.forward,other.transform.position - transform.position);
			if(ang < 90.0f) {
				forwardHitFlag = true;
			}else{
				backwardHitFlag = true;
			}
		}
	}
	
    void OnTriggerStay(Collider other) {	
		if(other.gameObject.layer == 12){
			float ang = Vector3.Angle(transform.forward,other.transform.position - transform.position);
			if(ang < 90.0f) {
				forwardHitFlag = true;
			}else{
				backwardHitFlag = true;
			}
		}	
	}
	
	void OnTriggerExit(Collider other) {
		if(other.gameObject.layer == 12) {
//			movable = true;
//			escaped = true;
			float ang = Vector3.Angle(transform.forward,other.transform.position - transform.position);
			if(ang < 90.0f) {
				forwardHitFlag = false;
			}else{
				backwardHitFlag = false;
			}
		}
	}
	
	bool IsGrounded(){
		bool flag = false;
		
		Ray mRay = new Ray(Vector3.zero,Vector3.down);
		RaycastHit hit = new RaycastHit();
		Vector3 tmp;
		for(int i=0;i<wheelCount;i++){
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
	
	IEnumerator RaiseDepartDust() {
		networkView.RPC("SetDepartDust",RPCMode.All,true);
		yield return new WaitForSeconds(3.0f);
		networkView.RPC("SetDepartDust",RPCMode.All,false);
	}

	void OnMoveForward() {
		mSpd = 1.0f;
	}

	void OnMoveBackWard() {
		mSpd = -1.0f;
	}

	void OnRotateToRight() {
		mStr = 1.0f;
	}

	void OnRotateToLeft() {
		mStr = -1.0f;
	}

	void OnStopRotate() {
		mStr = 0.0f;
	}

	void OnStopMove() {
		mSpd = 0.0f;
	}

	void OnSetName(string str) {
		nam = str;
	}

	void OnSetState(TankControlState ts) {
		tankControlState = ts;
		head.SendMessage("OnSetState",tankControlState,SendMessageOptions.DontRequireReceiver);
		if(tankControlState == TankControlState.Manual){
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
			Camera.main.gameObject.GetComponent<AudioListener>().enabled = true;
			Camera.main.gameObject.GetComponent<MotionBlur>().enabled = false;
			GlobalInfo.specialCamState = false;
			GlobalInfo.camPosState = 0;
		}else{
			GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,nam,(int)GetComponent<MyTankShellAttackedBehaviour>().equipKind,true,networkView.viewID);			
		}
		stateSetFlag = true;
	}
}