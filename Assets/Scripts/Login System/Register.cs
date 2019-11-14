using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
	private GlobalManager globalCanvas;
	
	public InputField emailText;
	public InputField usernameText; 
	public InputField passwordText;
	[SerializeField] private Text WarningMsg;
    
	UserInfo user = new UserInfo();

	private string databaseURL = "https://mmo-spiel.firebaseio.com/";
	private string AuthKey = "AIzaSyAUr_7gFkWnoOfPJvLnigo5KSq96lAlELg";

	public static fsSerializer serializer = new fsSerializer();

	public static string playerName;
	private string idToken;
	public static string localId;
	private string getLocalId;
	
    // Start is called before the first frame update
	void Start () {		
	globalCanvas = gameObject.GetComponent<GlobalManager>();
	}
	private void PostToDatabase(bool emptyScore = false, string idTokenTemp = "") 
	{
		if (idTokenTemp == "")  {
			idTokenTemp = idToken;
		}
		RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + idTokenTemp, user);
	}
	private void SignUpUser(string email, string username, string password)
	{
		string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
		RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(
			response =>
			{
				string emailVerification = "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + response.idToken + "\"}";
				RestClient.Post(
					"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=" + AuthKey,
					emailVerification);
				localId = response.localId;
				playerName = username;
				PostToDatabase(true, response.idToken);

			}).Catch(error =>
		{
			Debug.Log(error);
			WarningMsg.text = "Username already exists";
		});
	}
	private void GetUsername()
	{
		RestClient.Get<UserInfo>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
		{
			playerName = response.userN;
		});
	}
	public void RegisterButtonMethod(){
		SignUpUser(emailText.text, usernameText.text, passwordText.text);
		globalCanvas.ToggleCanvas("login");
	}
}
