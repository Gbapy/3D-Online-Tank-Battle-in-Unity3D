using UnityEngine;
using System.Collections;
using MagicBattle;

public class CollapseStick : MonoBehaviour {
	public bool thinTreeFlag = false;
	public Transform myPos;
	public float defensivePower = 20.0f;
	// Use this for initialization
	public bool falldowned;
	public float radius=0.18f;
	public float acceleration=0.1f;
	private bool drawflag2=false;
	private Vector3 rotationaxis;
	private bool emitter=false;
	private Vector3 resultvector;
	private float psangle=0f;
	private float upangle=0f;
	private float downangle=0f;
	private bool drawflag1=false;
	private float expval;
	private float velocity=0f;
	private float destructedAmount = 0.0f;
	private float destructionState = 0.0f;
	// Use this for initialization
	void Start () {
		falldowned=false;
		drawflag1=false;
		drawflag2=false;
	
	}
	void falldownoperate()
	{
		if(emitter==true){
			velocity+=acceleration;
			if((psangle+(velocity)*Time.deltaTime)<98f){
				this.transform.RotateAroundLocal(resultvector,(velocity*Time.deltaTime)*Mathf.PI/180);
				transform.position-=new Vector3(0,Time.deltaTime/10,0);
				psangle+=(velocity*Time.deltaTime);			
			}else{
				emitter=false;
				drawflag1=true;
				falldowned=true;
				collider.enabled=false;
			}
		}
		if(drawflag1==true){
			upangle+=Time.deltaTime*20;
				if(upangle<6f){
					this.transform.RotateAroundLocal(resultvector,-Time.deltaTime*20*Mathf.PI/180);
				}else{
					drawflag1=false;
					drawflag2=true;
				}
		}
		if(drawflag2==true){
			downangle+=Time.deltaTime*10;
				if(downangle<3f){
					this.transform.RotateAroundLocal(resultvector,Time.deltaTime*10*Mathf.PI/180);
				}else{
					drawflag2=false;
					enabled = false;
				}
		}
		
	}
	// Update is called once per frame
	void Update () {
		falldownoperate();
	}
	
	void OnHit(HouseActivityParam param){
		if(falldowned) return;
		falldowned = true;
		psangle=0;
		if(thinTreeFlag)
			collider.enabled = false;
		Vector3 contactpoint = param.origin;
		Vector3 vectora =  contactpoint-myPos.position;
		rotationaxis = Vector3.up;
		resultvector = Vector3.Cross(rotationaxis,-vectora);
		emitter=true;	
	}
	
	IEnumerator AttackedBehaviour (ShellAttackedSendMsgParam param) {
		yield return new WaitForSeconds(0);
		destructionState += destructedAmount;
		if(destructionState >= defensivePower){
			falldowned = true;
			psangle=0;
			if(thinTreeFlag)
				collider.enabled = false;
			Vector3 contactpoint = param.attackedPoint;
			Vector3 vectora =  contactpoint-myPos.position;
			rotationaxis = Vector3.up;
			resultvector = Vector3.Cross(rotationaxis,-vectora);
			emitter=true;	
		}
	}	
	
	void AttackedBehaviour_rjs (Vector3 param) {
		falldowned = true;
		psangle=0;
		if(thinTreeFlag)
			collider.enabled = false;
		Vector3 contactpoint = param;
		Vector3 vectora =  contactpoint-myPos.position;
		rotationaxis = Vector3.up;
		resultvector = Vector3.Cross(rotationaxis,-vectora);
		emitter=true;	
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		if(emitter) return;
		destructedAmount = GlobalInfo.shellProperty[(int)param.attackedShellKind].destructionPower;
		StartCoroutine("AttackedBehaviour",param);
	}	
	
	void OnShellAttacked_rjs(Vector3 param) {
		Vector3 tmp;
		tmp = myPos.position - param;
		if(tmp.magnitude <= 80){
			AttackedBehaviour_rjs(param);
		}
	}	
}
