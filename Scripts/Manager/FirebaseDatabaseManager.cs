using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FirebaseDatabaseManager : MonoBehaviour
{
    private GameManager gameManager;
    private FirebaseFirestore database;
    private DataManager data;

    private ListenerRegistration myCharacterDataListenerRegistration;

    private ListenerRegistration myPlayerStatusListenerRegistration;

    private ListenerRegistration myRankListenerRegistration;

    private ListenerRegistration myPlayerRoomListenerRegistration;

    private ListenerRegistration myRoomPlayersListenerRegistration;

    private ListenerRegistration matchListenerRegistration;

    private ListenerRegistration matchPlayersListenerRegistration;

    private ListenerRegistration matchAnnouceListenerRegistration;

    private ListenerRegistration libraryListListenerRegistration;


    public void InitializeFirebase(GameManager gameManager)
    {
        this.gameManager = gameManager;
        data = gameManager.data;
        database = FirebaseFirestore.DefaultInstance;
        Debug.Log("Done Database");
    }

    private void OnDestroy()
    {
        StopListen();
    }

    public void StopListen()
    {
        if (myCharacterDataListenerRegistration != null) myCharacterDataListenerRegistration.Stop();
        if (myPlayerStatusListenerRegistration != null) myPlayerStatusListenerRegistration.Stop();
        if (myRankListenerRegistration != null) myRankListenerRegistration.Stop();
        if (myPlayerRoomListenerRegistration != null) myPlayerRoomListenerRegistration.Stop();
        if (myRoomPlayersListenerRegistration != null) myRoomPlayersListenerRegistration.Stop();
        if (matchListenerRegistration != null) matchListenerRegistration.Stop();
        if (matchPlayersListenerRegistration != null) matchPlayersListenerRegistration.Stop();
        if (matchAnnouceListenerRegistration != null) matchAnnouceListenerRegistration.Stop();
        if (libraryListListenerRegistration != null) libraryListListenerRegistration.Stop();
    }

    public void MyCharacterDataListenerRegistration()
    {
        if (myCharacterDataListenerRegistration != null) myCharacterDataListenerRegistration.Stop();
        myCharacterDataListenerRegistration = database.Document(DataPath.playerCharacterData + data.player.userID).Listen(snapshot =>
        {
            if (!snapshot.Exists) return;
            data.SetCharacter(snapshot.ConvertTo<CharacterDataStruct>());
        });
    }

    public void MyPlayerStatusListenerRegistration()
    {
        if (myPlayerStatusListenerRegistration != null) myPlayerStatusListenerRegistration.Stop();
        myPlayerStatusListenerRegistration = database.Document(DataPath.playerStatus + data.player.userID).Listen(snapshot =>
        {
            if (!snapshot.Exists) return;
            data.SetPlayerStatus(snapshot.ConvertTo<PlayerStatusStruct>().playerStatus);
        });
    }

    public void MyRankListenerRegistration()
    {
        myRankListenerRegistration = database.Document(DataPath.playerRank + data.player.userID).Listen(snapshot =>
        {
            if (!snapshot.Exists) return;
            data.player.rankStruct = snapshot.ConvertTo<RankStruct>();
        });
    }

    public void MyPlayerRoomListenerRegistration(string roomID)
    {
        if (myPlayerRoomListenerRegistration != null) myPlayerRoomListenerRegistration.Stop();
        myPlayerRoomListenerRegistration = database.Document(DataPath.room + roomID).Listen(snapshot =>
        {
            if (!snapshot.Exists) return;
            data.SetRoomIDStruct(snapshot.ConvertTo<RoomIDStruct>());
        });
    }
    public void MyRoomPlayersListenerRegistration(string roomID)
    {
        if (myRoomPlayersListenerRegistration != null) myRoomPlayersListenerRegistration.Stop();
        myRoomPlayersListenerRegistration = database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayer).OrderBy(DataValue.timeStamp).Listen(snapshot =>
        {
            Debug.Log("Room player are " + snapshot.Count);
            if (snapshot.Count <= 0)
            {
                data.SetRoomPlayerStruct(new List<RoomPlayerStruct>());
                return;
            }
            List<RoomPlayerStruct> roomPlayerStructs = new List<RoomPlayerStruct>();
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                roomPlayerStructs.Add(documentSnapshot.ConvertTo<RoomPlayerStruct>());
            }
            data.SetRoomPlayerStruct(roomPlayerStructs);
        });
    }


    public void MatchAnnouceListenerRegistration(string listenID)
    {
        if (matchAnnouceListenerRegistration != null) matchAnnouceListenerRegistration.Stop();
        matchAnnouceListenerRegistration = database.Document(DataPath.annouceMatchID + listenID).Listen(snapshot =>
        {
            if (!snapshot.Exists)
            {
                Debug.Log("Match annouce not exist");
                return;
            }
            MatchAnnouceStruct matchAnnouce = snapshot.ConvertTo<MatchAnnouceStruct>();
            if(matchAnnouce.matchID == null || matchAnnouce.matchID.Length <6)
            {
                Debug.Log("matchID in match annouce not exist");
                return;
            }
            gameManager.matchManager.OnMatchAnnouce(matchAnnouce);
        });
    }

    public void MatchListenerRegistration(string matchID)
    {
        if (matchListenerRegistration != null) matchListenerRegistration.Stop();
        matchListenerRegistration = database.Document(DataPath.match + matchID).Listen(snapshot =>
        {
            if (!snapshot.Exists)
            {
                Debug.Log("Match not exist");
                return;
            }
            MatchStruct matchStruct = snapshot.ConvertTo<MatchStruct>();
            if(matchStruct.libraryPacketID == null || matchStruct.libraryPacketID.Length < 6)
            {
                Debug.Log("Match Library Packet ID Invalid");
                return;
            }
            gameManager.matchManager.OnMatchUpdate(matchStruct);
        });
    }
    public void MyMatchPlayersListenerRegistration(string matchID)
    {
        if (matchPlayersListenerRegistration != null) matchPlayersListenerRegistration.Stop();
        matchPlayersListenerRegistration = database.Document(DataPath.match + matchID).Collection(DataPath.matchPlayer).OrderByDescending(DataValue.score).Listen(snapshot =>
        {
            if (snapshot.Count <= 0)
            {
                Debug.Log("Match player not exist");
            }
            Debug.Log("Match players are " + snapshot.Count);
            List<MatchPlayer> matchPlayers = new List<MatchPlayer>();
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                matchPlayers.Add(documentSnapshot.ConvertTo<MatchPlayer>());
            }
            gameManager.matchManager.OnMatchPlayersUpdate(matchPlayers);
        });
    }

    public void LibraryListListenerRegistration(string userID)
    {
        if (libraryListListenerRegistration != null) libraryListListenerRegistration.Stop();
        libraryListListenerRegistration = database.Document(DataPath.libraryList + userID).Listen(snapshot =>
        {
            if (!snapshot.Exists)
            {
                Debug.Log("Library List not exist");
                return;
            }
            LibraryListStruct libraryList = snapshot.ConvertTo<LibraryListStruct>();
            data.SetLibraryList(libraryList);
        });
    }


    public void CreateNewPlayer(string userID, string name, string email, string playerName)
    {
        database.Document(DataPath.playerCharacterData + userID).SetAsync(new CharacterDataStruct(playerName, 100, 10, 0, 5));
        database.Document(DataPath.playerStatus + userID).SetAsync(new PlayerStatusStruct(PlayerStatus.offline));
        database.Document(DataPath.playerRank + userID).SetAsync(new RankStruct(1, 100));
        SetLibraryList(userID, new LibraryListStruct(0));
    }

    //public void GetCharacterData(string roomID, UnityAction action)
    //{
    //    database.Document(DataPath.room + roomID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //    {
    //        var snapshot = task.Result;
    //        if (snapshot.Exists)
    //        {
    //            Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
    //            data.SetRoomIDStruct(snapshot.ConvertTo<RoomIDStruct>());
    //            action();
    //        }
    //        else
    //        {
    //            gameManager.screenManager.Announce(String.Format("Room ID {0} does not exist!", snapshot.Id));
    //        }
    //    });
    //}

    public void SetPlayerStatus(string userID, PlayerStatus playerStatus)
    {
        database.Document(DataPath.playerStatus + userID).SetAsync(new PlayerStatusStruct(playerStatus));
    }

    public void CreatePlayerRoom(string roomID, GameModeEnum gameMode)
    {
        database.Document(DataPath.room + roomID).SetAsync(new RoomIDStruct(roomID, data.player.userID, data.player.userID, gameMode, 1));
    }

    public void DeletePlayerRoom(string roomID)
    {
        database.Document(DataPath.room + roomID).DeleteAsync();
    }

    public void UpdatePlayerRoomOwner(string roomID, string newOwner)
    {
        database.Document(DataPath.room + roomID).UpdateAsync(DataValue.ownerID, newOwner);
    }

    public void UpdateRoomPlayerCount(string roomID, int playerCount)
    {
        database.Document(DataPath.room + roomID).UpdateAsync(DataValue.roomPlayerCount, playerCount);
    }

    public void GetPlayerRoomInfo(string roomID, UnityAction action)
    {
        database.Document(DataPath.room + roomID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                data.SetRoomIDStruct(snapshot.ConvertTo<RoomIDStruct>());
                action();
            }
            else
            {
                gameManager.screenManager.Announce(String.Format("Room ID {0} does not exist!", snapshot.Id));
            }
        });
    }

    public void JoinPlayerRoom(string roomID, RoomPlayerStruct player)
    {
        DocumentReference docRef = database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayer).Document(player.userID);
        docRef.SetAsync(player);
        UpdateTimeStamp(docRef);
    }

    public void DeleteRoomPlayer(string roomID, string userID)
    {
        if(data.player.userID == userID)
        {
            if (matchAnnouceListenerRegistration != null) matchAnnouceListenerRegistration.Stop();
        }
        database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayer).Document(userID).DeleteAsync();
    }
    
    public void JoinPlayerRoomWatch(string roomID, RoomPlayerStruct player)
    {
        database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayerWatch).Document(player.userID).SetAsync(player);
    }

    public void DeleteRoomPlayerWatch(string roomID, string userID)
    {
        if (data.player.userID == userID)
        {
            if (matchAnnouceListenerRegistration != null) matchAnnouceListenerRegistration.Stop();
        }
        database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayerWatch).Document(userID).DeleteAsync();
    }

    public void DeleteMatchAnnouce(string listenID)
    {
        if (matchAnnouceListenerRegistration != null) matchAnnouceListenerRegistration.Stop();
        database.Document(DataPath.annouceMatchID + listenID).DeleteAsync();
    }


    public void UpdateRoomPlayerStatus(string roomID, string userID, PlayerStatus playerStatus)
    {
        database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayer).Document(userID).UpdateAsync(DataValue.playerStatus, playerStatus);
    }

    //public void UpdateRoomPlayerTeam(string roomID, string userID)
    //{
    //    database.Document(DataPath.room + roomID).Collection(DataPath.roomPlayer).Document(userID).UpdateAsync();
    //}

    public void UpLoadToFindingList()
    {
        RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
        DocumentReference fd = database.Document(DataPath.findingList + roomIDStruct.gameMode).Collection(data.roomRank.rank.ToString()).Document(roomIDStruct.roomID);
        fd.SetAsync(new FindingMatchRoomStruct(roomIDStruct.roomID, roomIDStruct.roomPlayerCount, new RankStruct(data.roomRank.rank, data.roomRank.correctRate), roomIDStruct.gameMode));
        UpdateTimeStamp(fd);
    }

    async public void CancelFindingMatch()
    {
        RoomIDStruct roomIDStruct = data.GetRoomIDStruct();
        DocumentReference player = database.Document(DataPath.findingList + roomIDStruct.gameMode).Collection(data.roomRank.rank.ToString()).Document(roomIDStruct.roomID);
        await player.DeleteAsync();
    }

    public void GetFindingList(UnityAction<List<FindingMatchRoomStruct>> action)
    {
        GameModeEnum gameMode = data.GetRoomIDStruct().gameMode;
        database.Document(DataPath.findingList + gameMode).Collection(data.roomRank.rank.ToString()).GetSnapshotAsync().ContinueWithOnMainThread(task =>
          {
              QuerySnapshot findingMatchQuerySnapshot = task.Result;
              List<FindingMatchRoomStruct> findingMatchRoomStructs = new List<FindingMatchRoomStruct>();
              foreach (DocumentSnapshot documentSnapshot in findingMatchQuerySnapshot.Documents)
              {
                  FindingMatchRoomStruct findingMatchStruct = documentSnapshot.ConvertTo<FindingMatchRoomStruct>();
                  findingMatchRoomStructs.Add(findingMatchStruct);
              }
              action(findingMatchRoomStructs);
          });
    }

    public void SetMatch(string matchID, MatchStruct match)
    {
        Debug.Log("Set match: " + match.libraryPacketID);
        database.Document(DataPath.match + matchID).SetAsync(match);
    }

    public void DeleteMatch(string matchID)
    {
        database.Document(DataPath.match + matchID).DeleteAsync();
    }

    public void SetMatchAnnouce(List<string> listenIDs, string matchID)
    {
        foreach (string listenID in listenIDs)
        {
            database.Document(DataPath.annouceMatchID + listenID).SetAsync(new MatchAnnouceStruct(matchID));
        }
    }

    public void SetMatchPlayer(string matchID, string playerID, MatchPlayer matchPlayer)
    {
        database.Document(DataPath.match + matchID).Collection(DataPath.matchPlayer).Document(playerID).SetAsync(matchPlayer);
    }

    public void DeleteMatchPlayer(string matchID, string playerID)
    {
        database.Document(DataPath.match + matchID).Collection(DataPath.matchPlayer).Document(playerID).DeleteAsync();
        matchListenerRegistration.Stop();
    }

    public void GetMatchPlayers(string matchID)
    {
        database.Document(DataPath.match + matchID).Collection(DataPath.matchPlayer).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            List<MatchPlayer> players = new List<MatchPlayer>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                players.Add(documentSnapshot.ConvertTo<MatchPlayer>());
            }
            gameManager.matchManager.SetMatchPlayers(players);
        }
        );
    }

    public void UpdateMatchPlayingPlayer(string matchID, string userID)
    {
        database.Document(DataPath.match + matchID).UpdateAsync(DataValue.playingPlayerID, userID);
    }

    public void UpdateMatchRound(string matchID, int round)
    {
        database.Document(DataPath.match + matchID).UpdateAsync(DataValue.round, round);
    }

    public void GetGameCardStruct(int cardID, UnityAction<GameCardStruct> action)
    {
        database.Document(DataPath.gameCard + cardID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Assert.IsNull(task.Exception);

            GameCardStruct gameCardStruct = task.Result.ConvertTo<GameCardStruct>();

            action(gameCardStruct);
        });
    }

    public void PlayCard(string matchID, int cardID, string owner, string[] targets)
    {
        database.Document(DataPath.match+matchID).Collection(DataPath.playCard).Document(owner).SetAsync(new PlayCardStruct(cardID, owner, targets));
    }

    public void CardGen()
    {
        for (int i = 0; i < 200; i++)
        {
            int rj = Random.Range(1, 1);
            List<CardActionStruct> cardActions = new List<CardActionStruct>();
            for (int j = 0; j < rj; j++)
            {
                int rk = Random.Range(1, 1);
                List<CardActionType> cardActionTypes = new List<CardActionType>();
                for (int k = 0; k < rk; k++)
                {
                    cardActionTypes.Add(new CardActionType(
                        ActionType.Attack,
                        ActionOrderType.Instant));
                }

                rk = Random.Range(1, 1);
                List<ActionEventCharacterInvoleStruct> characterScales = new List<ActionEventCharacterInvoleStruct>();
                for (int k = 0; k < rk; k++)
                {
                    characterScales.Add(new ActionEventCharacterInvoleStruct((int)Random.Range(0, 3), (CharacterDataEnum)Random.Range(0, (int)(CharacterDataEnum.count - 1))));
                }

                cardActions.Add(new CardActionStruct(
                    cardActionTypes.ToArray(),
                    ActionEventType.Attack,
                    new ActionEventStruct(Random.Range(0, 20), characterScales.ToArray(), new ActionType[] { ActionType.Attack })));
            }
            database.Document(DataPath.gameCard + i).SetAsync(new GameCardStruct(i, Random.Range(1, 10), cardActions.ToArray()));
        }
    }

    public void GetCards()
    {
        database.Collection(DataPath.gameCard).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            List<GameCardStruct> cards = new List<GameCardStruct>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                cards.Add(documentSnapshot.ConvertTo<GameCardStruct>());
            }
            data.cardStructList = cards;
        }
        );
    }

    public void RandomQuestGen()
    {
        DocumentReference quest = database.Document(DataPath.library + DataPath.question);
        for (int k = 0; k < 4; k++)
        {
            for (int i = 0; i < 20; i++)
            {
                int a = Random.Range(1, 200);
                int b = Random.Range(1, 200);
                int an = a + b;
                List<string> rAns = new List<string>() { an.ToString() };
                for (int j = 1; j < 4; j++)
                {
                    int plus = Random.Range(-20, 20);
                    rAns.Add((an + (plus != 0 ? plus : 1)).ToString());
                }
                quest.Collection("basic " + k).Document(i.ToString()).SetAsync(new LibraryQuestion((a + " + " + b).ToString(), QuestionModeEnum.oneRightAnswer, rAns, new List<int> { 0 }));
            }
        }
    }

    public void GetLibraryPacket(string libraryID, UnityAction postAction = null)
    {
        database.Document(DataPath.library + libraryID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                data.SetLibraryPacket((snapshot.ConvertTo<LibraryPacket>()));
                postAction();
            }
        }
        );
    }

    public void SetLibraryPacket(LibraryPacket libraryPacket)
    {
        database.Document(DataPath.library + libraryPacket.libraryID).SetAsync(libraryPacket).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated library packet " + libraryPacket.libraryID);
        }
        );
    }

    public void DeleteLibraryPacket(string libraryID)
    {
        database.Document(DataPath.library + libraryID).DeleteAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Deleted library packet " + libraryID);
        }
        );
    }

    public void SetLibraryList(string userID, LibraryListStruct libraryList)
    {
        database.Document(DataPath.libraryList + userID).SetAsync(libraryList).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated library list.");
        });
    }

    public void SetMyLibraryList(LibraryListStruct libraryList)
    {
        SetLibraryList(data.player.userID, libraryList);
    }

    public void UpdateTimeStamp(DocumentReference docRef)
    {
        Debug.Log("Update time stamp into " + docRef);
        docRef.UpdateAsync(DataValue.timeStamp, FieldValue.ServerTimestamp);
    }
}
