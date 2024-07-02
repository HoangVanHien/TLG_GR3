using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchManager : MonoBehaviour
{
    GameManager gameManager;
    FirebaseDatabaseManager firebaseDatabase;
    DataManager data;
    ScreenManager screenManager;
    PlayerRoomManager playerRoomManager;

    public bool isOwner = false;
    public string matchID;

    public MatchPhaseEnum matchPhase;
    public float timRemain  = 0;
    public float deafaultRankTime = 1;
    public float deafaultQuestionTime = 10;

    public int score = 0;
    public int curQuestScore = 0;
    public int deafaultQuestionScore = 10;

    public List<int> rightAnswer = new List<int>();
    public string rightTypeIn = "";

    public MatchStruct matchStruct;
    public LibraryPacket libraryPacket;

    public List<MatchPlayer> matchPlayers;
    public int roomPlayersCount;

    private UnityEvent onMatchStructChange = new UnityEvent();
    private UnityEvent onShowRank = new UnityEvent();
    private UnityEvent onShowQuestion = new UnityEvent();
    private UnityEvent onEndGame = new UnityEvent();

    public void SetUp(GameManager gameManager, FirebaseDatabaseManager firebaseDatabase, DataManager data, ScreenManager screenManager, PlayerRoomManager playerRoomManager)
    {
        this.gameManager = gameManager;
        this.firebaseDatabase = firebaseDatabase;
        this.data = data;
        this.screenManager = screenManager;
        this.playerRoomManager = playerRoomManager;
        //isDoneUpdate = true;
    }

    private void FixedUpdate()
    {
        if(matchPhase == MatchPhaseEnum.end)
        {
            return;
        }
        if (matchPhase == MatchPhaseEnum.question || matchPhase == MatchPhaseEnum.rank) timRemain -= Time.fixedDeltaTime;
        if (timRemain <= 0)
        {
            //change phase
            if(matchPhase == MatchPhaseEnum.question)//dont answer
            {
                matchPhase = MatchPhaseEnum.rank;
                timRemain = deafaultRankTime;
                ShowRank();
            }
            else if(isOwner && matchPhase == MatchPhaseEnum.rank)
            {
                NextQuestion();
            }
        }
    }

    public void OnMatchAnnouce(MatchAnnouceStruct matchAnnouce)
    {
        Debug.Log("Match begin");
        matchStruct = new MatchStruct(null);
        matchID = matchAnnouce.matchID;
        libraryPacket = data.GetLibraryPacket();
        firebaseDatabase.MatchListenerRegistration(matchID);
        firebaseDatabase.MyMatchPlayersListenerRegistration(matchID);
        data.isLibraryInMatch = true;
        score = 0;
        curQuestScore = 0;
        matchPhase = MatchPhaseEnum.beforeMatch;

        ShowRank();
        screenManager.BringUp(ScreenEnum.GamePlay);
    }

    private void NextQuestion()
    {
        if (!isOwner) return;
        Debug.Log("Set question to " + matchStruct.curLibraryQuestion + 1);
        firebaseDatabase.SetMatch(matchID, new MatchStruct(matchStruct.libraryPacketID, curLibraryQuestion: matchStruct.curLibraryQuestion + 1));
    }

    public void AnswerQuestion(bool isRight)
    {
        if (isRight) score += curQuestScore + (int) timRemain;
        else score -= curQuestScore;
        firebaseDatabase.SetMatchPlayer(matchID, data.player.userID, new MatchPlayer(score, data.player.name));
    }

    public void OnMatchUpdate(MatchStruct newMatchStruct)
    {
        Debug.Log("Match Update: " + newMatchStruct.libraryPacketID + " " + newMatchStruct.curLibraryQuestion);
        Debug.Log("Match From: " + matchStruct.libraryPacketID + " " + matchStruct.curLibraryQuestion);
        if(libraryPacket.libraryID != newMatchStruct.libraryPacketID)
        {
            Debug.Log("Not fit library in match");
            firebaseDatabase.GetLibraryPacket(newMatchStruct.libraryPacketID, () => {
                libraryPacket = data.GetLibraryPacket();
                firebaseDatabase.SetMatchPlayer(matchID, data.player.userID, new MatchPlayer(0, data.player.name));
            });
        }
        else
        {
            firebaseDatabase.SetMatchPlayer(matchID, data.player.userID, new MatchPlayer(0, data.player.name));
        }
        if(newMatchStruct.curLibraryQuestion > matchStruct.curLibraryQuestion)
        {
            if (newMatchStruct.curLibraryQuestion >= libraryPacket.libraryQuestions.Count)
            {
                //End game
                Debug.Log("End game");
                OnEndGame();
            }
            else
            {
                switch (libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].questionMode)
                {
                    case QuestionModeEnum.oneRightAnswer:
                        rightAnswer = libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].isRightAnswerIndexList;
                        break;
                    case QuestionModeEnum.multipleRightAnswer:
                        rightAnswer = libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].isRightAnswerIndexList;
                        break;
                    case QuestionModeEnum.typeIn:
                        rightTypeIn = libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].answers[0];
                        break;
                    default:
                        break;
                }
                matchStruct = newMatchStruct;
                onMatchStructChange.Invoke();
                ShowQuestion();
                timRemain = libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].time;
                if (timRemain <= 0) timRemain = deafaultQuestionTime;
                curQuestScore = libraryPacket.libraryQuestions[newMatchStruct.curLibraryQuestion].score;
                if (curQuestScore <= 0) curQuestScore = deafaultQuestionScore;
                matchPhase = MatchPhaseEnum.question;
            }
        }

        matchStruct = newMatchStruct;
    }

    public void OnMatchPlayersUpdate(List<MatchPlayer> matchPlayers)
    {
        this.matchPlayers = matchPlayers;
        if(matchPhase == MatchPhaseEnum.beforeMatch || matchPhase == MatchPhaseEnum.rank)
        {
            ShowRank();
        }
        if (isOwner && matchPhase == MatchPhaseEnum.beforeMatch)
        {
            if (matchPlayers.Count == data.GetRoomPlayerStructList().Count)
            {
                //game start
                Debug.Log("Game Start");
                NextQuestion();
            }
        }
    }

    public void OnMatchStructChangeAddListener(UnityAction action)
    {
        onMatchStructChange.AddListener(action);
    }
    public void OnShowRankAddListener(UnityAction action)
    {
        onShowRank.AddListener(action);
    }
    public void OnShowQuestion(UnityAction action)
    {
        onShowQuestion.AddListener(action);
    }
    public void OnEndGame(UnityAction action)
    {
        onEndGame.AddListener(action);
    }
    private void OnDestroy()
    {
        onMatchStructChange.RemoveAllListeners();
        onShowRank.RemoveAllListeners();
        onShowQuestion.RemoveAllListeners();
        onEndGame.RemoveAllListeners();
    }

    public void SetMatchPlayers(List<MatchPlayer> matchPlayers)
    {
        this.matchPlayers = matchPlayers;
    }

    public void ShowRank()
    {
        onShowRank.Invoke();
    }
    public void ShowQuestion()
    {
        onShowQuestion.Invoke();
    }

    public void OnEndGame()
    {
        matchPhase = MatchPhaseEnum.end;
        onEndGame.Invoke();
        timRemain = 0;
    }

    public void ExitMatch()
    {
        if (matchPlayers.Count == 1)
        {
            firebaseDatabase.DeleteMatch(matchID);
            firebaseDatabase.SetMatchAnnouce(new List<string>{ matchID}, "");
        }
        firebaseDatabase.DeleteMatchPlayer(matchID, gameManager.data.player.userID);
    }
}
