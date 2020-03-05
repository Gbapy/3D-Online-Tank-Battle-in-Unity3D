using UnityEngine;
using System.Collections;
using MagicBattle;
	
public class GuidedMisileBehaviour : MonoBehaviour {
	public float initSpeed = 50.0f;
	public float accel = 300.0f;
	public Transform target;
	
	private ShellKind kind;
	private NetworkViewID viewID;
	private float psTime = 0.0f;
	private bool departFlag = false;
	private bool destroyedFlag = false;
	private float speed = 0.0f;
	private string userName = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(departFlag){
			Vector3 tmp;
			psTime += Time.fixedDeltaTime;
			if(!destroyedFlag){
				if(target != null){
					tmp = target.position - transform.position;
					tmp.Normalize();
					float ang = Vector3.Angle(transform.forward,tmp);
					ang = ang * Mathf.PI / 180.0f;
					transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(tmp),Time.fixedDeltaTime / ang);
					transform.Translate(new Vector3(0,0,speed * Time.fixedDeltaTime));
				}else{
					transform.Translate(new Vector3(0,0,speed * Time.fixedDeltaTime));
				}
				speed += accel * Time.fixedDeltaTime;
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(new Ray(transform.position,transform.forward),out hit);
				if(hit.collider != null){
					tmp = hit.point - transform.position;
					if(tmp.magnitude < speed * Time.fixedDeltaTime){
						for(int i = 0;i < transform.childCount;i++){
							if(transform.GetChild(i).tag != "Tail"){
								transform.GetChild(i).renderer.enabled = false;
							}
						}
						if(viewID.Equals(GlobalInfo.playerViewID)){
							tmp.Normalize();
							GlobalInfo.rpcControl.RPC("OnShellAttackedRPC",RPCMode.All,hit.point,hit.normal,tmp,(int)kind,viewID,userName);
						}
						destroyedFlag = true;
					}
				}			
			}
			if(psTime >= GlobalInfo.shellProperty[(int)kind].lifeCycle){
				DestroyImmediate(gameObject);
			}
		}
	}
	
	void OnSetMisileInfo(ShootSendMsgParam param) {
		if(!param.targetViewID.Equals(param.viewID)){
			GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerHeli");
			foreach(GameObject a in go){
				if(a.networkView.viewID.Equals(param.targetViewID)){
					target = a.transform;
					break;
				}
			}
		}
		kind = param.kind;
		viewID = param.viewID;
		departFlag = true;
		transform.rotation = Quaternion.LookRotation(param.dir);
		speed = initSpeed;
		userName = param.userName;
	}
}
