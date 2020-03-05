using UnityEngine;
using System.Collections;
using MagicBattle;

public class MapBehaviour : MonoBehaviour {
	public GameObject enemyMark;
	public GameObject myMark;
	public GameObject ourMark;
	public Transform basePoint;
	public Transform cam;
	public Material mapMat;
	public Texture2D[] mapTex;

	// Use this for initialization
	void Start () {
		StartCoroutine ("StartDelay");
	}

	IEnumerator StartDelay() {
		yield return new WaitForSeconds(1.0f);
		GameObject tmpObj = gameObject;
		MapMarkBehaviour mmk;

		if(GlobalInfo.curBattleField == BattleFieldKind.TrainPaceNight)
			mapMat.mainTexture = mapTex [(int)BattleFieldKind.TrainPlace];
		else
			mapMat.mainTexture = mapTex [(int)GlobalInfo.curBattleField];
		GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerTank");
		foreach(GameObject a in go){
			if(a.networkView.viewID.Equals(GlobalInfo.playerViewID)){
				tmpObj = (GameObject)GameObject.Instantiate(myMark,Vector3.zero,Quaternion.identity);
				tmpObj.GetComponent<MapMarkBehaviour>().cam = cam;
			}else{
				foreach(UserInfoClass uf in GlobalInfo.userInfoList){
					if(a.networkView.viewID.Equals(uf.playerViewID)){
						if(uf.team.Equals(GlobalInfo.userInfo.team)){
							tmpObj = (GameObject)GameObject.Instantiate(ourMark,Vector3.zero,Quaternion.identity);
						}else{
							tmpObj = (GameObject)GameObject.Instantiate(enemyMark,Vector3.zero,Quaternion.identity);
						}
					}
				}
			}
			tmpObj.transform.parent = transform;
			mmk = (MapMarkBehaviour)tmpObj.GetComponent<MapMarkBehaviour>();
			mmk.map = transform;
			mmk.mapBasePoint = basePoint;
			mmk.target = a.transform;
		}
		go = GameObject.FindGameObjectsWithTag("PlayerHeli");
		foreach(GameObject a in go){
			if(a.networkView.viewID.Equals(GlobalInfo.playerViewID)){
				tmpObj = (GameObject)GameObject.Instantiate(myMark,Vector3.zero,Quaternion.identity);
				tmpObj.GetComponent<MapMarkBehaviour>().cam = cam;
			}else{
				foreach(UserInfoClass uf in GlobalInfo.userInfoList){
					if(a.networkView.viewID.Equals(uf.playerViewID)){
						if(uf.team.Equals(GlobalInfo.userInfo.team)){
							tmpObj = (GameObject)GameObject.Instantiate(ourMark,Vector3.zero,Quaternion.identity);
						}else{
							tmpObj = (GameObject)GameObject.Instantiate(enemyMark,Vector3.zero,Quaternion.identity);
						}
					}
				}
			}
			tmpObj.transform.parent = transform;
			mmk = (MapMarkBehaviour)tmpObj.GetComponent<MapMarkBehaviour>();
			mmk.map = transform;
			mmk.mapBasePoint = basePoint;
			mmk.target = a.transform;
		}
	}

	void Update () {

	}
}
