using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//define과 같은 개념. 상수 
static class MAX_VALUE_ITEM
{
    public const int retval = 8;
}

//캐릭터 움직임 관련 클래스
public class PlayerMove : MonoBehaviour {
	public GameObject m_Bomb;
	public GameObject[] m_Fire;
    public GameObject[] m_DropBomb;
    public static PlayerMove C_PM;
    public Animator animator_camera;
    public Animation character_anima;
    GameObject m_Front_Box;
    Animator m_TurtleMan_Animator;

    bool m_YouCanSetBomb = true;
    SkinnedMeshRenderer[] m_Player_Mesh_Renderers;

    // 캐릭터 상태
    bool m_isBoxSelected = false;
    bool m_isCrouch = false;
    bool m_isPushing = false;
    public static bool m_isAbleToPush = false;
    public static bool m_isHideinBush = false;

    //마우스 클릭
    bool m_isClicked = false;

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

    float m_push_Distance = 0.0f;

    // ======== Functions ========
    void Awake()
	{
		C_PM = this;
        m_TurtleMan_Animator = GetComponent<Animator>();
        m_Player_Mesh_Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Invoke("MakeAnimEnd", 6.0f); // 5.5초 후에 애니메이션 한번만 실행되도록 설정
	}

    void Update ()
    {
        if (m_isAlive && !StageManager.m_is_Stage_Clear && animator_camera.GetBool("Started"))
        {
            if (!m_isPushing)
                Move();
            else
                Pushing();

            
            Crouch();


            if (!m_isCrouch)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    SetBomb();
                if (m_isAbleToPush && !m_isPushing && Input.GetKeyDown(KeyCode.R))
                    BoxPush();
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // 아이템 획득 관련
        if (other.gameObject.CompareTag("Bomb_Item") && animator_camera.GetBool("Started"))
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
        // ========================


        // 화염과 접촉 시 사망 판정
        if ((other.gameObject.tag == "Flame" || other.gameObject.tag == "Flame_Bush") && !StageManager.m_is_Stage_Clear)
        {
            m_isAlive = false;
            m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
        }
        // ========================




        // 목표 지점 도달 시 스테이지 클리어
        if (other.gameObject.CompareTag("Goal"))
        {
            // 목표까지 이동시 별 획득
            StageManager.m_Stars += 1;

            if (StageManager.m_Left_Monster <= 1)
                StageManager.m_Stars += 1;

            // 일정 시간 내에 클리어시 별 획득
            // 일단 기본 5초로 설정.
            // 추후 변수로 할당할것.
            if (UI.time_Second >= 5.0f)
                StageManager.m_Stars += 1;

            StageManager.StageClear();
        }
        // ===============================
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
            other.isTrigger = false;
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
                    Debug.Log("BoxSelected");

                    m_Front_Box = collision.gameObject;
                    m_isBoxSelected = true;
                    m_isAbleToPush = true;
                }
            }
        }
        // ===========================



        // 몬스터와 접촉시 사망
        if (collision.gameObject.tag == "Monster")
        {
            m_isAlive = false;
            m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
        }
        // ====================

    }


    void OnCollisionExit(Collision collision)
    {
        if (m_isBoxSelected && collision.gameObject.CompareTag("Box"))
        {
            m_isBoxSelected = false;
        }
    }



    // ======== UDF =========

    void Move() 
    {
        // 플레이어 이동 관련
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
            transform.Translate(new Vector3((m_BasicSpeed + UI.m_speed_count)* inputx* inputx_minus * Time.deltaTime, 0.0f, (m_BasicSpeed + UI.m_speed_count)* inputz* inputz_minus * Time.deltaTime));
        }
        //*JoyStickMove.instance.GetJoyPosX() * JoyStickMove.instance.GetJoyPosZ()
        // ==================



        // 플레이어 앉은 채로 움직이기 관련 (애니메이션)
        if (m_isCrouch)
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
        }
        // ===================


        

        // 플레이어 회전 관련
        if (Input.GetMouseButton(0) && m_isClicked)
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
        }
        // ====================
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
        if (UI.m_releasable_bomb_count > 0 && animator_camera.GetBool("Started"))
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
                Instance_Bomb.transform.position = new Vector3(m_BombLocX, 0.35f, m_BombLocZ);

                // MCL 수정
                //int index = StageManager.Find_Own_MCL_Index(m_BombLocX, m_BombLocZ, false);
                //StageManager.Update_MCL_isBlocked(index, true);

                UI.m_releasable_bomb_count = UI.m_releasable_bomb_count - 1;
                m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", true);
                Invoke("SetBomb_Ani_False", 0.2f);
            }

        }
    }




    public void Crouch() // 플레이어 앉기
    {
        if(animator_camera.GetBool("Started"))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_isCrouch = true;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", true);
                animator_camera.SetBool("Crouch", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                m_isCrouch = false;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", false);
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
                animator_camera.SetBool("Crouch", false);
            }
        }
    }




    public void BoxPush() // 박스 밀기 시작
    {
        if (m_isBoxSelected && m_isAbleToPush && animator_camera.GetBool("Started"))
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




            // 플레이어 방향 변환 (수정할것)

            // 플레이어로부터 박스의 방향 벡터를 구한다.
            var heading = m_Front_Box.transform.position - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;

            // 플레이어의 방향 벡터와 위의 벡터를 내적한다.
            float Dot = Vector3.Dot(transform.forward, direction);

            // 내적값을 통해 두 벡터 사잇각을 구한다.
            float Angle = Mathf.Acos(Dot);
            Angle *= Mathf.Rad2Deg;
            //Debug.Log(Angle);

            // 구한 사잇각만큼 플레이어를 회전시킨다.
            transform.Rotate(transform.up, Angle);
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

    //시작 애니메이션의 종료 여부를 획득 하는 함수
    public bool GetAnimBool()
    {
        return animator_camera.GetBool("Started");
    }

    //시작 카메라 이동 애니메이션 반복 종료 함수
    void MakeAnimEnd()
    {
        animator_camera.SetBool("Started", true);
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
