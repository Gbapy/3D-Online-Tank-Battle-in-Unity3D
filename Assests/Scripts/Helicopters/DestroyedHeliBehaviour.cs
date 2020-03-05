using UnityEngine;
using System.Collections;

public class DestroyedHeliBehaviour : MonoBehaviour {
	public GameObject selfExplosion;
	public Transform camLookPos;
	public Transform explosionPos;
	
	const float LIFECYCLE = 30.0f;
	
	// Use this for initialization
	void Start () {
		Instantiate(selfExplosion,transform.position,Quaternion.LookRotation(Vector3.up));
		StartCoroutine("SelfDestroy");
//		Camera.main.gameObject.GetComponent<AudioListener>().enabled = true;
//		SmoothFollow sm = Camera.main.gameObject.GetComponent<SmoothFollow>();
//		sm.target = camLookPos;
	}
	
	IEnumerator SelfDestroy() {
		yield return new WaitForSeconds(LIFECYCLE);
		Destroy(gameObject);
	}
}
