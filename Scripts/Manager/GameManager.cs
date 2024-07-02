using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public DependencyStatus dependencyStatus;

    public FirebaseAuthManager firebaseAuth;
    public FirebaseDatabaseManager firebaseDatabase;

    public DataManager data;
    
    public ScreenManager screenManager;
    public PlayerRoomManager playerRoomManager;
    public MatchManager matchManager;

    public bool isDoneSetUp = false;

    private void Awake()
    {
        if (GameManager.instance)//if there is already a GameManager
        {
            Destroy(gameObject);//Destroy the new GameManager that being duplicated
            return;
        }
        instance = this;//To make all the instance being call become this
        DontDestroyOnLoad(gameObject);//prevent this gameObject (GameManager) from being deleted when load a new scene


    }

    // Start is called before the first frame update
    async void Start()
    {
        screenManager.Loading();
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseAuth.InitializeFirebase(this);
                firebaseDatabase.InitializeFirebase(this);
                data.SetUp(this, firebaseDatabase);
                screenManager.BringUp(ScreenEnum.Auth);
                playerRoomManager.SetUp(firebaseDatabase, data, screenManager, matchManager);
                matchManager.SetUp(this, firebaseDatabase, data, screenManager, playerRoomManager);
                //firebaseDatabase.GetCards();
                isDoneSetUp = true;
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
                Application.Quit();
            }
        });
        //firebaseDatabase.CardGen();
    }

    public void SetMyPlayerStatus(PlayerStatus playerStatus)
    {
        firebaseDatabase.SetPlayerStatus(data.player.userID, playerStatus);
    }

    public void OpenLibraryQuestion()
    {
        screenManager.BringUp(ScreenEnum.GamePlay);
    }

}
