using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataPath
{
    public static string playerCharacterData = "characterData/";
    public static string playerStatus = "playerStatus/";
    public static string playerRank = "playerRank/";
    public static string gameCard = "gameCard/";
    public static string library = "library/";
    public static string libraryList = "libraryList/";
    public static string question = "question/";
    public static string room = "room/";
    public static string roomPlayer = "roomPlayer/";
    public static string roomPlayerWatch = "roomPlayerWatch/";
    public static string findingList = "findingList/";
    public static string requestMatch = "requestMatch/";
    public static string makingMatch = "findingMatch/makingMatch";
    public static string match = "match/";
    public static string matchPlayer = "matchPlayer/";
    public static string annouceMatchID = "annouceMatchID/";
    public static string playCard = "playCard/";

    public static string timeStamp = "timeStamp/";

}

public static class DataValue
{
    public static string userID = "userID";
    public static string ownerID = "ownerID";
    public static string timeStamp = "timeStamp";
    public static string playerStatus = "playerStatus";
    public static string roomPlayerCount = "roomPlayerCount";

    public static string playingPlayerID = "playingPlayerID";
    public static string round = "round";
    public static string score = "score";

}

public enum PlayerStatus
{
    online = 0,
    offline,
    findingMatch,
    inMatch,
    count,
};

public enum CharacterDataEnum
{
    health = 0,
    ATK,
    DEF,
    shield,
    count,
}

public enum GameModeEnum
{
    m1v1 = 0,
    m2v2,
    m3v3,
    m4v4,
    m5v5,
    custom,
    count,
}

public enum LibraryQuestionModeEnum
{
    question = 0,
    word,
    count,
}

public enum QuestionModeEnum
{
    oneRightAnswer = 0,
    multipleRightAnswer,
    typeIn,
    count,
}

public enum MatchPhaseEnum
{
    beforeMatch = 0,
    question,
    rank,
    end,
    count,
}

[System.Serializable]
[FirestoreData]
public struct CharacterDataStruct
{
    [FirestoreProperty]
    public string playerName { get; set; }
    [FirestoreProperty]
    public int health { get; set; }
    [FirestoreProperty]
    public int ATK { get; set; }
    [FirestoreProperty]
    public int DEF { get; set; }
    [FirestoreProperty]
    public int shield { get; set; }

    public CharacterDataStruct(string playerName, int health, int ATK, int shield, int DEF)
    {
        this.playerName = playerName;
        this.health = health;
        this.ATK = ATK;
        this.DEF = DEF;
        this.shield = shield;
    }
}

[System.Serializable]
[FirestoreData]
public struct PlayerStatusStruct
{
    [FirestoreProperty]
    public PlayerStatus playerStatus { get; set; }

    public PlayerStatusStruct(PlayerStatus playerStatus)
    {
        this.playerStatus = playerStatus;
    }
}

[System.Serializable]
[FirestoreData]
public struct RankStruct
{
    [FirestoreProperty]
    public int rank { get; set; }
    [FirestoreProperty]
    public float correctRate { get; set; }

    public RankStruct(int rank, float correctRate)
    {
        this.rank = rank;
        this.correctRate = correctRate;
    }
}

[System.Serializable]
[FirestoreData]
public struct PlayerFindingMatchStruct
{
    [FirestoreProperty]
    public string userID { get; set; }
    [FirestoreProperty]
    public PlayerStatus playerStatus { get; set; }
    [FirestoreProperty]
    public RankStruct rankStruct { get; set; }

    public PlayerFindingMatchStruct(string userID, PlayerStatus playerStatus, RankStruct rankStruct)
    {
        this.userID = userID;
        this.playerStatus = playerStatus;
        this.rankStruct = rankStruct;
    }
}

[FirestoreData]
[System.Serializable]
public struct RoomPlayerStruct
{
    [FirestoreProperty]
    public string userID { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public PlayerStatus playerStatus { get; set; }
    [FirestoreProperty]
    public RankStruct rankStruct { get; set; }

    public RoomPlayerStruct(string userID, string name, PlayerStatus playerStatus, RankStruct rankStruct)
    {
        this.userID = userID;
        this.name = name;
        this.playerStatus = playerStatus;
        this.rankStruct = rankStruct;
    }

    public RoomPlayerStruct(string botID)
    {
        this.userID = botID;
        this.name = botID;
        this.playerStatus = PlayerStatus.online;
        this.rankStruct = new RankStruct();
    }
}

[System.Serializable]
[FirestoreData]
public struct RoomIDStruct
{
    [FirestoreProperty]
    public string roomID { get; set; }
    [FirestoreProperty]
    public string createrID { get; set; }
    [FirestoreProperty]
    public string ownerID { get; set; }
    [FirestoreProperty]
    public GameModeEnum gameMode { get; set; }
    [FirestoreProperty]
    public int roomPlayerCount { get; set; }

    public RoomIDStruct(string roomID, string createrID, string ownerID, GameModeEnum gameMode, int roomPlayerCount)
    {
        this.roomID = roomID;
        this.createrID = createrID;
        this.ownerID = ownerID;
        this.gameMode = gameMode;
        this.roomPlayerCount = roomPlayerCount;
    }
}

[System.Serializable]
[FirestoreData]
public struct FindingMatchRoomStruct
{
    [FirestoreProperty]
    public string roomID { get; set; }
    [FirestoreProperty]
    public int playerCount { get; set; }
    [FirestoreProperty]
    public RankStruct rank { get; set; }
    [FirestoreProperty]
    public GameModeEnum gameMode { get; set; }

    public FindingMatchRoomStruct(string roomID, int playerCount, RankStruct rank, GameModeEnum gameMode)
    {
        this.roomID = roomID;
        this.playerCount = playerCount;
        this.rank = rank;
        this.gameMode = gameMode;
    }
}

[System.Serializable]
[FirestoreData]
public struct LibraryListStruct
{
    [FirestoreProperty]
    public int createdPacketCount { get; set; }
    [FirestoreProperty]
    public List<LibraryPacketInfo> libraryPacketInfos { get; set; }

    public LibraryListStruct(int createdPacketCount, List<LibraryPacketInfo> libraryPacketInfos = null)
    {
        this.createdPacketCount = createdPacketCount;
        this.libraryPacketInfos = libraryPacketInfos;
    }
}
[System.Serializable]
[FirestoreData]
public struct LibraryPacketInfo
{
    [FirestoreProperty]
    public string libraryID { get; set; }
    [FirestoreProperty]
    public string libraryName { get; set; }

    public LibraryPacketInfo(string libraryID, string libraryName = "")
    {
        this.libraryID = libraryID;
        this.libraryName = libraryName;
    }
}

[System.Serializable]
[FirestoreData]
public struct LibraryPacket
{
    [FirestoreProperty]
    public string libraryID { get; set; }
    [FirestoreProperty]
    public string libraryName { get; set; }
    [FirestoreProperty]
    public LibraryQuestionModeEnum modeEnum { get; set; }
    [FirestoreProperty]
    public List<LibraryQuestion> libraryQuestions { get; set; }
    [FirestoreProperty]
    public List<LibraryWord> libraryWords { get; set; }

    public LibraryPacket(string libraryID, string libraryName = "", LibraryQuestionModeEnum modeEnum = LibraryQuestionModeEnum.question, List<LibraryQuestion> libraryQuestions = null, List < LibraryWord > libraryWords = null)
    {
        this.libraryID = libraryID;
        this.libraryName = libraryName;
        this.modeEnum = modeEnum;
        this.libraryQuestions = libraryQuestions;
        this.libraryWords = libraryWords;
    }
}

[System.Serializable]
[FirestoreData]
public struct LibraryQuestion
{
    [FirestoreProperty]
    public string question { get; set; }
    [FirestoreProperty]
    public QuestionModeEnum questionMode { get; set; }
    [FirestoreProperty]
    public float time { get; set; }
    [FirestoreProperty]
    public int score { get; set; }
    [FirestoreProperty]
    public List<string> answers { get; set; }
    [FirestoreProperty]
    public List<int> isRightAnswerIndexList { get; set; }

    public LibraryQuestion(string question, QuestionModeEnum questionMode = QuestionModeEnum.oneRightAnswer, List<string> answers = null, List<int> isRightAnswerIndexList = null, float time = 10f, int score = 10)
    {
        this.question = question;
        this.questionMode = questionMode;
        this.time = time;
        this.score = score;
        this.answers = answers;
        this.isRightAnswerIndexList = isRightAnswerIndexList;
    }
}

[System.Serializable]
[FirestoreData]
public struct LibraryWord
{
    [FirestoreProperty]
    public string word { get; set; }
    [FirestoreProperty]
    public string meaning { get; set; }

    public LibraryWord(string word, string meaning)
    {
        this.word = word;
        this.meaning = meaning;
    }
}

[System.Serializable]
[FirestoreData]
public struct GameIndexInServerStruct
{
    public int gameID;
    public int gameIndex;

    public GameIndexInServerStruct(int gameID, int gameIndex)
    {
        this.gameID = gameID;
        this.gameIndex = gameIndex;
    }
}


public enum ActionType
{
    Action = 0,
    Turn,
    Round,
    Attack,
    Defend,
    HealthChange,
    HealthDecrease,
    HealthIncrease,
    Count,
}

public enum ActionOrderType
{
    Instant = 0,
    PreAction,
    AfterAction,
    Both,
    Count,
}

[System.Serializable]
[FirestoreData]
public struct CardActionType
{
    [FirestoreProperty]
    public ActionType actionType { get; set; }
    [FirestoreProperty]
    public ActionOrderType actionOrder { get; set; }

    public CardActionType(ActionType actionType, ActionOrderType actionOrder)
    {
        this.actionType = actionType;
        this.actionOrder = actionOrder;
    }
}

public enum ActionEventType
{
    TakeDamage = 0,
    Attack,
    Defend,
    Count,
}

[System.Serializable]
[FirestoreData]
public struct ActionEventCharacterInvoleStruct
{
    [FirestoreProperty]
    public float characterScaleNumber { get; set; }
    [FirestoreProperty]
    public CharacterDataEnum characterStat { get; set; }

    public ActionEventCharacterInvoleStruct(float characterScaleNumber, CharacterDataEnum characterSacle)
    {
        this.characterScaleNumber = characterScaleNumber;
        this.characterStat = characterSacle;
    }
}

[System.Serializable]
[FirestoreData]
public struct ActionEventStruct
{
    [FirestoreProperty]
    public int pureNumber { get; set; }
    [FirestoreProperty]
    public ActionEventCharacterInvoleStruct[] characterScales { get; set; }
    [FirestoreProperty]
    public ActionType[] actionTypes { get; set; }

    public ActionEventStruct(int pureNumber, ActionEventCharacterInvoleStruct[] characterScales, ActionType[] actionTypes)
    {
        this.pureNumber = pureNumber;
        this.characterScales = characterScales;
        this.actionTypes = actionTypes;
    }
}

[System.Serializable]
[FirestoreData]
public struct CardActionStruct
{
    [FirestoreProperty]
    public CardActionType[] cardActionTypes { get; set; }
    [FirestoreProperty]
    public ActionEventType actionEventType { get; set; }
    [FirestoreProperty]
    public ActionEventStruct actionEventData { get; set; }

    public CardActionStruct(CardActionType[] cardActionTypes, ActionEventType actionEventType, ActionEventStruct actionEventData)
    {
        this.cardActionTypes = cardActionTypes;
        this.actionEventType = actionEventType;
        this.actionEventData = actionEventData;
    }
}

[System.Serializable]
[FirestoreData]
public struct GameCardStruct
{
    [FirestoreProperty]
    public int cardID { get; set; }
    [FirestoreProperty]
    public int cardMana { get; set; }
    [FirestoreProperty]
    public CardActionStruct[] cardActions { get; set; }
    public GameCardStruct(int cardID, int cardMana, CardActionStruct[] cardActions)
    {
        this.cardID = cardID;
        this.cardMana = cardMana;
        this.cardActions = cardActions;
    }
}

[System.Serializable]
[FirestoreData]
public struct MatchStruct
{
    [FirestoreProperty]
    public string libraryPacketID { get; set; }
    [FirestoreProperty]
    public int curLibraryQuestion { get; set; }
    [FirestoreProperty]
    public int round { get; set; }

    public MatchStruct(string libraryPacketID, int curLibraryQuestion = -1, int round = -1)
    {
        this.libraryPacketID = libraryPacketID;
        this.curLibraryQuestion = curLibraryQuestion;
        this.round = round;
    }
}

[System.Serializable]
[FirestoreData]
public struct MatchPlayer
{
    [FirestoreProperty]
    public int score { get; set; }
    [FirestoreProperty]
    public string name { get; set; }

    public MatchPlayer(int score, string name)
    {
        this.score = score;
        this.name = name;
    }
}

[System.Serializable]
[FirestoreData]
public struct MatchPersonalDataStruct
{
    [FirestoreProperty]
    public CharacterDataStruct CharacterData { get; set; }
    [FirestoreProperty]
    public string userID { get; set; }

    public MatchPersonalDataStruct(CharacterDataStruct characterData, string userID)
    {
        this.CharacterData = characterData;
        this.userID = userID;
    }
}

[System.Serializable]
[FirestoreData]
public struct MatchAnnouceStruct
{
    [FirestoreProperty]
    public string matchID { get; set; }

    public MatchAnnouceStruct(string matchID)
    {
        this.matchID = matchID;
    }
}

[System.Serializable]
[FirestoreData]
public struct PlayCardStruct
{
    [FirestoreProperty]
    public int cardID { get; set; }
    [FirestoreProperty]
    public string owner { get; set; }
    [FirestoreProperty]
    public string[] targets { get; set; }

    public PlayCardStruct(int cardID, string owner, string[] targets)
    {
        this.cardID = cardID;
        this.owner = owner;
        this.targets = targets;
    }
}