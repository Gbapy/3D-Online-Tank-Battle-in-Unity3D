using UnityEngine;
using System.Collections;

namespace MagicBattle{
	public enum TankControlState {
		Manual = 1,
		AutoDrive = 2
	}
	
	public enum TankState
	{
		None = 0,
		isMovingForward = 1,
		isMovingBackward = 2,
		isRotating = 3
	}
	
	public enum TankManner {
		Aggressive = 0,
		Defensive = 1,
		Cowardy = 2
	}

	public enum ShellKind {
		HeavyShell = 0,
		GeneralMisile = 1,
		LightShell = 2,
		GuidedAirMisile = 3,
		GiantShell = 4,
		MediumShell = 5,
		NuclearShell = 6,
		Bullet = 7
	}
	
	public enum EquipmentKind {
		PlayerTank1 = 0,
		PlayerTank2 = 1,
		PlayerTank3 = 2,
		KV1 = 3,
		Tiger = 4,
		Panther = 5,
		T64 = 6,
		Tr85M1 = 7,
		Leopard2A3 = 8,
		T34_85 = 9,
		CV = 10,
		PL = 11,
		Ariete = 12,
		Challenger = 13,
		TK_X = 14,
		Revolution = 15,
		T90S = 16,
		Abrams = 17,
		Helicopter1 = 18,
		Helicopter2 = 19,
		Ghost = 20
	}
	
	public enum BattleFieldKind{
		Depod = 0,
		Village = 1,
		Freedom = 2,
		Port = 3,
		RadarBase = 4,
		Trench = 5,
		Green = 6,
		Cliff = 7,
		Snow = 8,
		TrainPlace = 9,
		TrainPaceNight = 10
	}
	
	public enum TeamKind{
		Lightening = 0,
		Thunder = 1
	}
	
	public class ShellAttackedSendMsgParam{
		public Vector3 attackedPoint;
		public Vector3 normal;
		public Vector3 direction;
		public ShellKind attackedShellKind;
		public NetworkViewID viewID;
		public string userName;
	}

	public class DestroyedSendMsgParam{
		public int kind;
		public Vector3 position;
		public Quaternion rotation;
		public NetworkViewID viewID;
	}
	
	public class ShootSendMsgParam{
		public ShellKind kind;
		public Vector3 position;
		public Vector3 dir;
		public NetworkViewID viewID;
		public NetworkViewID targetViewID;
		public string userName;
		public int useGravity;
	}
	
	public class ShellClass{
		public float explosionPower;
		public float explosionRadius;
		public float speed;
		public float destructionPower;
		public float lifeCycle = 5.0f;
	}
	
	public class CertResultClass {
		public bool certResult = true;	
		public string warning = "";
	}
	
	public class UserInfoClass{
		public string name = "";
		public string ipAddress = "";
		public string password = "";
		public TeamKind team;
		public EquipmentKind equipment;
		public bool joined = false;
		public float score = 0.0f;
		public int hitCount = 0;
		public int shootCount = 0;
		public bool destroyed = false;
		public bool isReady = false;
		public NetworkViewID playerViewID;
		public bool aiFlag = false;
	}
	
	public class ChatClass{
		public string msg = "";
		public float psTime = 0.0f;
	}
	
	public class HouseActivityParam{
		public Vector3 hitpoint;
		public Vector3 globalHitPoint;
		public float radius = 0.0f;
		public bool destroy = false;
		public bool colflag = false;
		public Vector3 origin;
	}

	public class ReportClass{
		public NetworkViewID nViewID;
		public float reportedTime;
	}

	public static class GlobalInfo{
		public static System.Collections.Generic.List<UserInfoClass> userInfoList = new System.Collections.Generic.List<UserInfoClass>();
		public static System.Collections.Generic.List<ReportClass> report = new System.Collections.Generic.List<ReportClass>();
		public static UserInfoClass userInfo;
		public static NetworkViewID playerViewID;
		public static Camera playerCamera;
		public static GameObject myPlayer;
		public static bool disconnected = false;
		public static ShellKind attackedShellKind;
		public static Vector3 shellAttackedPosition;
		public static Transform curPlayer;
		public static int kindsOfShell = 8;
		public static ShellClass[] shellProperty;
		public static float ATTACKABLE_RADIUS = 6.0f;
		public static NetworkView rpcControl;
		public static bool rpcFound = false;
		public static bool isMoving = false;
		public static GameObject eventHandler;
		public static bool chatScreenFlag = false;
		public static int camPosState = 0;
		public static bool specialCamState = false;
		public static bool logged = false;
		public static GameObject curExhibitionEquip;
		public static bool gameStarted = false;
		public static bool gameEnded = true;
		public static bool connected = false;
		public static bool isConnecting = false;
		public static bool serverFound = false;
		public static bool certificated = false;
		public static bool warnFlag = false;
		public static string warnText = "";
		public static GameObject LoginWindow;
		public static bool mainMenuFlag = false;
		public static Transform mainMenu;
		public static bool optionWindowFlag = false;
		public static Transform optionWindow;
		public static bool helpWindowFlag = false;
		public static Transform helpWindow;
		public static bool teamSelWindowFlag = false;
		public static Transform teamSelWindow;
		public static bool battleFieldSelWindowFlag = false;
		public static Transform battleFieldSelWindow;
		public static bool equipSelWindowFlag = false;
		public static Transform equipSelWindow;
		public static bool exhibitionFlag = false;
		public static Transform exhibitionWindow;
		public static bool promptFlag = false;
		public static Transform promptWindow;
		public static BattleFieldKind curBattleField = BattleFieldKind.TrainPlace;
		public static bool destroyed = true;
		public static bool guiLoaded = false;
		public static bool battleFieldSelected = false;
		public static Vector3 realAimPos = Vector3.zero;
		public static bool targetAimed = false;
		public static GameObject canon;
		public static bool camAnimFlag = false;
		public static bool playerLoaded = false;
		public static float water_Level = 0.0f;
		public static int nightOrNoonFlag = 1;
		public static bool fogFlag = false;
		public static bool logoPlayed = false;
		public static bool[,] aiMapData;
		public static int mapWidth = 0;
		public static int mapHeight = 0;
		public static int terrainCount = 10;
		public static string[] treeDataFileName;
		public static int curDetail = 0;
	}	
}
