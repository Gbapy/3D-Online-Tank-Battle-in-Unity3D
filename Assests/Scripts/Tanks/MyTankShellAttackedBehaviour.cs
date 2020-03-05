using UnityEngine;
using System.Collections;
using MagicBattle;

public class MyTankShellAttackedBehaviour : MonoBehaviour {
	public Transform shellAttackedFlame;
	public bool selfExplosionFlag = false;
	public float defensivePower = 100.0f;
	public GameObject canon;
	public Transform blackSteam;
	public GameObject oilExplosion;
	public Transform[] breakables;
	public Material brokenMat;
	public EquipmentKind equipKind;
	public string defeatLabel1 = "";
	public string defeatLabel2 = "";
	
	private float destructionState = 0;
	private Material mat;
	private float destructedAmount = 0.0f;
	private bool blurFlag = false;
	private float blurTime = 0.0f;
	private float upsideDownTime = 0.0f;
	private TankControlState tankControlState = TankControlState.Manual;

	// Use this for initialization
	void Start () {
		mat = new Material(brokenMat);
		mat.SetFloat("_TotalBrokenCount",defensivePower);
		mat.SetFloat("_CurBrokenCount",destructionState);
	}
	
	void Update() {
		foreach(Transform a in shellAttackedFlame){
			a.particleEmitter.minSize -= Time.deltaTime * 0.15f;
			a.particleEmitter.maxSize -= Time.deltaTime * 0.45f;
			if(a.particleEmitter.minSize < 0){
				a.particleEmitter.minSize = 0.0f;
				a.particleEmitter.maxSize = 0.0f;
			}
		}

		if(destructionState > 0.0f){
			blackSteam.particleEmitter.minSize = 1.5f;
			blackSteam.particleEmitter.maxSize = 3.0f;		
		}
		if(networkView.isMine){
			if(IsUpsideDown()){
				upsideDownTime += Time.deltaTime;
				if(upsideDownTime > 0.5f) selfExplosionFlag = true;		
			}
			if(selfExplosionFlag){
				if(networkView.isMine){
					if(tankControlState == TankControlState.Manual) FinalOperation();	
					GlobalInfo.rpcControl.RPC("OnDestroyedRPC",RPCMode.All,(int)equipKind,transform.position,transform.rotation,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
					Network.RemoveRPCs(networkView.viewID);
					Network.Destroy(this.gameObject);
				}
			}
			if(tankControlState == TankControlState.Manual){
				if(blurFlag){
					blurTime += Time.deltaTime;
					Camera.mainCamera.GetComponent<MotionBlur>().blurAmount = 1 - blurTime * 2.0f;
					if(blurTime >= 0.5f){
						blurFlag = false;
						blurTime = 0.0f;
						Camera.mainCamera.GetComponent<MotionBlur>().enabled = false;
					}
				}
			}
		}
	}

	IEnumerator AttackedBehaviour (ShellAttackedSendMsgParam param) {
		yield return new WaitForSeconds(0);
		destructionState += destructedAmount;
		foreach(Transform a in shellAttackedFlame){
			a.particleEmitter.minSize = 4.0f;
			a.particleEmitter.maxSize = 9.0f;
		}
		GameObject.Instantiate (oilExplosion, param.attackedPoint, Quaternion.identity);
		mat.SetFloat("_CurBrokenCount",destructionState * 3.0f);
		foreach(Transform a in breakables){
			if(a.renderer.materials.Length > 1)
				a.renderer.materials[0] = mat;
			else
				a.renderer.material = mat;
		}
		if(networkView.isMine){
			if(tankControlState == TankControlState.Manual)
				Camera.mainCamera.GetComponent<MotionBlur>().enabled = true;
			
			blurFlag = true;
			blurTime = 0.0f;
			if(destructionState > defensivePower){
				if(Network.isServer){
					for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
						if(GlobalInfo.userInfoList[i].name.Equals(param.userName)){
							GlobalInfo.userInfoList[i].score += defensivePower;
							break;
						}
					}	
					GlobalInfo.eventHandler.SendMessage("OnUpdateUserScore",SendMessageOptions.DontRequireReceiver);				
				}else
					GlobalInfo.rpcControl.RPC("OnUpdateUserScoreRPC",RPCMode.Server,param.userName,defensivePower);
//				string chatString = GlobalInfo.userInfo.name + defeatLabel1 + param.userName + defeatLabel2;
//				GlobalInfo.rpcControl.RPC("OnChatMessageRPC",RPCMode.All,chatString);
				if(tankControlState == TankControlState.Manual) {
					FinalOperation();
					GlobalInfo.rpcControl.networkView.RPC("OnDestroyedRPC",RPCMode.All,(int)equipKind,transform.position,transform.rotation,GlobalInfo.playerViewID,GlobalInfo.userInfo.name);
				}else{
					string nam = "";
					foreach(UserInfoClass uf in GlobalInfo.userInfoList) {
						if(uf.playerViewID.Equals(networkView.viewID)) {
							nam = uf.name;
						}
					}
					GlobalInfo.rpcControl.networkView.RPC("OnDestroyedRPC",RPCMode.All,(int)equipKind,transform.position,transform.rotation,networkView.viewID,nam);
				}
				Network.RemoveRPCs(networkView.viewID);
				Network.Destroy(gameObject);
			}				
		}
	}

	void OnSetState(TankControlState ts) {
		tankControlState = ts;
	}

	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		if(networkView.viewID.Equals(param.viewID)) return;
		destructedAmount = GlobalInfo.shellProperty[(int)param.attackedShellKind].destructionPower;
		StartCoroutine("AttackedBehaviour",param);
		if(networkView.isMine){
			GlobalInfo.rpcControl.RPC("OnUpdateUserHitCountRPC",RPCMode.Server,param.userName);
		}
	}
	
	void OnDisconnectPlyer(NetworkViewID viewID) {
		if(networkView.viewID.Equals(viewID)){
			selfExplosionFlag = true;
		}
	}	
	
	void FinalOperation() {
		GlobalInfo.destroyed = true;
		Camera.mainCamera.transform.parent = null;
		Camera.mainCamera.GetComponent<UltraRedRayCamera>().enabled = false;
		Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
	}
	
	bool IsUpsideDown(){
		float ang = Vector3.Angle(Vector3.up,transform.up);
		if(ang >= 80.0f)
			return true;
		else{
			upsideDownTime = 0.0f;
			return false;
		}
	}			
}
