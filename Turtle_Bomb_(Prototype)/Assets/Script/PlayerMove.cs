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
    
    //마우스 클릭
    bool m_isClicked = false;

    // 이전 터치 지점 (x값)
    float m_Touch_PrevPoint_X;

    // 회전 감도
    float m_RotateSensX = 150.0f;

    // 회전 각
    float m_RotationX = 0.0f;

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


    // 맵이동애니메이션 관련

    float m_MPA_Elapsed_Time = 0.0f;
    float m_MPA_Rising_Speed = 5.0f;
    float m_MPA_Rotating_Speed = 3.0f;


    // ======== Functions ========
    void Awake()
	{
        C_PM = this;
        m_TurtleMan_Animator = GetComponent<Animator>();
        m_Player_Mesh_Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Vector3 pos;
        pos.x = transform.position.x;
        pos.y = transform.position.y + 2.0f;
        pos.z = transform.position.z + 0.7f;
        StageManager.c_Stage_Manager.m_CameraOffset.transform.position = pos;
        
	}

    public void Player_Set_Start_Point(Vector3 p)
    {
        transform.position = p;
    }

    void Update ()
    {
        if (m_isAlive && !StageManager.m_is_Stage_Clear && StageManager.c_Stage_Manager.m_is_Intro_Over && !StageManager.c_Stage_Manager.m_is_Pause)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);

            if (!m_isPushing)
            {
                Move();
                BodyRotation();
            }
            else
                Pushing();

            Crouch();
            CrouchForPC();

            if (!m_isCrouch)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    SetBomb();

                if (m_isAbleToPush && !m_isPushing && Input.GetKeyDown(KeyCode.R))
                {
                    if (m_Front_Box != null)
                        BoxPush();
                    else
                    {
                        m_isBoxSelected = false;
                        m_isAbleToPush = false;
                    }
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (m_isAlive)
        {
            // 아이템 획득 관련
            if (other.gameObject.CompareTag("Bomb_Item") && StageManager.c_Stage_Manager.m_is_Intro_Over)
            {
                Destroy(other.gameObject);
                if (UI.m_cur_Max_Bomb_Count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_cur_Max_Bomb_Count += 1;
                    UI.m_releasable_bomb_count += 1;
                    UI.m_getItemText = "Bomb UP~!";
                }
            }
            if (other.gameObject.CompareTag("Fire_Item"))
            {
                Destroy(other.gameObject);
                if (UI.m_fire_count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_fire_count += 1;
                    UI.m_getItemText = "Fire UP~!";
                }
            }
            if (other.gameObject.CompareTag("Speed_Item"))
            {
                Destroy(other.gameObject);
                if (UI.m_speed_count < MAX_VALUE_ITEM.retval)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    UI.m_speed_count += 1;
                    UI.m_getItemText = "Speed UP~!";
                }
            }
            if (other.gameObject.CompareTag("Kick_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_KickItem)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    m_isGot_KickItem = true;
                    UI.m_getItemText = "Kick Activated~!";
                }
            }
            if (other.gameObject.CompareTag("Throw_Item"))
            {
                Destroy(other.gameObject);
                if (!m_isGot_ThrowItem)
                {
                    MusicManager.manage_ESound.ItemGetSound();
                    m_isGot_ThrowItem = true;
                    UI.m_getItemText = "Throw Activated~!";
                }
            }
            // ========================


            // 사망 판정
            // 스테이지 클리어시, 맵 이동시는 사망하지 않는다.
            if ((other.gameObject.tag == "Flame" || other.gameObject.tag == "Flame_Bush" || other.gameObject.tag == "Monster_Attack_Collider") && !StageManager.m_is_Stage_Clear)
            {
                m_isAlive = false;
                m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
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
        /*
        // 박스와 접촉시 밀기 활성화
        if (!m_isBoxSelected && collision.gameObject.CompareTag("Box"))
        {
            Vector3 tmpPosition;
            tmpPosition = collision.gameObject.transform.position + transform.forward * 1.2f;
            int index = StageManager.Find_Own_MCL_Index(tmpPosition.x, tmpPosition.z, false);
            if (index != -1)
            {
                if (StageManager.m_Map_Coordinate_List[index].isBlocked == false)
                {
                    m_Front_Box = collision.gameObject;
                    m_isBoxSelected = true;
                    m_isAbleToPush = true;
                }
            }
        }
        */
        // ===========================
        


        // 폭탄 발차기
        if (m_isGot_KickItem && collision.gameObject.CompareTag("Bomb"))
        {
            // 현재 캐릭터가 발차기가 가능한 상태이면서, 폭탄은 던져진 상태가 아니어야 한다.
            if (m_isAbleToKick && !collision.gameObject.GetComponent<Bomb>().m_isThrown)
            {
                collision.gameObject.GetComponent<Bomb>().SetBombDir();
                collision.gameObject.GetComponent<Bomb>().m_isKicked = true;
                m_isAbleToKick = false;

                m_TurtleMan_Animator.SetBool("TurtleMan_isKick", true);
                Invoke("Kick_Ani_False", 0.4f);
            }
        }

    }

    /*
    void OnCollisionExit(Collision collision)
    {
        // 박스와 접촉 해제시 밀기 비활성화

        if (m_isBoxSelected && collision.gameObject.CompareTag("Box"))
        {
            m_isBoxSelected = false;
            m_isAbleToPush = false;
        }

    }
    */


    // ======== UDF =========

    void Move() 
    {
        // 플레이어 이동 관련
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, ((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime)));

            if (m_isCrouch)
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
            else 
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime)));

            if (m_isCrouch)
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
            else
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime), 0.0f, 0.0f));

            if (m_isCrouch)
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
            else
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(((m_BasicSpeed + UI.m_speed_count) * Time.deltaTime), 0.0f, 0.0f));

            if (m_isCrouch)
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
            else
                m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", true);
        }


        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
            m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
        }

        // ===================




        // 플레이어 이동 관련 (조이스틱)
        if (JoyStickMove.instance.GetJoyPosX()!=0 || JoyStickMove.instance.GetJoyPosZ() != 0)
        {
            //inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            float inputx = (JoyStickMove.instance.GetJoyPosX() >= 25.0f|| JoyStickMove.instance.GetJoyPosX() <= -25.0f) ? 1.0f : 0.0f;
            float inputz = (JoyStickMove.instance.GetJoyPosZ() >= 25.0f|| JoyStickMove.instance.GetJoyPosZ() <= 25.0f) ? 1.0f : 0.0f;
            float inputx_minus=0.0f;
            float inputz_minus=0.0f;
            if (inputx!=0)
                inputx_minus = (JoyStickMove.instance.GetJoyPosX() <= -25.0f) ? -1.0f : 1.0f;
            if (inputz != 0)
                inputz_minus = (JoyStickMove.instance.GetJoyPosZ() <= 25.0f) ? -1.0f : 1.0f;
            transform.Translate(new Vector3((m_BasicSpeed + UI.m_speed_count) * inputx * inputx_minus * Time.deltaTime, 0.0f, (m_BasicSpeed + UI.m_speed_count)* inputz* inputz_minus * Time.deltaTime));
        }
        //*JoyStickMove.instance.GetJoyPosX() * JoyStickMove.instance.GetJoyPosZ()
        // ==================


        

        // 플레이어 회전 관련
        
        if (Input.GetMouseButton(0) && m_isClicked)
        {
            m_RotationX = Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.Rotate(transform.up, m_RotationX);
        }
        // ====================
    }

    // 실제환경에서 테스트 해야한다...
    // (PC로는 안됨)
    void BodyRotation()
    {
        if (Input.touchCount >= 1)
        {
            Debug.Log("Touched!");
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                m_Touch_PrevPoint_X = Input.GetTouch(0).position.x;
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Debug.Log("Rotate");
                transform.Rotate(0, Input.GetTouch(0).position.x - m_Touch_PrevPoint_X, 0);
                m_Touch_PrevPoint_X = Input.GetTouch(0).position.x;
            }
        }
    }

    public void isClicked()
    {
        m_isClicked = true;
    }
    public void isClickedOff()
    {
        m_isClicked = false;
    }


    public void SetBomb() // 폭탄 설치
    {
        if (UI.m_releasable_bomb_count > 0 && !m_isCrouch)
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
        if(StageManager.c_Stage_Manager.m_is_Intro_Over && !m_isPress_LShift)
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
        if (m_isBoxSelected && m_isAbleToPush && StageManager.c_Stage_Manager.m_is_Intro_Over && !m_isCrouch)
        {
            // 플레이어 위치 변환

            // 플레이어의 현재 MCL 인덱스를 찾는다.
            int index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z);

            // 플레이어의 위치를 MCL 내부 좌표로 이동시킨다.
            Vector3 Loc;
            Loc.x = StageManager.m_Map_Coordinate_List[index].x;
            Loc.y = transform.position.y;
            Loc.z = StageManager.m_Map_Coordinate_List[index].z;
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
            index = StageManager.Find_Own_MCL_Index(m_Front_Box.transform.position.x, m_Front_Box.transform.position.z, true);

            StageManager.Update_MCL_isBlocked(index, false);
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
            int index = StageManager.Find_Own_MCL_Index(m_Front_Box.transform.position.x, m_Front_Box.transform.position.z, false);

            Vector3 Loc;
            Loc.x = StageManager.m_Map_Coordinate_List[index].x;
            Loc.y = m_Front_Box.transform.position.y;
            Loc.z = StageManager.m_Map_Coordinate_List[index].z;
            m_Front_Box.transform.position = Loc;

            StageManager.Update_MCL_isBlocked(index, true);

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
        m_Selected_Bomb_For_Throwing.GetComponent<MeshRenderer>().enabled = false;

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
    }
    
    //다른 스크립트에서 플레이어를 죽게 하는 함수-R
    public void Set_Dead()
	{
        m_isAlive = false;
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
        UI.m_releasable_bomb_count += 1;
    }
}
