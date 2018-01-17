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

    Animator m_TurtleMan_Animator;

    bool m_YouCanSetBomb = true;
    SkinnedMeshRenderer[] m_Player_Mesh_Renderers;

    bool m_isCrouch = false;


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
    float m_BasicSpeed = 5.0f;

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
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_isCrouch = true;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                m_isCrouch = false;
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch", false);
                m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
            }

            Move();

            if (!m_isCrouch && Input.GetKeyDown(KeyCode.Space))
            {
                SetBomb();
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
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

        //화염과 접촉 시 사망 -R
        if (other.gameObject.tag == "Flame" && !StageManager.m_is_Stage_Clear)
        {
            m_isAlive = false;
            m_TurtleMan_Animator.SetBool("TurtleMan_isDead", true);
        }

        // 목표 도달 시 스테이지 클리어
        if (other.gameObject.CompareTag("Goal"))
        {
            // 목표까지 이동시 별 획득
            StageManager.m_Stars += 1;

            if (StageManager.m_Left_Monster <= 0)
                StageManager.m_Stars += 1;

            // 일정 시간 내에 클리어시 별 획득
            // 일단 기본 5초로 설정.
            // 추후 변수로 할당할것.
            if (UI.time_Second >= 5.0f)
                StageManager.m_Stars += 1;

            StageManager.StageClear();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Bush")
        {
            foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                pmr.enabled = false;
        }
    }

    // 폭탄 트리거 비활성
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bomb")
        {
            other.isTrigger = false;
        }

        if (other.gameObject.tag == "Bush")
        {
            foreach (SkinnedMeshRenderer pmr in m_Player_Mesh_Renderers)
                pmr.enabled = true;
        }
    }




    // ======== UDF =========

    void Move() // 플레이어 이동 및 회전
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

        if (m_isCrouch)
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", true);
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            m_TurtleMan_Animator.SetBool("TurtleMan_isWalk", false);
            m_TurtleMan_Animator.SetBool("TurtleMan_isCrouch_Move", false);
        }


        if (Input.GetMouseButton(0))
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
        }

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
                UI.m_releasable_bomb_count = UI.m_releasable_bomb_count - 1;
                m_TurtleMan_Animator.SetBool("TurtleMan_isDrop", true);
                Invoke("SetBomb_Ani_False", 0.2f);
            }

        }
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


    //다른 스크립트에서 폭탄을 장전할 때 쓰는 함수
    public void ReloadUp()
    {
        UI.m_releasable_bomb_count += 1;
    }

}
