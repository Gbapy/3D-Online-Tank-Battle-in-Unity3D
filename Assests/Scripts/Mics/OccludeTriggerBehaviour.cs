using UnityEngine;
using System.Collections;

public class OccludeTriggerBehaviour : MonoBehaviour {
	public Renderer[] occludees;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.networkView.isMine) {
			foreach(Renderer a in occludees){
				if(a != null)a.enabled = true;
			}
		}
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.networkView.isMine) {
			foreach(Renderer a in occludees){
				if(a != null)a.enabled = true;
			}			
		}
	}

}
