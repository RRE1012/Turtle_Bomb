using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Mode_Adventure_Stage_Select_Scene_Manager : MonoBehaviour {


    public static Mode_Adventure_Stage_Select_Scene_Manager c_AdvMode_Stage_Slt_Manager;
    public Button m_Stage_1_1_button;
    public Button m_Stage_1_2_button;
    public Button m_Stage_1_3_button;
    public Button m_Stage_2_1_button;
    public Button m_Stage_2_2_button;
    public Button m_Stage_2_3_button;
    public Button m_Stage_3_1_button;
    public Button m_Stage_3_2_button;
    public Button m_Stage_3_3_button;
    public static Button[] m_Stage_Buttons;

    void Awake()
    {
        c_AdvMode_Stage_Slt_Manager = this;
        m_Stage_Buttons = new Button[9];
        m_Stage_Buttons[0] = m_Stage_1_1_button;
        m_Stage_Buttons[1] = m_Stage_1_2_button;
        m_Stage_Buttons[2] = m_Stage_1_3_button;
        m_Stage_Buttons[3] = m_Stage_2_1_button;
        m_Stage_Buttons[4] = m_Stage_2_2_button;
        m_Stage_Buttons[5] = m_Stage_2_3_button;
        m_Stage_Buttons[6] = m_Stage_3_1_button;
        m_Stage_Buttons[7] = m_Stage_3_2_button;
        m_Stage_Buttons[8] = m_Stage_3_3_button;
    }

}
