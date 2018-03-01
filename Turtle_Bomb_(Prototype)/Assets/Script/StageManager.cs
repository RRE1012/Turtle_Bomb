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


// #define 맵 생성 관련
static class MAP
{
    public const int NOT_SET = -1; // 설정이 안됐음

    public const int THEME_FOREST = 1;
    public const int THEME_OCEAN = 2;
    public const int THEME_SNOWLAND = 3;
}


public class StageManager : MonoBehaviour {

    // 맵 사이즈
    int m_Map_Size_X = 17;
    int m_Map_Size_Z = 17;

    // 프리팹들
    public GameObject m_Forest_Theme_Terrain;
    public GameObject m_Ocean_Theme_Terrain;
    public GameObject m_SnowLand_Theme_Terrain;

    public GameObject[] m_Forest_Theme_Stage_1_Maps;
    public GameObject[] m_Forest_Theme_Stage_2_Maps;
    public GameObject[] m_Forest_Theme_Stage_3_Maps;

    
    public GameObject[] m_Ocean_Theme_Stage_1_Maps;
    public GameObject[] m_Ocean_Theme_Stage_2_Maps;
    public GameObject[] m_Ocean_Theme_Stage_3_Maps;

    
    public GameObject[] m_SnowLand_Theme_Stage_1_Maps;
    public GameObject[] m_SnowLand_Theme_Stage_2_Maps;
    public GameObject[] m_SnowLand_Theme_Stage_3_Maps;

    // 현재 생성된 프리팹
    GameObject m_Current_Terrain;
    GameObject m_Current_Map;

    // 맵(프리팹) 배열 임시 보관소
    GameObject[] m_Temp_Maps;

    // ??
    public Text m_Text;
    
    // 현재 맵 번호 (인덱스)
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


    // 맵 이동 시 폭탄을 무효화 하기 위한 변수
    public bool m_is_Map_Changing;

    // 현재 스테이지 번호
    public int m_Theme_Num;
    public int m_Stage_Num;

    // 현재 스테이지의 제한시간 설정
    // -1로 설정할 경우 기획 설정에 따른다.
    public float m_Stage_Time_Limit;


    // 현재 스테이지가 보스전인지?
    public bool m_is_Boss_Stage;

    // 서든데스 고블맨 객체
    public GameObject m_SuddenDeath_JetGoblin;

    // 서든데스 고블맨을 소환했는가? 
    bool m_is_Summon_SJG;

    // 서든데스 활성화?
    public bool m_is_SuddenDeath;

    // 보스 스테이지의 보스 몬스터가 죽었는가?
    public bool m_is_Boss_Dead;


    // 현재 스테이지의 퀘스트
    List<Adventure_Quest_Data> m_QuestList;


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

        m_is_SuddenDeath = false;
        m_is_Summon_SJG = false;

        // 맵 좌표 리스트 초기화
        MCL_init();

        // 테마번호와 스테이지번호를 설정
        if (m_Theme_Num == MAP.NOT_SET)
            m_Theme_Num = PlayerPrefs.GetInt("Mode_Adventure_Selected_Theme_Number");
        if (m_Stage_Num == MAP.NOT_SET)
            m_Stage_Num = PlayerPrefs.GetInt("Mode_Adventure_Selected_Stage_Number");

        // 설정된 번호를 받아서 맵 생성
        Create_Map(m_Theme_Num, m_Stage_Num);


        // 퀘스트 목록 로딩
        m_QuestList = new List<Adventure_Quest_Data>();
        m_QuestList = CSV_Manager.GetInstance().Get_Adventure_Quest_List(m_Theme_Num, m_Stage_Num);
    }




    void Update()
    {
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            m_Text.text = "Game Over";
        }
        if (!m_is_Boss_Stage && m_is_SuddenDeath && !m_is_Summon_SJG)
        {
            Instantiate(m_SuddenDeath_JetGoblin);
            m_is_Summon_SJG = true;
        }
	}




    // 맵 생성 및 설정
    void Create_Map(int theme_Num, int stage_Num)
    {
        float temp_TimeLimit_Debug = m_Stage_Time_Limit;
        

        switch(theme_Num)
        {
            // 숲 테마
            case MAP.THEME_FOREST:

                // 터레인 생성
                m_Current_Terrain = Instantiate(m_Forest_Theme_Terrain);
                
                if (stage_Num == 1)
                {
                    m_Temp_Maps = m_Forest_Theme_Stage_1_Maps;
                    m_Stage_Time_Limit = 90.0f;
                }
                else if (stage_Num == 2)
                {
                    m_Temp_Maps = m_Forest_Theme_Stage_2_Maps;
                    m_Stage_Time_Limit = 90.0f;
                }
                else if(stage_Num == 3)
                {
                    m_Temp_Maps = m_Forest_Theme_Stage_3_Maps;
                    m_is_Boss_Stage = true;
                    m_Stage_Time_Limit = 90.0f;
                }

                // 오브젝트 생성
                m_Current_Map = Instantiate(m_Temp_Maps[m_Current_Map_Number]);

                break;


            // 바다 테마
            case MAP.THEME_OCEAN:

                // 터레인 생성
                m_Current_Terrain = Instantiate(m_Ocean_Theme_Terrain);
                
                if (stage_Num == 1)
                {
                    m_Temp_Maps = m_Ocean_Theme_Stage_1_Maps;
                    m_Stage_Time_Limit = 90.0f;
                }
                else if (stage_Num == 2)
                {
                    m_Temp_Maps = m_Ocean_Theme_Stage_2_Maps;
                    m_Stage_Time_Limit = 90.0f;
                }
                else if (stage_Num == 3)
                {
                    m_Temp_Maps = m_Ocean_Theme_Stage_3_Maps;
                    m_is_Boss_Stage = true;
                    m_Stage_Time_Limit = 90.0f;
                }

                // 오브젝트 생성
                m_Current_Map = Instantiate(m_Temp_Maps[m_Current_Map_Number]);

                break;



            // 설원 테마
            case MAP.THEME_SNOWLAND:

                // 터레인 생성
                m_Current_Terrain = Instantiate(m_SnowLand_Theme_Terrain);

                
                if (stage_Num == 1)
                {
                    m_Temp_Maps = m_SnowLand_Theme_Stage_1_Maps;
                    m_Stage_Time_Limit = 90.0f;
                }
                else if (stage_Num == 2)
                {
                    m_Temp_Maps = m_SnowLand_Theme_Stage_2_Maps;
                    m_Stage_Time_Limit = 80.0f;
                }
                else if (stage_Num == 3)
                {
                    m_Temp_Maps = m_SnowLand_Theme_Stage_3_Maps;
                    m_Stage_Time_Limit = 90.0f;
                    m_is_Boss_Stage = true;
                }

                // 오브젝트 생성
                m_Current_Map = Instantiate(m_Temp_Maps[m_Current_Map_Number]);

                break;
        }

        if (temp_TimeLimit_Debug != MAP.NOT_SET)
        {
            m_Stage_Time_Limit = temp_TimeLimit_Debug;
        }

        // UI에도 적용한다
        UI.time_Second = m_Stage_Time_Limit;
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
        if (index != -1)
        {
            m_tmpCoordinate.x = m_Map_Coordinate_List[index].x;
            m_tmpCoordinate.z = m_Map_Coordinate_List[index].z;
            m_tmpCoordinate.isBlocked = isBlocked;

            m_Map_Coordinate_List.Insert(index, m_tmpCoordinate);
            m_Map_Coordinate_List.RemoveAt(index + 1);
        }
        
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
        m_Current_Map = Instantiate(m_Temp_Maps[m_Current_Map_Number]);



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




    // 맵 전환이 끝났음을 알리는
    // Invoke를 위한 메소드
    void Map_Changing_Over()
    {
        m_is_Map_Changing = false;
    }




    // 스테이지 클리어!
    public void Stage_Clear()
    {
        m_is_Stage_Clear = true;

        // 목표까지 이동시 or 보스 처치시 별 획득
        m_Stars += 1;

        // 다른 조건 체크
        Condition_For_Getting_Stars();

        // 별 개수 저장 및 플레이 가능 스테이지 증가
        string tempString = "Mode_Adventure_Stage_" + m_Theme_Num + "-" + m_Stage_Num + "_Stars";
        PlayerPrefs.SetInt(tempString, m_Stars);

        // 현재 스테이지가 플레이 가능 최대 스테이지라면
        int tempMaxStage = PlayerPrefs.GetInt("Mode_Adventure_Playable_Max_Stage");

        if ((m_Theme_Num - 1) * 3 + m_Stage_Num == tempMaxStage)
        {
            tempMaxStage += 1;

            // 최대 스테이지 범위 내에서 플레이가능 최대 스테이지 증가!
            if (tempMaxStage <= PlayerPrefs_Manager_Constants.MAX_STAGE_NUM)
            {
                PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", tempMaxStage);
                Debug.Log("SetINT");
            }
        }

        PlayerPrefs.Save();

        // 클리어 화면 출력
        UI.Draw_StageClearPage();
    }


    // 별 획득 조건 관리
    void Condition_For_Getting_Stars()
    {
        foreach(Adventure_Quest_Data QuestData in m_QuestList)
        {
            if (QuestData.isCountable == 1)
            {
                if (QuestData.Quest_Script == "잔여 시간")
                {
                    if (UI.time_Second >= QuestData.Quest_Goal)
                        m_Stars += 1;
                }
                else if (QuestData.Quest_Script == "일반 몬스터 처치")
                {
                    if (m_Total_Monster_Count - m_Left_Monster_Count >= QuestData.Quest_Goal)
                        m_Stars += 1;
                }
                else if (m_is_Boss_Stage && QuestData.Quest_Script == "보스 몬스터 처치")
                {
                    if (m_is_Boss_Dead)
                        m_Stars += 1;
                }
            }
        }
    }


    // 퀘스트 리스트를 리턴해준다.
    public List<Adventure_Quest_Data> GetQuestList()
    {
        return m_QuestList;
    }

}
