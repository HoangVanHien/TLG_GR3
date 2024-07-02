using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModePickUI : MonoBehaviour
{
    private int maxMode = 5;
    //[SerializeField] private List<Button> modeButtonList = new List<Button>();
    [SerializeField] private Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        //if (modeButtonList.Count < maxMode) Application.Quit();
        //for (int i = 0; i < maxMode; i++)
        //{
        //    modeButtonList[i].onClick.AddListener(() => { CreateRoom(i); });
        //}
        backButton.onClick.AddListener(Back);
    }

    public void CreateRoom(int mode)
    {
        Debug.Log("game mode create " + mode + " " + ((GameModeEnum)mode).ToString());
        GameManager.instance.screenManager.BringDown(ScreenEnum.GameModePick);
        GameManager.instance.playerRoomManager.CreateMyRoom((GameModeEnum)mode);
    }

    private void Back()
    {
        GameManager.instance.screenManager.BringDown(ScreenEnum.GameModePick);
    }
}
