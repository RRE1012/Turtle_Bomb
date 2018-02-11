using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool m_isClicked = false;
    

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isClicked = false;
    }
}
