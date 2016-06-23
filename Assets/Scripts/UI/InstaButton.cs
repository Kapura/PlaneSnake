using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class InstaButton : MonoBehaviour, IPointerDownHandler {

    [SerializeField] public Action OnClick;

    public void OnPointerDown( PointerEventData eventData )
    {
        OnClick();
    }
}
