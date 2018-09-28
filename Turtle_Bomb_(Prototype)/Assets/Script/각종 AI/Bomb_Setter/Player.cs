using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


static class DEFAULT_STATUS
{
    public const int BOMB = 5;
    public const int FIRE = 5;
    public const int SPEED = 5;
}

static class MAX_STATUS
{
    public const int BOMB = 5;
    public const int FIRE = 5;
    public const int SPEED = 5;
}

static class STATUS_CORRECTION
{
    public const float SPEED = 1.1f;
}


public class Player : Bomb_Setter
{
    Animator m_TurtleMan_Animator;
    Animator m_Camera_Animator;
    GameObject m_Front_Box; public void Set_Front_Box(GameObject g) { m_Front_Box = g; }
    GameObject m_Temp_Bomb;
    GameObject m_Selected_Bomb_For_Throwing;

    Player_Sound m_Player_Sound;

    SkinnedMeshRenderer[] m_Player_Mesh_Renderers;

    // 캐릭터 상태
    bool m_isBoxSelected = false; public void Set_is_Box_Selected(bool b) { m_isBoxSelected = b; }
    public bool Get_is_Box_Selected() { return m_isBoxSelected; }
    bool m_isPushing = false;

    bool m_isGot_KickItem = false;
    bool m_isGot_ThrowItem = false;

    bool m_isAbleToPush = false; public void Set_is_Able_to_Push(bool b) { m_isAbleToPush = b; }
    public bool Get_is_Able_to_Push() { return m_isAbleToPush; }
    bool m_isHideinBush = false; public void Set_is_Hide_in_Bush(bool b) { m_isHideinBush = b; }
    public bool Get_is_Hide_in_Bush() { return m_isHideinBush; }
    bool m_isAbleToKick = false; public void Set_is_Able_to_Kick(bool b) { m_isAbleToKick = b; }
    public bool Get_is_Able_to_Kick() { return m_isAbleToKick; }
    bool m_isAbleToThrow = false; public void Set_is_Able_to_Throw(bool b) { m_isAbleToThrow = b; }
    public bool Get_is_Able_to_Throw() { return m_isAbleToThrow; }


    int m_Touch_Num = 0;
    bool m_is_Touch_Started = false;

    // 이전 터치 지점 (x값)
    float m_Touch_PrevPoint_X;

    // 회전 감도
    float m_RotateSensX = 120.0f;

    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;

    // 폭탄 배치를 위한 인덱스값
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;

    // 플레이어 생존 여부
    bool m_isAlive = true;

    // 기본 속도
    float m_BasicSpeed = 3.0f;

    // 아이템 속도
    int m_Speed_Count;

    // 현재 속도
    float m_Curr_Speed = 0.0f;

    // 박스 밀기 거리
    float m_push_Distance = 0.0f;

    IEnumerator m_Wait_For_Intro;

    // ======== Functions ========
    void Awake()
    {
        m_TurtleMan_Animator = transform.GetChild(0).GetComponent<Animator>();
        m_Camera_Animator = transform.GetChild(1).GetComponent<Animator>();

        m_Player_Mesh_Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        StageManager.GetInstance().Set_is_Player_Alive(true);

        m_Max_Bomb_Count = DEFAULT_STATUS.BOMB;
        m_Curr_Bomb_Count = m_Max_Bomb_Count;
        m_Fire_Count = DEFAULT_STATUS.FIRE;
        m_Speed_Count = DEFAULT_STATUS.SPEED;
        Curr_Move_Speed_Update();

        m_Player_Sound = GetComponentInChildren<Player_Sound>();

        m_Wait_For_Intro = Wait_For_Intro();
        StartCoroutine(m_Wait_For_Intro);
    }

    public void Player_Set_Start_Point(Vector3 p)
    {
        transform.position = p; // 플레이어 위치를 시작지점으로 옮긴다.
    }

    IEnumerator Wait_For_Intro()
    {
        while (true)
        {
            if (StageManager.GetInstance().Get_is_Intro_Over())
                StopCoroutine(m_Wait_For_Intro);
            yield return null;
        }
    }

    void Update()
    {
        if (StageManager.GetInstance().Get_is_Intro_Over() && !StageManager.GetInstance().Get_is_Pause())
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);

            if (!m_isPushing)
                Move();
            else
                Pushing();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (m_isAlive)
        {
            // 아이템 획득 관련
            if (other.gameObject.CompareTag("Bomb_Item"))
            {
                Destroy(other.gameObject);
                if (Get_Curr_Bomb_Count() < MAX_STATUS.BOMB)
                {
                    m_Player_Sound.Play_Item_Get_Sound();
                    m_Max_Bomb_Count += 1;
                    m_Curr_Bomb_Count += 1;
                    UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);
                    UI.GetInstance().GetItemUI_Activate(0);
                }
            }
            if (other.gameObject.CompareTag("Fire_Item"))
            {
                Destroy(other.gameObject);
                if (Get_Fire_Count() < MAX_STATUS.FIRE)
                {
                    m_Player_Sound.Play_Item_Get_Sound();
                    m_Fire_Count += 1;
                    UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);
                    UI.GetInstance().GetItemUI_Activate(1);
                }
            }
            if (other.gameObject.CompareTag("Speed_Item"))
            {
                Destroy(other.gameObject);
                if (m_Speed_Count < MAX_STATUS.SPEED)
                {
                    m_Player_Sound.Play_Item_Get_Sound();
                    m_Speed_Count += 1;
                    Curr_Move_Speed_Update();
                    UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);
                    UI.GetInstance().GetItemUI_Activate(2);
                }
            }
            if (other.gameObject.CompareTag("Kick_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_KickItem)
                {
                    m_Player_Sound.Play_Item_Get_Sound();
                    m_isGot_KickItem = true;
                    UI.GetInstance().Kick_Icon_Activate();
                    UI.GetInstance().GetItemUI_Activate(3);
                }
            }
            if (other.gameObject.CompareTag("Throw_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_ThrowItem)
                {
                    m_Player_Sound.Play_Item_Get_Sound();
                    m_isGot_ThrowItem = true;
                    UI.GetInstance().Throw_Icon_Activate();
                    UI.GetInstance().GetItemUI_Activate(4);
                }
            }

            if (other.gameObject.CompareTag("Airdrop_Item"))
            {
                Destroy(other.gameObject);
                m_Player_Sound.Play_Item_Get_Sound();

                //int temp = Random.Range(1, 3);

                m_Max_Bomb_Count += 1; //temp;
                m_Curr_Bomb_Count += 1; // temp;
                if (m_Max_Bomb_Count > MAX_STATUS.BOMB)
                {
                    m_Max_Bomb_Count = MAX_STATUS.BOMB;
                    m_Curr_Bomb_Count = MAX_STATUS.BOMB;
                }

                //temp = Random.Range(1, 3);
                m_Fire_Count += 1; // temp;
                if (m_Fire_Count > MAX_STATUS.FIRE)
                    m_Fire_Count = MAX_STATUS.FIRE;

                //temp = Random.Range(1, 3);
                m_Speed_Count += 1; // temp;
                if (m_Speed_Count > MAX_STATUS.SPEED) m_Speed_Count = MAX_STATUS.SPEED;

                Curr_Move_Speed_Update();

                UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);
                UI.GetInstance().GetItemUI_Activate(5);
            }
            // ========================


            // 사망 판정
            // 스테이지 클리어시, 맵 이동시는 사망하지 않는다.
            if ((other.gameObject.tag == "Flame" ||
                other.gameObject.tag == "Flame_Bush" ||
                other.gameObject.tag == "Monster_Attack_Collider" ||
                other.gameObject.tag == "icicle_Body") && !StageManager.GetInstance().Get_is_Stage_Clear())
            {
                Set_Dead();
            }
            // ========================


            // 맵 전환 포탈
            if (other.gameObject.CompareTag("Map_Portal"))
            {
                // 맵 이동 시작 연출 수행. (Fade out 까지 수행)
                // Do();

                // 캐릭터를 특정 위치로 이동시킨다.
                // 현재는 초기 위치로 설정.
                //transform.position = new Vector3(0.0f, transform.position.y, 50.0f);
                //transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));

                // 다음 맵을 로드한다.
                StageManager.GetInstance().Next_Map_Load();


                // 맵 이동 마무리 연출 수행 (Fade In 먼저 수행)
                // Do();
            }
            // ===============================





            // 목표 지점 도달 시 스테이지 클리어
            if (other.gameObject.CompareTag("Goal"))
            {
                UI.GetInstance().Set_is_Goal(true);
                StageManager.GetInstance().SetGoalIn(true);
                StageManager.GetInstance().Stage_Clear();
            }
            // ===============================

            if (other.gameObject.tag == "Bush")
            {
                foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                    pmr.enabled = false;
                m_isHideinBush = true;
                UI.GetInstance().HideInBush_Management(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 폭탄 트리거 비활성
        if (other.gameObject.tag == "Bomb")
        {
            if (!other.gameObject.GetComponent<Bomb_Remaster>().Get_is_Hardend())
            {
                m_isAbleToThrow = false;
                other.isTrigger = false;
                m_Selected_Bomb_For_Throwing = null;
                other.gameObject.GetComponent<Bomb_Remaster>().Set_is_Hardend(true);
                UI.GetInstance().Throw_Button_Management(false);
            }
        }
        // =====================



        // 부쉬를 벗어날 경우
        if (other.gameObject.tag == "Bush")
        {
            foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                pmr.enabled = true;
            m_isHideinBush = false;
            UI.GetInstance().HideInBush_Management(false);
        }
        // =======================

    }


    void OnCollisionEnter(Collision collision)
    {
        // 폭탄 발차기
        if (m_isGot_KickItem && m_isAbleToKick)
        {
            // 현재 캐릭터가 발차기가 가능한 상태이면서, 폭탄은 던져진 상태가 아니어야 한다.
            if (collision.gameObject.CompareTag("Bomb"))
            {
                if (!collision.gameObject.GetComponent<Bomb_Remaster>().Get_is_Thrown())
                {
                    collision.gameObject.GetComponent<Bomb_Remaster>().Kick_Bomb(gameObject);

                    m_isAbleToKick = false;

                    m_TurtleMan_Animator.SetBool("TurtleMan_isKick", true);
                    Invoke("Kick_Ani_False", 0.4f);
                }
            }
        }

    }







    // ======== UDF =========

    void Move()
    {
        Body_MoveControl_ForMobile(); // 릴리즈용
        Body_MoveControl_ForPC(); // 디버그용

        Body_RotateControl_ForPC(); // 디버그용
        Body_RotateControl_ForMobile(); // 릴리즈용

        OtherControl_ForPC(); // 디버그용
    }

    void Body_MoveControl_ForPC()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, (m_Curr_Speed * Time.deltaTime)));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            //m_Player_Sound.Play_Move_Sound();
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -(m_Curr_Speed * Time.deltaTime)));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            //m_Player_Sound.Play_Move_Sound();
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-(m_Curr_Speed * Time.deltaTime), 0.0f, 0.0f));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            //m_Player_Sound.Play_Move_Sound();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3((m_Curr_Speed * Time.deltaTime), 0.0f, 0.0f));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            //m_Player_Sound.Play_Move_Sound();
        }


        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
        }

    }

    void Body_MoveControl_ForMobile()
    {
        if (JoyStickMove.instance.Get_NormalizedVector() != Vector3.zero)
        {
            Vector3 normal = JoyStickMove.instance.Get_NormalizedVector();
            normal.z = normal.y;
            normal.y = 0.0f;
            transform.Translate(m_Curr_Speed * normal * Time.deltaTime);
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
            //m_Player_Sound.Play_Move_Sound();
        }


        else
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
        }

    }

    void Body_RotateControl_ForPC()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(transform.up, -m_RotateSensX * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(transform.up, m_RotateSensX * Time.deltaTime);
        }
    }

    void Body_RotateControl_ForMobile()
    {
        if (Input.touchCount > 0 && UI.GetInstance().Get_isClicked()) // 회전 터치가 감지됨.
        {
            if (!m_is_Touch_Started) // 터치가 이제 막 시작됐다! (손을 모두 뗄 때 까지 더이상 수행하지 않음.)
            {
                if (JoyStickMove.instance.Get_is_Joystick_First_Touched())
                    m_Touch_Num = 1; // 조이스틱이 먼저다!
                else m_Touch_Num = 0; // 회전이 먼저다!

                m_is_Touch_Started = true; // OK.
            }

            switch (Input.GetTouch(m_Touch_Num).phase) // 회전 처리
            {
                case TouchPhase.Began:
                    m_Touch_PrevPoint_X = Input.GetTouch(m_Touch_Num).position.x;
                    break;

                case TouchPhase.Moved:
                    transform.Rotate(0, (Input.GetTouch(m_Touch_Num).position.x - m_Touch_PrevPoint_X) * 0.5f, 0);
                    m_Touch_PrevPoint_X = Input.GetTouch(m_Touch_Num).position.x;
                    break;
            }
        }

        else m_is_Touch_Started = false;
    }

    void OtherControl_ForPC()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetBomb();

        if (m_isAbleToPush && !m_isPushing && Input.GetKeyDown(KeyCode.R))
        {
            if (m_Front_Box != null)
                BoxPush();
        }
    }
    
    void Curr_Move_Speed_Update()
    {
        m_Curr_Speed = m_BasicSpeed + (m_Speed_Count * STATUS_CORRECTION.SPEED);
    }

    public void SetBomb() // 폭탄 설치
    {
        if (StageManager.GetInstance().Get_is_Intro_Over() && !StageManager.GetInstance().Get_is_Pause())
        {
            if (m_Curr_Bomb_Count > 0 && !StageManager.GetInstance().Get_MCL_is_Blocked_With_Coord(transform.position.x, transform.position.z))
            {
                m_Player_Sound.Play_Bomb_Set_Sound();
                m_Temp_Bomb = Bomb_Set(CALL_BOMB_STATE.NORMAL);
                m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", true);
                m_Camera_Animator.SetBool("Set_Bomb", true);
                Invoke("SetBomb_Ani_False", 0.3f);
                UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);

                if (m_isGot_ThrowItem)
                {
                    m_isAbleToThrow = true;
                    m_Selected_Bomb_For_Throwing = m_Temp_Bomb;
                    UI.GetInstance().Throw_Button_Management(true);
                }

                m_Temp_Bomb = null;
            }
        }
    }

    
    public void BoxPush() // 박스 밀기 시작
    {
        if (m_isBoxSelected && m_isAbleToPush && StageManager.GetInstance().Get_is_Intro_Over() && !StageManager.GetInstance().Get_is_Pause())
        {
            // 플레이어 위치 변환

            // 플레이어의 현재 MCL 인덱스를 찾는다.
            int index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);

            // 플레이어의 위치를 MCL 내부 좌표로 이동시킨다.
            Vector3 Loc;
            Loc.x = StageManager.GetInstance().m_Map_Coordinate_List[index].x;
            Loc.y = transform.position.y;
            Loc.z = StageManager.GetInstance().m_Map_Coordinate_List[index].z;
            transform.position = Loc;
            // --------------------------------




            // 플레이어 방향 변환
            Vector3 dir = m_Front_Box.transform.position - transform.position;
            Vector3 dirXZ = new Vector3(dir.x, 0f, dir.z);

            if (dirXZ != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirXZ);

                transform.rotation = targetRot;
            }
            //--------------------------------




            // 밀기 애니메이션 실행
            m_TurtleMan_Animator.SetBool("TurtleMan_isPush", true);
            m_Camera_Animator.SetBool("Push", true);


            // 원활한 밀기를 위해..
            m_isPushing = true;
            m_isAbleToPush = false;


            // MCL 갱신
            Vector3 pos = m_Front_Box.transform.position + transform.forward * 2.0f;
            index = StageManager.GetInstance().Find_Own_MCL_Index(pos.x, pos.z);
            m_Front_Box.GetComponent<Box>().Reset_MCL_index(index);
        }
    }


    void Pushing() // 박스를 미는 과정과 마무리
    {
        if (m_Front_Box != null)
        {
            m_push_Distance += m_BasicSpeed * Time.deltaTime;

            if (m_push_Distance <= 2.0f)
            {
                m_Front_Box.transform.position += transform.forward * m_BasicSpeed * Time.deltaTime;
                transform.position += transform.forward * m_BasicSpeed * Time.deltaTime;
            }

            // 밀고난 뒤 박스가 정확한 자리로 가도록 조정해준다.
            else
            {
                int index = StageManager.GetInstance().Find_Own_MCL_Index(m_Front_Box.transform.position.x, m_Front_Box.transform.position.z);

                Vector3 Loc;
                Loc.x = StageManager.GetInstance().m_Map_Coordinate_List[index].x;
                Loc.y = m_Front_Box.transform.position.y;
                Loc.z = StageManager.GetInstance().m_Map_Coordinate_List[index].z;
                m_Front_Box.transform.position = Loc;

                StageManager.GetInstance().Update_MCL_isBlocked(index, true);

                m_push_Distance = 0.0f;
                m_isPushing = false;
                Push_Ani_False();
            }
        }

        else
        {
            m_push_Distance = 0.0f;
            m_isPushing = false;
            Push_Ani_False();
        }
    }


    public void Bomb_Throw() // 폭탄 던지기
    {
        // 플레이어 위치 조정 및 애니메이션 수행
        transform.position = new Vector3(m_Selected_Bomb_For_Throwing.transform.position.x, transform.position.y, m_Selected_Bomb_For_Throwing.transform.position.z);

        float PlayerAngleY = 0.0f;
        if (transform.localEulerAngles.y >= 315.0f && transform.localEulerAngles.y < 45.0f)
            PlayerAngleY = 0.0f;
        else if (transform.localEulerAngles.y >= 45.0f && transform.localEulerAngles.y < 135.0f)
            PlayerAngleY = 90.0f;
        else if (transform.localEulerAngles.y >= 135.0f && transform.localEulerAngles.y < 225.0f)
            PlayerAngleY = 180.0f;
        else if (transform.localEulerAngles.y >= 225.0f && transform.localEulerAngles.y < 315.0f)
            PlayerAngleY = 270.0f;
        transform.localEulerAngles = new Vector3(0.0f, PlayerAngleY, 0.0f);

        m_Selected_Bomb_For_Throwing.GetComponent<Bomb_Remaster>().Throw_Bomb(gameObject);

        m_TurtleMan_Animator.SetBool("TurtleMan_isThrow", true);
        m_Camera_Animator.SetTrigger("Throw");
        Invoke("Throw_Ani_False", 0.3f);
        
        // 선택 폭탄 해제
        m_Selected_Bomb_For_Throwing = null;
        m_isAbleToThrow = false;
        UI.GetInstance().Throw_Button_Management(false);
    }


    void Throw_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isThrow", false);
    }

    void Kick_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isKick", false);
    }

    void Push_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isPush", false);
        m_Camera_Animator.SetBool("Push", false);
        m_Front_Box = null;
    }

    public void SetBomb_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", false);
        m_Camera_Animator.SetBool("Set_Bomb", false);
    }

    //다른 스크립트에서 플레이어를 죽게 하는 함수
    public void Set_Dead()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
        m_Camera_Animator.SetTrigger("Dead");
        m_isAlive = false;
        StageManager.GetInstance().Set_is_Player_Alive(false);
    }

    //살아있는지에 대한 여부를 다른 스크립트에서 get하는 함수-R
    public bool Get_IsAlive()
    {
        return m_isAlive;
    }

    //게임오버 애니메이션 함수
    public void MakeGameOverAni()
    {
        m_Camera_Animator.SetTrigger("Dead");
    }

    public void AniBomb_Start()
    {
        m_Camera_Animator.SetTrigger("Ring");
        if (!Audio_Manager.GetInstance().Get_is_Vibration_Mute())
        {
#if UNITY_ANDROID
            Handheld.Vibrate(); // 1초간 진동
#endif
        }
    }

    public void UI_Status_Update()
    {
        UI.GetInstance().Stat_UI_Management(m_Curr_Bomb_Count, m_Max_Bomb_Count, m_Fire_Count, m_Speed_Count);
    }
}
