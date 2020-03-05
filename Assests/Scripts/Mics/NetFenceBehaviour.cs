using UnityEngine;
using System.Collections;
using MagicBattle;

public class NetFenceBehaviour : MonoBehaviour {
	private bool animFlag = false;
	private bool collapsed = false;
	private bool subRotFlag = false;
	private float rotWay = 0.0f;
	private float subRotWay = 0.0f;
	private bool hitFlag = false;
	private float psTime = 0.0f;
	private float rotAng = 0.0f;
	private float subRotAng = 0.0f;
	
	void Update() {
		if(!collapsed && animFlag){
			float ang = 0.0f;
			
			if(subRotFlag){
				if(Mathf.Abs(subRotAng) < Mathf.PI / 4.0f){
					ang = subRotWay * Time.deltaTime * 2.0f;
					subRotAng += ang;
					transform.RotateAround(Vector3.up,ang);
					//return;
				}else{
					subRotFlag = false;
					psTime = 0.5f;
				}
			}
			psTime += Time.deltaTime;
			ang = rotWay * Mathf.Lerp(Mathf.PI / 3.0f,Mathf.PI * 2.0f,psTime) * Time.deltaTime;
			rotAng += ang;
			transform.RotateAroundLocal(transform.right,ang);
			if(Mathf.Abs(rotAng) > Mathf.PI * 0.4f)collapsed = true;
		}
	}
	
	void OnHit(HouseActivityParam param) {
		float ang = 0.0f;
		Vector3 tmp;
		
		if(!animFlag && !hitFlag) {
			networkView.RPC("OnHitRPC",RPCMode.Others,param.origin);
			hitFlag = true;
		
			Object.Destroy(GetComponent<MeshCollider>());
			tmp = param.origin - transform.position;
			tmp = Vector3.Cross(Vector3.up,tmp);
			ang = Vector3.Angle(transform.right,tmp);
			if(ang <= 90.0f){
				rotWay = -1.0f;
				if(ang <= 45.0f){
					subRotFlag = false;
				}else{
					subRotFlag = true;
					ang = Vector3.Angle(transform.forward,tmp);
					if(ang > 90.0f){
						subRotWay = 1.0f;
					}else{
						subRotWay = -1.0f;
					}
				}
			}else{
				rotWay = 1.0f;
				if(ang >= 135.0f) {
					subRotFlag = false;
				}else{
					subRotFlag = true;
					ang = Vector3.Angle(transform.forward,tmp);
					if(ang > 90.0f){
						subRotWay = -1.0f;
					}else{
						subRotWay = +1.0f;
					}
				}
			}
			animFlag = true;
		}
	}
	
	[RPC]
	void OnHitRPC(Vector3 orgin) {
		float ang = 0.0f;
		Vector3 tmp;
		
		if(!animFlag && !hitFlag) {
			hitFlag = true;
		
			Object.Destroy(GetComponent<MeshCollider>());
			tmp = orgin - transform.position;
			tmp = Vector3.Cross(Vector3.up,tmp);
			ang = Vector3.Angle(transform.right,tmp);
			if(ang <= 90.0f){
				rotWay = -1.0f;
				if(ang <= 45.0f){
					subRotFlag = false;
				}else{
					subRotFlag = true;
					ang = Vector3.Angle(transform.forward,tmp);
					if(ang > 90.0f){
						subRotWay = 1.0f;
					}else{
						subRotWay = -1.0f;
					}
				}
			}else{
				rotWay = 1.0f;
				if(ang >= 135.0f) {
					subRotFlag = false;
				}else{
					subRotFlag = true;
					ang = Vector3.Angle(transform.forward,tmp);
					if(ang > 90.0f){
						subRotWay = -1.0f;
					}else{
						subRotWay = +1.0f;
					}
				}
			}
			animFlag = true;
		}		
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param){
		Vector3 tmp = param.attackedPoint - param.direction;
		HouseActivityParam p = new HouseActivityParam();
		p.origin = tmp;
		OnHit(p);
	}
}
