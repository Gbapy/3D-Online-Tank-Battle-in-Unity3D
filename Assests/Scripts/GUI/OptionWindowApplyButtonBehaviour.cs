using UnityEngine;
using System.Collections;
using MagicBattle;

public class OptionWindowApplyButtonBehaviour : MonoBehaviour {
	public GUITexture backGround;
	public GUIStyle btn1Style;
	public GUIStyle btn2Style;
	
	private int curResolusion = 0;
	private float curVolumn = 0.5f;
	private int curDetail = 0;
	
	private Resolution[] resol;
	// Use this for initialization
	void Start () {
		resol = Screen.resolutions;
		curResolusion = PlayerPrefs.GetInt("CurResolusion");
		curResolusion = Mathf.Clamp(curResolusion,0,Screen.resolutions.Length);
		curVolumn = PlayerPrefs.GetFloat("CurVolumn");
		curVolumn = Mathf.Clamp(curVolumn,0.0f,1.0f);
		curDetail = PlayerPrefs.GetInt("CurDetail");
		curDetail = Mathf.Clamp(curDetail,0,5);
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.optionWindowFlag){
			guiTexture.enabled = true;
			backGround.enabled = true;
			backGround.pixelInset = new Rect(-Screen.width / 2.0f,-Screen.height / 2.0f,Screen.width,Screen.height);
			guiTexture.pixelInset = new Rect(-Screen.width * 0.17f,-Screen.height * 0.2f,Screen.width * 0.08f,Screen.height * 0.045f);			
		}else{
			guiTexture.enabled = false;
			backGround.enabled = false;
			enabled = false;
		}	
	}
	
	void OnEnable() {
		curResolusion = PlayerPrefs.GetInt("CurResolusion");
	}
	
	void OnGUI() {
		string tmp;
		
		string wid = resol[curResolusion].width.ToString();
		string hig = resol[curResolusion].height.ToString();
		int charCount = wid.Length + hig.Length + 1;
		float charWid = Screen.width * 0.136718f / charCount;
		
		charCount = 0;
		for(int i=0;i<wid.Length;i++){
			charCount++;
			tmp = wid.Substring(i,1);
			GUI.DrawTexture(new Rect(Screen.width * 0.47461f + charWid  * (charCount - 1),Screen.height * 0.36681223f,charWid,Screen.height * 0.035f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));
		}
		charCount++;
		for(int i=0;i<hig.Length;i++){
			charCount++;
			tmp = hig.Substring(i,1);
			GUI.DrawTexture(new Rect(Screen.width * 0.47461f + charWid  * (charCount - 1),Screen.height * 0.36681223f,charWid,Screen.height * 0.035f),(Texture)Resources.Load("GUI/SpecialCamera/0" + tmp + "_G"));			
		}
		if(GUI.Button(new Rect(Screen.width * 0.4501f,Screen.height * 0.36098981f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn1Style)){
			curResolusion--;
			if(curResolusion < 0) curResolusion = Screen.resolutions.Length - 1;
		}
		if(GUI.Button(new Rect(Screen.width * 0.6201171875f,Screen.height * 0.36098981f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn2Style)){
			curResolusion++;
			if(curResolusion > Screen.resolutions.Length - 1) curResolusion = 0;
		}
		if(GUI.Button(new Rect(Screen.width * 0.4501f,Screen.height * 0.44395924f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn1Style)){
			curDetail--;
			if(curDetail < 0) curDetail = 0;
		}
		if(GUI.Button(new Rect(Screen.width * 0.6201171875f,Screen.height * 0.44395924f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn2Style)){
			curDetail++;
			if(curDetail > 5) curDetail = 5;
		}
		GUI.DrawTexture(new Rect(Screen.width * 0.470703125f + curDetail * Screen.width * 0.022515625f,Screen.height * 0.44695924f
			,Screen.width * 0.03f,Screen.height * 0.0436681f),(Texture)Resources.Load("GUI/Option/L_2"));
		if(GUI.Button(new Rect(Screen.width * 0.4501f,Screen.height * 0.526928675f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn1Style)){
			curVolumn -= 0.1f;
			if(curVolumn < 0.0f) curVolumn = 0.0f;
		}
		if(GUI.Button(new Rect(Screen.width * 0.6201171875f,Screen.height * 0.526928675f,Screen.width * 0.0136771875f,Screen.height * 0.0436681f),"",btn2Style)){
			curVolumn += 0.1f;
			if(curVolumn > 1.0f) curVolumn = 1.0f;
		}
		GUI.DrawTexture(new Rect(Screen.width * 0.470703125f + curVolumn * Screen.width * 0.112578125f,Screen.height * 0.529928675f
			,Screen.width * 0.03f,Screen.height * 0.0425f),(Texture)Resources.Load("GUI/Option/L_2"));	
	}
	
	void OnMouseDown() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/Select_2");
	}
	
	void OnMouseUp() {
		guiTexture.texture = (Texture)Resources.Load("GUI/Option/Select_1");
		GlobalInfo.optionWindowFlag = false;
		GlobalInfo.mainMenuFlag = true;
		PlayerPrefs.SetInt("CurResolusion",curResolusion);
		PlayerPrefs.SetFloat("CurVolumn",curVolumn);
		PlayerPrefs.SetInt("CurDetail",curDetail);
		Screen.SetResolution(resol[curResolusion].width,resol[curResolusion].height,true);
		QualitySettings.SetQualityLevel(curDetail);
		foreach(Transform tr in GlobalInfo.mainMenu){
			tr.SendMessage("SetEnabled",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void DrawDigit() {

	}
}
