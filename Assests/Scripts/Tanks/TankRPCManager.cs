using UnityEngine;
using System.Collections;

public class TankRPCManager : MonoBehaviour {
	public ParticleEmitter departDust;
	public ParticleEmitter runningDust;
	public Transform waterFoam;
	public Material crawlerMat;
	public Transform crawler;
	public GameObject skidMarkPref;

	private Material myCrawlerMat;
	private Renderer leftCrawler;
	private Renderer rightCrawler;
	private Material leftCrawlerMat;
	private Material rightCrawlerMat;
	private int leftLastIndex = -1;
	private int rightLastIndex = -1;
	private Skidmarks skid;
	private GameObject skidObj;

	void Start() {
		myCrawlerMat = new Material (crawlerMat);
		leftCrawler = crawler.FindChild ("LeftCrawler").renderer;
		rightCrawler = crawler.FindChild ("RightCrawler").renderer;
		leftCrawlerMat = new Material (crawlerMat);
		rightCrawlerMat = new Material (crawlerMat);
		leftCrawler.sharedMaterial = leftCrawlerMat;
		rightCrawler.sharedMaterial = rightCrawlerMat;
		skidObj = (GameObject)GameObject.Instantiate (skidMarkPref, Vector3.zero, Quaternion.identity);
		skid = skidObj.GetComponent<Skidmarks>();
	}

	[RPC]
	void SetDepartDust(bool flag) {
//		if (networkView.isMine)return;
		departDust.emit = flag;
	}
	
	[RPC]
	void SetRunningDust(bool flag) {
//		if (networkView.isMine)return;
		runningDust.emit = flag;
	}

	[RPC]
	void SetWaterFoam(bool flag) {
//		if (networkView.isMine)return;
		foreach(Transform a in waterFoam){
			a.particleEmitter.emit = flag;
		}	
	}

	[RPC]
	void SetCrawlerMatOffset(float leftOffset,float rightOffset) {
		if (networkView.isMine)return;
		leftCrawler.sharedMaterial.mainTextureOffset = new Vector2 (leftOffset, leftCrawler.sharedMaterial.mainTextureOffset.y);;
		rightCrawler.sharedMaterial.mainTextureOffset = new Vector2 (rightOffset, rightCrawler.sharedMaterial.mainTextureOffset.y);
	}

	[RPC]
	void SetBodyPosAndRot(Vector3 pos,Quaternion rot) {
		transform.position = pos;
		transform.rotation = rot;
	}

	[RPC]
	void SetLeftSkidMarkParamsRPC(Vector3 pos,Vector3 nor) {
		leftLastIndex = skid.AddSkidMark(pos,nor,1.0f,0.6f,leftLastIndex);
	}

	[RPC]
	void SetCutLeftSkidMarkRPC() {
		leftLastIndex = -1;
	}

	[RPC]
	void SetRightSkidMarkParamsRPC(Vector3 pos,Vector3 nor) {
		rightLastIndex = skid.AddSkidMark(pos,nor,1.0f,0.6f,rightLastIndex);
	}

	[RPC]
	void SetCutRightSkidMarkRPC() {
		rightLastIndex = -1;
	}
	
	void OnDestroy() {
		DestroyImmediate (skidObj);
	}
}
