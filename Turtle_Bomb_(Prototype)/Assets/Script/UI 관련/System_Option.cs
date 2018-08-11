using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_Option : MonoBehaviour {

    public GameObject m_System_Option_Windows;

    public Button m_BGM_Switch;
    public Button m_SE_Switch;
    public Button m_Vib_Switch;
    public Button m_Auto_Login_Toggle;
    public Button m_Logout_Button;

    public Texture m_Switch_On;
    public Texture m_Switch_Off;
    public Texture m_Toggle_On;
    public Texture m_Toggle_Off;

    void Awake ()
    {
        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0)
            m_BGM_Switch.GetComponent<RawImage>().texture = m_Switch_Off;

        if (PlayerPrefs.GetInt("System_Option_SE_ON") == 0)
            m_SE_Switch.GetComponent<RawImage>().texture = m_Switch_Off;

        if (PlayerPrefs.GetInt("System_Option_Vib_ON") == 0)
            m_Vib_Switch.GetComponent<RawImage>().texture = m_Switch_Off;

        if (PlayerPrefs.GetInt("System_Option_Auto_Login_ON") == 0)
            m_Auto_Login_Toggle.GetComponent<RawImage>().texture = m_Toggle_Off;
    }


    public void Switch_BGM()
    {
        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0)
        {
            m_BGM_Switch.GetComponent<RawImage>().texture = m_Switch_On;
            PlayerPrefs.SetInt("System_Option_BGM_ON", 1);
        }
        else
        {
            m_BGM_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
            PlayerPrefs.SetInt("System_Option_BGM_ON", 0);
        }

        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0) LobbySound.instanceLS.SoundStop();
        else LobbySound.instanceLS.SoundStart();
    }

    public void Switch_SoundEffect()
    {
        if (PlayerPrefs.GetInt("System_Option_SE_ON") == 0)
        {
            m_SE_Switch.GetComponent<RawImage>().texture = m_Switch_On;
            PlayerPrefs.SetInt("System_Option_SE_ON", 1);
        }
        else
        {
            m_SE_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
            PlayerPrefs.SetInt("System_Option_SE_ON", 0);
        }
    }

    public void Switch_Vibration()
    {
        if (PlayerPrefs.GetInt("System_Option_Vib_ON") == 0)
        {
            m_Vib_Switch.GetComponent<RawImage>().texture = m_Switch_On;
            PlayerPrefs.SetInt("System_Option_Vib_ON", 1);
        }
        else
        {
            m_Vib_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
            PlayerPrefs.SetInt("System_Option_Vib_ON", 0);
        }
    }

    public void Toggle_Auto_Login()
    {
        if (PlayerPrefs.GetInt("System_Option_Auto_Login_ON") == 0)
        {
            m_Auto_Login_Toggle.GetComponent<RawImage>().texture = m_Toggle_On;
            PlayerPrefs.SetInt("System_Option_Auto_Login_ON", 1);
        }
        else
        {
            m_Auto_Login_Toggle.GetComponent<RawImage>().texture = m_Toggle_Off;
            PlayerPrefs.SetInt("System_Option_Auto_Login_ON", 0);
        }
    }

    public void System_Option_Window_On()
    {
        m_System_Option_Windows.SetActive(true);
        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0)
            m_BGM_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
        else m_BGM_Switch.GetComponent<RawImage>().texture = m_Switch_On;
        if (PlayerPrefs.GetInt("System_Option_SE_ON") == 0)
            m_SE_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
        else m_SE_Switch.GetComponent<RawImage>().texture = m_Switch_On;
        if (PlayerPrefs.GetInt("System_Option_Vib_ON") == 0)
            m_Vib_Switch.GetComponent<RawImage>().texture = m_Switch_Off;
        else m_Vib_Switch.GetComponent<RawImage>().texture = m_Switch_On;
        if (PlayerPrefs.GetInt("System_Option_Auto_Login_ON") == 0)
            m_Auto_Login_Toggle.GetComponent<RawImage>().texture = m_Toggle_Off;
        else m_Auto_Login_Toggle.GetComponent<RawImage>().texture = m_Toggle_On;
    }

    public void System_Option_Window_Off()
    {
        m_System_Option_Windows.SetActive(false);
    }
}
