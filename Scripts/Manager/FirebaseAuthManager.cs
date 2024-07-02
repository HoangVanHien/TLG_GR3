using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Firebase")]
    
    public FirebaseAuth auth;
    private FirebaseUser user;


    public void InitializeFirebase(GameManager gameManager)
    {
        this.gameManager = gameManager;
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        Debug.Log("Done Auth");
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);  
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
            }
        }
    }

    void OnDestroy()
    {
        SignOut();
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public void SignUp(string email, string password, string playerName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                gameManager.screenManager.Announce("Create user was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                gameManager.screenManager.Announce(task.Exception.InnerException.Message);
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            gameManager.firebaseDatabase.CreateNewPlayer(result.User.UserId, email, password, playerName);
            Debug.LogFormat("User created successfully");
            gameManager.screenManager.Announce("User created successfully");
            Debug.LogFormat("After show announce");
        });
    }

    public void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                gameManager.screenManager.Announce("Sign in was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was faulted.");
                gameManager.screenManager.Announce(task.Exception.InnerException.Message);
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            gameManager.data.player.userID = user.UserId;
            gameManager.firebaseDatabase.MyCharacterDataListenerRegistration();
            gameManager.firebaseDatabase.MyPlayerStatusListenerRegistration();
            gameManager.firebaseDatabase.MyRankListenerRegistration();
            //gameManager.firebaseDatabase.SetLibraryList(user.UserId, new LibraryListStruct(0));
            gameManager.firebaseDatabase.LibraryListListenerRegistration(user.UserId);
            gameManager.screenManager.BringUp(ScreenEnum.MainMenu);
        });
    }

    public void SignOut()
    {
        if (auth == null || user == null)
        {
            return;
        }
        gameManager.firebaseDatabase.SetPlayerStatus(user.UserId, PlayerStatus.offline);
        gameManager.firebaseDatabase.StopListen();
        auth.SignOut();
    }

    public void DeleteAcount()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }
    }
}

