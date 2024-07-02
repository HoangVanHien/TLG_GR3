using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardUI : DragCard
{
    public TMP_Text cardInfo;
    public Image cardImage;

    public int cardID;

    private bool isInstant = false;
    private bool isFirstCard = false;

    public void SetUp(GameCardStruct gameCardStruct)
    {
        gameObject.SetActive(true);
        cardID = gameCardStruct.cardID;
        string info = "mana: " + gameCardStruct.cardMana + "\n";
        foreach (CardActionStruct cardAction in gameCardStruct.cardActions)
        {
            isFirstCard = true;
            foreach (CardActionType cardActionType in cardAction.cardActionTypes)
            {
                if(!isFirstCard) info += "and ";
                switch (cardActionType.actionOrder)
                {
                    case ActionOrderType.Instant:
                        info += "instant ";
                        isInstant = true;
                        break;
                    case ActionOrderType.PreAction:
                        info += "before ";
                        break;
                    case ActionOrderType.AfterAction:
                        info += "after ";
                        break;
                    case ActionOrderType.Both:
                        info += "before and after ";
                        break;
                    default:
                        break;
                }
                if (!isInstant) {
                    switch (cardActionType.actionType)
                    {
                        case ActionType.Action:
                            info += "any action ";
                            break;
                        case ActionType.Turn:
                            info += "any turn ";
                            break;
                        case ActionType.Round:
                            info += "any round ";
                            break;
                        case ActionType.Attack:
                            info += "any attack ";
                            break;
                        case ActionType.Defend:
                            info += "any defend ";
                            break;
                        case ActionType.HealthChange:
                            info += "any health change ";
                            break;
                        case ActionType.HealthDecrease:
                            info += "any health decrease ";
                            break;
                        case ActionType.HealthIncrease:
                            info += "any health increase ";
                            break;
                        default:
                            break;
                    }
                }
            }
            info.Remove(info.Length - 4, 4);
            switch (cardAction.actionEventType)
            {
                case ActionEventType.TakeDamage:
                    info += "take damage ";
                    break;
                case ActionEventType.Attack:
                    info += "attack ";
                    break;
                case ActionEventType.Defend:
                    info += "defend ";
                    break;
                default:
                    break;
            }
            info += "equal ";
            info += cardAction.actionEventData.pureNumber + " + ";
            foreach (ActionEventCharacterInvoleStruct charaterScale in cardAction.actionEventData.characterScales)
            {
                info += charaterScale.characterScaleNumber + " " + charaterScale.characterScaleNumber.ToString();
            }
            info += "\n";
        }
        cardInfo.text = info;
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}
