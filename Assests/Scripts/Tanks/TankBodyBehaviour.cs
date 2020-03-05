using UnityEngine;
using System.Collections;
using MagicBattle;

public class TankBodyBehaviour : MonoBehaviour {
	public TankControlState tankControlState = TankControlState.Manual;
	public Transform wheelParent;
	public Transform wheelColliderParent;
	public Material crawlerMat;
	public Transform crawler;
	public float maxSpeed = 50.0f;
	public float maxSteerAngle = 0.5f;
	public float maxSteerTorque = 20.0f;
	public float breakTorque = 500.0f;
	public Transform CoM;
	public Transform surgeForcePos;
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
	public GameObject lighter;
	public float downForce = -10000.0f;
	public float maxFriction = 5000.0f;
	public GameObject panelPref;

	private int wheelCount = 16;
	private Transform[] leftWheel;
	private Transform[] rightWheel;
	private WheelCollider[] leftWheelCollider;
	private WheelCollider[] rightWheelCollider;
	private Renderer leftCrawler;
	private Renderer rightCrawler;
	private Material leftCrawlerMat;
	private Material rightCrawlerMat;
	private float psTime = 0.0f;
	private float surgeForce = 0.0f;
	private float surgeAnimTime = 0.0f;
	private bool surgeAnimFlag = false;
	private bool movable = true;
	private bool floatFlag = false;
	private float spd = 0.0f;
	private float str = 0.0f;
	private bool escaped = false;
	private float mSpd = 0.0f;
	private float mStr = 0.0f;
	private string nam = "";
	private bool stateSetFlag = false;
	private float leftAverRpm = 0.0f;
	private float rightAverRpm = 0.0f;
	private GUITexture panel;

	private bool forwardHitFlag = false;
	private bool backwardHitFlag = false;
	private bool forwardRightHitFlag = false;
	private bool backwardRightHitFlag = false;
	private bool forwardLeftHitFlag = false;
	private bool backwardLeftHitFlag = false;
	private WheelFrictionCurve wf = new WheelFrictionCurve();
	private bool isMovingForward = false;
	private bool attackedFlag = false;

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
		GameObject per = GameObject.Find ("Performance");
		per.SendMessage ("OnSetEnabledFlag", false, SendMessageOptions.DontRequireReceiver);
		InitRigidBody ();
		InitCrawler ();
		GameObject go = (GameObject)GameObject.Instantiate (panelPref, new Vector3(0,0,-1), Quaternion.identity);
		panel = go.guiTexture;
		panel.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		float curSpeed = 0.0f;
		float curSteer = 0.0f;
		float curSteerTorque = 0.0f;
		float ang = 0.0f;

		bool keyUpFlag = false;
		if(GlobalInfo.gameStarted) {
			if(GlobalInfo.chatScreenFlag) return;
			SetWheelAndCrawler ();
			panel.enabled = true;
			panel.pixelInset = new Rect(0,0,Screen.width,Screen.height);
			if(Input.GetKey(KeyCode.LeftShift)) {
				curSpeed = 3.0f;//maxSpeed * 0.3f;
				curSteer = maxSteerAngle * 0.3f;
				curSteerTorque = maxSteerTorque * 0.3f;
			}else{
				curSpeed = maxSpeed;
				curSteer = maxSteerAngle;
				curSteerTorque = maxSteerTorque;
			}
			if(tankControlState == TankControlState.Manual){
				if(Input.GetKeyDown(KeyCode.L) && !GlobalInfo.chatScreenFlag){
					GlobalInfo.rpcControl.RPC("OnDisconnectPlayerRPC",RPCMode.All,networkView.viewID,GlobalInfo.userInfo.name,false);
				}
				spd = Input.GetAxis("Vertical");
				str = Input.GetAxis("Horizontal");
//				if(forwardHitFlag && spd > 0.0f) {spd = 0.0f;psTime = 0.0f;}
//				if(backwardHitFlag && spd < 0.0f) {spd = 0.0f;psTime = 0.0f;}
//				if(forwardRightHitFlag && str > 0.0f) {str = 0.0f;psTime = 0.0f;}
//				if(forwardLeftHitFlag && str < 0.0f) {str = 0.0f;psTime = 0.0f;}
//				if(backwardRightHitFlag && str < 0.0f) {str = 0.0f;psTime = 0.0f;}
//				if(backwardLeftHitFlag && str > 0.0f) {str = 0.0f;psTime = 0.0f;}
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
				networkView.RPC("SetRunningDust",RPCMode.All,false);
//				foreach(Transform a in waterFoam){
//					a.particleEmitter.emit = true;
//				}	
				if(GlobalInfo.camPosState == 2)
					head.SendMessage("SwitchCamera",false,SendMessageOptions.DontRequireReceiver);
			}else{
				floatFlag = false;
//				foreach(Transform a in waterFoam){
//					a.particleEmitter.emit = false;
//				}	
			}
			networkView.RPC("SetWaterFoam",RPCMode.All,floatFlag);
			if(spd == 0.0f && str == 0.0f){
				head.SendMessage("OnNotMove",SendMessageOptions.DontRequireReceiver);
				GlobalInfo.isMoving = false;
			}else{
				GlobalInfo.isMoving = true;
				head.SendMessage("OnMove",SendMessageOptions.DontRequireReceiver);	
			}
			if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow)){
				//psTime = 0.0f;
				keyUpFlag = true;
				spd = 0.0f;
			}
			if(attackedFlag){
				attackedFlag = false;
				if(isMovingForward){
					spd = 0.0f;
				}else{
					rigidbody.AddForceAtPosition(new Vector3(0.0f,forwadSurgeForce * 3.0f,0.0f),surgeForcePos.position,ForceMode.Acceleration);
				}
			}
			GlobalInfo.curPlayer = myPos;
			if(surgeAnimFlag){
				surgeAnimTime += Time.deltaTime;
				if(surgeAnimTime > SURGEANIMPERIOD){
					surgeAnimFlag = false;
				}
			}		
			ang = GetTiltAngle(spd);
			if(spd != 0){
				isMovingForward = true;
				if(psTime == 0.0f && !keyUpFlag){
					if(spd > 0) surgeForce = forwadSurgeForce; else surgeForce = backwardSurgeForce;
					if(surgeAnimFlag) return;
					rigidbody.AddForceAtPosition(new Vector3(0.0f,surgeForce,0.0f),surgeForcePos.position,ForceMode.Acceleration);
					surgeAnimFlag = true;
					surgeAnimTime = 0.0f;
					StartCoroutine("RaiseDepartDust");
					if(!floatFlag) networkView.RPC("SetRunningDust",RPCMode.All,true);
				}
				psTime += Time.deltaTime / 2;

				for(int i = 0;i< wheelCount;i++) {
					leftWheelCollider[i].brakeTorque = 0.0f;
					leftWheelCollider[i].motorTorque = curSpeed * ang;//Mathf.Lerp(0 , curSpeed, (psTime - SURGE_TIME)) * Time.deltaTime;
					rightWheelCollider[i].brakeTorque = 0.0f;
					rightWheelCollider[i].motorTorque = curSpeed * ang;//Mathf.Lerp(0 , curSpeed, (psTime - SURGE_TIME)) * Time.deltaTime;
				}
			}else{
				isMovingForward = false;
				for(int i = 0;i< wheelCount;i++) {
					leftWheelCollider[i].brakeTorque = breakTorque;
					rightWheelCollider[i].brakeTorque = breakTorque;
				}
				if(psTime != 0){
					surgeAnimFlag = true;
					surgeAnimTime = 0.0f;
					networkView.RPC("SetRunningDust",RPCMode.All,false);
					psTime = 0.0f;
				}
			}
			if(str != 0.0f ){
//				if(spd == 0.0f && leftAverRpm == 0.0f && rightAverRpm == 0.0f){
//					for(int i = 0;i < wheelCount;i++) {
//						leftWheelCollider[i].brakeTorque = 0.0f;
//						leftWheelCollider[i].motorTorque = curSteerTorque * str;
//						rightWheelCollider[i].brakeTorque = 0.0f;
//						rightWheelCollider[i].motorTorque = -curSteerTorque * str;
//						wf.extremumValue = 0.0f;//maxFriction * 0.01f;
//						wf.asymptoteValue = 0.0f;//maxFriction * 0.005f;
//						leftWheelCollider[i].sidewaysFriction = wf;
//						rightWheelCollider[i].sidewaysFriction = wf;
//					}
//				}else{
					transform.RotateAroundLocal(new Vector3(0,1,0),str * curSteer * Time.deltaTime);
//				}
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
				GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,nam,(int)GetComponent<TankShellAttackedBehaviour>().equipKind,true,networkView.viewID);			
			}
		}
	}

	void OnDestroy() {
		GameObject.Destroy (panel.gameObject);
	}

	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		if(param.attackedShellKind != ShellKind.Bullet)
			attackedFlag = true;
	}

	float GetTiltAngle(float spd) {
//		float ang = Vector3.Angle(Vector3.up,transform.forward);
//		if(spd > 0.0f){
//			if(ang > 90.0f){
//				ang = ang - 90.0f;
//				//							ang = ang * 2.0f;
//				ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
//			}else{
//				ang = 90.0f - ang;
//				//							ang = ang * 2.0f;
//				ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
//			}
//			if(ang < 0)ang += spd;else ang = spd;
//		}else{
//			if(ang > 90.0f){
//				ang = ang - 90.0f;
//				//							ang = ang * 2.0f;
//				ang = Mathf.Sin(ang * Mathf.PI / 180.0f);
//			}else{
//				ang = 90.0f - ang;
//				//							ang = ang * 2.0f;
//				ang = -Mathf.Sin(ang * Mathf.PI / 180.0f);
//			}	
//			if(ang > 0)ang = spd + ang;else ang = spd;
//		}
//		ang = spd;
		return spd;
	}

	void InitRigidBody() {
		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = 0.0f;
		rb.angularDrag = 0.0f;
		rb.useGravity = true;
		rb.isKinematic = false;
		rigidbody.centerOfMass = CoM.localPosition;
		ConstantForce cf = gameObject.AddComponent<ConstantForce> ();
		cf.force = new Vector3 (0.0f, downForce, 0.0f);
		head.GetComponent<TankHeadBehaviour>().mTank = rb;
	}

	void InitCrawler() {
		wheelCount = wheelParent.childCount / 2;
		leftWheel = new Transform[wheelCount];
		rightWheel = new Transform[wheelCount];
		leftWheelCollider = new WheelCollider[wheelCount];
		rightWheelCollider = new WheelCollider[wheelCount];
		foreach(Transform obj in wheelParent) {
			string n = obj.name;
			n = n.Substring(5);
			if(n.Substring(0,1) == "L"){
				leftWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;
			}else{
				rightWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;
			}
		}
		leftCrawlerMat = new Material(crawlerMat);
		rightCrawlerMat = new Material(crawlerMat);
		leftCrawler = crawler.FindChild("LeftCrawler").renderer;
		rightCrawler = crawler.FindChild ("RightCrawler").renderer;
		leftCrawler.sharedMaterial = leftCrawlerMat;
		rightCrawler.sharedMaterial = rightCrawlerMat;
		wf.asymptoteSlip = 2.0f;
		wf.extremumSlip = 1.0f;
		wf.stiffness = 1.0f;
		JointSpring js;
		for(int i=0;i<wheelCount;i++){
			GameObject a = new GameObject("WheelCollider");
			a.transform.parent = wheelColliderParent;
			a.transform.localRotation = Quaternion.identity;
			a.transform.localPosition = leftWheel[i].localPosition;
			leftWheelCollider[i] = a.AddComponent<WheelCollider>();
			leftWheelCollider[i].mass = wheelMass;
			leftWheelCollider[i].radius = wheelRadius;
			leftWheelCollider[i].suspensionDistance = wheelSuspensionDistance;
			js = new JointSpring();
			js.spring = wheelSpring;
			js.damper = wheelDamper;
			js.targetPosition = wheelTargetPosition;
			leftWheelCollider[i].suspensionSpring = js;
			leftWheelCollider[i].brakeTorque = Mathf.Infinity;
		}	
		leftWheelCollider [0].suspensionDistance = 0;
		leftWheelCollider [wheelCount - 1].suspensionDistance = 0;
		for(int i=0;i<wheelCount;i++){
			GameObject a = new GameObject("WheelCollider");
			a.transform.parent = wheelColliderParent;
			a.transform.localRotation = Quaternion.identity;
			a.transform.localPosition = rightWheel[i].localPosition;
			rightWheelCollider[i] = a.AddComponent<WheelCollider>();
			rightWheelCollider[i].mass = wheelMass;
			rightWheelCollider[i].radius = wheelRadius;
			rightWheelCollider[i].suspensionDistance = wheelSuspensionDistance;
			js = new JointSpring();
			js.spring = wheelSpring;
			js.damper = wheelDamper;
			js.targetPosition = wheelTargetPosition;
			rightWheelCollider[i].suspensionSpring = js;
			rightWheelCollider[i].brakeTorque = Mathf.Infinity;
		}	
		rightWheelCollider [0].suspensionDistance = 0;
		rightWheelCollider [wheelCount - 1].suspensionDistance = 0;
	}

	void SetWheelAndCrawler() {
		RaycastHit hit = new RaycastHit ();
		float rpm = 0.0f;

		leftAverRpm = 0.0f;
		rightAverRpm = 0.0f;
		for(int i = 0;i < wheelCount;i++){
			if(i != 0 && i != wheelCount - 1);leftAverRpm += leftWheelCollider[i].rpm / 60.0f;
			if(leftWheelCollider[i].rpm == 0.0f) rpm = 1; else rpm = leftWheelCollider[i].rpm;
			rpm = rpm / 60.0f;
			wf.extremumValue = maxFriction / Mathf.Pow(rpm , 3);
			wf.asymptoteValue = maxFriction * 0.5f / Mathf.Pow(rpm , 3);
			leftWheelCollider[i].forwardFriction = wf;
			Vector3 cp = leftWheelCollider[i].transform.TransformPoint(leftWheelCollider[i].center);
			if(Physics.Raycast (cp,-leftWheelCollider[i].transform.up,out hit,leftWheelCollider[i].suspensionDistance + leftWheelCollider[i].radius)){
				leftWheel[i].position = hit.point + (leftWheelCollider[i].transform.up * leftWheelCollider[i].radius);
			}else{
				leftWheel[i].position = cp - (leftWheelCollider[i].transform.up * leftWheelCollider[i].suspensionDistance);
			}
			leftWheel[i].Rotate(leftWheelCollider[i].rpm * 6 * Time.deltaTime,0,0);
		}
		leftAverRpm = -leftAverRpm / (wheelCount * 2);
		leftCrawler.sharedMaterial.mainTextureOffset = new Vector2(leftCrawler.sharedMaterial.mainTextureOffset.x + leftAverRpm * Time.deltaTime, leftCrawler.sharedMaterial.mainTextureOffset.y);
		for(int i = 0;i < wheelCount;i++){
			if(i != 0 && i != wheelCount - 1)rightAverRpm += rightWheelCollider[i].rpm / 60.0f;
			if(rightWheelCollider[i].rpm == 0.0f) rpm = 1; else rpm = rightWheelCollider[i].rpm;
			rpm = rpm / 60.0f;
			wf.extremumValue = maxFriction / Mathf.Pow(rpm , 3);
			wf.asymptoteValue = maxFriction * 0.5f / Mathf.Pow(rpm , 3);
			rightWheelCollider[i].forwardFriction = wf;
			Vector3 cp = rightWheelCollider[i].transform.TransformPoint(rightWheelCollider[i].center);
			if(Physics.Raycast (cp,-rightWheelCollider[i].transform.up,out hit,rightWheelCollider[i].suspensionDistance + rightWheelCollider[i].radius)){
				rightWheel[i].position = hit.point + (rightWheelCollider[i].transform.up * rightWheelCollider[i].radius);
			}else{
				rightWheel[i].position = cp - (rightWheelCollider[i].transform.up * rightWheelCollider[i].suspensionDistance);
			}
			rightWheel[i].Rotate(rightWheelCollider[i].rpm * 6 * Time.deltaTime,0,0);
		}
		rightAverRpm = -rightAverRpm / (wheelCount * 2);
		rightCrawler.sharedMaterial.mainTextureOffset = new Vector2(rightCrawler.sharedMaterial.mainTextureOffset.x + rightAverRpm * Time.deltaTime, rightCrawler.sharedMaterial.mainTextureOffset.y);
		networkView.RPC ("SetCrawlerMatOffset", RPCMode.All, rightCrawler.sharedMaterial.mainTextureOffset.x,rightCrawler.sharedMaterial.mainTextureOffset.x);
		if(!floatFlag) {
			int skidWheel = wheelCount / 2;
			Vector3 t = rightWheelCollider[skidWheel].transform.TransformPoint(rightWheelCollider[skidWheel].center);
			if(Physics.Raycast (t,-rightWheelCollider[skidWheel].transform.up,out hit,rightWheelCollider[skidWheel].suspensionDistance + rightWheelCollider[skidWheel].radius)){
				networkView.RPC("SetRightSkidMarkParamsRPC",RPCMode.All,hit.point,hit.normal);
			}else{
				networkView.RPC("SetCutRightSkidMarkRPC",RPCMode.All);
			}
			t = leftWheelCollider[skidWheel].transform.TransformPoint(leftWheelCollider[skidWheel].center);
			if(Physics.Raycast (t,-leftWheelCollider[skidWheel].transform.up,out hit,leftWheelCollider[skidWheel].suspensionDistance + leftWheelCollider[skidWheel].radius)){
				networkView.RPC("SetLeftSkidMarkParamsRPC",RPCMode.All,hit.point,hit.normal);
			}else{
				networkView.RPC("SetCutLeftSkidMarkRPC",RPCMode.All);
			}
		}else{
			networkView.RPC("SetCutRightSkidMarkRPC",RPCMode.All);
			networkView.RPC("SetCutLeftSkidMarkRPC",RPCMode.All);
		}
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
			float ang = Vector3.Angle(transform.forward,other.transform.position - transform.position);
			if(ang < 90.0f) {
				forwardHitFlag = false;
			}else{
				backwardHitFlag = false;
			}
		}
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
		psTime = 0.0f;
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
			GlobalInfo.rpcControl.RPC("OnRegistUserEquipRPC",RPCMode.Server,nam,(int)GetComponent<TankShellAttackedBehaviour>().equipKind,true,networkView.viewID);			
		}
		stateSetFlag = true;
	}
}