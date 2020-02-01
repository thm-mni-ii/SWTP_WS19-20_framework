using System;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

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
	/// ForgotCanvas manage Forgot password window
	/// </summary>
    public Canvas ForgotCanvas;
	/// <summary>
    /// GameCanvas manage displays the Game
    /// </summary>
    public Canvas GameCanvas;
	/// <summary>
	/// TOP10Canvas manage displays the Game
	/// </summary>
	public Canvas TOP10Canvas;
	/// <summary>
	/// logout Button to sign out from the game
	/// </summary>
	public Button logout = null;
	/// <summary>
	/// auxiliary variable to know if the logout button has been hidden
	/// </summary>
	public bool hideLogoutButton = false;
	/// <summary>
	/// auxiliary variable to know if the chat canvas has been hidden
	/// </summary>
	public bool hideChatCanvas = false;
	/// <summary>
	/// auxiliary variable to know if the TOP10 Canvas has been hidden
	/// </summary>
	public bool hideTOP10Canvas = false;
	/// <summary>
	/// auxiliary variable to hide all windows on lobby
	/// </summary>
	public bool hideall = true;

    /// <summary>
    /// Use this for initialization canvases, which we need
    /// canvases are:
    /// 1. RegisterCanvas
    /// 2. LoginCanvas
    /// 3. ChatCanvas
    /// 4. ForgotCanvas
    /// 5. GameCanvas
    /// </summary>
    void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
        ForgotCanvas.enabled = false;
        GameCanvas.enabled = false;
        //TOP10Canvas.enabled = false;
    }

    private void Update()
    {
	    // hide/show logout button by pressed on ESC
	    if (Input.GetKey(KeyCode.Escape))
	    {
		    if (hideLogoutButton)
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    logout.gameObject.SetActive(true);
				    hideLogoutButton = false;
			    }
		    }
		    else
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    logout.gameObject.SetActive(false);
				    hideLogoutButton = true;
			    }
		    }
	    }
	    
	    // hide/show chat canvas by pressed on c
	    if (Input.GetKey(KeyCode.C))
	    {
		    if (hideChatCanvas)
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    ChatCanvas.gameObject.SetActive(true);
				    hideChatCanvas = false;
			    }
		    }
		    else
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    ChatCanvas.gameObject.SetActive(false);
				    hideChatCanvas = true;
			    }
		    }
	    }
	    
	    // hide/show TOP10 Canvas by pressed on t
	    if (Input.GetKey(KeyCode.T))
	    {
		    if (hideTOP10Canvas)
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    TOP10Canvas.gameObject.SetActive(true);
				    hideTOP10Canvas = false;
			    }
		    }
		    else
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    TOP10Canvas.gameObject.SetActive(false);
				    hideTOP10Canvas = true;
			    }
		    }
	    }
	    
	    // hide/show by pressed on h
	    if (Input.GetKey(KeyCode.H))
	    {
		    if (hideall)
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    logout.gameObject.SetActive(true);
				    hideLogoutButton = false;
				    ChatCanvas.gameObject.SetActive(true);
				    hideChatCanvas = false;
				    TOP10Canvas.gameObject.SetActive(true);
				    hideTOP10Canvas = false;
				    hideall = false;
			    }
		    }
		    else
		    {
			    for (int i = 0; i < 1000; i++)
			    {
				    logout.gameObject.SetActive(false);
				    hideLogoutButton = true;
				    ChatCanvas.gameObject.SetActive(false);
				    hideChatCanvas = true;
				    TOP10Canvas.gameObject.SetActive(false);
				    hideTOP10Canvas = true;
				    hideall = true;
			    }
		    }
	    }
    }

    /// <summary>
    /// change between canvases
    /// </summary>
    /// <param name="open"></param>
	public void ToggleCanvas(string open){
        if (open == "login") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = true;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
        } else if (open == "register") {
            RegisterCanvas.enabled = true;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = false;
        } else if (open == "chat") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
        }
        else if (open == "forgot")
        {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
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
