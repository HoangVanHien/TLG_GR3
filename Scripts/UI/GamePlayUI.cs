using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GamePlayUI : MonoBehaviour
{
    private GameManager gameManager;
    private MatchManager matchManager;
    private MatchAnnouceStruct playerMatchID;
    private UnityAction<PlayerStatus> onMyPlayerStatusChange;

    public GameObject mainUI;

    private int maxPlayer = 5;

    [SerializeField] private List<MatchPlayerUI> enemyUIs;
    [SerializeField] private List<MatchPlayerUI> playerUIs;
    public List<CardUI> cardUIs = new List<CardUI>();


    public TMP_Text turnText;
    public TMP_Text timeText;
    public TMP_Text manaText;

    private bool isAddListener = false;
    private bool isMatchStructChange = false;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (!gameManager.isDoneSetUp) return;
        if (!isAddListener) {
            matchManager = gameManager.matchManager;
            gameManager.matchManager.OnMatchStructChangeAddListener(SetUpMatch);
            isAddListener = true;
        }
        if (isMatchStructChange) SetUpMatchInMain(gameManager.matchManager.matchStruct);
        timeText.text = "Time\n" + GameManager.instance.matchManager.timRemain;
    }

    private void SetUpMatch()
    {
        isMatchStructChange = true;
    }

    private void SetUpMatchInMain(MatchStruct matchStruct)
    {
        turnText.text = /*"Player: " + matchStruct.playingPlayerID +*/ "Round: " + matchStruct.round;
        

        //SetUpCharacters(matchStruct, playerMatchID.isTeam1);
        //SetUpCharacters(matchStruct.team2, !playerMatchID.isTeam1);
        //SetUpCards();
    }

    private void SetUpCharacters(List<MatchPersonalDataStruct> matchPersonalDatas, bool isMyTeam)
    {
        Debug.Log("begin set up character");
        for (int i = 0; i < maxPlayer; i++)
        {
            if (isMyTeam)
            {
                if (i < matchPersonalDatas.Count)
                {
                    playerUIs[i].SetUp(matchPersonalDatas[i].userID, matchPersonalDatas[i].CharacterData);
                    //MatchPlayerUI playerUI = Instantiate<MatchPlayerUI>(playerUIPrefab, playerSlot);
                    //playerUIs.Add(playerUI);
                }
                else playerUIs[i].Reset();
            }
            else
            {
                if (i < matchPersonalDatas.Count)
                {
                    enemyUIs[i].SetUp(matchPersonalDatas[i].userID, matchPersonalDatas[i].CharacterData);
                }
                else enemyUIs[i].Reset();
            }
        }
    }

    //private void SetUpCards()
    //{
    //    List<int> gameCards = GameManager.instance.matchManager.cardsOnHand;
    //    for (int i = 0; i < cardUIs.Count; i++)
    //    {
    //        if (i < gameCards.Count)
    //            cardUIs[i].SetUp(GameManager.instance.data.cardStructList[gameCards[i]]);
    //        else cardUIs[i].Reset();
    //    }
    //}


}
