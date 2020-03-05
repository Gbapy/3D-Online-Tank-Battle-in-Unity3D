using UnityEngine;
using System.Collections;

public class UltraRedRayCamera : MonoBehaviour {
	public Material mat;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnRenderImage(RenderTexture source,RenderTexture destination) {
		Graphics.Blit ((Texture)source, destination, mat);
	}
}
