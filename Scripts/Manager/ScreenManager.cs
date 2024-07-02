using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ScreenEnum {
    Announcement = 0,
    Auth,
    MainMenu,
    PlayPick,
    FindRoom,
    Library,
    GameModePick,
    PlayerRoom,
    GamePlay,
    count,
}

public struct ScreenBring
{
    public ScreenEnum screen;
    public bool isBringUp;
    public string announcement;
    public bool isUsingCofirm;
    public string confirmText;
    public UnityAction confirmAction;
    public bool isUsingBack;
    public string backText;
    public UnityAction backAction;

    public ScreenBring(ScreenEnum screen, bool isBringUp, string announcement = "", bool isUsingCofirm = false, string confirmText = "confirm", UnityAction confirmAction = null,
        bool isUsingBack = true, string backText = "back", UnityAction backAction = null)
    {
        this.screen = screen;
        this.isBringUp = isBringUp;
        this.announcement = announcement;
        this.isUsingCofirm = isUsingCofirm;
        this.confirmText = confirmText;
        this.confirmAction = confirmAction;
        this.isUsingBack = isUsingBack;
        this.backText = backText;
        this.backAction = backAction;
    }
}

public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text announcementText;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private TMP_Text confirmText;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private TMP_Text backText;

    [SerializeField]
    private int topScreen = 1;
    [SerializeField]
    private List<Canvas> screens = new List<Canvas>();

    

    [SerializeField]
    private Queue<ScreenBring> screenBringQueue = new Queue<ScreenBring>();


    private void Update()
    {
        if (screenBringQueue.Count > 0) 
            BringOnMainThread(screenBringQueue.Dequeue());
    }

    private void BringOnMainThread(ScreenBring screenBring)
    {
        Debug.Log(screenBring.screen + " is bring " + (screenBring.isBringUp ? "up to " : "down from ") + topScreen);
        if (screenBring.isBringUp)
        {
            if (screenBring.screen == ScreenEnum.Announcement)
            {
                announcementText.text = screenBring.announcement;
                screens[(int)ScreenEnum.Announcement].sortingOrder = 10000;
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(screenBring.confirmAction);
                confirmButton.gameObject.SetActive(screenBring.isUsingCofirm);
                this.confirmText.text = screenBring.confirmText;

                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(screenBring.backAction == null ? DeafaultBackButton : screenBring.backAction);
                backButton.gameObject.SetActive(screenBring.isUsingBack);
                this.backText.text = screenBring.backText;
                return;
            }
            if (screens[(int)screenBring.screen].sortingOrder != topScreen)
            screens[(int)screenBring.screen].sortingOrder = ++topScreen;
        }
        else
        {
            if (screens[(int)screenBring.screen].sortingOrder == topScreen) --topScreen;
            screens[(int)screenBring.screen].sortingOrder = -1;
        }

        screens[(int)ScreenEnum.Announcement].sortingOrder = -2;
    }
    public void BringUp(ScreenEnum screen, string announcement = "", bool isUsingCofirm = false, string confirmText = "confirm", UnityAction confirmAction = null,
        bool isUsingBack = true, string backText = "back", UnityAction backAction = null)
    {
        Debug.Log("Enqueue " + announcement);
        screenBringQueue.Enqueue(new ScreenBring(screen, true, announcement, isUsingCofirm, confirmText, confirmAction, isUsingBack, backText, backAction));
    }

    public void BringDown(ScreenEnum screen)
    {
        screenBringQueue.Enqueue(new ScreenBring(screen, false));
    }

    public void Announce(string announce,
        bool isUsingCofirm = false, string confirmText = "confirm", UnityAction confirmAction = null,
        bool isUsingBack = true, string backText = "back", UnityAction backAction = null)
    {
        Debug.Log("Annouce: " + announce);
        BringUp(ScreenEnum.Announcement, announce, isUsingCofirm, confirmText, confirmAction, isUsingBack, backText, backAction);
    }

    public void Loading()
    {
        Announce("Loading", isUsingBack: false);
    }

    public void DeafaultBackButton()
    {
        BringDown(ScreenEnum.Announcement);
    }
}
