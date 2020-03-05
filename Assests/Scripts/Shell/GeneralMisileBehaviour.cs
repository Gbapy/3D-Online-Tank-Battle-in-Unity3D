using UnityEngine;
using System.Collections;
using MagicBattle;
	
public class GeneralMisileBehaviour : MonoBehaviour {
	public GameObject explosion;
	public NetworkView rpcControl;
	public float initSpeed = 0.0f;
	public float accel = 10.0f;
	
	private Vector3 lastPos;
	private ShellKind shellKind;
	private Vector3 dir;
	private float psTime = 0.0f;
	private float speed = 0.0f;
	private Vector3 normal;
	private bool destroyedFlag = false;
	private RaycastHit hit;
	private bool departFlag = false;
	private NetworkViewID viewID;
	private string userName = "";
	
	void Start() {
		rpcControl = GlobalInfo.rpcControl;	
		hit = new RaycastHit();
	}
	
	void Update () {
		if(departFlag){
			psTime += Time.deltaTime;			
			if(!destroyedFlag){
				speed += accel * Time.deltaTime;
				lastPos = transform.position;
				transform.position = transform.position + speed * dir * Time.deltaTime;	
				transform.LookAt(transform.position + dir);
				Physics.Raycast(new Ray(lastPos,dir),out hit);
				if(hit.collider != null){
					Vector3 tmp = hit.point - transform.position;
					if(tmp.magnitude <= speed * Time.deltaTime){
						for(int i = 0;i < transform.childCount;i++){
							if(transform.GetChild(i).tag != "Tail"){
								transform.GetChild(i).renderer.enabled = false;
							}
						}
						if(viewID.Equals(GlobalInfo.playerViewID)){
							tmp.Normalize();
							GlobalInfo.rpcControl.RPC("OnShellAttackedRPC",RPCMode.All,hit.point,hit.normal,tmp,(int)shellKind,viewID,userName);
						}
						destroyedFlag = true;
					}
				}
			}
			if(psTime > GlobalInfo.shellProperty[(int)shellKind].lifeCycle){
				Destroy(this.gameObject);
				destroyedFlag = true;
			}			
		}
	}
	
	void OnSetShellInfo(ShootSendMsgParam param){
		shellKind = param.kind;
		speed = GlobalInfo.shellProperty[(int)shellKind].speed;
		dir = param.dir;
		viewID = param.viewID;
		if(shellKind == ShellKind.GeneralMisile){		
			speed = initSpeed;
		}else{
			accel = 0.0f;
		}
		userName = param.userName;
		departFlag = true;
	}
}
