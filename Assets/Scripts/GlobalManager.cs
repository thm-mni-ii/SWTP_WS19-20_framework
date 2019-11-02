using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour {
	public Canvas RegisterCanvas;
	public Canvas LoginCanvas;
	public Canvas ChatCanvas;
	//public Canvas ForgetCanvas;
	//public Canvas ResetCanvas;
	//public Canvas ActiveCanvas;

	// Use this for initialization
	void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
	//	ResetCanvas.enabled = false;
		//ForgetCanvas.enabled = false;
		//ActiveCanvas.enabled = false;

	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ToggleCanvas(string open){
		if (open == "login") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = true;
		//	ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;

		} else if (open == "register") {
			RegisterCanvas.enabled = true;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;

		} else if (open == "chat") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			ChatCanvas.enabled  = true;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = true;
			//ActiveCanvas.enabled = false;

		} else if (open == "forget") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = true;
			//ActiveCanvas.enabled = false;

		} else if (open == "reset") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = true;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;

		} else if (open == "active") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = true;
		}
	
	}

}
