using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button libraryButton;
    public Button signOutButton;
    public Button exitButton;

    //public Transform playerSlot;
    //public MatchPlayerUI playerUIPrefab;
    //private MatchPlayerUI playerUI;
    //private UnityAction<CharacterDataStruct> onMyCharacterDataChange;
    //private UnityAction<PlayerStatus> onMyPlayerStatusChange;

    //public void SetUp()
    //{
    //    if (!playerUI)
    //    {
    //        playerUI = Instantiate<MatchPlayerUI>(playerUIPrefab, playerSlot);
    //        onMyCharacterDataChange = MyCharacterSetUp;
    //        GameManager.instance.data.OnMyCharacterDataChangeAddListener(onMyCharacterDataChange);
    //        onMyPlayerStatusChange = OnMyPlayerStatusChange;
    //        GameManager.instance.data.OnMyPlayerStatusChangeAddListener(onMyPlayerStatusChange);
    //    }
    //    mainUI.SetActive(true);
    //}

    //public void MyCharacterSetUp(CharacterDataStruct character)
    //{
    //    playerUI.SetUp(character);
    //}

    private void Start()
    {
        playButton.onClick.AddListener(Play);
        libraryButton.onClick.AddListener(Library);
        signOutButton.onClick.AddListener(SignOut);
        exitButton.onClick.AddListener(Exit);
    }

    public void Play()
    {
        GameManager.instance.screenManager.BringUp(ScreenEnum.PlayPick);
    }

    public void Library()
    {
        GameManager.instance.data.isOnLibrary = true;
        GameManager.instance.data.isLibraryInMatch = false;
        GameManager.instance.data.LibraryListReset();
        GameManager.instance.screenManager.BringUp(ScreenEnum.Library);
    }

    public void SignOut()
    {
        GameManager.instance.screenManager.Loading();
        GameManager.instance.firebaseAuth.SignOut();
        GameManager.instance.screenManager.BringDown(ScreenEnum.MainMenu);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OnMyPlayerStatusChange(PlayerStatus playerStatus)
    {
        switch (playerStatus)
        {
            case PlayerStatus.offline:
                GameManager.instance.SetMyPlayerStatus(PlayerStatus.online);
                break;
            case PlayerStatus.findingMatch:

                break;
            case PlayerStatus.inMatch:
                GameManager.instance.screenManager.BringUp(ScreenEnum.GamePlay);
                break;
            default:
                break;
        }
    }
}
