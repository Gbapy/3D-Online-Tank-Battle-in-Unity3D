using UnityEngine;
using System.Collections;
using MagicBattle;

public class CameraTankBehaviour : MonoBehaviour {
	public Transform cameraPos;
	public float maxSpeed = 25.0f;
	public float maxAngle = 3.0f;
	public Texture2D aimCross;

	private Transform target;
	private bool flag = false;
	private Transform par;
	private GameObject parObj;
	private SmoothFollow sm;
	private bool cursorFlag = true;
	// Use this for initialization
	void Start () {
		if (!networkView.isMine)return;
		Camera.main.GetComponent<SmoothFollow> ().enabled = false;
		Camera.main.transform.parent = transform;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
		GlobalInfo.playerViewID = networkView.viewID;
		GlobalInfo.userInfo.playerViewID = networkView.viewID;
		GlobalInfo.myPlayer = gameObject;
		GlobalInfo.playerCamera = Camera.main;
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
		aimCross = (Texture2D)Resources.Load ("GUI/GunSights/GhostGunSight");
		sm = GetComponent<SmoothFollow>();
	}
	
	// Update is called once per frame
	void Update () {
		float ms = 0.0f;
		float curSpeed = 0.0f;

		if (!networkView.isMine)return;
		if(Input.GetKeyDown(KeyCode.L) && !GlobalInfo.chatScreenFlag){
			GlobalInfo.rpcControl.RPC("OnDisconnectPlayerRPC",RPCMode.All,networkView.viewID,GlobalInfo.userInfo.name,false);
		}
		if(Input.GetMouseButtonDown(0)){
			Ray mRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f,Screen.height / 2.0f,Camera.main.near));
			RaycastHit mHit = new RaycastHit();
			Physics.Raycast(mRay,out mHit);
			if(mHit.collider != null){
				if(mHit.collider.gameObject.layer == 12){
					sm.enabled = true;
					target = mHit.collider.transform;
					parObj = new GameObject();
					par = parObj.transform;
					par.position = target.position + target.up * 2.0f;
					par.rotation = Quaternion.identity;
					par.parent = target;
					flag = true;
					sm.target = par;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)) cursorFlag = !cursorFlag;
		if(Input.GetMouseButtonDown(1)) {
			sm.enabled = false;
			flag = false;
			target = null;
			transform.parent = null;
			transform.rotation = Quaternion.identity;
		}
		if(Input.GetKey (KeyCode.LeftShift))ms = maxSpeed * 0.15f;else ms = maxSpeed;
		if(Input.GetKey(KeyCode.LeftControl))ms = maxSpeed * 2.0f;
		float mz = Input.GetAxis ("Mouse X") * maxAngle * Time.deltaTime;
		float mm = Input.GetAxis ("Mouse Y") * maxAngle * Time.deltaTime;
		float mx = Input.GetAxis ("Horizontal") * ms * Time.deltaTime;
		float my = Input.GetAxis ("Vertical") * ms * Time.deltaTime;

		
		if(Input.GetKey(KeyCode.LeftShift)){
			curSpeed = maxSpeed * 0.3f;
		}else{
			curSpeed = maxSpeed;
		}
		if(!networkView.isMine) return;
		if (Input.GetKey (KeyCode.R)) {
			if(flag){
				sm.height += curSpeed * Time.deltaTime;
			}else{
				transform.position += transform.up * curSpeed * Time.deltaTime;
			}
		}
		if (Input.GetKey (KeyCode.F)) {
			if(flag){
				sm.height -= curSpeed * Time.deltaTime;
			}else{
				transform.position -= transform.up * curSpeed * Time.deltaTime;
			}
		}
		if(flag){
			sm.distance -= my;
			par.Rotate(Vector3.up,-mx * 3.0f);
		}else{
			transform.RotateAroundLocal (Vector3.up, mz);
			Camera.main.transform.RotateAroundLocal (Vector3.right, -mm);
			transform.position = transform.position + transform.forward * my + transform.right * mx;
		}
	}

	void OnGUI() {
		if(!cursorFlag) return;
		if(!flag)
			GUI.DrawTexture (new Rect (Screen.width / 2.0f - 15.0f, Screen.height / 2.0f - 15.0f, 30.0f, 30.0f), aimCross);
	}
}
