using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRoomUI : MonoBehaviour
{
    private DataManager data;
    public RoomPlayerUI roomPlayerUIPrefab;
    public List<RoomPlayerUI> playerUIList = new List<RoomPlayerUI>();
    public Transform roomPlayerContainer;
    private int maxSlot = 50;

    public TMP_Text roomIDText;
    public TMP_Text gameModeText;

    public Button playButton;
    public TMP_Text playText;
    public Button exitButton;
    public Button watchButton;
    public TMP_Text watchText;
    public Button questionButton;
    public TMP_Text questionText;
    public Button switchTeamButton;

    public bool isFindingMatch;
    public bool isPlayerRoomChange = false;
    public bool isRoomPlayersChange = false;
    public bool isCustom = false;
    public bool isOwner = false;
    public bool isWatch = false;
    public bool isLibraryPacketPicked = false;

    private void Start()
    {
        data = GameManager.instance.data;
        data.OnMyRoomIDStructChangeAddListener(SetUpPlayerRoom);
        data.OnRoomPlayerStructListChangeAddListener(SetUpRoomPlayers);

        playButton.onClick.AddListener(Play);
        watchButton.onClick.AddListener(Watch);
        switchTeamButton.onClick.AddListener(SwitchTeam);
        questionButton.onClick.AddListener(QuestionPick);
        exitButton.onClick.AddListener(Exit);
    }

    private void Update()
    {
        if (isPlayerRoomChange) SetUpPlayerRoomInMain();
        if (isRoomPlayersChange) SetUpRoomPlayersInMain();
        LibraryPacket libraryPacket = data.GetLibraryPacket();
        if (libraryPacket.libraryID != null && libraryPacket.libraryID.Length >= 6)
        {
            questionText.text = libraryPacket.libraryName;
        }
        else
        {
            questionText.text = "Question Pick";
        }
    }

    public void SetUpPlayerRoom()
    {
        isPlayerRoomChange = true;
    }
    public void SetUpPlayerRoomInMain()
    {
        RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
        if (roomIDStruct.roomID.Length <= 0) return;

        isCustom = roomIDStruct.gameMode == GameModeEnum.custom;
        //team2.SetActive(isCustom);
        watchButton.gameObject.SetActive(isCustom);
        //switchTeamButton.gameObject.SetActive(isCustom);

        roomIDText.text = roomIDStruct.roomID;
        gameModeText.text = roomIDStruct.gameMode.ToString();
        isPlayerRoomChange = false;
    }

    public void SetUpRoomPlayers()
    {
        isRoomPlayersChange = true;
    }
    public void SetUpRoomPlayersInMain()
    {
        List<RoomPlayerStruct> roomPlayerStructList = GameManager.instance.data.GetRoomPlayerStructList();
        //int t1 = 0, t2 = 0;
        int i = 0;//count
        if (roomPlayerStructList == null || roomPlayerStructList.Count <= 0)
        {
            Debug.Log("room player empty");
            if (!isCustom) return;
            isOwner = false;
            isWatch = true;
        }
        else
        {
            isOwner = roomPlayerStructList[0].userID == data.player.userID;
            isWatch = true;
            foreach (RoomPlayerStruct player in roomPlayerStructList)
            {
                //i = player.isTeam1 ? t1++ : maxSlot + t2++;
                Debug.Log("Set up " + player.name + " on " + i);
                if (playerUIList.Count <= i)
                {
                    //add new
                    RoomPlayerUI newSlot = Instantiate(roomPlayerUIPrefab, roomPlayerContainer);
                    playerUIList.Add(newSlot);
                }
                playerUIList[i].SetupRoomPlayer(player, isOwner);
                if (player.userID == data.player.userID) isWatch = false;
                i++;
            }
        }

        if (isCustom)
        {
            //if (isOwner)
            //{
            //    if (t1 < maxSlot) playerUIList[t1++].SetUpAddBot(true);
            //    if (t2 < maxSlot) playerUIList[maxSlot + t2++].SetUpAddBot(false);
            //}
            //for (; t2 < maxSlot; t2++)
            //{
            //    playerUIList[t2 + maxSlot].Reset();
            //}
        }
        for (; i  < playerUIList.Count; i++)
        {
            playerUIList[i].Reset();
        }
        //playButton.gameObject.SetActive(!isOwner);
        if (isOwner)
        {
            bool isPlayable = GameManager.instance.matchManager.matchPlayers == null || GameManager.instance.matchManager.matchPlayers.Count <= 0;
            questionButton.gameObject.SetActive(isPlayable);
        }
        playButton.gameObject.SetActive(isOwner);
        watchText.text = isWatch ? "Unwatch" : "Watch";
        //playText.text = isOwner ? "Play" : "Ready";
        playText.text = "Play";
        isRoomPlayersChange = false;
        Debug.Log("Done set up player room");
    }

    public void Ready()
    {
        GameManager.instance.playerRoomManager.Ready();
        playText.text = "Cancel Ready";
        isFindingMatch = true;
    }

    public void CancelReady()
    {
        GameManager.instance.playerRoomManager.CancelReady();
        playText.text = "Ready";
        isFindingMatch = false;
    }

    public void FindMatch()
    {
        GameManager.instance.playerRoomManager.FindMatch();
        GameManager.instance.SetMyPlayerStatus(PlayerStatus.findingMatch);
        playText.text = "Cancel Find Match";
        isFindingMatch = true;
    }

    public void CancelFindMatch()
    {
        GameManager.instance.playerRoomManager.CancelFindMatch();
        GameManager.instance.SetMyPlayerStatus(PlayerStatus.online);
        playText.text = "Play";
        isFindingMatch = false;
    }

    public void Play()
    {
        //if (GameManager.instance.data.GetLibraryPacket().libraryID.Length <= 0) GameManager.instance.firebaseDatabase.GetLibraryPacket(GameManager.instance.data.GetLibraryList().libraryIDList[0]);
        //if (isCustom)
        //{
        //    GameManager.instance.playerRoomManager.GameStart();
        //}
        //else if (isOwner)
        //{
        //    if (isFindingMatch) CancelFindMatch();
        //    else FindMatch();
        //}
        //else
        //{
        //    if (isFindingMatch) CancelReady();
        //    else Ready();
        //}
        GameManager.instance.playerRoomManager.GameStart();
    }

    public void Watch()
    {
        GameManager.instance.playerRoomManager.Watch(isWatch);
    }

    public void SwitchTeam()
    {
        GameManager.instance.playerRoomManager.SwitchTeam();
    }

    public void QuestionPick()
    {
        GameManager.instance.playerRoomManager.QuestionPick();
    }

    public void Exit()
    {
        if (isFindingMatch) CancelFindMatch();
        else GameManager.instance.playerRoomManager.Exit();
    }
}
