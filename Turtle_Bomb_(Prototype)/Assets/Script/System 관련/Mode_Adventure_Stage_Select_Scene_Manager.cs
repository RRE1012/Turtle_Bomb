using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Mode_Adventure_Stage_Select_Scene_Manager : MonoBehaviour {


    public static Mode_Adventure_Stage_Select_Scene_Manager c_AdvMode_Stage_Slt_Manager;
    public Button m_Forest_Stage_1_button;
    public Button m_Forest_Stage_2_button;
    public Button m_Forest_Stage_3_button;
    public Button m_Forest_Stage_4_button;
    public Button m_Forest_Stage_5_button;
    public Button m_Forest_Stage_6_button;
    public Button m_Forest_Stage_7_button;
    public Button m_Forest_Stage_8_button;
    public Button m_Forest_Stage_Boss_button;
    public static Button[] m_Stage_Buttons;

    void Awake()
    {
        c_AdvMode_Stage_Slt_Manager = this;
        m_Stage_Buttons = new Button[9];
        m_Stage_Buttons[0] = m_Forest_Stage_1_button;
        m_Stage_Buttons[1] = m_Forest_Stage_2_button;
        m_Stage_Buttons[2] = m_Forest_Stage_3_button;
        m_Stage_Buttons[3] = m_Forest_Stage_4_button;
        m_Stage_Buttons[4] = m_Forest_Stage_5_button;
        m_Stage_Buttons[5] = m_Forest_Stage_6_button;
        m_Stage_Buttons[6] = m_Forest_Stage_7_button;
        m_Stage_Buttons[7] = m_Forest_Stage_8_button;
        m_Stage_Buttons[8] = m_Forest_Stage_Boss_button;
    }

}
