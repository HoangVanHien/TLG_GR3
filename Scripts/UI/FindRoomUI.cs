using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindRoomUI : MonoBehaviour
{
    public TMP_InputField roomIDInput;
    public Button findButton;
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        findButton.onClick.AddListener(FindRoom);
        backButton.onClick.AddListener(Back);
    }

    private void FindRoom()
    {
        if(roomIDInput.text.Length != 6)
        {
            GameManager.instance.screenManager.Announce("Invalid room ID");
            return;
        }
        GameManager.instance.playerRoomManager.SelfJoinRoomWithCheck(roomIDInput.text.ToUpper());
    }

    private void Back()
    {
        GameManager.instance.screenManager.BringDown(ScreenEnum.FindRoom);
    }
}
