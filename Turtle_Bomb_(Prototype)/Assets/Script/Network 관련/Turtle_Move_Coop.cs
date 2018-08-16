using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Turtle_Move_Coop : MonoBehaviour
{
    public static Turtle_Move_Coop instance;
    public Image fade_image;
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;
    int touchNum = 0;
    bool m_is_Touch_Started = false;
    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;
    public GameObject[] m_DropBomb;
    bool m_YouCanSetBomb = false;
    byte id = 0;
    public Text[] state_text;
    public RawImage[] state_image;
    public GameObject m_gameover;
    public Button bombButton;
    public Button throwButton;
    float m_Touch_PrevPoint_X;
    bool fade = false;
    public float m_PlayerSpeed = 3.0f;
    float m_RotateSensX = 150.0f;
    bool dead_ani = false;
    bool throw_ani = false;
    bool walk_ani = false;
    bool push_ani = false;
    bool kick_ani = false;
    byte direction;
    byte fire_power = 1;
    byte bomb_power = 2;
    byte bomb_set = 2;
    byte speed_power = 1;
    public bool can_kick = false;
    public bool can_throw = false;
    Animator m_TurtleMan_Animator;
    
    int itemtype = 0;
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
        fade_image.gameObject.SetActive(false);
        m_TurtleMan_Animator = GetComponent<Animator>();
        //Invoke("SetAnimation", 1.0f);
        fire_power = 1;
        bomb_power = 2;
        speed_power = 1;
        fade = false;
        kick_bomb = false;
        alive = 1;
        direction = 0;
        //Invoke("SetPosition", 2.0f);
        SetPosition();
    }
    void Throw_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isThrow", false);
    }
    void Walk_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
    }
    void Push_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isPush", false);
    }
    void Kick_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isKick", false);
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
            NetManager_Coop.instance.SetmoveTrue();
        }
        if (other.gameObject.CompareTag("Monster_Attack_Collider"))
        {
            alive = 0;
            NetManager_Coop.instance.SetmoveTrue();
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
        fade_image.gameObject.SetActive(true);
        fade = true;
        gameObject.tag = "Untagged";
        m_gameover.SetActive(true);
        //gameObject.SetActive(false);
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
           
        }
    }
    public void Move_Case(byte m_id)
    {
        
       
    }
    public void Dead_Case(byte m_id)
    {
        if (id == m_id)
        {
            //죽음
            dead_ani = true;
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
      

    }

    void SetPosition()
    {
        switch (VariableManager_Coop.instance.pos_id)
        {
            case 1:
                id = 0;

                transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
                Performance_Network.instance.Intro_Performance(id);
                break;
            case 2:

                id = 1;

                transform.position = new Vector3(28.0f, transform.position.y, 0.0f);
                Performance_Network.instance.Intro_Performance(id);
                break;
            case 3:
                id = 2;

                transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
                transform.Rotate(new Vector3(0, 180.0f, 0));
                Performance_Network.instance.Intro_Performance(id);
                break;
            case 4:

                id = 3;

                transform.position = new Vector3(28.0f, transform.position.y, 28.0f);

                transform.Rotate(new Vector3(0, 180.0f, 0));
                Performance_Network.instance.Intro_Performance(id);
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
        if (fade)
        {
            fade_image.color = new Color(0.0f, 0.0f, 0.0f, fade_image.color.a + (0.05f * Time.deltaTime));
        }
        if (can_kick)
        {
            state_image[0].gameObject.SetActive(false);
        }
        if (can_throw)
        {
            state_image[1].gameObject.SetActive(false);
        }
        if (!Performance_Network.instance.ani_is_working && gameObject.CompareTag("Player"))
        {
            BodyRotation();

            if (getItem)
            {
                CoOpManager.instance.GetItemUI_Activate(itemtype);
                MusicManager.manage_ESound.ItemGetSound2();
                getItem = false;
            }

            if (dead_ani)
            {
                //Debug.Log("Dead!!!");
                m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);

                Invoke("SetFalse", 2.1f);
                dead_ani = false;
            }
            if (push_ani)
            {
                m_TurtleMan_Animator.SetBool("TurtleMan_isPush", true);
                Invoke("Push_Ani_False", 1.0f);
                push_ani = false;
            }
            if (walk_ani)
            {
                
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
                Invoke("Walk_Ani_False",0.1f);
                walk_ani = false;
            }
            if (throw_ani)
            {
                m_TurtleMan_Animator.SetBool("TurtleMan_isThrow", true);
                Invoke("Throw_Ani_False", 1.0f);
                throw_ani = false;
            }
            //키보드-마우스 움직임
            /*
            if (Input.GetMouseButton(0))
            {
                m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
                transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
                //if (!VSModeManager.instance.game_set) <-나중에 협동모드매니저를 만들어야 할듯
                    NetManager_Coop.instance.SetMyPos(transform.position.x, transform.localEulerAngles.y, transform.position.z);

            }
            */

            //스마트폰 용 조이스틱 조종
            

            if (JoyStickMove.instance.Get_NormalizedVector() != Vector3.zero)
            {
                Vector3 normal = JoyStickMove.instance.Get_NormalizedVector();
                normal.z = normal.y;
                normal.y = 0.0f;
                transform.Translate((m_PlayerSpeed + speed_power) * normal * Time.deltaTime);
                //if (!VSModeManager.instance.game_set)
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.localEulerAngles.y, transform.position.z);
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            }
            else
            {
                // 릴리즈 빌드할 때는 적용할 것!
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
            }
            

            //Debug.Log(gameObject.transform.rotation.y);

            // ==================
            if (Input.GetKey(KeyCode.W))
            {

                transform.Translate(new Vector3(0.0f, 0.0f, (m_PlayerSpeed + speed_power) * Time.deltaTime));
                //if (!VSModeManager.instance.game_set)
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0.0f, 0.0f, -(m_PlayerSpeed + speed_power) * Time.deltaTime));
                //if (!VSModeManager.instance.game_set)
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-(m_PlayerSpeed + speed_power) * Time.deltaTime, 0.0f, 0.0f));
                //if (!VSModeManager.instance.game_set)
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3((m_PlayerSpeed + speed_power) * Time.deltaTime, 0.0f, 0.0f));
                //if (!VSModeManager.instance.game_set)
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //if (!VSModeManager.instance.game_set)
                    SetBomb();
            }
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

        if (MapManager_COop.instance.Check_BombSet((int)(m_BombLocX / 2), (int)(m_BombLocZ / 2)))
        {
            //Debug.Log("You can't set bomb");
            m_YouCanSetBomb = false;

        }
        // 이미 놓인 폭탄 검사

        // 폭탄 생성
        if (m_YouCanSetBomb && bomb_set > 0)
        {

            bomb_set--;
            NetManager_Coop.instance.SetBombPos((int)m_BombLocX, (int)m_BombLocZ, fire_power);
            NetManager_Coop.instance.SendBombPacket();
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
        NetManager_Coop.instance.PushBox_Packet(direction, (int)m_BombLocX, (int)m_BombLocZ);
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
        NetManager_Coop.instance.SendBomb_TPacket(direction, (int)m_BombLocX, (int)m_BombLocZ);
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
        NetManager_Coop.instance.SendBomb_KPacket(direction, (int)m_BombLocX, (int)m_BombLocZ);
    }

    void BodyRotation()
    {

        if (CoOpManager.instance.Get_isClicked()) // 조이스틱 + 회전 + ...
        {

            if (!m_is_Touch_Started)
            {
                if (JoyStickMove.instance.Get_is_Joystick_First_Touched_Net()) // 조이스틱이 먼저면
                    touchNum = 1;
                else touchNum = 0; // 회전이 먼저면
                m_is_Touch_Started = true;
            }
            switch (Input.GetTouch(touchNum).phase) // 회전 처리
            {
                case TouchPhase.Began:
                    m_Touch_PrevPoint_X = Input.GetTouch(touchNum).position.x;
                    break;

                case TouchPhase.Moved:
                    transform.Rotate(0, (Input.GetTouch(touchNum).position.x - m_Touch_PrevPoint_X) * 0.5f, 0);
                    m_Touch_PrevPoint_X = Input.GetTouch(touchNum).position.x;
                   
                    NetTest.instance.SetMyPos(transform.position.x, transform.localEulerAngles.y, transform.position.z);
                    break;
            }
        }
        else m_is_Touch_Started = false;
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
