using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GetID : MonoBehaviour {
    public static GetID instance = null;
    public InputField[] m_inputfield;
    string myID = null;
    string password = null;
    public GameObject[] m_inputcase;
    public GameObject[] m_button;
    public Button[] m_loginButton;
    public byte login_type;
    bool inputID = false;
    bool inputPass=false;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
        myID = "test";
        password = "123";
        DontDestroyOnLoad(this);
        login_type = 1;
    }
    public string GetIDD()
    {
        return myID;
    }
    public string GETPW()
    {
        return password;
    }

    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Destroy(this.gameObject);
        }
        if (m_loginButton[0] != null)
        {
            if (login_type == 0)
            {
                m_loginButton[0].gameObject.SetActive(false);
                m_loginButton[1].gameObject.SetActive(false);
                inputID = false;
                inputPass = false;
            }
            else if (login_type != 0 && inputID && inputPass)
            {
                m_loginButton[0].gameObject.SetActive(true);
                m_loginButton[1].gameObject.SetActive(true);
            }
        }
    }
    public void openField()
    {
        m_inputcase[0].gameObject.SetActive(true);
        m_button[0].gameObject.SetActive(false);
        m_button[1].gameObject.SetActive(false);
        login_type = 1;
    }
    public void openField2()
    {
        m_inputcase[1].gameObject.SetActive(true);
        m_button[0].gameObject.SetActive(false);
        m_button[1].gameObject.SetActive(false);
        login_type = 2;
    }
    public void closeField()
    {
        m_inputcase[0].gameObject.SetActive(false);
        m_inputcase[1].gameObject.SetActive(false);
        m_button[0].gameObject.SetActive(true);
        m_button[1].gameObject.SetActive(true);
        login_type = 0;
    }

    public void SetId()
    {
        
        myID = m_inputfield[0].text;
        login_type = 1;
        Debug.Log("MyID:" +myID);
    }
    public void SetId_Create()
    {

        myID = m_inputfield[2].text;
        login_type = 2;
        //Debug.Log(myID);
    }
    public void IDInput()
    {
        inputID = true;
        //Debug.Log("changed ID");
    }
    public void PasswordInput()
    {
        inputPass = true;
        //Debug.Log("changed PW");
    }
    public void SetPassword()
    {
        password = m_inputfield[1].text;
        
        Debug.Log("MyPW:"+password);
    }
    public void SetPasswordJoin()
    {
        password = m_inputfield[3].text;
    }
    public void KeyBoardOpen()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
        //Debug.Log("Open!");
    }
}
