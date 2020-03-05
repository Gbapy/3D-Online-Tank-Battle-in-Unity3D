using UnityEngine;
using System.Collections;

public class ButtonActiveController : MonoBehaviour {
	public MonoBehaviour myScript;
	
	void SetEnabled() {
		myScript.enabled = true;
	}
}
