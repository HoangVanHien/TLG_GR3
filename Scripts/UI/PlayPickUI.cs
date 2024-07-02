using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPickUI : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button customRoomButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        customRoomButton.onClick.AddListener(CustomRoom);
        backButton.onClick.AddListener(Back);
    }

    private void CreateRoom()
    {
        GameManager.instance.screenManager.BringUp(ScreenEnum.GameModePick);
        
    }

    private void JoinRoom()
    {
        GameManager.instance.screenManager.BringUp(ScreenEnum.FindRoom);
    }

    private void CustomRoom()
    {
        GameManager.instance.playerRoomManager.CreateMyRoom(GameModeEnum.custom);
    }

    private void Back()
    {
        GameManager.instance.screenManager.BringDown(ScreenEnum.PlayPick);
    }
}
