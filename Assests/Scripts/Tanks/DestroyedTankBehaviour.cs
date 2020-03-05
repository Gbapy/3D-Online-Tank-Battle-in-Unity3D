using UnityEngine;
using System.Collections;

public class DestroyedTankBehaviour : MonoBehaviour {
	public GameObject head;
	public float explosionForce = 300000f;
	public float explosionRadius = 3.0f;
	public GameObject explosion;
	public Transform explosionPos;
	
	const float LIFECYCLE = 30.0f;
	// Use this for initialization
	void Start () {
		Instantiate(explosion,transform.position,Quaternion.LookRotation(Vector3.up));
		if(head != null){
			Rigidbody tmp = (Rigidbody)head.AddComponent(typeof(Rigidbody));
			tmp.mass = 5000;
			tmp.drag = 0;
			tmp.angularDrag = 0;
			tmp.AddExplosionForce(explosionForce,explosionPos.position,explosionRadius);
		}
		Destroy(gameObject,LIFECYCLE);
	}
}
