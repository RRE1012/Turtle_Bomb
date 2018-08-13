using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//define과 같은 개념. 상수 
static class MAX_VALUE_ITEM
{
    public const int retval = 8;
}

// 캐릭터 행동 관련 클래스
public class PlayerMove : MonoBehaviour {
	public GameObject m_Bomb;
    public GameObject[] m_DropBomb;
    public static PlayerMove C_PM;
    public Animator animator_camera;
    public GameObject m_Front_Box;
    Animator m_TurtleMan_Animator;
    GameObject m_Selected_Bomb_For_Throwing;

    bool m_YouCanSetBomb = true;
    SkinnedMeshRenderer[] m_Player_Mesh_Renderers;

    // 캐릭터 상태
    public bool m_isBoxSelected = false;
    bool m_isCrouch = false;
    bool m_isPushing = false;

    bool m_isPress_LShift = false;
    bool m_isClicked_CrouchButton = false;

    bool m_isGot_KickItem = false;
    bool m_isGot_ThrowItem = false;
    public bool m_isAbleToPush = false;
    public static bool m_isHideinBush = false;
    public bool m_isAbleToKick = false;
    public static bool m_isAbleToThrow = false;


    int m_Touch_Num = 0;
    bool m_is_Touch_Started = false;

    // 이전 터치 지점 (x값)
    float m_Touch_PrevPoint_X;

    // 회전 감도
    float m_RotateSensX = 150.0f;

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

    // 박스 밀기 거리
    float m_push_Distance = 0.0f;


    // ======== Functions ========
    void Awake()
	{
        C_PM = this;
        m_TurtleMan_Animator = GetComponent<Animator>();
        m_Player_Mesh_Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        StageManager.c_Stage_Manager.Set_is_Player_Alive(true);
    }

    public void Player_Set_Start_Point(Vector3 p)
    {
        transform.position = p; // 플레이어 위치를 시작지점으로 옮긴다.
    }

    void Update ()
    {
        if (!StageManager.c_Stage_Manager.Get_is_Pause() && StageManager.c_Stage_Manager.Get_is_Intro_Over())
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
                if (UI.m_cur_Max_Bomb_Count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_cur_Max_Bomb_Count += 1;
                    UI.m_releasable_bomb_count += 1;
                    UI.c_UI.GetItemUI_Activate(0);
                }
            }
            if (other.gameObject.CompareTag("Fire_Item"))
            {
                Destroy(other.gameObject);
                if (UI.m_fire_count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_fire_count += 1;
                    UI.c_UI.GetItemUI_Activate(1);
                }
            }
            if (other.gameObject.CompareTag("Speed_Item"))
            {
                Destroy(other.gameObject);
                if (UI.m_speed_count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_speed_count += 1;
                    UI.c_UI.GetItemUI_Activate(2);
                }
            }
            if (other.gameObject.CompareTag("Kick_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_KickItem)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    m_isGot_KickItem = true;
                    UI.c_UI.Kick_Icon_Activate();
                    UI.c_UI.GetItemUI_Activate(3);
                }
            }
            if (other.gameObject.CompareTag("Throw_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_ThrowItem)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    m_isGot_ThrowItem = true;
                    UI.c_UI.Throw_Icon_Activate();
                    UI.c_UI.GetItemUI_Activate(4);
                }
            }

            if (other.gameObject.CompareTag("Airdrop_Item"))
            {
                Destroy(other.gameObject);
                MusicManager.manage_ESound.ItemGetSound();

                //int temp = Random.Range(1, 3);

                UI.m_cur_Max_Bomb_Count += 1; //temp;
                UI.m_releasable_bomb_count += 1; // temp;
                if (UI.m_cur_Max_Bomb_Count > MAX_VALUE_ITEM.retval)
                {
                    UI.m_cur_Max_Bomb_Count = MAX_VALUE_ITEM.retval;
                    UI.m_releasable_bomb_count = MAX_VALUE_ITEM.retval;
                }

                //temp = Random.Range(1, 3);
                UI.m_fire_count += 1; // temp;
                if (UI.m_fire_count > MAX_VALUE_ITEM.retval)
                    UI.m_fire_count = MAX_VALUE_ITEM.retval;

                //temp = Random.Range(1, 3);
                UI.m_speed_count += 1; // temp;
                if (UI.m_speed_count > MAX_VALUE_ITEM.retval)
                    UI.m_speed_count = MAX_VALUE_ITEM.retval;

                UI.c_UI.GetItemUI_Activate(5);
            }
            // ========================


            // 사망 판정
            // 스테이지 클리어시, 맵 이동시는 사망하지 않는다.
            if ((other.gameObject.tag == "Flame" ||
                other.gameObject.tag == "Flame_Bush" ||
                other.gameObject.tag == "Monster_Attack_Collider" ||
                other.gameObject.tag == "icicle_Body") && !StageManager.c_Stage_Manager.Get_is_Stage_Clear())
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
                StageManager.c_Stage_Manager.Next_Map_Load();


                // 맵 이동 마무리 연출 수행 (Fade In 먼저 수행)
                // Do();
            }
            // ===============================





            // 목표 지점 도달 시 스테이지 클리어
            if (other.gameObject.CompareTag("Goal"))
            {
                StageManager.c_Stage_Manager.SetGoalIn(true);
                StageManager.c_Stage_Manager.Stage_Clear();
            }
            // ===============================
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 부쉬 관련
        if (other.gameObject.tag == "Bush")
        {
            foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                pmr.enabled = false;
            m_isHideinBush = true;
        }
        // ===================

    }

    void OnTriggerExit(Collider other)
    {
        // 폭탄 트리거 비활성
        if (other.gameObject.tag == "Bomb")
        {
            m_isAbleToThrow = false;
            other.isTrigger = false;
            m_Selected_Bomb_For_Throwing = null;
        }
        // =====================



        // 부쉬를 벗어날 경우
        if (other.gameObject.tag == "Bush")
        {
            foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                pmr.enabled = true;
            m_isHideinBush = false;
        }
        // =======================

    }


    void OnCollisionEnter(Collision collision)
    {
        // 폭탄 발차기
        if (m_isGot_KickItem && collision.gameObject.CompareTag("Bomb"))
        {
            // 현재 캐릭터가 발차기가 가능한 상태이면서, 폭탄은 던져진 상태가 아니어야 한다.
            if (m_isAbleToKick && !collision.gameObject.GetComponent<Bomb>().m_isThrown)
            {
                collision.gameObject.GetComponent<Bomb>().m_isKicked = true;
                collision.gameObject.GetComponent<Bomb>().SetBombDir(gameObject);
                
                m_isAbleToKick = false;

                m_TurtleMan_Animator.SetBool("TurtleMan_isKick", true);
                Invoke("Kick_Ani_False", 0.4f);
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
            transform.Translate(new Vector3(0.0f, 0.0f, ((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime)));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime)));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime), 0.0f, 0.0f));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime), 0.0f, 0.0f));
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
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
            transform.Translate((m_BasicSpeed + UI.m_speed_count) * normal * Time.deltaTime);
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }

        
        else
        {
            // 릴리즈 빌드할 때는 적용할 것!
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
        if (Input.touchCount > 0) // 터치가 감지됨.
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
    


    public void SetBomb() // 폭탄 설치
    {
        if (UI.m_releasable_bomb_count > 0 && StageManager.c_Stage_Manager.Get_is_Intro_Over())
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

            // 폭탄을 놓을 수 있다면 폭탄 설치
            if (m_YouCanSetBomb)
            {
                MusicManager.manage_ESound.BombSetSound();
                GameObject Instance_Bomb = Instantiate(m_Bomb);

                //폭탄 위치 수정 -R
                Instance_Bomb.transform.position = new Vector3(m_BombLocX, -0.2f, m_BombLocZ);

                // 주인 마킹
                Instance_Bomb.GetComponent<Bomb>().m_Whose_Bomb = gameObject;
                Instance_Bomb.GetComponent<Bomb>().m_Whose_Bomb_Type = WHOSE_BOMB.PLAYER;

                UI.m_releasable_bomb_count = UI.m_releasable_bomb_count - 1;
                m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", true);
                animator_camera.SetBool("Set_Bomb", true);
                Invoke("SetBomb_Ani_False", 0.3f);

                // 던지기 아이템을 획득한 상태라면
                if (m_isGot_ThrowItem)
                {
                    m_isAbleToThrow = true;
                    m_Selected_Bomb_For_Throwing = Instance_Bomb;
                }
            }

        }
    }


    void CrouchForPC()
    {
        if (!m_isClicked_CrouchButton)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_isCrouch = true;
                m_isPress_LShift = true;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", true);
                animator_camera.SetBool("Crouch", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                m_isCrouch = false;
                m_isPress_LShift = false;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", false);
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
                animator_camera.SetBool("Crouch", false);
            }
        }
    }

    public void Crouch() // 플레이어 숙이기
    {
        if(StageManager.c_Stage_Manager.Get_is_Intro_Over() && !m_isPress_LShift)
        {
            if (CrouchButton.m_isClicked)
            {
                m_isCrouch = true;
                m_isClicked_CrouchButton = true;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", true);
                animator_camera.SetBool("Crouch", true);
            }
            else
            {
                m_isCrouch = false;
                m_isClicked_CrouchButton = false;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", false);
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
                animator_camera.SetBool("Crouch", false);
            }
        }
    }

    
    public void BoxPush() // 박스 밀기 시작
    {
        if (m_isBoxSelected && m_isAbleToPush && StageManager.c_Stage_Manager.Get_is_Intro_Over())
        {
            // 플레이어 위치 변환

            // 플레이어의 현재 MCL 인덱스를 찾는다.
            int index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);

            // 플레이어의 위치를 MCL 내부 좌표로 이동시킨다.
            Vector3 Loc;
            Loc.x = StageManager.c_Stage_Manager.m_Map_Coordinate_List[index].x;
            Loc.y = transform.position.y;
            Loc.z = StageManager.c_Stage_Manager.m_Map_Coordinate_List[index].z;
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
            animator_camera.SetBool("Push", true);


            // 원활한 밀기를 위해..
            m_isPushing = true;
            m_isAbleToPush = false;


            // MCL 갱신
            index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(m_Front_Box.transform.position.x, m_Front_Box.transform.position.z);

            StageManager.c_Stage_Manager.Update_MCL_isBlocked(index, false);
        }
    }

    
    void Pushing() // 박스를 미는 과정과 마무리
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
            int index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(m_Front_Box.transform.position.x, m_Front_Box.transform.position.z);

            Vector3 Loc;
            Loc.x = StageManager.c_Stage_Manager.m_Map_Coordinate_List[index].x;
            Loc.y = m_Front_Box.transform.position.y;
            Loc.z = StageManager.c_Stage_Manager.m_Map_Coordinate_List[index].z;
            m_Front_Box.transform.position = Loc;

            StageManager.c_Stage_Manager.Update_MCL_isBlocked(index, true);

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

        m_TurtleMan_Animator.SetBool("TurtleMan_isThrow", true);
        animator_camera.SetTrigger("Throw");
        Invoke("Throw_Ani_False", 0.3f);

        // 폭탄 방향 및 상태 설정
        m_Selected_Bomb_For_Throwing.transform.Rotate(0.0f, transform.localEulerAngles.y, 0.0f);
        m_Selected_Bomb_For_Throwing.GetComponent<Bomb>().m_is_Thrown_Bomb_Moving = true;
        m_Selected_Bomb_For_Throwing.GetComponent<Bomb>().m_is_Rising_Start = true;
        m_Selected_Bomb_For_Throwing.GetComponent<Bomb>().m_isThrown = true;
        m_Selected_Bomb_For_Throwing.GetComponent<Bomb>().m_isKicked = false;
        m_Selected_Bomb_For_Throwing.GetComponentInChildren<MeshRenderer>().enabled = false;

        // 선택 폭탄 해제
        m_Selected_Bomb_For_Throwing = null;
        m_isAbleToThrow = false;
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
        animator_camera.SetBool("Push", false);
        m_Front_Box = null;
    }

    public void SetBomb_Ani_False()
    {
        m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", false);
        animator_camera.SetBool("Set_Bomb", false);
    }
    
    //다른 스크립트에서 플레이어를 죽게 하는 함수
    public void Set_Dead()
	{
        m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
        animator_camera.SetTrigger("Dead");
        m_isAlive = false;
        StageManager.c_Stage_Manager.Set_is_Player_Alive(false);
    }

    //살아있는지에 대한 여부를 다른 스크립트에서 get하는 함수-R
    public bool Get_IsAlive()
	{
		return m_isAlive;
	}

    //게임오버 애니메이션 함수
    public void MakeGameOverAni()
    {
        animator_camera.SetTrigger("Dead");
    }
    public void AniBomb_Start()
    {
        animator_camera.SetTrigger("Ring");
    }

    //다른 스크립트에서 폭탄을 장전할 때 쓰는 함수
    public void ReloadUp()
    {
        if (UI.m_releasable_bomb_count < MAX_VALUE_ITEM.retval)
            UI.m_releasable_bomb_count += 1;
    }
}
