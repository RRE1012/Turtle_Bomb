using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// MCL에 들어갈 좌표 구조체
public struct Coordinate
{
    public float x, z;
    public bool isBlocked;
    public Coordinate(float x, float z, bool isBlocked)
    {
        this.x = x;
        this.z = z;
        this.isBlocked = isBlocked;
    }
}
// ========================


public class StageManager : MonoBehaviour {

    int m_Map_Size_X = 17;
    int m_Map_Size_Z = 17;

    public GameObject[] m_Map;
    GameObject m_Current_Map;

    public Text m_Text;
    
    int m_Current_Map_Number = 0;

    public static bool m_is_Stage_Clear = false;
    public static int m_Stars = 0;

    public static int m_Total_Monster_Count = 0;
    public static int m_Left_Monster_Count = 0;

    public static Coordinate m_tmpCoordinate;
    public static Coordinate m_tmpCoord_For_Objects;

    public static List<Coordinate> m_Map_Coordinate_List;

    public static StageManager c_Stage_Manager;
    public static bool m_is_init_MCL;

    // 해당 스테이지의 제한시간
    public float m_Stage_Time_Limit;

    // 맵 이동 시 폭탄을 무효화 하기 위한 변수
    public bool m_is_Map_Changing;

    void Awake()
    {
        m_is_Stage_Clear = false;

        m_Stars = 0;

        m_Total_Monster_Count = 0;
        m_Left_Monster_Count = 0;

        m_is_init_MCL = false;
        m_Map_Coordinate_List = new List<Coordinate>();
        
        c_Stage_Manager = this;
        m_Current_Map_Number = 0;
        m_is_Map_Changing = false;
        
        MCL_init();
        m_Current_Map = Instantiate(m_Map[m_Current_Map_Number]);
        
    }

    void Update()
    {
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            m_Text.text = "Game Over";
        }
	}



    // 좌표 초기화
    void MCL_init()
    { 

        m_Map_Coordinate_List.Clear();

        for (int i = -1; i < m_Map_Size_X-1; ++i)
        {
            for (int j = -1; j < m_Map_Size_Z-1; ++j)
            {
                m_tmpCoordinate.x = i * 2;
                m_tmpCoordinate.z = j * 2 + 50;
                m_tmpCoordinate.isBlocked = false;
                m_Map_Coordinate_List.Add(m_tmpCoordinate);
            }
        }

        for (int i = 0; i < 17; ++i)
        {
            // z라인 울타리
            Update_MCL_isBlocked(i, true);
            Update_MCL_isBlocked(i + 272, true);

            // x라인 울타리
            Update_MCL_isBlocked(i * 17, true);
            Update_MCL_isBlocked(i * 17 + 16, true);
        }

        m_is_init_MCL = true;
    }



    // 오브젝트들이 MCL의 isBlocked를 갱신할 수 있도록 해주는 메소드
    public static void Update_MCL_isBlocked(int index, bool isBlocked)
    {
        m_tmpCoordinate.x = m_Map_Coordinate_List[index].x;
        m_tmpCoordinate.z = m_Map_Coordinate_List[index].z;
        m_tmpCoordinate.isBlocked = isBlocked;

        m_Map_Coordinate_List.Insert(index, m_tmpCoordinate);
        m_Map_Coordinate_List.RemoveAt(index + 1);
    }
    // =============================================



    // 받아온 좌표에 대한 MCL 인덱스 반환 (isBlocked 까지 사용)
    public static int Find_Own_MCL_Index(float x, float z, bool isBlocked)
    {
        // 받아온 좌표에서 가장 가까운 좌표로 설정한다.
        int index_X = (int)x;
        int index_Z = (int)z;
        
        if (index_X % 2 == 1)
        {
            m_tmpCoord_For_Objects.x = index_X + 1;
        }
        else if (index_X % 2 == -1)
        {
            m_tmpCoord_For_Objects.x = index_X - 1;
        }
        else
            m_tmpCoord_For_Objects.x = index_X;


        if (index_Z % 2 == 1)
        {
            m_tmpCoord_For_Objects.z = index_Z + 1;
        }
        else if (index_Z % 2 == -1)
        {
            m_tmpCoord_For_Objects.z = index_Z - 1;
        }
        else
            m_tmpCoord_For_Objects.z = index_Z;

        m_tmpCoord_For_Objects.isBlocked = isBlocked;

        // 속한 좌표의 인덱스를 뽑아서 반환한다.
        return m_Map_Coordinate_List.IndexOf(m_tmpCoord_For_Objects);
    }
    // =====================================



    // 받아온 좌표에 대한 MCL 인덱스 반환
    public static int Find_Own_MCL_Index(float x, float z)
    {

        // 받아온 좌표에서 가장 가까운 좌표로 설정한다.
        int index_X = (int)x;
        int index_Z = (int)z;

        if (index_X % 2 == 1)
        {
            m_tmpCoord_For_Objects.x = index_X + 1;
        }
        else if (index_X % 2 == -1)
        {
            m_tmpCoord_For_Objects.x = index_X - 1;
        }
        else
            m_tmpCoord_For_Objects.x = index_X;


        if (index_Z % 2 == 1)
        {
            m_tmpCoord_For_Objects.z = index_Z + 1;
        }
        else if (index_Z % 2 == -1)
        {
            m_tmpCoord_For_Objects.z = index_Z - 1;
        }
        else
            m_tmpCoord_For_Objects.z = index_Z;


        // 속한 좌표의 인덱스를 뽑아서 반환한다.
        return m_Map_Coordinate_List.IndexOf(m_tmpCoord_For_Objects);
    }
    // ======================================



    public void Next_Map_Load()
    {
        m_is_Map_Changing = true;

        // MCL을 재설정한다.
        MCL_init();
        
        // 현재 맵을 제거
        Destroy(m_Current_Map);

        // 다음 맵 번호를 지정하고
        ++m_Current_Map_Number;

        // 새 맵을 할당
        m_Current_Map = Instantiate(m_Map[m_Current_Map_Number]);



        // 이하는 인스턴스 객체들을 제거한다.
        // =========================================

        // 혹여나 있을 폭탄들 제거
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach(GameObject b in bombs)
            Destroy(b);

        // 불 붙은 부쉬의 파티클도 제거
        GameObject[] fBushs = GameObject.FindGameObjectsWithTag("Flame_Bush_Particle");
        foreach (GameObject fb in fBushs)
            Destroy(fb);

        // 아이템들도 제거
        GameObject[] items = GameObject.FindGameObjectsWithTag("Bomb_Item");
        foreach (GameObject i in items)
            Destroy(i);
        items = GameObject.FindGameObjectsWithTag("Fire_Item");
        foreach (GameObject i in items)
            Destroy(i);
        items = GameObject.FindGameObjectsWithTag("Speed_Item");
        foreach (GameObject i in items)
            Destroy(i);
        items = GameObject.FindGameObjectsWithTag("Kick_Item");
        foreach (GameObject i in items)
            Destroy(i);
        items = GameObject.FindGameObjectsWithTag("Throw_Item");
        foreach (GameObject i in items)
            Destroy(i);
        // ========================================


        Invoke("Map_Changing_Over", 1.0f);
    }

    void Map_Changing_Over()
    {
        m_is_Map_Changing = false;
    }

    public void Stage_Clear()
    {
        m_is_Stage_Clear = true;

        // 목표까지 이동시 별 획득
        m_Stars += 1;

        // 다른 조건 체크
        Condition_For_Getting_Stars();
        
        // 클리어 화면 출력
        UI.Draw_StageClearPage();
    }

    void Condition_For_Getting_Stars()
    {
        // 남은 몬스터가 1마리 이하일 경우
        if (m_Total_Monster_Count - m_Left_Monster_Count >= 5)
            m_Stars += 1;

        // 일정 시간 내에 클리어시 별 획득
        if (UI.time_Second >= 5.0f)
            m_Stars += 1;
    }

}
