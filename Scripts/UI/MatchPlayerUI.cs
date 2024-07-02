using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchPlayerUI : DropZone
{
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text ATKText;
    public TMP_Text DEFText;
    public TMP_Text shieldText;

    private string userID;

    public void SetUp(CharacterDataStruct characterData)
    {
        SetUp("", characterData);
    }

    public void SetUp(string userID, CharacterDataStruct characterData)
    {
        this.userID = userID;
        nameText.text = characterData.playerName;
        healthText.text = "heatlh " + characterData.health;
        ATKText.text = "ATK    " + characterData.ATK;
        DEFText.text = "DEF    " + characterData.DEF;
        shieldText.text = "shield " + characterData.shield;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            //int cardID = eventData.pointerDrag.GetComponent<CardUI>().cardID;
            //if (GameManager.instance.matchManager.PlayCard(cardID, new string[] { userID })) eventData.pointerDrag.SetActive(false);
            //Debug.Log(eventData.pointerDrag.name + " on drop " + transform.name);
        }
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}
