using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Mirror;
/// <summary>
/// Login class contains all necessary Methods, Variables and Database access to handle login Requests.
/// 1. (sign in) verify if the access data are right and make a connection between client and server
/// 2. (sign up/register)  register a new data in the database
/// 3. (sign out) logout from the connection between client and server
/// 4. other Functions like: forget the password
/// </summary>
[RequireComponent(typeof(NetworkManager))]
public class Login : MonoBehaviour
{
    /// <summary>
    /// Take username from GUI (login portal)
    /// </summary>
    [SerializeField] public InputField inputUserName = null;

    /// <summary>
    /// Take password from GUI (login portal)
    /// </summary>
    [SerializeField] private InputField inputPassword = null;

    /// <summary>
    /// Take E-Mail address from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rEmail = null;

    /// <summary>
    /// Take desired username from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rUsername = null;

    /// <summary>
    /// Take desired password from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rPassword = null;

    /// <summary>
    /// verification phase: Take desired password from GUI (register portal) 
    /// </summary>
    [SerializeField] private InputField rPasswordVerification = null;

    /// <summary>
    /// manage the whole game. Hide and show the components.
    /// </summary>
    private GlobalManager globalCanvas;

    /// <summary>
    /// warning message (by login portal)
    /// </summary>
    [SerializeField] private Text WarningMsg = null;

    /// <summary>
    /// warning message from forgot password portal
    /// </summary>
    [SerializeField] private Text ResPassWarningMsg = null;

    /// <summary>
    /// Take E-Mail address from forgot password portal
    /// </summary>
    [SerializeField] private InputField ResPassEmail = null;

    /// <summary>
    /// warning message from register portal
    /// </summary>
    [SerializeField] private Text regWarningMsg = null;

    /// <summary>
    /// data struct of user information. The struct include: username, email, id, score
    /// </summary>
    private UserInfo user;

    /// <summary>
    /// chat manager by client
    /// </summary>
    private Client chat;

    /// <summary>
    /// DatabaseReference from Firebase packages
    /// </summary>
    public DatabaseReference reference;

    /// <summary>
    /// Database Authentication from Firebase packages
    /// </summary>
    private Firebase.Auth.FirebaseAuth auth;

    /// <summary>
    /// NetworkManager is responsible for the network connection.
    /// He has connection settings (network address, maxConnections, etc)
    /// </summary>
    NetworkManager manager;

    /// <summary>
    /// response from database server (to know if all thinks right or to do some think)
    /// </summary>
    private int report = 0;

    /// <summary>
    /// auxiliary variable to take the Email address form checkUserExist() and put it in completeLogin(string resEmail)
    /// </summary>
    private string responseEmail = null;

    /// <summary>
    /// Show the connection status after clicking the Login button
    /// </summary>
    private bool connecting = false;

    /// <summary>
    /// User class have the user struct.
    /// The class is an auxiliary class to create a new user and to makes it easier to place it in the database.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The username of the player. There are unique usernames.
        /// </summary>
        public string username;

        /// <summary>
        /// Email address of player. There are unique emails.
        /// </summary>
        public string email;

        /// <summary>
        /// Player id (unique)
        /// </summary>
        public string id;

        /// <summary>
        /// Score of player. 
        /// </summary>
        public int score;

        /// <summary>
        /// User construct to create a new user. 
        /// </summary>
        /// <param name="username">The username of the player</param>
        /// <param name="email">Email address of player.</param>
        /// <param name="id">Player id</param>
        public User(string username, string email, string id)
        {
            this.username = username;
            this.email = email;
            this.id = id;
            score = 0; //Initially the player has 0 points
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// connection to database (Firebase) configuration and show login portal
    /// </summary>
    void Start()
    {
        //setup canvases and game objekts
        globalCanvas = gameObject.GetComponent<GlobalManager>();
        user = gameObject.GetComponent<UserInfo>();
        chat = gameObject.GetComponent<Client>();
        GameObject NM = GameObject.FindWithTag("NetworkManager");
        if (NM != null) manager = NM.GetComponent<NetworkManager>();
        //Firebase configuration
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mmo-spiel-1920.firebaseio.com"); //setup database url
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    /// <summary>
    /// Update is called once per frame
    /// The warning text objects of the login/register interfaces are updated here
    /// Unity is not Thread save, therefore it allows the game objects to be updated ONLY from the main thread
    /// upon clicking a button the response is set to the variable "report" and then is read and handled once the frame is called
    /// Also after clicking the login button if the userinformation is correct the connection is established here.
    ///
    /// There are many types of requests/responses:
    /// case 1: Login Succesful
    /// case 10: or case 11: Invalid username or password
    /// case 12: Registration Succesful
    /// case 13: Error Registration was canceled
    /// case 14: Email is already registered
    /// case 15: please confirm your email
    /// case 16: Password reset email sent successfully
    /// case 17: Error Send request was canceled
    /// case 18: Could not send reset E-mail
    /// case 19: User not found in database (everything OK) -> complete register (by register)
    /// case 20: Username is Taken
    /// case 21: username was found -> check: if email is Verified (by login)
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !connecting && globalCanvas.LoginCanvas.enabled)
        {
            checkUserExist();
        }
       
        /* Show the connection status after clicking the Login button */
        if (connecting)
        {
            if (NetworkClient.isConnected)
            {
                connecting = false;
                chat.EstablishConnection(user);
                globalCanvas.ToggleCanvas("chat");
                globalCanvas.ToggleCanvas("partylist");
                if (NetworkClient.isConnected && !ClientScene.ready)
                {
                    ClientScene.Ready(NetworkClient.connection);
                    if (ClientScene.localPlayer == null)
                    {
                        ClientScene.AddPlayer();
                    }
                }
            }
            else if (!NetworkClient.isConnected && !NetworkClient.active)
            {
                connecting = false;
                WarningMsg.text = "NetworkServer Offline!";
            }
        }

        /* Login-Menu messages are displayed from this switch-case becasue this action must be done from he main Thread */
        if (report != 0)
        {
            switch (report)
            {
                case 1: //Connection Request
                    if (!NetworkClient.isConnected && !NetworkClient.active && !connecting)
                    {
                        connecting = true;
                        WarningMsg.text = "Connecting to Server..";
                        manager.StartClient();
                    }

                    break;
                case 10:
                    WarningMsg.text = "Invalid Username or password #1";
                    break;
                case 11:
                    WarningMsg.text = "Invalid username or password";
                    break;
                case 12:
                    globalCanvas.ToggleCanvas("login");
                    WarningMsg.text = "Registration Succesful";
                    break;
                case 13:
                    regWarningMsg.text = "Error Registration was canceled)";
                    break;
                case 14:
                    regWarningMsg.text = "Email is already registered";
                    break;
                case 15:
                    WarningMsg.text = "please confirm your email";
                    break;
                case 16:
                    ResPassWarningMsg.text = "Password reset email sent successfully";
                    break;
                case 17:
                    ResPassWarningMsg.text = "Error Send request was canceled";
                    break;
                case 18:
                    ResPassWarningMsg.text = "Could not send reset E-mail";
                    break;
                // 19: User not found in database (everything OK) -> complete register (by register)
                case 19:
                    report = 0;
                    registerNewUser();
                    break;
                case 20:
                    regWarningMsg.text = "Username is Taken";
                    break;
                // 21: username was found -> check: if email is Verified (by login)
                case 21:
                    report = 0;
                    completeLogin(responseEmail);
                    break;
                default:
                    
                    break;
            }

            report = 0;
        }
    }

    /// <summary>
    /// login function checks if user is registered on the database and returns the result (response) accordingly.
    /// The function take the username and check in the database, if this user have an email and save it in the variable responseEmail.
    /// </summary>
    public void checkUserExist()
    {

        if (inputUserName.text != null && inputUserName.text != "" && inputPassword.text != null &&
            inputPassword.text != "")
        {
            responseEmail = null;
            FirebaseDatabase.DefaultInstance
                .GetReference("users/" + inputUserName.text + "/email")
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        responseEmail = (string) snapshot.Value;
                        if (responseEmail == null)
                        {
                            //Invalid username
                            report = 11;
                        }
                        else
                        {
                            // 21: username was found -> check: if email is Verified
                            report = 21;
                        }
                    }
                });
        }
    }

    /// <summary>
    /// This function completes the login procedure and checks if the email is verified.
    /// once checkUserExist() finds the email of the users it's given here as a parameter.
    /// This Methode logs in the user once the login is succesful it sets report to 1 which then starts the connection on 'update'
    /// after the next frame is called.
    /// </summary>
    /// <param name="resEmail">response Email from checkUserExist()</param>
    public void completeLogin(string resEmail)
    {
        if (resEmail != null && resEmail != "" && inputPassword.text != null && inputPassword.text != "")
        {
            auth.SignInWithEmailAndPasswordAsync(resEmail, inputPassword.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    report = 10;
                    return;
                }

                if (task.IsFaulted)
                {
                    report = 11;
                    return;
                }

                Firebase.Auth.FirebaseUser userInfoFromDatabase = task.Result;
                if (userInfoFromDatabase.IsEmailVerified)
                {
                    user.email = userInfoFromDatabase.Email;
                    user.id = userInfoFromDatabase.UserId;
                    user.username = userInfoFromDatabase.DisplayName;
                   
                    report = 1;
                }
                else
                {
                    userInfoFromDatabase.SendEmailVerificationAsync();
                    report = 15;
                }
            });
        }
        else
        {
            WarningMsg.text = "Please Enter a username and password";
        }
    }

    /// <summary>
    /// logs out user and disconnects the connection between client and server
    /// </summary>
    public void logoutAndDisconnect()
    {
        chat.Disconnection();
        if (NetworkClient.active)
        {
            manager.StopClient();
        }

        globalCanvas.ToggleCanvas("login");
        inputPassword.text = "";
    }

    /// <summary>
    /// Registers new user to the Database (send report number 19) or handles the request if the user is already registered.
    /// User not found in database (everything OK) -> complete register (by register)
    /// </summary>
    public void checkIfUserRegister()
    {
        if (rUsername.text != null && rUsername.text != "" && rEmail.text != null && rEmail.text != "" &&
            rPassword.text != null && rPassword.text != "" && rPasswordVerification.text != null &&
            rPasswordVerification.text != "")
        {
            if (!rEmail.text.Contains("@"))
            {
                regWarningMsg.text = "Please enter your E-mail";
                return;
            }

            string u = null;
            FirebaseDatabase.DefaultInstance.GetReference("users/" + rUsername.text + "/username")
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        u = (string) snapshot.Value;
                        if (u == null)
                        {
                            report = 19;
                        }
                        else
                        {
                            report = 20;
                        }
                    }
                });
        }
    }

    /// <summary>
    /// check if every thing okay (like length of the password) an put the new user information in the database (based writeNewUser() function)
    /// </summary>
    public void registerNewUser()
    {
        if (rUsername.text != null && rUsername.text != "" && rEmail.text != null && rEmail.text != "" &&
            rPassword.text != null && rPassword.text != "" && rPasswordVerification.text != null &&
            rPasswordVerification.text != "")
        {
            if (rPassword.text.Length < 6 || rPasswordVerification.text.Length < 6)
            {
                regWarningMsg.text = "Password must contain at least 6 Charachters";
                return;
            }

            if (string.Compare(rPassword.text, rPasswordVerification.text) != 0)
            {
                regWarningMsg.text = "Passwords don't match";
                return;
            }

            auth.CreateUserWithEmailAndPasswordAsync(rEmail.text, rPassword.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    report = 13;
                    return;
                }

                if (task.IsFaulted)
                {
                    report = 14;
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                newUser.SendEmailVerificationAsync();
                setDisplayName(newUser);
                writeNewUser(newUser.UserId, rUsername.text, rEmail.text);
                report = 12;
            });
        }
        else
        {
            regWarningMsg.text = "Please fill in all the Fields";
        }
    }

    /// <summary>
    /// Saved the username in the Database because firebase saves only the email and password
    /// </summary>
    /// <param name="newUser">Username from login system</param>
    public void setDisplayName(Firebase.Auth.FirebaseUser newUser)
    {
        if (newUser != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = rUsername.text,
            };
            newUser.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                if (task.IsFaulted)
                {
                    return;
                }

            });
        }
    }

    /// <summary>
    /// sends a recovery Email to reset password
    /// </summary>
    public void resetPassword()
    {
        if (ResPassEmail.text != null && ResPassEmail.text != "")
        {
            if (!ResPassEmail.text.Contains("@"))
            {
                ResPassWarningMsg.text = "Please enter your E-mail to reset password";
                return;
            }

            auth.SendPasswordResetEmailAsync(ResPassEmail.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    report = 17;
                    return;
                }

                if (task.IsFaulted)
                {
                    report = 18;
                    return;
                }

                report = 16;
            });
        }
        else
        {
            ResPassWarningMsg.text = "Please enter your E-mail to reset password";
        }
    }

    /// <summary>
    /// after register take the new user and include him in the database
    /// </summary>
    /// <param name="userId">id of the new user</param>
    /// <param name="name">username of the new user</param>
    /// <param name="email">email of the new user</param>
    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email, userId);
        reference.Child("users").Child(name).Child("username").SetValueAsync(name);
        reference.Child("users").Child(name).Child("email").SetValueAsync(email);
        reference.Child("users").Child(name).Child("id").SetValueAsync(userId);
        reference.Child("users").Child(name).Child("score").SetValueAsync(0);
        
        //string json = JsonUtility.ToJson(user);
        //reference.Child("users").Child(name).SetRawJsonValueAsync(json);
    }

    /// <summary>
    /// display "register" Canvas
    /// </summary>
    public void RegisterButton()
    {
        globalCanvas.ToggleCanvas("register");
    }

    /// <summary>
    /// display "forgot" Canvas
    /// </summary>
    public void ResetButton()
    {
        globalCanvas.ToggleCanvas("forgot");
    }

    /// <summary>
    /// display "login" Canvas
    /// </summary>
    public void backButton()
    {
        globalCanvas.ToggleCanvas("login");
    }
}