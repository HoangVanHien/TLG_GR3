using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryUI : MonoBehaviour
{
    private GameManager gameManager;
    private LibraryListStruct libraryList;

    private LibraryPacket libraryPacket;
    private int libraryQuestionID;

    public LibraryQuestionUI libraryQuestionPrefab;
    public List<LibraryQuestionUI> lQLibraryList = new List<LibraryQuestionUI>();
    public List<LibraryQuestionUI> lQQuestionList = new List<LibraryQuestionUI>();
    public Transform questionUI;
    public Transform libraryContainer;
    public Transform questContainer;
    public bool isLibraryListChange = false;
    public bool isLibraryPacketChange = false;

    public GameObject libraryPacketScreen;

    public Button addLibraryPacketButton;
    public Button addLibraryQuestionButton;

    public TMP_InputField nameInput;

    public Button backButton;
    public Button midButton;
    public Button confirmButton;

    

    private void Start()
    {
        gameManager = GameManager.instance;
        gameManager.data.OnLibraryListChangeAddListener(() => { isLibraryListChange = true; });
        gameManager.data.OnLibraryPacketChangeAddListener(OnLibraryPacketChange);
        backButton.onClick.AddListener(OnBackButtonClick);
        midButton.onClick.AddListener(OnMidButtonClick);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        addLibraryPacketButton.onClick.AddListener(OnAddLibraryPacketButtonClick);
        addLibraryQuestionButton.onClick.AddListener(OnAddLibraryQuestionButtonClick);
        libraryPacketScreen.SetActive(false);
    }

    private void Update()
    {
        if (isLibraryPacketChange)
        {
            isLibraryPacketChange = false;
            SetUpLibraryPacket();
        }
        else if (isLibraryListChange)
        {
            isLibraryListChange = false;
            SetUpLibraryList();
        }
    }

    public void SetUpLibraryList()
    {
        bool inMatch = gameManager.data.isLibraryInMatch;
        libraryList = gameManager.data.GetLibraryList();
        addLibraryPacketButton.gameObject.SetActive(!inMatch);
        if (libraryList.libraryPacketInfos == null) return;
        int libraryListCount = libraryList.libraryPacketInfos.Count;
        if (libraryListCount <= 0) return;
        for (int i = 0; i < lQLibraryList.Count || i < libraryListCount; i++)
        {
            if (i >= lQLibraryList.Count)
            {
                lQLibraryList.Add(Instantiate(libraryQuestionPrefab, libraryContainer));
            }
            else if (i >= libraryListCount)
            {
                GameObject destroyObject = lQLibraryList[i].gameObject;
                lQLibraryList.RemoveAt(i);
                Destroy(destroyObject);
                --i;
                continue;
            }
            lQLibraryList[i].SetUpLibraryList(libraryList.libraryPacketInfos[i].libraryID, libraryList.libraryPacketInfos[i].libraryName, OnLibraryClick, OnLibraryDelete, !inMatch);
        }
        midButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(libraryPacketScreen.active);
        Debug.Log("Set Up LibraryList Done");
    }

    public void SetUpLibraryPacket()
    {
        libraryPacket = gameManager.data.GetLibraryPacket();
        Debug.Log(libraryPacket.libraryQuestions);
        if (libraryPacket.libraryID == null) return;
        nameInput.text = libraryPacket.libraryName;

        int libraryQuestionCount = 0;
        switch (libraryPacket.modeEnum)
        {
            case LibraryQuestionModeEnum.question:
                if (libraryPacket.libraryQuestions == null) libraryPacket.libraryQuestions = new List<LibraryQuestion>();
                libraryQuestionCount = libraryPacket.libraryQuestions.Count;
                break;
            case LibraryQuestionModeEnum.word:
                if (libraryPacket.libraryWords == null) libraryPacket.libraryWords = new List<LibraryWord>();
                libraryQuestionCount = libraryPacket.libraryWords.Count;
                break;
            default:
                break;
        }
        
        for (int i = 0; i < lQQuestionList.Count || i < libraryQuestionCount; i++)
        {
            if (i >= lQQuestionList.Count)
            {
                lQQuestionList.Add(Instantiate(libraryQuestionPrefab, questContainer));
            }
            else if (i >= libraryQuestionCount)
            {
                GameObject destroyObject = lQQuestionList[i].gameObject;
                lQQuestionList.RemoveAt(i);
                Destroy(destroyObject);
                --i;
                continue;
            }
            lQQuestionList[i].SetUpLibraryQuestion(i, libraryPacket.libraryQuestions[i].question, OnQuestionClick, OnQuestionDelete);

        }
            libraryPacketScreen.SetActive(true);
            confirmButton.gameObject.SetActive(true);
        Debug.Log("Set Up LibraryPacket Done");
    }

    private void OnLibraryClick(string libraryID)
    {
        if (!gameManager.data.isOnLibrary) gameManager.screenManager.BringDown(ScreenEnum.Library);
        gameManager.screenManager.Loading();
        libraryPacket.libraryID = libraryID;
        gameManager.firebaseDatabase.GetLibraryPacket(libraryID, gameManager.screenManager.DeafaultBackButton);
    }

    private void OnLibraryDelete(string libraryID)
    {
        libraryPacket.libraryID = libraryID;
        gameManager.screenManager.Announce("Do you want to delete " + libraryID, true, "Yes", DeleteLibraryPacket, true, "No");
    }

    private void DeleteLibraryPacket()
    {
        foreach (LibraryPacketInfo packetInfo in libraryList.libraryPacketInfos)
        {
            if(packetInfo.libraryID == libraryPacket.libraryID)
            {
                libraryList.libraryPacketInfos.Remove(packetInfo);
                break;
            }
        }
        gameManager.firebaseDatabase.SetMyLibraryList(libraryList);
        gameManager.firebaseDatabase.DeleteLibraryPacket(libraryPacket.libraryID);
        gameManager.screenManager.DeafaultBackButton();
    }

    private void OnQuestionClick(int questionID)
    {
        gameManager.screenManager.Loading();
        gameManager.data.SetLibraryQuestionID(questionID);
    }

    private void OnQuestionDelete(int questionID)
    {
        libraryQuestionID = questionID;
        gameManager.screenManager.Announce("Do you want to delete " + questionID, true, "Yes", DeleteLibraryQuestion, true, "No");
    }

    private void DeleteLibraryQuestion()
    {
        libraryPacket.libraryQuestions.RemoveAt(libraryQuestionID);
        gameManager.firebaseDatabase.SetLibraryPacket(libraryPacket);
        gameManager.screenManager.DeafaultBackButton();
    }

    private void OnLibraryPacketChange()
    {
        if (!gameManager.data.isOnLibrary) return;
        Debug.Log("Library Packet Change");
        isLibraryPacketChange = true;
    }


    public void OnBackButtonClick()
    {
        if (!libraryPacketScreen.active)
        {
            gameManager.data.isOnLibrary = false;
            gameManager.screenManager.BringDown(ScreenEnum.Library);
        }
        else
        {
            libraryPacketScreen.SetActive(false);
        }
    }

    public void OnMidButtonClick()
    {

    }

    public void OnConfirmButtonClick()
    {
        if (libraryPacketScreen.active)//Save Packet
        {
            libraryPacket.libraryName = nameInput.text;
            gameManager.data.UpdateLibrary(libraryPacket);
        }
    }

    private void OnAddLibraryPacketButtonClick()
    {
        string userID = gameManager.data.player.userID;
        libraryPacket.libraryID = userID.Substring(userID.Length - 5) + libraryList.createdPacketCount;
        libraryPacket = new LibraryPacket(libraryPacket.libraryID);
        gameManager.data.SetLibraryPacket(libraryPacket);
    }

    private void OnAddLibraryQuestionButtonClick()
    {
        int libraryCount = 0;
        switch (libraryPacket.modeEnum)
        {
            case LibraryQuestionModeEnum.question:
                libraryCount = libraryPacket.libraryQuestions == null ? 0 : libraryPacket.libraryQuestions.Count;
                break;
            case LibraryQuestionModeEnum.word:
                libraryCount = libraryPacket.libraryWords == null ? 0 : libraryPacket.libraryWords.Count;
                break;
            default:
                break;
        }
        gameManager.data.isLibraryInMatch = false;
        gameManager.data.SetLibraryQuestionID(libraryCount);
    }

}
