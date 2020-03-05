using UnityEngine;
using System.Collections;
using MagicBattle;

public class HelicopterShellAttackedBehaviour : MonoBehaviour {
	public Transform shellAttackedFlame;
	public bool selfExplosionFlag = false;
	public float defensivePower = 30.0f;
	public Transform blackSteam;
	public Transform helicopter;
	public Transform camPos;
	public GameObject brokenHelicopter;
	public EquipmentKind equipKind;
	public string defeatLabel1 = "";
	public string defeatLabel2 = "";
	
	private float destructionState = 0;
	private float destructedAmount = 0.0f;
	private bool destroyedFlag = false;
	private Vector3 pos;
	private bool blurFlag = false;
	private float blurTime = 0.0f;
	private Transform body;
	private Texture2D healthBar;
	private Material healthBarMat;

	const float CAM_VIBRATE_RADIUS = 0.00005f;
	const float MAX_HEIGHT = 90.0f;
	const float MIN_HEIGHT = 3.0f;	
	// Use this for initialization
	void Start () {
		if(!networkView.isMine) return;
		pos = camPos.localPosition;
		camPos.GetComponent<MotionBlur>().enabled = false;
		body = transform.GetChild (0);
		healthBar = (Texture2D)Resources.Load ("GUI/HealthBar");
		healthBarMat = new Material(Shader.Find("Diffuse"));
		healthBarMat.color = new Color (0, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Transform a in shellAttackedFlame){
			a.particleEmitter.minSize -= Time.deltaTime * 0.05f;
			a.particleEmitter.maxSize -= Time.deltaTime * 0.15f;
		}
		if(Input.GetKeyDown(KeyCode.L)) destroyedFlag = true;
		if(destructionState > 0.0f) {
			blackSteam.particleEmitter.minSize = 0.5f;
			blackSteam.particleEmitter.maxSize = 1.0f;
		}
	
		if(networkView.isMine){
			if(selfExplosionFlag){
				destroyedFlag = true;
			}
			
			if(destroyedFlag){
				float t1;
				float t2;
				float t3;

				camPos.GetComponent<UltraRedRayCamera>().enabled = false;
				Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
				body.Rotate(body.up,180.0f * Time.deltaTime);
				transform.Rotate(Vector3.up,60.0f * Time.deltaTime);
				transform.Translate(20.0f * Time.deltaTime,-15.0f * Time.deltaTime,0.0f);
				t1 = Random.Range(-CAM_VIBRATE_RADIUS,CAM_VIBRATE_RADIUS);
				t2 = Random.Range(-CAM_VIBRATE_RADIUS,CAM_VIBRATE_RADIUS);
				t3 = Random.Range(-CAM_VIBRATE_RADIUS,CAM_VIBRATE_RADIUS);

				camPos.localPosition = pos + new Vector3(t1,t2,t3);
				if(GetHeight(helicopter.position) < MIN_HEIGHT){
					GlobalInfo.destroyed = true;
					GlobalInfo.rpcControl.networkView.RPC("OnDestroyedRPC",RPCMode.All,(int)equipKind,transform.position,transform.rotation,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
					Network.RemoveRPCs(networkView.viewID);
					Network.Destroy(this.gameObject);
				}				
			}
			if(blurFlag){
				blurTime += Time.deltaTime;
				camPos.GetComponent<MotionBlur>().blurAmount = 1 - blurTime * 0.5f;
				if(blurTime >= 2.0f){
					blurFlag = false;
					blurTime = 0.0f;
					camPos.GetComponent<MotionBlur>().enabled = false;
				}
			}		
		}
	}

	void OnGUI() {
		if (!networkView.isMine) return;
		if(GlobalInfo.gameStarted && !GlobalInfo.gameEnded){
			healthBarMat.color = new Color(destructionState / defensivePower,(defensivePower - destructionState) / defensivePower,0.0f);
			Graphics.DrawTexture (new Rect (Screen.width * 0.2632f,Screen.height * 0.971f,Screen.width * 0.4524f * (defensivePower - destructionState) / defensivePower,Screen.height * 0.0132f), healthBar,healthBarMat);
		}
	}

	IEnumerator AttackedBehaviour (ShellAttackedSendMsgParam param) {
		yield return new WaitForSeconds(0);
		if(networkView.isMine){
			camPos.GetComponent<MotionBlur>().enabled = true;
			blurFlag = true;
			blurTime = 0.0f;
		}
		foreach(Transform a in shellAttackedFlame){
			a.particleEmitter.emit = true;	
			a.particleEmitter.minSize = 3.0f;
			a.particleEmitter.maxSize = 9.0f;
		}
		destructionState += GlobalInfo.shellProperty[(int)param.attackedShellKind].destructionPower;
		if(networkView.isMine){
			if(destructionState >= defensivePower){
				if(Network.isServer){
					for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
						if(GlobalInfo.userInfoList[i].name.Equals(param.userName)){
							GlobalInfo.userInfoList[i].score += defensivePower;
							break;
						}
					}	
					GlobalInfo.eventHandler.SendMessage("OnUpdateUserScore",SendMessageOptions.DontRequireReceiver);					
				}else{
					GlobalInfo.rpcControl.RPC("OnUpdateUserScoreRPC",RPCMode.Server,param.userName,defensivePower);	
				}
//				string chatString = GlobalInfo.userInfo.name + defeatLabel1 + param.userName + defeatLabel2;
//				GlobalInfo.rpcControl.RPC("OnChatMessageRPC",RPCMode.All,chatString);
				destroyedFlag = true;
			}
		}
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		if(networkView.viewID.Equals(param.viewID))return;
		StartCoroutine("AttackedBehaviour",param);
		if(networkView.isMine){
			GlobalInfo.rpcControl.RPC("OnUpdateUserHitCountRPC",RPCMode.Server,param.userName);
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
	
	void OnDisconnectPlyer(NetworkViewID viewID) {
		if(networkView.viewID.Equals(viewID))
			selfExplosionFlag = true;
	}		
}
