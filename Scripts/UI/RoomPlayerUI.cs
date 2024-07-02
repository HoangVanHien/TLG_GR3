using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerUI : MonoBehaviour
{
    private bool isTeam1;
    private string userID;
    private bool isBot = false;

    public TMP_Text nameText;
    public TMP_Text rankText;
    public TMP_Text correctRateText;
    public TMP_Text statusText;
    public Button bodyButton;
    public Button kickButton;

    private void Start()
    {
        bodyButton.onClick.AddListener(BodyButton);
        kickButton.onClick.AddListener(Kick);
    }

    public void SetupRoomPlayer(RoomPlayerStruct roomPlayer, bool isOwner)
    {
        isBot = roomPlayer.userID.Length < 10;
        this.userID = roomPlayer.userID;
        nameText.text = roomPlayer.name;
        rankText.text = "rank: " + roomPlayer.rankStruct.rank.ToString();
        correctRateText.text = "correct rate: " + roomPlayer.rankStruct.correctRate.ToString();
        statusText.text = roomPlayer.playerStatus.ToString();

        nameText.gameObject.SetActive(true);
        rankText.gameObject.SetActive(!isBot);
        correctRateText.gameObject.SetActive(!isBot);
        statusText.gameObject.SetActive(!isBot);
        kickButton.gameObject.SetActive(isBot && isOwner);
        bodyButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void SetUpAddBot(bool isTeam1)
    {
        nameText.text = "Add Bot";
        nameText.gameObject.SetActive(true);
        rankText.gameObject.SetActive(false);
        correctRateText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);
        kickButton.gameObject.SetActive(false);
        this.isTeam1 = isTeam1;
        bodyButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void SetupRankPlayer(MatchPlayer matchPlayer)
    {
        nameText.text = matchPlayer.name;
        statusText.text = "Score: " + matchPlayer.score; 

        nameText.gameObject.SetActive(true);
        rankText.gameObject.SetActive(false);
        correctRateText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        kickButton.gameObject.SetActive(false);
        bodyButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
    }

    public void BodyButton()
    {
        GameManager.instance.playerRoomManager.AddBot();
    }

    public void Kick()
    {
        GameManager.instance.playerRoomManager.RoomPlayerKick(userID);
    }
}
