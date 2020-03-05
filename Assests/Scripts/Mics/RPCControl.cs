using UnityEngine;
using System.Collections;
using MagicBattle;

public class RPCControl : MonoBehaviour {
	public bool enabledFlag = false;
	public string warn1;
	public string warn2;
	
	[RPC]
	void OnShellAttackedRPC(Vector3 attackedPos,Vector3 normal,Vector3 dir,int shellKind,NetworkViewID viewID,string userName) {
		if(!enabledFlag) return;
		ShellAttackedSendMsgParam param = new ShellAttackedSendMsgParam();
		param.attackedPoint = attackedPos;
		param.attackedShellKind = (ShellKind)shellKind;
		param.normal = normal;
		param.direction = dir;
		param.viewID = viewID;
		param.userName = userName;
		GlobalInfo.eventHandler.SendMessage("OnShellAttacked",param,SendMessageOptions.DontRequireReceiver);
	}

	[RPC]
	void SetTimeScaleRPC(float timeScale) {
		GlobalInfo.eventHandler.SendMessage ("OnSetTimeScale", timeScale);
	}

	[RPC]
	void OnShootRPC(int shellKind,Vector3 position,Vector3 dir,int useGravity,NetworkViewID viewID,string userName) {
		ShootSendMsgParam param = new ShootSendMsgParam();
		param.kind = (ShellKind)shellKind;
		param.position = position;
		param.dir = dir;
		param.viewID = viewID;
		param.userName = userName;
		param.useGravity = useGravity;
		GlobalInfo.eventHandler.SendMessage("OnShoot",param,SendMessageOptions.DontRequireReceiver);
	}	
	
	[RPC]
	void OnLaunchRPC(int misileKind,Vector3 position,Vector3 dir,NetworkViewID viewID,NetworkViewID targetViewID,string userName){
		ShootSendMsgParam param = new ShootSendMsgParam();
		param.kind = (ShellKind)misileKind;
		param.position = position;
		param.targetViewID = targetViewID;
		param.dir = dir;
		param.viewID = viewID;
		param.userName = userName;
		GlobalInfo.eventHandler.SendMessage("OnLaunch",param,SendMessageOptions.DontRequireReceiver);
	}
		
	[RPC]
	void OnDisconnectPlayerRPC(NetworkViewID viewID,string usrName,bool isReal) {
		if(isReal){
			if(Network.isServer){
				foreach(UserInfoClass uf in GlobalInfo.userInfoList){
					if(uf.name.Equals(usrName)){
						GlobalInfo.userInfoList.Remove(uf);
						break;
					}
				}
			}
		}
		GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerTank");
		foreach(GameObject a in go){
			a.SendMessage("OnDisconnectPlyer",viewID,SendMessageOptions.DontRequireReceiver);
		}		
		go = GameObject.FindGameObjectsWithTag("PlayerHeli");
		foreach(GameObject a in go){
			a.SendMessage("OnDisconnectPlyer",viewID,SendMessageOptions.DontRequireReceiver);
		}		
	}
	
	[RPC]
	void OnDestroyedRPC(int kind,Vector3 position,Quaternion rotation,NetworkViewID viewID,string userName){
		DestroyedSendMsgParam param = new DestroyedSendMsgParam();
		param.kind = kind;
		param.position = position;
		param.rotation = rotation;
		param.viewID = viewID;
		GlobalInfo.eventHandler.SendMessage("OnDestroyed",param,SendMessageOptions.DontRequireReceiver);
		if(Network.isServer){
			for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
				if(GlobalInfo.userInfoList[i].name.Equals(userName)){
					GlobalInfo.userInfoList[i].destroyed = true;
					break;
				}
			}
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);		
			if(GlobalInfo.gameStarted)
				if(DetectGameOver()) GlobalInfo.rpcControl.RPC("OnGameOverRPC",RPCMode.All);
		}
	}
	
	[RPC]
	void OnCertificateUserRPC(string usrName,string password,NetworkMessageInfo info) {
		bool certFlag = true;
		string warning = "";
		
		if(!GlobalInfo.gameStarted){
			foreach(UserInfoClass uf in GlobalInfo.userInfoList){
				if(uf.name.Equals(usrName)){
					certFlag = false;
					warning = warn1;
					break;
				}
			}
		}else{
			certFlag = false;
			warning = warn2;			
		}
		networkView.RPC("OnCertficateResultRPC",info.sender,certFlag,warning);
		if(certFlag){
			UserInfoClass uf = new UserInfoClass();
			uf.name = usrName;
			uf.password = password;
			uf.ipAddress = info.sender.ipAddress;
			GlobalInfo.userInfoList.Add(uf);
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC]
	void OnRequestWatherInfoRPC(NetworkMessageInfo info) {
		networkView.RPC("OnResponseWatherInfoRPC",info.sender,GlobalInfo.nightOrNoonFlag,GlobalInfo.fogFlag);
	}
	
	[RPC]
	void OnResponseWatherInfoRPC(int nigthOrNoon,bool fogFlag) {
		GlobalInfo.nightOrNoonFlag = nigthOrNoon;
		GlobalInfo.fogFlag = fogFlag;
		Camera.main.SendMessage("OnSetWather",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnCertficateResultRPC(bool result,string warning){
		CertResultClass cr = new CertResultClass();
		cr.certResult = result;
		cr.warning = warning;
		GlobalInfo.LoginWindow.SendMessage("OnCertficateResult",cr,SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnChatMessageRPC(string chatMsg) {
		GlobalInfo.eventHandler.SendMessage("OnChatMessage",chatMsg,SendMessageOptions.DontRequireReceiver);
	}

	[RPC]

	void OnReportClientState(NetworkMessageInfo info)  {

	}

	[RPC]
	void OnGameStartedRPC() {
		if(GlobalInfo.userInfo.isReady){
			GlobalInfo.gameStarted = true;
			GlobalInfo.gameEnded = false;
			GlobalInfo.eventHandler.SendMessage("OnGameStarted",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC]
	void OnGameOverRPC() {
		if(GlobalInfo.gameStarted){
			GlobalInfo.eventHandler.SendMessage("OnGameEnded",SendMessageOptions.DontRequireReceiver);
			foreach(UserInfoClass uf in GlobalInfo.userInfoList) {
				uf.isReady = false;
			}
			GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC]
	void RequestGameStartedStateRPC(NetworkMessageInfo info) {
		GlobalInfo.rpcControl.RPC("RecieveGameStartedStateRPC",info.sender,GlobalInfo.gameStarted);
	}
	
	[RPC]
	void RecieveGameStartedStateRPC(bool isGameStarted) {
		foreach(Transform tr in GlobalInfo.equipSelWindow){
			tr.SendMessage("RecieveGameStartedState",isGameStarted,SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC]
	void OnUpdateUserTeamRPC(string name,int team) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].team = (TeamKind)team;
				GlobalInfo.userInfoList[i].score = 0;
				GlobalInfo.userInfoList[i].hitCount = 0;
				GlobalInfo.userInfoList[i].shootCount = 0;
				GlobalInfo.userInfoList[i].joined = true;
				break;
			}
		}
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnUpdateUserJoinedRPC(string name, bool joined) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].joined = joined;
				break;
			}
		}
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnRegistUserEquipRPC(string name,int equipment,bool isReady,NetworkViewID playerViewID) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].equipment = (EquipmentKind)equipment;
				GlobalInfo.userInfoList[i].destroyed = false;
				GlobalInfo.userInfoList[i].isReady = isReady;
				GlobalInfo.userInfoList[i].playerViewID = playerViewID;
				break;
			}
		}
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnSelectUserEquipRPC(string name,int equipment) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].equipment = (EquipmentKind)equipment;
				break;
			}
		}
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserList",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnChangeBattleFieldRPC() {
		GlobalInfo.eventHandler.SendMessage("OnChangeBattleField",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnRequestBattleFieldRPC(NetworkMessageInfo info) {
		GlobalInfo.rpcControl.RPC("OnResponseBattleFieldRPC",info.sender,(int)GlobalInfo.curBattleField,GlobalInfo.battleFieldSelected);
	}

	[RPC]
	void OnResponseBattleFieldRPC(int curBattleField,bool battleFieldSelected) {
		if(battleFieldSelected){
			GlobalInfo.battleFieldSelected = battleFieldSelected;
			GlobalInfo.curBattleField = (BattleFieldKind)curBattleField;
		}
	}
	
	[RPC]
	void OnUpdateUserScoreRPC(string name,float newScore) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].score += newScore;
				break;
			}
		}	
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserScore",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnUpdateUserHitCountRPC(string name) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].hitCount++;
				break;
			}
		}	
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserHitCount",SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	void OnUpdateUserShootCountRPC(string name) {
		for(int i=0;i<GlobalInfo.userInfoList.Count;i++){
			if(GlobalInfo.userInfoList[i].name.Equals(name)){
				GlobalInfo.userInfoList[i].shootCount++;
				break;
			}
		}	
		GlobalInfo.eventHandler.SendMessage("OnUpdateUserShootCount",SendMessageOptions.DontRequireReceiver);		
	}
	
	[RPC]
	void OnDownLoadUserListRPC(bool firstFlag,string name,string password,int team,int equip,float score,int hitCount,int shootCount,bool joined,bool destroyed,bool isReady,NetworkViewID playerViewID){
		if(firstFlag) GlobalInfo.userInfoList.Clear();
		UserInfoClass uf = new UserInfoClass();
		uf.name = name;
		uf.password = password;
		uf.team = (TeamKind)team;
		uf.joined = joined;
		uf.destroyed = destroyed;
		uf.isReady = isReady;
		uf.equipment = (EquipmentKind)equip;
		uf.score = score;
		uf.hitCount = hitCount;
		uf.shootCount = shootCount;
		uf.playerViewID = playerViewID;
		GlobalInfo.userInfoList.Add(uf);
	}
	
	[RPC]
	void OnDownLoadUserScoreRPC(string name,float score){
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			if(uf.name.Equals(name)){
				uf.score = score;
				break;
			}
		}
	}
	
	[RPC]
	void OnDownLoadUserHitCountRPC(string name,int hitCount) {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			if(uf.name.Equals(name)){
				uf.hitCount = hitCount;
				break;
			}
		}		
	}
	
	[RPC]
	void OnDownLoadUserShootCountRPC(string name,int shootCount) {
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			if(uf.name.Equals(name)){
				uf.shootCount = shootCount;
				break;
			}
		}		
	}
	
	void OnSetEnabled(bool flag){
		enabledFlag = flag;
	}
	
	bool DetectGameOver() {
		int n = 0;
		int m = 0;
		
		foreach(UserInfoClass uf in GlobalInfo.userInfoList){
			if(uf.team.Equals(TeamKind.Lightening)){
				if(!uf.destroyed) n++;
			}else{
				if(!uf.destroyed) m++;
			}
		}
		if(n == 0 || m == 0)
			return true;
		else
			return false;
	}	
}
