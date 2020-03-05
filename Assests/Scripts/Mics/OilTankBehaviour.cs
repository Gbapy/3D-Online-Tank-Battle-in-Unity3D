using UnityEngine;
using System.Collections;
using MagicBattle;

public class OilTankBehaviour : MonoBehaviour {
	public GameObject oilExplosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		if(oilExplosion == null) return;
		if(param.attackedShellKind != ShellKind.Bullet)
		GameObject.Instantiate (oilExplosion, param.attackedPoint, Quaternion.identity);
	}
}
