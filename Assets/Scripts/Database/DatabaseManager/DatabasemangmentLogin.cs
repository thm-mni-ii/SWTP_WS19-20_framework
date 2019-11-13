using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DatabasemangmentLogin : MonoBehaviour
{

    public Text nametext;
    public InputField emailTextlogin;
    public InputField passwordTextlogin;
    UserInfo user = new UserInfo();
    private string databaseURL = "https://mein-5d0b2.firebaseio.com/";
    private string AuthKey = "AIzaSyCscgviTiGsQOjWxxJJ4cERbxycPo4OdCg";
    public static fsSerializer serializer = new fsSerializer();


   

    public static string playerName;
    private string idToken;
    public static string localId;
    private string getLocalId;


    private void Start()
    {
        
    }
    

    private void Updatename()
    {
        
        nametext.text = "welcome :) " + playerName;
    }
    public void SignInUserButton()
    {
        SignInUser(emailTextlogin.text, passwordTextlogin.text);
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
                            idToken = response.idToken;
                            localId = response.localId;
                            getLocalId = localId;
                            RetrieveFromDatabase();
                            GetUsername();
                           
                        }
                        else
                        {
                            Debug.Log("You are stupid, you need to verify your email dumb");
                        }
                    });

            }).Catch(error =>
            {
                Debug.Log(error);
            });
    }
    private void GetUsername()
    {
        RestClient.Get<UserInfo>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerName = response.userN;
            Debug.Log("hallo form " + playerName);
            Updatename();


        });
    }
    
}
