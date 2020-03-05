using UnityEngine;
using System.Collections;
using MagicBattle;
public class DrumAttackedbehaviour : MonoBehaviour {
	public ParticleEmitter Inner;		
	public ParticleEmitter Smoke;	
	private float drumexppower=2500f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		float explosionPower = GlobalInfo.shellProperty[(int)param.attackedShellKind].explosionPower;
		rigidbody.AddExplosionForce(drumexppower,param.attackedPoint,3.0f,10f,ForceMode.Impulse);
		Inner.emit=true;
		Smoke.emit=true;						
		Destroy(this.gameObject,5f);
	}
}
