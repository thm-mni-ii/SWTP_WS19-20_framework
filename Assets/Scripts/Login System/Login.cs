using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
	[SerializeField] private InputField userName;
	[SerializeField] private InputField passwordName;
	private GlobalManager globalCanvas;
	[SerializeField] private Text WarningMsg;
	private UserInfo user;
	private Chat chat;

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
        user = gameObject.GetComponent<UserInfo>();
        chat = gameObject.GetComponent<Chat>();
    }
    private void RetrieveFromDatabase()
    {
        RestClient.Get<UserInfo>(databaseURL + "/" + getLocalId + ".json?auth=" + idToken).Then(response =>
        {
            user = response;
        });
    }
    private void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post(
                    "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=" + AuthKey,
                    emailVerification).Then(
                    emailResponse =>
                    {
                        fsData emailVerificationData = fsJsonParser.Parse(emailResponse.Text);
                        EmailConfirmationInfo emailConfirmationInfo = new EmailConfirmationInfo();
                        serializer.TryDeserialize(emailVerificationData, ref emailConfirmationInfo).AssertSuccessWithoutWarnings();
                        
                        if (emailConfirmationInfo.users[0].emailVerified)
                        {
                            Debug.Log("You are in Email-Verified");
                            idToken = response.idToken;
                            localId = response.localId;
                            getLocalId = localId;
                            RetrieveFromDatabase();
                            GetUsername();
                            chat.EstablishConnection(user);
                            globalCanvas.ToggleCanvas("lobby");
                        } else
                        {
                            WarningMsg.text = "you need to verify your email!!";
                            Debug.Log("You are stupid, you need to verify your email dumb");
                        }
                        Debug.Log("End of Email-Verified");
                    });

            }).Catch(error =>
            {
                Debug.Log(error);
                WarningMsg.text = "Wrong username or password";
            });
    }
    private void GetUsername()
    {
        RestClient.Get<UserInfo>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerName = response.userN;
            Debug.Log("Login User " + playerName);
        });
    }
    public void LoginMethod(){
        SignInUser(userName.text, passwordName.text);
    }
    public void LogoutMethod()
    {
        chat.Disconnection();
        globalCanvas.ToggleCanvas("login");
    }
}
