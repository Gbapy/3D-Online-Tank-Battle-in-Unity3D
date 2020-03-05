using UnityEngine;
using System.Collections;
using System.IO;
using MagicBattle;

public class PathNodeClass {
	public int i;
	public int j;
	public float distance;
}

public class AITankBehaviour : MonoBehaviour {
//	public int mapWidth;
//	public int mapHeight;
	public Transform basePoint;
//	public GameObject pathNode;
	public Transform head;

	public Transform target;
 	private Transform tarObj;
	private PathNodeClass nextPathNode = new PathNodeClass();
	private PathNodeClass curPathNode = new PathNodeClass();
//	private bool[,] aiMapData;
	private bool pathFound = false;
	private bool isFinding = false;
	private bool isTargetFinding = false;
	private bool rotEnd = false;
	private int aiRange = 35;
	private int targetRange = 90;
	private Vector2 targetPos;
	private Vector2 targetPathNode;
	private TeamKind team;
	private bool startDelayed = false;
	private bool forwardJammedFlag = false;
	private bool backwardJammedFlag = false;
	private bool targetFound = false;
	private bool forwardRightHitFlag = false;
	private bool backwardRightHitFlag = false;
	private bool forwardLeftHitFlag = false;
	private bool backwardLeftHitFlag = false;

	void Start() {
		if(!networkView.isMine){
			enabled = false;
			return;
		}
		basePoint = GameObject.Find ("BasePoint").transform;
		tarObj = GameObject.Find ("TargetObj").transform;

		StartCoroutine ("StartDelay");
	}

	IEnumerator StartDelay() {
		yield return new WaitForSeconds (3.0f);
		startDelayed = true;
	}

	void Update() {
		Vector2 moveDir;
		Vector2 rotDir;
		float ang;

		if(!startDelayed) return;
		if(!GlobalInfo.gameStarted) return;
		if(target == null) {
			GetTarget ();
		}else{
			curPathNode.i = (int)(transform.position.x - basePoint.position.x);
			curPathNode.j = (int)(basePoint.position .z - transform.position.z);

			float curDist = (target.position - transform.position).magnitude;
//			RaycastHit mHit = new RaycastHit ();
//			Vector3 dir = target.position - head.position;
//			Physics.Raycast (new Ray (head.position, dir), out mHit, dir.magnitude);
			if(curDist < 100.0f){
//				this.gameObject.SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
//				this.gameObject.SendMessage("OnStopMove",SendMessageOptions.DontRequireReceiver);
				head.SendMessage("OnSetAimPos",target.position,SendMessageOptions.DontRequireReceiver);
				head.SendMessage("OnFire",SendMessageOptions.DontRequireReceiver);
				return;
			}
			if(!pathFound){
				if(!isFinding) {
					aiRange -= 5;
					StartCoroutine("GetNextPathNode");
				}
			}else{
				rotDir = new Vector2 (nextPathNode.i - curPathNode.i, curPathNode.j - nextPathNode.j);
				ang = Vector2.Angle (new Vector2 (transform.forward.x, transform.forward.z), rotDir);
				if (Vector2.Angle (new Vector2 (transform.right.x, transform.right.z), rotDir) < 90.0f)ang = -ang;
				if(Mathf.Abs(ang) >= 2.0f){
					if(ang < 0.0f) {
						this.gameObject.SendMessage("OnRotateToRight",SendMessageOptions.DontRequireReceiver);
					}else{
						this.gameObject.SendMessage("OnRotateToLeft",SendMessageOptions.DontRequireReceiver);
					}
				}else{
					this.gameObject.SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
				}
				if(Mathf.Abs(ang) > 60.0f){
					this.gameObject.SendMessage("OnStopMove",SendMessageOptions.DontRequireReceiver);
				}else{
					moveDir = new Vector2 (nextPathNode.i - curPathNode.i, nextPathNode.j - curPathNode.j);
					if(moveDir.magnitude >= 5.0f){
						this.gameObject.SendMessage("OnMoveForward",SendMessageOptions.DontRequireReceiver);
					}else{
						aiRange = 35;
						pathFound = false;
					}
				}
			}
		}
	}

	void OnForwardJammed(bool flag) {
		forwardJammedFlag = flag;
		if (flag) {
			SendMessage("OnStopMove",SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnBackwardJammed(bool flag) {
		backwardJammedFlag = flag;
		if (flag) {
			SendMessage("OnStopMove",SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnForwardRightJammed(bool flag) {
		forwardRightHitFlag = flag;
		if(flag){
			SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnForwardLeftJammed(bool flag) {
		forwardLeftHitFlag = flag;
		if(flag){
			SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnBackwardRightJammed(bool flag) {
		backwardRightHitFlag = flag;
		if(flag){
			SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnBackwardLeftJammed(bool flag) {
		backwardLeftHitFlag = flag;
		if(flag){
			SendMessage("OnStopRotate",SendMessageOptions.DontRequireReceiver);
		}
	}

	void GetTarget() {
		int n = 0;
		GameObject[] go;
		System .Collections.Generic.List<UserInfoClass> uList = new System.Collections.Generic.List<UserInfoClass> ();

		foreach(UserInfoClass a in GlobalInfo.userInfoList) {
			if(a.playerViewID.Equals(networkView.viewID)){
				team = a.team;
			}
		}
		foreach(UserInfoClass a in GlobalInfo.userInfoList){
			if(a.team != team && !a.destroyed) {
				uList.Add(a);
			}
		}
		n = Random.Range (1, uList.Count);
		go = GameObject.FindGameObjectsWithTag ("PlayerTank");
		foreach(GameObject a in go){
			if(a.networkView.viewID.Equals(uList[n - 1].playerViewID)){
				target = a.transform;
				break;
			}
		}
	}

	IEnumerator GetNextPathNode() {
		isFinding = true;
		yield return new WaitForSeconds(0.0f);
		System.Collections.Generic.List<PathNodeClass> pathNodeList = new System.Collections.Generic.List<PathNodeClass> ();
		int pi, pj;
		PathNodeClass pn;

		targetPos = new Vector2 (target.position.x - basePoint.position.x, basePoint.position.z - target.position.z);
		if (aiRange < 0){
			aiRange = 35;
			this.gameObject.SendMessage("OnStopMove",SendMessageOptions.DontRequireReceiver);
//			SendMessage("OnMoveBackWard",SendMessageOptions.DontRequireReceiver);
		}
		float curDist = Mathf.Sqrt(Mathf.Pow(targetPos.x - curPathNode.i,2) + Mathf.Pow(targetPos.y - curPathNode.j,2));
		pj = curPathNode.j + aiRange;
		for(int i = -aiRange;i<aiRange;i++) {
			pi = curPathNode.i + i;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(targetPos.x - pi,2) + Mathf.Pow(targetPos.y - pj,2));
//						if(pn.distance <= curDist){
							pathNodeList.Add(pn);
//						}
					}
				}
			}
		}
		pi = curPathNode.i + aiRange;
		for(int j = -aiRange;j<aiRange;j++) {
			pj = curPathNode.j + j;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(targetPos.x - pi,2) + Mathf.Pow(targetPos.y - pj,2));
//						if(pn.distance <= curDist){
							pathNodeList.Add(pn);
//						}
					}
				}
			}
		}
		pj = curPathNode.j - aiRange;
		for(int i = -aiRange;i<aiRange;i++) {
			pi = curPathNode.i + i;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(targetPos.x - pi,2) + Mathf.Pow(targetPos.y - pj,2));
//						if(pn.distance <= curDist){
							pathNodeList.Add(pn);
//						}
					}
				}
			}
		}
		pi = curPathNode.i - aiRange;

		for(int j = -aiRange;j<aiRange;j++) {
			pj = curPathNode.j + j;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(targetPos.x - pi,2) + Mathf.Pow(targetPos.y - pj,2));
//						if(pn.distance <= curDist){
							pathNodeList.Add(pn);
//						}
					}
				}
			}
		}


		if(pathNodeList.Count > 0){
			float min = 10000.0f;
			pn = new PathNodeClass ();
			foreach(PathNodeClass a in pathNodeList){
				if(min > a.distance) {
					min = a.distance;
					pn = a;
				}
			}
			nextPathNode = pn;
			pathFound = true;
		}else{
			pathFound = false;
		}
		isFinding = false;
	}

	IEnumerator GetTargetPathNode() {
		isTargetFinding = true;
		yield return new WaitForSeconds(0.0f);
		System.Collections.Generic.List<PathNodeClass> pathNodeList = new System.Collections.Generic.List<PathNodeClass> ();
		int pi, pj;
		PathNodeClass pn;
		
		if (targetRange < 0){
			targetRange = 90;
		}
		pj = (int)targetPos.y + targetRange;
		for(int i = -targetRange;i<targetRange;i++) {
			pi = (int)targetPos.x + i;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(curPathNode.i - pi,2) + Mathf.Pow(curPathNode.j - pj,2));
						pathNodeList.Add(pn);
					}
				}
			}
		}
		pi = (int)targetPos.x + targetRange;
		for(int j = -targetRange;j<targetRange;j++) {
			pj = (int)targetPos.y + j;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(curPathNode.i - pi,2) + Mathf.Pow(curPathNode.j - pj,2));
						pathNodeList.Add(pn);
					}
				}
			}
		}
		pj = (int)targetPos.y - targetRange;
		for(int i = -targetRange;i<targetRange;i++) {
			pi = (int)targetPos.x + i;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(curPathNode.i - pi,2) + Mathf.Pow(curPathNode.j - pj,2));
						pathNodeList.Add(pn);
					}
				}
			}
		}
		pi = (int)targetPos.x - targetRange;
		
		for(int j = -targetRange;j<targetRange;j++) {
			pj = (int)targetPos.y + j;
			if(pi >= 0 && pi < GlobalInfo.mapWidth && pj >= 0 && pj < GlobalInfo.mapHeight) {
				if(GlobalInfo.aiMapData[pi,pj]){
					if(IsLinked(pi,pj,curPathNode.i,curPathNode.j)){
						pn = new PathNodeClass();
						pn.i = pi;
						pn.j = pj;
						pn.distance = Mathf.Sqrt(Mathf.Pow(curPathNode.i - pi,2) + Mathf.Pow(curPathNode.j - pj,2));
						pathNodeList.Add(pn);
					}
				}
			}
		}
		if(pathNodeList.Count > 0){
			pn = new PathNodeClass ();
			float min = 10000.0f;
			foreach(PathNodeClass a in pathNodeList){
				if(a.distance < min){
					min = a.distance;
					pn = a;
				}
			}
			targetPathNode = new Vector2(pn.i,pn.j);
			targetFound = true;
		}else{
			targetFound = false;
		}
		isTargetFinding = false;
	}

	bool IsLinked(int pi,int pj,int ci,int cj) {
		bool ret = true;
		float max = Mathf.Max (Mathf.Abs (pi - ci), Mathf.Abs (pj - cj));
		float stpI = Mathf.Abs (pi - ci) / max;
		float stpJ = Mathf.Abs (pj - cj) / max;
		float minI = Mathf.Min (pi, ci);
		float minJ = Mathf.Min (pj, cj);
		for(int i = 1;i < max;i++) {
			minI += stpI;
			minJ += stpJ;
			if(!GlobalInfo.aiMapData[(int)minI,(int)minJ]){
				ret = false;
				break;
			}
		}
		return ret;
	}
}
