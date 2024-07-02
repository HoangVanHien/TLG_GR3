using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo;
    public CanvasGroup canvasGroup;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(transform.parent.parent);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo);
        canvasGroup.blocksRaycasts = true;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        parentToReturnTo = transform.parent;
    }
}
