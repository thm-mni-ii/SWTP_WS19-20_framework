using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;

/**
 * GlobalManager class to manage the whole game (like: Canvases)
 * Hide and show the components
 */
public class GlobalManager : MonoBehaviour {
	
	/// <summary>
	/// RegisterCanvas manage the Register portal
	/// </summary>
	public Canvas RegisterCanvas;
	
	/// <summary>
	/// LoginCanvas manage the login portal
	/// </summary>
	public Canvas LoginCanvas;
	
	/// <summary>
	/// ChatCanvas manage Chat window
	/// </summary>
	public Canvas ChatCanvas;
	
	/// <summary>
	/// PartyCanvas manage party System and his components
	/// </summary>
    public Canvas PartyCanvas;
	
	/// <summary>
	/// ForgotCanvas manage Forgot password window
	/// </summary>
    public Canvas ForgotCanvas;

    /// <summary>
    /// GameCanvas manage displays the Game
    /// </summary>
    public Canvas GameCanvas;

    /**
     * Use this for initialization canvases, which we need
     */
    void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
        PartyCanvas.enabled = false;
        ForgotCanvas.enabled = false;
        GameCanvas.enabled = false;
    }
	void Start () { }
	
	/**
	 * Update is called once per frame
	 */
	void Update () { }
	
	/**
	 * change between canvases
	 */
	public void ToggleCanvas(string open){
        if (open == "login") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = true;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
        } else if (open == "register") {
            RegisterCanvas.enabled = true;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = false;
        } else if (open == "chat") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
            PartyCanvas.enabled = true;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
        }
        else if (open == "party")
        {
            PartyCanvas.enabled = true;
        } else if (open == "forgot")
        {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = true;
        }
        else if (open == "gameOn") {
            GameCanvas.enabled = true;
         }
        else if (open == "gameOff")
        {
            GameCanvas.enabled = false;
        }

    }
}
