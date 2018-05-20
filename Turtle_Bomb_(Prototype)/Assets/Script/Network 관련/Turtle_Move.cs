using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Turtle_Move : MonoBehaviour
{
    public static Turtle_Move instance;
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;
    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;
    public GameObject[] m_DropBomb;
    bool m_YouCanSetBomb = false;
    byte id = 0;
    public Text[] state_text;
    public Button bombButton;
    public Button throwButton;
    float m_Touch_PrevPoint_X;

    public float m_PlayerSpeed = 3.0f;
    float m_RotateSensX = 150.0f;
    bool dead_ani = false;
    bool throw_ani = false;
    bool walk_ani = false;
    bool push_ani = false;
    bool kick_ani = false;
    byte direction;
    byte fire_power = 1;
    byte bomb_power = 1;
    byte bomb_set = 1;
    byte speed_power = 1;
    public bool can_kick = false;
    public bool can_throw = false;
    Animator m_TurtleMan_Animator;
    Animator m_animator;
    int itemtype=0;
    bool getItem = false;
    bool kick_bomb;
    // 회전 각
    public byte alive = 1;
    float m_RotationX = 0.0f;
    public Animator animator_camera;
    void Awake()
    {
        instance = this;

    }
    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        //Invoke("SetAnimation", 1.0f);
        fire_power = 1;
        bomb_power = 1;
        speed_power = 1;
        kick_bomb = false;
        alive = 1;
        direction = 0;
        //Invoke("SetPosition", 2.0f);
        SetPosition();
    }
    void Throw_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isThrow", false);
    }
    void Walk_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isWalk", false);
    }
    void Push_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isPush", false);
    }
    void Kick_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isKick", false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (can_kick && collision.gameObject.CompareTag("Bomb"))
        {

            //kick_bomb = true;
            Bomb_Kick();

        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            if (can_throw)
            {
                bombButton.gameObject.SetActive(false);
                throwButton.gameObject.SetActive(true);
            }

        }
        if (other.gameObject.CompareTag("Flame_Bush"))
        {
            alive = 0;
            NetTest.instance.SetmoveTrue();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            if (can_throw)
            {
                bombButton.gameObject.SetActive(true);
                throwButton.gameObject.SetActive(false);
            }
            other.isTrigger = false;


        }
    }
    void SetFalse()
    {
        gameObject.SetActive(false);
    }
    public byte GetFirePower()
    {
        return fire_power;
    }
    public void Throw_Case(byte m_id)
    {
        if (id == m_id)
        {
            throw_ani = true;
        }
        else
        {
            switch (m_id)
            {

                case 0:
                    NetUser.instance.SetThrowMotion();
                    break;
                case 1:
                    NetUser2.instance.SetThrowMotion();
                    break;
                case 2:
                    NetUser3.instance.SetThrowMotion();
                    break;
                case 3:
                    NetUser4.instance.SetThrowMotion();
                    break;
                default:
                    break;
            }
        }
    }
    public void Kick_Case(byte m_id)
    {
        if (id == m_id)
        {
            kick_ani = true;
            kick_bomb = false;
        }
        else
        {
            switch (m_id)
            {

                case 0:
                    NetUser.instance.SetKickMotion();
                    break;
                case 1:
                    NetUser2.instance.SetKickMotion();
                    break;
                case 2:
                    NetUser3.instance.SetKickMotion();
                    break;
                case 3:
                    NetUser4.instance.SetKickMotion();
                    break;
                default:
                    break;
            }
        }
    }
    public void Push_Case(byte m_id)
    {
        if (id == m_id)
        {
            push_ani = true;
        }
        else
        {
            switch (m_id)
            {

                case 0:
                    NetUser.instance.SetPushMotion();
                    break;
                case 1:
                    NetUser2.instance.SetPushMotion();
                    break;
                case 2:
                    NetUser3.instance.SetPushMotion();
                    break;
                case 3:
                    NetUser4.instance.SetPushMotion();
                    break;
                default:
                    break;
            }
        }
    }
    public void Move_Case(byte m_id)
    {
        if (id == m_id)
        {
            walk_ani = true;
        }
        else
        {
            switch (m_id)
            {

                case 0:
                    NetUser.instance.SetMoveMotion();
                    break;
                case 1:
                    NetUser2.instance.SetMoveMotion();
                    break;
                case 2:
                    NetUser3.instance.SetMoveMotion();
                    break;
                case 3:
                    NetUser4.instance.SetMoveMotion();
                    break;
                default:
                    break;
            }
        }
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
        //getItem = true;
        /*
        switch (type)
        {
            case 0:
                bomb_power++;
                bomb_set++;
                //Debug.Log("Bomb Up~");
                break;
            case 1:
                fire_power++;
                //Debug.Log("Fire Up~");
                break;
            case 2:
                speed_power++;
                //Debug.Log("Speed Up~");
                break;
            case 3:
                can_kick = true;
                break;
            case 4:
                can_throw = true;
                break;
            default:
                break;
        }*/

        if (id == m_id)
        {
            getItem = true;
            switch (type)
            {
                case 0:
                    bomb_power++;
                    bomb_set++;
                    itemtype = 0;
                    //Debug.Log("Bomb Up~");
                    break;
                case 1:
                    fire_power++;
                    itemtype = 1;
                    //Debug.Log("Fire Up~");
                    break;
                case 2:
                    speed_power++;
                    itemtype = 2;
                    //Debug.Log("Speed Up~");
                    break;
                case 3:
                    can_kick = true;
                    itemtype = 3;
                    break;
                case 4:
                    can_throw = true;
                    itemtype = 4;
                    break;
                default:
                    break;
            }
        }
        else
        {
            //Debug.Log("Not Mine" + id + "!=" + m_id);
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
                transform.Rotate(new Vector3(0, 180.0f, 0));
                break;
            case 4:

                id = 3;
                transform.position = new Vector3(28.0f, transform.position.y, 28.0f);
                transform.Rotate(new Vector3(0, 180.0f, 0));
                break;
            default:
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        state_text[0].text = bomb_set + " / " + bomb_power;
        state_text[1].text = "" + fire_power;
        state_text[2].text = "" + speed_power;
        BodyRotation();

        if (getItem)
        {
            VSModeManager.instance.GetItemUI_Activate(itemtype);
            MusicManager.manage_ESound.ItemGetSound();
            getItem = false;
        }

        if (dead_ani)
        {
            //Debug.Log("Dead!!!");
            m_animator.SetBool("TurtleMan_isDead", true);

            Invoke("SetFalse", 2.1f);
            dead_ani = false;
        }
        if (push_ani)
        {
            m_animator.SetBool("TurtleMan_isPush", true);
            Invoke("Push_Ani_False", 1.0f);
            push_ani = false;
        }
        if (walk_ani)
        {
            m_animator.SetBool("TurtleMan_isWalk", true);
            Invoke("Walk_Ani_False", 1.0f);
            walk_ani = false;
        }
        if (throw_ani)
        {
            m_animator.SetBool("TurtleMan_isThrow", true);
            Invoke("Throw_Ani_False", 1.0f);
            throw_ani = false;
        }
        /*
        if (Input.GetMouseButton(0))
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
            if(!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

        }*/

        if (JoyStickMove.instance.Get_NormalizedVector() != Vector3.zero)
        {
            Vector3 normal = JoyStickMove.instance.Get_NormalizedVector();
            normal.z = normal.y;
            normal.y = 0.0f;
            transform.Translate((m_PlayerSpeed + speed_power) * normal * Time.deltaTime);
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }

        // ==================
        if (Input.GetKey(KeyCode.W))
        {

            transform.Translate(new Vector3(0.0f, 0.0f, (m_PlayerSpeed + speed_power) * Time.deltaTime));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -(m_PlayerSpeed + speed_power) * Time.deltaTime));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-(m_PlayerSpeed + speed_power) * Time.deltaTime, 0.0f, 0.0f));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3((m_PlayerSpeed + speed_power) * Time.deltaTime, 0.0f, 0.0f));
            if (!VSModeManager.instance.game_set)
                NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!VSModeManager.instance.game_set)
                SetBomb();
        }

    }
    public void ReloadBomb(byte tID)
    {
        if (id == tID)
        {
            bomb_set++;
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
            //Debug.Log("You can't set bomb");
            m_YouCanSetBomb = false;

        }
        // 이미 놓인 폭탄 검사

        // 폭탄 생성
        if (m_YouCanSetBomb && bomb_set > 0)
        {

            bomb_set--;
            NetTest.instance.SetBombPos((int)m_BombLocX, (int)m_BombLocZ, fire_power);
            NetTest.instance.SendBombPacket();
            //UI.m_bomb_count = UI.m_bomb_count - 1;
        }



    }

    public void Box_Push()
    {
        float yRotation = gameObject.transform.eulerAngles.y;
        ////Debug.Log(yRotation);
        if (yRotation >= 315.0f || yRotation < 45.0f)
            direction = 3;
        else if (yRotation >= 45.0f && yRotation < 135.0f)
            direction = 1;
        else if (yRotation >= 135.0f && yRotation < 225.0f)
            direction = 4;
        else if (yRotation >= 225.0f && yRotation < 315.0f)
            direction = 2;
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
        m_BombLocX = m_BombLocX / 2;
        m_BombLocZ = m_BombLocZ / 2;
        NetTest.instance.PushBox_Packet(direction, (int)m_BombLocX, (int)m_BombLocZ);
    }
    public void Bomb_Throw() // 폭탄 던지기
    {

        float yRotation = gameObject.transform.eulerAngles.y;
        ////Debug.Log(yRotation);
        if (yRotation >= 315.0f || yRotation < 45.0f)
            direction = 3;
        else if (yRotation >= 45.0f && yRotation < 135.0f)
            direction = 1;
        else if (yRotation >= 135.0f && yRotation < 225.0f)
            direction = 4;
        else if (yRotation >= 225.0f && yRotation < 315.0f)
            direction = 2;
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
        m_BombLocX = m_BombLocX / 2;
        m_BombLocZ = m_BombLocZ / 2;
        NetTest.instance.SendBomb_TPacket(direction, (int)m_BombLocX, (int)m_BombLocZ);
        bombButton.gameObject.SetActive(true);
        throwButton.gameObject.SetActive(false);
    }
    public void Bomb_Kick()
    {
        float yRotation = gameObject.transform.eulerAngles.y;
        ////Debug.Log(yRotation);
        if (yRotation >= 315.0f || yRotation < 45.0f)
            direction = 3;
        else if (yRotation >= 45.0f && yRotation < 135.0f)
            direction = 1;
        else if (yRotation >= 135.0f && yRotation < 225.0f)
            direction = 4;
        else if (yRotation >= 225.0f && yRotation < 315.0f)
            direction = 2;
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
        m_BombLocX = m_BombLocX / 2;
        m_BombLocZ = m_BombLocZ / 2;
        NetTest.instance.SendBomb_KPacket(direction, (int)m_BombLocX, (int)m_BombLocZ);
    }

    void BodyRotation()
    {

        if (Input.touchCount > 0 && VSModeManager.instance.Get_isClicked()) // 조이스틱 + 회전 + ...
        {
            int touchNum;

            if (JoyStickMove.instance.Get_is_Joystick_First_Touched_Net()) // 조이스틱이 먼저면
                touchNum = 1;
            else touchNum = 0; // 회전이 먼저면

            switch (Input.GetTouch(touchNum).phase) // 회전 처리
            {
                case TouchPhase.Began:
                    m_Touch_PrevPoint_X = Input.GetTouch(touchNum).position.x;
                    break;

                case TouchPhase.Moved:
                    transform.Rotate(0, (Input.GetTouch(touchNum).position.x - m_Touch_PrevPoint_X) * 0.5f, 0);
                    m_Touch_PrevPoint_X = Input.GetTouch(touchNum).position.x;
                    if (!VSModeManager.instance.game_set)
                        NetTest.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
                    break;
            }
        }
    }
    public void MakeGameOverAni()
    {
        animator_camera.SetTrigger("Dead");
    }
    public void AniBomb_Start()
    {
        animator_camera.SetTrigger("Ring");
    }

    public void KeyBoard_Move() // 플레이어 이동 및 회전
    {

    }
}
