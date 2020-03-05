using UnityEngine;
using System.Collections;
using MagicBattle;

public class GUIController : MonoBehaviour {
	public AudioSource audio;
	public GameObject gui;
	public MovieTexture mov;
	public Texture2D logoTex;

	private bool movieStarted = false;

	// Use this for initialization
	void Start () {
		if(!GlobalInfo.guiLoaded){
			StartCoroutine("StartMovie");
		}
	}

	IEnumerator StartMovie() {
		yield return new WaitForSeconds (0.3f);
		mov.Play();
		audio.Play();
		Screen.showCursor = false;
		movieStarted = true;
	}

	void Update(){
		if(!movieStarted) return;
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) {
			if(mov.isPlaying){
				mov.Stop();
				audio.Stop();
			}else{
				GlobalInfo.logoPlayed = true;
				if(!GlobalInfo.guiLoaded){
					Instantiate(gui,Vector3.zero,Quaternion.identity);
					GlobalInfo.guiLoaded = true;
				}
			}
		}
	}

	void OnGUI() {
		if(!movieStarted) return;
		if (!GlobalInfo.logoPlayed) {
			if (mov.isPlaying) 
				Graphics.DrawTexture (new Rect (-Screen.width * 0.2f, 0, Screen.width * 1.4f, Screen.height), (Texture)mov);
			else
				Graphics.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), (Texture)logoTex);
		}
	}
}
