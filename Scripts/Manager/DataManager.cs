using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    GameManager gameManager;
    FirebaseDatabaseManager firebase;

    public void SetUp(GameManager gameManager, FirebaseDatabaseManager dataManager)
    {
        this.gameManager = gameManager;
        this.firebase = dataManager;
    }

    [SerializeField] private CharacterDataStruct character;
    private UnityEvent<CharacterDataStruct> onMyCharacterDataChange = new UnityEvent<CharacterDataStruct>();
    public void SetCharacter(CharacterDataStruct character)
    {
        this.character = character;
        player.name = character.playerName;
        onMyCharacterDataChange.Invoke(character);
    }
    public CharacterDataStruct GetCharacter()
    {
        return character;
    }
    public void OnMyCharacterDataChangeAddListener(UnityAction<CharacterDataStruct> action)
    {
        onMyCharacterDataChange.RemoveListener(action);
        onMyCharacterDataChange.AddListener(action);
    }


    [SerializeField] private PlayerStatus playerStatus;
    private UnityEvent<PlayerStatus> onMyPlayerStatusChange = new UnityEvent<PlayerStatus>();
    public void SetPlayerStatus(PlayerStatus playerStatus)
    {
        this.playerStatus = playerStatus;
        player.playerStatus = playerStatus;
        onMyPlayerStatusChange.Invoke(playerStatus);
    }
    public PlayerStatus GetPlayerStatus()
    {
        return playerStatus;
    }
    public void OnMyPlayerStatusChangeAddListener(UnityAction<PlayerStatus> action)
    {
        onMyPlayerStatusChange.RemoveListener(action);
        onMyPlayerStatusChange.AddListener(action);
    }


    [SerializeField] private RoomIDStruct roomIDStruct;
    private UnityEvent onMyRoomIDStructChange = new UnityEvent();
    public void SetRoomIDStruct(RoomIDStruct roomIDStruct)
    {
        this.roomIDStruct = roomIDStruct;
        onMyRoomIDStructChange.Invoke();
    }
    public RoomIDStruct GetRoomIDStruct()
    {
        return roomIDStruct;
    }
    public void OnMyRoomIDStructChangeAddListener(UnityAction action)
    {
        onMyRoomIDStructChange.RemoveListener(action);
        onMyRoomIDStructChange.AddListener(action);
    }

    private List<RoomPlayerStruct> roomPlayerStructList;
    private UnityEvent onRoomPlayerStructListChange = new UnityEvent();
    public void SetRoomPlayerStruct(List<RoomPlayerStruct> playerRoomStructList)
    {
        this.roomPlayerStructList = playerRoomStructList;
        onRoomPlayerStructListChange.Invoke();
    }
    public List<RoomPlayerStruct> GetRoomPlayerStructList()
    {
        return roomPlayerStructList;
    }
    public void OnRoomPlayerStructListChangeAddListener(UnityAction action)
    {
        onRoomPlayerStructListChange.RemoveListener(action);
        onRoomPlayerStructListChange.AddListener(action);
    }

    [SerializeField]
    private LibraryListStruct libraryList;
    private UnityEvent onLibraryListChange = new UnityEvent();
    public void LibraryListReset()
    {
        onLibraryListChange.Invoke();
    }
    public void SetLibraryList(LibraryListStruct libraryList)
    {
        this.libraryList = libraryList;
        onLibraryListChange.Invoke();
    }
    public LibraryListStruct GetLibraryList()
    {
        return libraryList;
    }
    public void OnLibraryListChangeAddListener(UnityAction action)
    {
        onLibraryListChange.RemoveListener(action);
        onLibraryListChange.AddListener(action);
    }

    [SerializeField]
    private LibraryPacket libraryPacket;
    private UnityEvent onLibraryPacketChange = new UnityEvent();
    public void SetLibraryPacket(LibraryPacket libraryPacket)
    {
        this.libraryPacket = libraryPacket;
        onLibraryPacketChange.Invoke();
    }
    public LibraryPacket GetLibraryPacket()
    {
        return libraryPacket;
    }
    public void OnLibraryPacketChangeAddListener(UnityAction action)
    {
        onLibraryPacketChange.RemoveListener(action);
        onLibraryPacketChange.AddListener(action);
    }

    private int libraryQuestionID;
    private UnityEvent onLibraryQuestionIDChange = new UnityEvent();
    public void SetLibraryQuestionID(int libraryQuestionID)
    {
        this.libraryQuestionID = libraryQuestionID;
        onLibraryQuestionIDChange.Invoke();
    }
    public int GetLibraryQuestionID()
    {
        return libraryQuestionID;
    }
    public void OnLibraryQuestionIDChangeAddListener(UnityAction action)
    {
        onLibraryQuestionIDChange.RemoveListener(action);
        onLibraryQuestionIDChange.AddListener(action);
    }

    public bool isLibraryInMatch = false;
    public bool isOnLibrary = false;

    public void UpdateLibrary(LibraryPacket libraryPacket)
    {
        bool isAlreadyHave = false;
        if (libraryList.libraryPacketInfos == null) libraryList.libraryPacketInfos = new List<LibraryPacketInfo>();
        foreach (LibraryPacketInfo packet in libraryList.libraryPacketInfos)
        {
            if(packet.libraryID == libraryPacket.libraryID)
            {
                isAlreadyHave = true;
                break;
            }
        }
        if (!isAlreadyHave)
        {
            libraryList.libraryPacketInfos.Add(new LibraryPacketInfo(libraryPacket.libraryID, libraryPacket.libraryName));
            libraryList.createdPacketCount++;
            firebase.SetMyLibraryList(libraryList);
        }
        firebase.SetLibraryPacket(libraryPacket);
        SetLibraryPacket(libraryPacket);
    }

    public RankStruct roomRank;
    public RoomPlayerStruct player;
    public List<GameCardStruct> cardStructList = new List<GameCardStruct>();

    public string matchRoomID;


    //public void OnMatchChangeAddListener(UnityAction<MatchStruct> action)
    //{
    //    onMatchChange.RemoveListener(action);
    //    onMatchChange.AddListener(action);
    //}


    public void OnDestroy()
    {
        onMyCharacterDataChange.RemoveAllListeners();
        onMyPlayerStatusChange.RemoveAllListeners();
        onMyRoomIDStructChange.RemoveAllListeners();
        onRoomPlayerStructListChange.RemoveAllListeners();
        onLibraryListChange.RemoveAllListeners();
        onLibraryPacketChange.RemoveAllListeners();
        onLibraryQuestionIDChange.RemoveAllListeners();
    }
}
