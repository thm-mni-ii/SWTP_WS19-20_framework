using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Databasemanagment : MonoBehaviour
{

    public InputField emailText;
    public InputField usernameText;
    public InputField passwordText;


    User1 user = new User1();

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
    
    

    private void PostToDatabase(bool emptyScore = false, string idTokenTemp = "")
    {
        if (idTokenTemp == "")
        {
            idTokenTemp = idToken;
        }

        User1 user = new User1();



        RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + idTokenTemp, user);
    }

    

    public void SignUpUserButton()
    {
        SignUpUser(emailText.text, usernameText.text, passwordText.text);
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
            });
    }

    

    private void GetUsername()
    {
        RestClient.Get<User1>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerName = response.userN;


        });
    }

    
}
