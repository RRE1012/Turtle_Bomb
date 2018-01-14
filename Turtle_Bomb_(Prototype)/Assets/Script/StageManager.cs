using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    public Text m_Text;

    public static bool m_is_Stage_Clear = false;
    public static int m_Stars = 0;

    void Start()
    {
        m_is_Stage_Clear = false;
        m_Stars = 0;
    }

    void Update()
    {
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            m_Text.text = "Game Over";
        }
	}
    
    public static void StageClear()
    {
        m_is_Stage_Clear = true;
        UI.Draw_StageClearPage();
    }
}
