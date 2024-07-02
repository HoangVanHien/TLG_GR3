//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class OldMatchManager : MonoBehaviour
//{
//    private FirebaseDatabaseManager firebaseDatabase;

//    private DataManager data;
//    private ScreenManager screenManager;
//    private PlayerRoomManager playerRoomManager;

//    public MatchStruct matchStruct;
//    public MatchAnnouceStruct matchAnnouce;
//    public string currentPlayer;

//    public float timRemain;
//    public float myTimRemain = 30;
//    public float otherTimRemain = 50;

//    public bool isOwner = false;
//    public bool isPlaying = false;

//    public List<int> cardsOnHand;
//    private int maxCard = 5;
//    private int randomNum = 0;

//    private UnityEvent onMatchStructChange = new UnityEvent();

//    private int t1, t2;

//    private bool isDoneUpdate = false;
//    public bool isAnswerRight = false;
//    public bool isAnswerYet = false;

//    public QuestionUI questionUI;

//    public void SetUp(FirebaseDatabaseManager firebaseDatabase, DataManager data, ScreenManager screenManager, PlayerRoomManager playerRoomManager)
//    {
//        this.firebaseDatabase = firebaseDatabase;
//        this.data = data;
//        this.screenManager = screenManager;
//        this.playerRoomManager = playerRoomManager;
//        isDoneUpdate = true;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (!isDoneUpdate) return;
//        randomNum = Random.Range(0, data.cardStructList.Count);
//        timRemain -= Time.deltaTime;
//        if (timRemain <= 0)
//        {
//            NextPlayer();
//        }
//    }

//    private void OnDestroy()
//    {
//        onMatchStructChange.RemoveAllListeners();
//    }

//    public void OnMatchStructChangeAddListener(UnityAction action)
//    {
//        onMatchStructChange.AddListener(action);
//    }

//    public void AnnouceMatch(MatchAnnouceStruct matchAnnouceStruct)
//    {
//        matchAnnouce = matchAnnouceStruct;
//        firebaseDatabase.MatchListenerRegistration(matchAnnouce.matchID);
//    }

//    public void OnMatchUpdate(MatchStruct matchStruct)
//    {
//        this.matchStruct = matchStruct;
//        if (matchStruct.playingPlayerID == "")
//        {
//            PickACard();
//            PickACard();
//            PickACard();
//            if (!isOwner) return;
//            t1 = t2 = 0;
//            firebaseDatabase.UpdateMatchPlayingPlayer(matchAnnouce.matchID, matchStruct.roomPlayers[t1++].userID);
//        }
//        if (matchStruct.playingPlayerID == data.player.userID)
//        {
//            isPlaying = true;
//            timRemain = myTimRemain;
//            PickACard();
//        }
//        else
//        {
//            isPlaying = false;
//            timRemain = otherTimRemain;
//        }
//        onMatchStructChange.Invoke();
//    }

//    private void NextPlayer()
//    {
//        if (matchStruct.roomPlayers == null || matchStruct.roomPlayers.Count <= 0) return;
//        if (t2 < t1) firebaseDatabase.UpdateMatchPlayingPlayer(matchAnnouce.matchID, matchStruct.team2[t2++].userID);
//        else if (t1 < matchStruct.roomPlayers.Count) firebaseDatabase.UpdateMatchPlayingPlayer(matchAnnouce.matchID, matchStruct.roomPlayers[t1++].userID);
//        else//next round
//        {
//            t1 = t2 = 0;
//            firebaseDatabase.UpdateMatchRound(matchAnnouce.matchID, matchStruct.round + 1);
//            firebaseDatabase.UpdateMatchPlayingPlayer(matchAnnouce.matchID, matchStruct.roomPlayers[t1++].userID);
//        }
//    }

//    public bool PlayCard(int playedCard, string[] targets)
//    {
//        if (!isPlaying) return false;
//        if (data.cardStructList[playedCard].cardMana > matchStruct.round) return false;
//        isAnswerYet = false;
//        isAnswerRight = false;
//        //questionUI.SetUp(data.questionPlayList[0]);
//        questionUI.gameObject.SetActive(true);
//        firebaseDatabase.PlayCard(matchAnnouce.matchID, playedCard, data.player.userID, targets);
//        return true;
//    }

//    public void PickACard()
//    {
//        if (cardsOnHand.Count < maxCard)
//        {
//            cardsOnHand.Add(randomNum);
//        }
//    }
//}
