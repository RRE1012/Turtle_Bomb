using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Turtle_Move : MonoBehaviour {
    public static Turtle_Move instance;
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;
    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;
    public GameObject[] m_DropBomb;
    bool m_YouCanSetBomb=false;
    public byte id = 0;
    public Text state_text;
    public float m_PlayerSpeed;
    float m_RotateSensX = 150.0f;
    bool dead_ani=false;
    byte fire_power=2;
    byte bomb_power=2;
    byte speed_power=2;
    // 회전 각
    public byte alive = 1;
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
        bomb_power = 2;
        speed_power = 2;
        alive = 1;
        //Invoke("SetPosition", 2.0f);
        SetPosition();
    }
    
    void OnTriggerEnter(Collider other)
    {
        
    }
    void SetFalse()
    {
        gameObject.SetActive(false);
    }
    public byte GetFirePower()
    {
        return fire_power;
    }
    public void Dead_Case(byte m_id)
    {
        if (id == m_id)
        {
            //죽음
            dead_ani = true;
        }
        else
        {
            switch (m_id)
            {
                //상대방이 죽음
                case 0:
                    NetUser.instance.SetDeadMotion();
                    break;
                case 1:
                    NetUser2.instance.SetDeadMotion();
                    break;
                case 2:
                    NetUser3.instance.SetDeadMotion();
                    break;
                case 3:
                    NetUser4.instance.SetDeadMotion();
                    break;
                default:
                    break;
            }
        }

    }
    public void SetItem_Ability(byte m_id, byte type)
    {
        if(id == m_id)
        {
            switch (type)
            {
                case 0:
                    bomb_power++;
                    Debug.Log("Bomb Up~");
                    break;
                case 1:
                    fire_power++;
                    Debug.Log("Fire Up~");
                    break;
                case 2:
                    speed_power++;
                    Debug.Log("Speed Up~");
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("Not Mine");
            switch (m_id)
            {
                case 0:
                    NetUser.instance.SetText(type);
                    break;
                case 1:
                    NetUser2.instance.SetText(type);
                    break;
                case 2:
                    NetUser3.instance.SetText(type);
                    break;
                case 3:
                    NetUser4.instance.SetText(type);
                    break;
                default:
                    break;
            }
        }
    }

    void SetPosition()
    {
        switch (VariableManager.instance.pos_inRoom)
        {
            case 1:
                id = 0;
                transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
                break;
            case 2:

                id = 1;
                transform.position = new Vector3(28.0f, transform.position.y, 0.0f);
                break;
            case 3:
                id = 2;

                transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
                break;
            case 4:

                id = 3;
                transform.position = new Vector3(28.0f, transform.position.y, 28.0f);
                break;
            default:
                break;

        }
    }
    void SetAnimation()
    {
        switch (GameRoom.instance.pos_inRoom)
        {
            case 1:
                id = 0;
                m_animator.SetTrigger("Jump01");
                //transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
                break;
            case 2:
                id = 1;
                m_animator.SetTrigger("Jump02");
                //transform.position = new Vector3(28.0f, transform.position.y, 0.0f);
                break;
            case 3:
                id = 2;
                m_animator.SetTrigger("Jump03");
                //transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
                break;
            case 4:
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
        state_text.text = "Bomb : " + bomb_power + "\nFire : " + fire_power + "\nSpeed: " + speed_power;
        if (dead_ani)
        {
            Debug.Log("Dead!!!");
            m_animator.SetBool("death", true);

            Invoke("SetFalse", 2.1f);
            dead_ani = false;
        }
        if (Input.GetMouseButton(0))
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
            if(!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

        }
        if (Input.GetKey(KeyCode.W))
        {
            
            transform.Translate(new Vector3(0.0f, 0.0f, m_PlayerSpeed * Time.deltaTime));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
            
        }
        if (Input.GetKey(KeyCode.S)) { transform.Translate(new Vector3(0.0f, 0.0f, -m_PlayerSpeed * Time.deltaTime));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A)) { transform.Translate(new Vector3(-m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        KeyBoard_Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!VSModeManager.instance.game_set)
                SetBomb();
        }
        
    }

    public byte GetId()
    {
        return id;
    }
    public void SetBomb() // 폭탄 설치
    {
        m_YouCanSetBomb = true;

        // 폭탄 위치 설정
        
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

        if (MapManager.instance.Check_BombSet((int)(m_BombLocX / 2), (int)(m_BombLocZ / 2)))
        {
            Debug.Log("You can't set bomb");
            m_YouCanSetBomb = false;

        }
        // 이미 놓인 폭탄 검사
        
            // 폭탄 생성
            if (m_YouCanSetBomb)
            {

               
                NetTest.instance.SetBombPos((int)m_BombLocX, (int)m_BombLocZ,fire_power);
                NetTest.instance.SendBombPacket();
                //UI.m_bomb_count = UI.m_bomb_count - 1;
            }
        
 

    }
    public void KeyBoard_Move() // 플레이어 이동 및 회전
    {
       
    }
}
