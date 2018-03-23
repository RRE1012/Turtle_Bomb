using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle_Move : MonoBehaviour {
    public static Turtle_Move instance;
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;
    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;
    public GameObject[] m_DropBomb;
    bool m_YouCanSetBomb=false;
    int id = 0;
    public float m_PlayerSpeed;
    float m_RotateSensX = 150.0f;
    byte fire_power;
    // 회전 각
    float m_RotationX = 0.0f;
    Animator m_animator;
    void Awake()
    {
        instance = this;

    }
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        //Invoke("SetAnimation", 1.0f);
        fire_power = 2;
        Invoke("SetPosition", 2.0f);

    }

    public byte GetFirePower()
    {
        return fire_power;
    }

    void SetPosition()
    {
        switch (NetTest.instance.GetId())
        {
            case 0:
                id = 0;
                transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
                break;
            case 1:

                id = 1;
                transform.position = new Vector3(28.0f, transform.position.y, 0.0f);
                break;
            case 2:
                id = 2;

                transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
                break;
            case 3:

                id = 3;
                transform.position = new Vector3(28.0f, transform.position.y, 28.0f);
                break;
            default:
                break;

        }
    }
    void SetAnimation()
    {
        switch (NetworkTest2.instance.GetId())
        {
            case 0:
                id = 0;
                m_animator.SetTrigger("Jump01");
                //transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
                break;
            case 1:
                id = 1;
                m_animator.SetTrigger("Jump02");
                //transform.position = new Vector3(28.0f, transform.position.y, 0.0f);
                break;
            case 2:
                id = 2;
                m_animator.SetTrigger("Jump03");
                //transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
                break;
            case 3:
                id = 3;
                m_animator.SetTrigger("Jump04");
                //transform.position = new Vector3(28.0f, transform.position.y, 28.0f);
                break;
            default:
                break;
        }

    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
             NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

        }
        if (Input.GetKey(KeyCode.W))
        {
            
            transform.Translate(new Vector3(0.0f, 0.0f, m_PlayerSpeed * Time.deltaTime));
            NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
            
        }
        if (Input.GetKey(KeyCode.S)) { transform.Translate(new Vector3(0.0f, 0.0f, -m_PlayerSpeed * Time.deltaTime));
            NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A)) { transform.Translate(new Vector3(-m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));
            NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));
            NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        KeyBoard_Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBomb();
        }
    }

    public int GetId()
    {
        return id;
    }
    public void SetBomb() // 폭탄 설치
    {
        m_YouCanSetBomb = true;

        // 폭탄 위치 설정
        if (UI.m_bomb_count > 0)
        {
            m_Bombindex_X = (int)transform.position.x;
            m_Bombindex_Z = (int)transform.position.z;

            if (m_Bombindex_X % 2 == 1)
            {
                m_BombLocX = m_Bombindex_X + 1.0f;
            }
            else if (m_Bombindex_X % 2 == -1)
            {
                m_BombLocX = m_Bombindex_X - 1.0f;
            }
            else
                m_BombLocX = m_Bombindex_X;


            if (m_Bombindex_Z % 2 == 1)
            {
                m_BombLocZ = m_Bombindex_Z + 1.0f;
            }
            else if (m_Bombindex_Z % 2 == -1)
            {
                m_BombLocZ = m_Bombindex_Z - 1.0f;
            }
            else
                m_BombLocZ = m_Bombindex_Z;


            // 이미 놓인 폭탄 검사
            m_DropBomb = GameObject.FindGameObjectsWithTag("Bomb");
            foreach (GameObject bomb in m_DropBomb)
            {
                if (bomb != null)
                {
                    if (m_BombLocX == bomb.transform.position.x && m_BombLocZ == bomb.transform.position.z)
                    {
                        m_YouCanSetBomb = false;
                        break;
                    }
                    else m_YouCanSetBomb = true;
                }
            }

            // 폭탄 생성
            if (m_YouCanSetBomb)
            {
               
                
                NetTest.instance.SetBombPos((int)m_BombLocX, (int)m_BombLocZ,fire_power);
                //UI.m_bomb_count = UI.m_bomb_count - 1;
            }
        }
 

    }
    public void KeyBoard_Move() // 플레이어 이동 및 회전
    {
       
    }
}
