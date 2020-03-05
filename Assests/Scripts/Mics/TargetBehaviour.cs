using UnityEngine;
using System.Collections;
using MagicBattle;

public class TargetBehaviour : MonoBehaviour {
	public GameObject oilExplosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		GameObject.Instantiate (oilExplosion, param.attackedPoint, Quaternion.identity);
		DestroyImmediate (gameObject);
	}
}
