using UnityEngine;
using System.Collections;
using MagicBattle;

public class BuildingBehaviour : MonoBehaviour {
	
	public GameObject explosion;
	
	// Use this for initialization
	void OnShellAttacked(ShellAttackedSendMsgParam param){
		RaycastHit hit = new RaycastHit();
		Vector3 tmp;
		
		Physics.Raycast(new Ray(param.attackedPoint + new Vector3(0,3.0f,0),Vector3.down),out hit);
		if(hit.collider != null){
			if(hit.collider.gameObject.Equals(this.gameObject)){
				GameObject.Instantiate (explosion, param.attackedPoint, Quaternion.identity);
				
//				tmp = param.attackedPoint - hit.point;
//				if(tmp.magnitude <= EP)
//					StartCoroutine("AttackedBehaviour",param);
			}
		}			
	}
	
//	IEnumerator AttackedBehaviour(ShellAttackedSendMsgParam p){
//		yield return new WaitForSeconds(0);
//		Instantiate(bombedSite[(int)p.attackedShellKind],p.attackedPoint + new Vector3(0,0.05f,0),Quaternion.FromToRotation(Vector3.up,p.normal));
//	}
}
