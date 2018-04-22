using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mode_Select_Scene_Manager : MonoBehaviour
{
    public Button m_Competition_Button;
    public Button m_Coop_Button;
    public RawImage m_Comp_CommingSoon;
    public RawImage m_Coop_CommingSoon;

    public static Mode_Select_Scene_Manager c_Mode_Select_manager;

    void Start ()
    {
        c_Mode_Select_manager = this;
    }

    public void Open_Competition_Mode()
    {
        m_Comp_CommingSoon.gameObject.SetActive(false);
        m_Competition_Button.gameObject.SetActive(true);
    }

    public void Open_Coop_Mode()
    {
        m_Coop_CommingSoon.gameObject.SetActive(false);
        m_Coop_Button.gameObject.SetActive(true);
    }
}
