using UnityEngine;
using System.Collections;
using MagicBattle;
public class ThingActivity : MonoBehaviour {
	public ParticleEmitter dustemitter;
	public GameObject spawnseg;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		spawnseg.GetComponent<SpawnTreesegmentsBehaviour>().detail*=2;
		spawnseg.GetComponent<SpawnTreesegmentsBehaviour>().explodeShellflag=true;
		spawnseg.transform.parent=null;
		this.rigidbody.isKinematic=false;
		Destroy(this.gameObject);
	}
	void OnCollisionEnter(Collision col){
		if((col.gameObject.tag=="PlayerTank")||(col.gameObject.tag=="EnemyTank"))
		{
			dustemitter.transform.parent=null;
			dustemitter.emit=true;
			spawnseg.GetComponent<SpawnTreesegmentsBehaviour>().explodeflag=true;
			spawnseg.transform.parent=null;
			Destroy(this.gameObject);
		}
	}
}
