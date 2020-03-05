using UnityEngine;
using System.Collections;
using MagicBattle;

public class TreeColorBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GlobalInfo.nightOrNoonFlag == 1) {
			foreach(Material a in renderer.materials){
				a.color = new Color(0.6235294f,0.6235294f,0.6235294f,1.0f);
			}
		}else if (GlobalInfo.nightOrNoonFlag == 0){
			foreach(Material a in renderer.materials){
				a.color = new Color(0.067f,0.067f,0.067f,1.0f);
          	}
		}
	}
	
	// Update is called once per frame
	void Update () {
//		Vector3 dist = transform.position - Camera.main.transform.position;
//		if (dist.magnitude > 300.0f) 
//			renderer.enabled = false;
//		else
//			renderer.enabled = true;
	}
}
