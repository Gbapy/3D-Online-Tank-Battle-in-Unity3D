using UnityEngine;
using System.Collections;
using MagicBattle;

public class ShellBehaviour : MonoBehaviour {
	public NetworkView rpcControl;
	public float drag = 0.8f;
	
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
	private string userName;
	private int useGravity = 0;
	
	void Start() {
		rpcControl = GlobalInfo.rpcControl;	
		hit = new RaycastHit();
	}
	
	void Update () {
		if(departFlag && !destroyedFlag){
			psTime += Time.deltaTime;			
			lastPos = transform.position;
			speed -= speed * drag * Time.deltaTime;
			transform.position = transform.position + speed * dir * Time.deltaTime - new Vector3(0,useGravity * 9.8f * Time.deltaTime,0);	
			transform.LookAt(transform.position + (transform.position - lastPos));
			Physics.Raycast(new Ray(lastPos,dir),out hit);
			if(hit.collider != null){
				Vector3 tmp = hit.point - transform.position;
				if(tmp.magnitude <= speed * Time.deltaTime){
					destroyedFlag = true;
					if(viewID.Equals(GlobalInfo.playerViewID)){
						tmp.Normalize();
						GlobalInfo.rpcControl.RPC("OnShellAttackedRPC",RPCMode.All,hit.point,hit.normal,tmp,(int)shellKind,viewID,userName);
					}
					Destroy(this.gameObject);
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
		userName = param.userName;
		departFlag = true;
		useGravity = param.useGravity;
	}
}
