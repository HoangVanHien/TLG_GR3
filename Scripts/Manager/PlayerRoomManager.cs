using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomManager : MonoBehaviour
{
    private FirebaseDatabaseManager firebaseDatabase;

    private DataManager data;
    private ScreenManager screenManager;
    private MatchManager matchManager;

    [SerializeField] private int botCount = 0;

    public void SetUp(FirebaseDatabaseManager firebaseDatabase, DataManager data, ScreenManager screenManager, MatchManager matchManager)
    {
        this.firebaseDatabase = firebaseDatabase;
        this.data = data;
        this.screenManager = screenManager;
        this.matchManager = matchManager;

        data.OnRoomPlayerStructListChangeAddListener(OnRoomPlayersChange);
    }

    public void CreateMyRoom(GameModeEnum gameMode)
    {
        screenManager.Loading();
        int[] randNum = { Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10) };

        string roomID = (
            data.player.userID[randNum[0]].ToString() + (randNum[0]).ToString() +
            data.player.userID[randNum[1]].ToString() + (randNum[1]).ToString() +
            data.player.userID[randNum[2]].ToString() + (randNum[2]).ToString()).ToUpper();
        Debug.Log("create room " + roomID);
        data.isLibraryInMatch = true;
        firebaseDatabase.CreatePlayerRoom(roomID, gameMode);
        SelfJoinRoom(roomID);
    }

    public void SelfJoinRoomWithCheck(string roomID)
    {
        screenManager.Loading();
        firebaseDatabase.GetPlayerRoomInfo(roomID, () =>
        {
            RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
            if (roomIDStruct.roomID == "")
            {
                screenManager.Announce("Room not found");
                return;
            }
            int maxPlayer = (roomIDStruct.gameMode == GameModeEnum.custom ? 10 : ((int)roomIDStruct.gameMode + 1));
            Debug.Log("Check before join: " + roomIDStruct.roomPlayerCount + " " + maxPlayer);
            if (roomIDStruct.roomPlayerCount >= maxPlayer)
            {
                screenManager.Announce("Room is full");
                return;
            }
            SelfJoinRoom(roomID);

        });
    }

    public void SelfJoinRoom(string roomID)
    {
        Debug.Log("Join room " + roomID);
        firebaseDatabase.MyPlayerRoomListenerRegistration(roomID);
        firebaseDatabase.MyRoomPlayersListenerRegistration(roomID);
        firebaseDatabase.JoinPlayerRoom(roomID, data.player);
        firebaseDatabase.MatchAnnouceListenerRegistration(roomID);
        screenManager.BringUp(ScreenEnum.PlayerRoom);
    }

    public void AddBot()
    {
        firebaseDatabase.JoinPlayerRoom(data.GetRoomIDStruct().roomID, new RoomPlayerStruct("bot" + botCount++));
    }

    private void OnRoomPlayersChange()
    {
        List<RoomPlayerStruct> playerRoomStructList = data.GetRoomPlayerStructList();
        if(playerRoomStructList.Count <= 0)
        {
            data.roomRank = new RankStruct(0, 0);
            return;
        }
        if (playerRoomStructList[0].userID != data.player.userID) return;
        RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
        if(roomIDStruct.ownerID != playerRoomStructList[0].userID)//Update Owner
        {
            firebaseDatabase.UpdatePlayerRoomOwner(roomIDStruct.roomID, playerRoomStructList[0].userID);
            firebaseDatabase.UpdateRoomPlayerStatus(roomIDStruct.roomID, playerRoomStructList[0].userID, PlayerStatus.online);
        }
        firebaseDatabase.UpdateRoomPlayerCount(roomIDStruct.roomID, playerRoomStructList.Count);//Update Player Count
        RoomRankCal(playerRoomStructList);
    }

    private void RoomRankCal(List<RoomPlayerStruct> playerRoomStructList)
    {
        RankStruct roomRank = new RankStruct(0, 0);
        foreach (RoomPlayerStruct player in playerRoomStructList)
        {
            roomRank.rank += player.rankStruct.rank;
            roomRank.correctRate += player.rankStruct.correctRate;
        }
        roomRank.rank /= playerRoomStructList.Count;
        roomRank.correctRate /= playerRoomStructList.Count;
        data.roomRank = roomRank;
    }

    public void QuestionPick()
    {
        screenManager.Loading();
        data.LibraryListReset();
        screenManager.BringUp(ScreenEnum.Library);
    }

    public void RoomPlayerKick(string userID)
    {
        firebaseDatabase.DeleteRoomPlayer(data.GetRoomIDStruct().roomID, userID);
    }

    public void SwitchTeam()
    {
        //firebaseDatabase.UpdateRoomPlayerTeam(data.GetRoomIDStruct().roomID, data.player.userID);
    }

    public void Watch(bool isWatch)
    {
        screenManager.Loading();
        string roomID = data.GetRoomIDStruct().roomID;
        string userID = data.player.userID;
        if (isWatch)
        {
            firebaseDatabase.JoinPlayerRoom(roomID, data.player);
            firebaseDatabase.DeleteRoomPlayerWatch(roomID, data.player.userID);
        }
        else
        {
            firebaseDatabase.JoinPlayerRoomWatch(roomID, data.player);
            firebaseDatabase.DeleteRoomPlayer(roomID, data.player.userID);
        }
        screenManager.BringDown(ScreenEnum.Announcement);
    }

    public void Exit()
    {
        screenManager.Loading();
        string roomID = data.GetRoomIDStruct().roomID;
        firebaseDatabase.DeleteRoomPlayer(roomID, data.player.userID);
        firebaseDatabase.DeleteRoomPlayerWatch(roomID, data.player.userID);
        if (data.GetRoomPlayerStructList().Count <= 1)
        {
            firebaseDatabase.DeletePlayerRoom(roomID);
            firebaseDatabase.DeleteMatchAnnouce(roomID);
        }
        screenManager.BringDown(ScreenEnum.PlayerRoom);
    }

    public void Ready()
    {
        firebaseDatabase.UpdateRoomPlayerStatus(data.GetRoomIDStruct().roomID, data.player.userID, PlayerStatus.online);
    }

    public void CancelReady()
    {
        firebaseDatabase.UpdateRoomPlayerStatus(data.GetRoomIDStruct().roomID, data.player.userID, PlayerStatus.online);
    }

    public void FindMatch()
    {
        firebaseDatabase.GetFindingList(GroupOfRooms);

        firebaseDatabase.UpLoadToFindingList();
    }
    public void CancelFindMatch()
    {
        firebaseDatabase.GetFindingList(GroupOfRooms);

        firebaseDatabase.UpLoadToFindingList();
    }

    public MatchPersonalDataStruct CreateBot(string botID, string botName)
    {
        return new MatchPersonalDataStruct(new CharacterDataStruct(botName, 100, 20, 0, 10), botID);
    }

    public void GameStart()
    {
        screenManager.Loading();
        LibraryPacket libraryPacket = data.GetLibraryPacket();
        if(libraryPacket.libraryID == null)
        {
            screenManager.Announce("Please pick questions from library first.");
            return;
        }
        matchManager.isOwner = true;
        matchManager.roomPlayersCount = data.GetRoomPlayerStructList().Count;
        string matchID = data.GetRoomIDStruct().roomID;
        //matchManager.matchAnnouce = new(matchID, true);
        //List<MatchPersonalDataStruct> team1 = new List<MatchPersonalDataStruct>();
        //List<MatchPersonalDataStruct> team2 = new List<MatchPersonalDataStruct>();
        //foreach (RoomPlayerStruct player in data.GetRoomPlayerStructList())
        //{
        //    if (player.i) team1.Add(CreateBot(player.userID, player.name));
        //    else team2.Add(CreateBot(player.userID, player.name));

        //}
        //MatchStruct match = new MatchStruct("", 1, team1, team2);

        MatchStruct match = new MatchStruct(libraryPacket.libraryID);
        firebaseDatabase.SetMatch(matchID, match);
        firebaseDatabase.SetMatchAnnouce(new List<string> { matchID }, matchID);
    }


    private void GroupOfRooms(List<FindingMatchRoomStruct> findingMatchRoomStructs)
    {
        RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
        int maxPlayer = (int)roomIDStruct.gameMode + 1;
        List<FindingMatchRoomStruct> finalGroup = new List<FindingMatchRoomStruct> { new FindingMatchRoomStruct(roomIDStruct.roomID, roomIDStruct.roomPlayerCount, data.roomRank, roomIDStruct.gameMode) };
        finalGroup = RecursiveGroupOfRooms(maxPlayer, roomIDStruct.roomPlayerCount, 0, finalGroup, findingMatchRoomStructs);
        if (finalGroup == null) firebaseDatabase.UpLoadToFindingList();
    }

    private List<FindingMatchRoomStruct> RecursiveGroupOfRooms(int maxPlayer, int team1, int team2, List<FindingMatchRoomStruct> ourGroup, List<FindingMatchRoomStruct> list)
    {
        List<FindingMatchRoomStruct> curGroup = ourGroup;
        foreach (FindingMatchRoomStruct room in list)
        {
            if (curGroup.Contains(room)) continue;

            if (room.playerCount + team1 <= maxPlayer)
            {
                curGroup.Add(room);
                team1 += room.playerCount;
                if (team1 + team2 == 2 * maxPlayer)
                {
                    return curGroup;
                }
                ourGroup = RecursiveGroupOfRooms(maxPlayer, team1, team2, curGroup, list);
                if (ourGroup != null) return ourGroup;
            }
            else if (room.playerCount + team2 <= maxPlayer)
            {
                curGroup.Add(room);
                team2 += room.playerCount;
                if (team1 + team2 == 2 * maxPlayer)
                {
                    return curGroup;
                }
                ourGroup = RecursiveGroupOfRooms(maxPlayer, team1, team2, curGroup, list);
                if (ourGroup != null) return ourGroup;
            }
        }
        return null;
    }
}
