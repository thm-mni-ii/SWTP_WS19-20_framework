using UnityEngine;

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
