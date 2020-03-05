using UnityEngine;
using System.Collections;
using MagicBattle;

public class GhostTankHeadBehaviour : MonoBehaviour {
	public Transform secondaryCamPos;
	public float maxSpeed = 3.0f;
	public float maxAngle = Mathf.PI;

//	private float ang = 0.0f;
//	private bool flag = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

//		if (Input.GetKey (KeyCode.G)) {
//			ang += maxAngle * Time.deltaTime;
//		}
//		if (Input.GetKey (KeyCode.T)) {
//			ang -= maxAngle * Time.deltaTime;
//		}
//		if(Input.GetKeyDown(KeyCode.Tab)) {
//			flag = !flag;
//		}
//		secondaryCamPos.localRotation = Quaternion.Euler(new Vector3(ang,0.0f,0.0f));
//		if(flag)
//			secondaryCamPos.parent.localRotation = Quaternion.Euler(new Vector3(0,-90,0));
//		else
//			secondaryCamPos.parent.localRotation = Quaternion.Euler(new Vector3(0,0,0));
	}
}
