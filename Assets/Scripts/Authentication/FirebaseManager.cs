using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    public FirebaseAuth auth;
    public FirebaseUser user;

    [SerializeField]
    private TMP_InputField loginEmail;
    [SerializeField]
    private TMP_InputField loginPassword;
    [SerializeField]
    private TMP_Text loginOutputText;

    [SerializeField]
    private TMP_InputField registerUsername;
    [SerializeField]
    private TMP_InputField registerEmail;
    [SerializeField]
    private TMP_InputField registerPassword;
    [SerializeField]
    private TMP_InputField registerPasswordConfirm;
    [SerializeField]
    private TMP_Text registerOutputText;
    [SerializeField]
    private TMP_Text verificationOutput;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(CheckAndFixDependencies());
    }
    private IEnumerator CheckAutoLogin() 
    { 
        yield return new WaitForEndOfFrame();

        if(user != null)
        {
            var reloadTask = user.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadTask.IsCompleted);

            AutoLogin();
        }
        else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }

    private void AutoLogin()
    {
        if(user != null)
        {
            if(user.IsEmailVerified)
                GameManager.instance.ChangeScene(1);
            else
            {
                StartCoroutine(sendVerificationEmail());
            }
        }
       else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }
    private IEnumerator CheckAndFixDependencies()
    {
        var checkAndFixDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependenciesTask.IsCompleted);
        
        var dependencyResult = checkAndFixDependenciesTask.Result;

        if (dependencyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError("Dependencies not found!");
        }
    }
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if(signedIn && user != null)
            {
                Debug.Log("Signed out");
            }
            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed in as: {user.DisplayName}");
            }
        
       }
    }
    public string getDisplayName()
    {
        return user.DisplayName;
    }
    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void LoginButtonClick()
    {
        StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));
    }
    public void RegisterButtonClick()
    {
        StartCoroutine(RegisterLogic(registerUsername.text, registerEmail.text, registerPassword.text, registerPasswordConfirm.text));
    }

    public IEnumerator LoginLogic(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

       var loginTask = auth.SignInWithCredentialAsync(credential);
        
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Unknown error!";
            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Enter your Email!";
                    break;
                case AuthError.MissingPassword:
                    output = "Enter your Password!";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email!";
                    break;
                case AuthError.WrongPassword:
                    output = "Wrong Password!";
                    break;
                case AuthError.UserNotFound:
                    output = "User not found!";
                    break;
            }
            loginOutputText.text = output;
        
        }
        else
        {
            if(user.IsEmailVerified)
            {
                yield return new WaitForSeconds(1f);
                GameManager.instance.ChangeScene(1);
            }
            else
            {
                StartCoroutine(sendVerificationEmail());
            }
        }
    }
    private IEnumerator RegisterLogic(string _username, string _email, string _password, string _confirmpassword)
    {
        if(_username == "")
        {
            registerOutputText.text = "Enter Username!";
        }else if(_password != _confirmpassword)
        {
            registerOutputText.text = "Passwords doesn't match!";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if(registerTask.Exception != null)
            {
                string output = "Unknown error!";

                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                switch (error)
                {
                    case AuthError.MissingEmail:
                        output = "Enter your Email!";
                        break;
                    case AuthError.MissingPassword:
                        output = "Enter your Password!";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak Password!";
                        break;
                    case AuthError.InvalidEmail:
                        output = "Invalid Email!";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email is already in use!";
                        break;
                }
                registerOutputText.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = _username,

                };
                var defaultUserTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);

                if(defaultUserTask.Exception != null)
                {
                    user.DeleteAsync();
                    FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                    string output = "Unknown error!";
                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "Update cancelled!";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired";
                            break;
                        case AuthError.WeakPassword:
                            output = "Weak Password!";
                            break;
                        case AuthError.InvalidEmail:
                            output = "Invalid Email!";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            output = "Email is already in use!";
                            break;
                    }
                    registerOutputText.text = output;
                }
                else
                {
                    Debug.Log("Created succesfully!");
                    StartCoroutine(sendVerificationEmail());
                }
            }
        }
    }
    private IEnumerator sendVerificationEmail()
    {
        if (user != null)
        {
            var emailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);
            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown error!";
                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verification cancelled!";
                        break;
                    case AuthError.InvalidEmail:
                        output = "Invalid Email!";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Too many request!";
                        break;
                }
                verificationOutput.text = output;
                AuthUIManager.instance.AwaitVerification(false, user.Email, output);
            }
            else
            {
                AuthUIManager.instance.AwaitVerification(true, user.Email, null);
                Debug.Log("Sent succesfully!");
            }
        }
    }
}

