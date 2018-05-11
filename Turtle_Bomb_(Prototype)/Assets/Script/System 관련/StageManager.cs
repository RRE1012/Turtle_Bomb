using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// MCL에 들어갈 좌표 구조체
public struct Map_Coordinate
{
    public float x, z;
    public Map_Coordinate(float x, float z)
    {
        this.x = x;
        this.z = z;
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

static class OBJECT_TABLE_NUMBER
{
    public const int BOX = 1;
    public const int ROCK = 2;
    public const int GRASS = 3;
    public const int GOBLIN = 4;
    public const int BOSS_GOBLIN = 5;
    public const int START_POINT = 6;
    public const int NEXT_POINT = 7;
    public const int END_POINT = 8;
    public const int CHARACTER_SPAWN = 9;
    public const int BOSS_GIANT_1 = 10;
    public const int ITEM_BOMB = 11;
    public const int ITEM_SPEED = 12;
    public const int ITEM_FIRE = 13;
    public const int ITEM_KICK = 14;
    public const int ITEM_THROW = 15;
    public const int INFO_TRIGGER_BOMB = 16;
    public const int INFO_TRIGGER_SPEED = 17;
    public const int INFO_TRIGGER_FIRE = 18;
    public const int INFO_TRIGGER_KICK = 19;
    public const int INFO_TRIGGER_THROW = 20;
    public const int BOX_NONE_ITEM = 21;
}

public class StageManager : MonoBehaviour
{
    // ================================
    // =========== 프리팹들 ===========

    public GameObject m_Forest_Theme_Terrain;
    public GameObject m_SnowLand_Theme_Terrain;
    
    //public GameObject m_Prefab_Intro_Boss; // 인트로용 객체

    // 오브젝트 프리팹들
    public GameObject m_Prefab_Box;
    public GameObject m_Prefab_Rock;
    public GameObject m_Prefab_Grass;
    public GameObject m_Prefab_Goblin;
    public GameObject m_Prefab_Goblin_Boss;
    public GameObject m_Prefab_Start_Point;
    public GameObject m_Prefab_Next_Portal;
    public GameObject m_Prefab_End_Portal;
    //public GameObject m_Prefab_Chractrer_Spawn;
    public GameObject m_Prefab_Ork_Boss;
    public GameObject m_Prefab_Airplane;
    public GameObject m_Prefab_Item_Bomb;
    public GameObject m_Prefab_Item_Speed;
    public GameObject m_Prefab_Item_Fire;
    public GameObject m_Prefab_Item_Kick;
    public GameObject m_Prefab_Item_Throw;
    public GameObject m_Prefab_Info_Trigger_Bomb;
    public GameObject m_Prefab_Info_Trigger_Speed;
    public GameObject m_Prefab_Info_Trigger_Fire;
    public GameObject m_Prefab_Info_Trigger_Kick;
    public GameObject m_Prefab_Info_Trigger_Throw;
    public GameObject m_Prefab_Box_None_Item;







    // ================================
    // ===== 생성한 오브젝트 관리 =====

    List<GameObject> m_Current_Map_Objects = new List<GameObject>(); // 테이블을 통해 현재 맵에 생성된 오브젝트들 + 비행기

    int m_Current_Map_Objects_Count; // 생성된 오브젝트 개수

    int m_Current_Stage_index_Count = 0; // 현재 스테이지의 맵 인덱스 카운트

    Vector3 m_Object_Position; // 오브젝트 생성시 위치 변경에 이용할 벡터

    GameObject m_Airplane; // 비행기 객체
    






    // ================================
    // =========== UI 관련 ============

    public Text m_Text; // 게임 오버 텍스트

    int m_Star_Count = 0; // 획득한 별 개수

    int m_Total_Monster_Count = 0; // 총 스폰된 일반 몹 수

    int m_Left_Monster_Count = 0; // 남은 일반 몹 수



    





    // ==================================
    // =========== 보스관련 =============
    
    public bool m_is_Boss_Stage; // 현재 스테이지가 보스전인가? (*** 디버깅용 public ***)

    public int m_Boss_ID; // 보스 테이블 번호 (*** 디버깅용 public ***)

    public GameObject m_SuddenDeath_JetGoblin; // 서든데스 고블맨 객체
    
    bool m_is_Boss_Dead; // 보스 스테이지의 보스 몬스터가 죽었는가?








    // ==================================
    // ========== 시스템관련 ============

    public static StageManager c_Stage_Manager; // 스테이지 매니저 객체

    public List<Map_Coordinate> m_Map_Coordinate_List; // MCL

    public static List<bool> m_MCL_is_Blocked_List; // MCL의 is_Blocked를 따로 분리했다..

    public static bool m_is_init_MCL = false; // MCL이 초기화 되었는가?

    bool m_is_Map_Changing = false; // 맵 이동 시 폭탄을 무효화 하기 위한 변수

    public int m_Stage_ID; // 현재 스테이지 번호 (*** 디버깅용 public ***)

    public float m_Stage_Time_Limit; // 현재 스테이지의 제한시간
                                     // 디버깅용 변수이며, -1로 설정할 경우 기획 설정(테이블)에 따른다

    public bool m_is_Intro_Over = false; // 인트로가 끝났는가?

    public bool m_is_Pause = false; // 게임이 일시정지 되었는가?

    bool m_is_Goal_In = false; // 목표 지점에 들어갔는가?

    bool m_is_Stage_Clear = false; // 스테이지를 클리어했는가?

    public bool m_Game_Over = false; // 게임이 끝났는가?

    // 맵 사이즈
    int m_Map_Size_X = 17;
    int m_Map_Size_Z = 17;

    public GameObject m_CameraOffset; // 퍼포먼스 카메라 객체
    







    // =========== CSV 관련 ===========
    // ================================

    // 현재 스테이지의 퀘스트 목록
    List<Adventure_Quest_Data> m_QuestList = new List<Adventure_Quest_Data>();

    // 오브젝트 테이블
    List<Object_Table_Data> m_Object_Table_List;

    // 현재 스테이지의 오브젝트 배치 목록
    List<Object_Spawn_Position_Data> m_Object_Spawn_Position_List = new List<Object_Spawn_Position_Data>();

    // 스크립트(대사) 테이블
    List<Script_Data> m_Script_List = new List<Script_Data>();

    // 보스 스테이터스 데이터
    Adventure_Boss_Data m_Adventure_Boss_Data = new Adventure_Boss_Data();

    Adventure_Stage_Data m_Adventure_Stage_Data = new Adventure_Stage_Data();

    // 빅보스 AI 데이터들
    Adventure_Big_Boss_Normal_Mode_AI_Data m_Adv_Big_Boss_Normal_AI = new Adventure_Big_Boss_Normal_Mode_AI_Data();
    Adventure_Big_Boss_Angry_Mode_AI_Data m_Adv_Big_Boss_Angry_AI = new Adventure_Big_Boss_Angry_Mode_AI_Data();
    Adventure_Big_Boss_Groggy_Mode_AI_Data m_Adv_Big_Boss_Groggy_AI = new Adventure_Big_Boss_Groggy_Mode_AI_Data();








    // ================================
    // =========== Methods ============


    void Awake() // 생성자
    {
        c_Stage_Manager = this; // 스테이지 매니저 인스턴스 지정

        m_Map_Coordinate_List = new List<Map_Coordinate>();
        m_MCL_is_Blocked_List = new List<bool>();
        
        MCL_init(); // MCL 초기화

        m_CameraOffset = GameObject.Find("Camera_Offset"); // 카메라 객체 지정

        m_Object_Table_List = new List<Object_Table_Data>(CSV_Manager.GetInstance().Get_Object_Table_List()); // 오브젝트 테이블 목록 로드

        if (m_Stage_ID == MAP.NOT_SET)
            m_Stage_ID = PlayerPrefs.GetInt("Mode_Adventure_Current_Stage_ID"); // 스테이지 ID를 받아온다.

        // 스테이지 데이터 내부 리스트들의 공간 할당
        m_Adventure_Stage_Data.Adventure_Quest_ID_List = new int[3];
        m_Adventure_Stage_Data.Stage_Pattern_ID_List = new int[3];

        CSV_Manager.GetInstance().Get_Adventure_Stage_Data(ref m_Adventure_Stage_Data, m_Stage_ID); // 스테이지 테이블 데이터 로드
        
        CSV_Manager.GetInstance().Get_Adventure_Quest_List(ref m_QuestList, ref m_Adventure_Stage_Data.Adventure_Quest_ID_List); // 퀘스트 데이터 로딩

        foreach (Adventure_Quest_Data questdata in m_QuestList) // 보스 스테이지 검사
        {
            if (questdata.Quest_ID == 4) // 퀘스트 목록에 보스 몬스터 처치가 있다는것은
            {
                m_is_Boss_Stage = true; // 해당 스테이지가 보스 스테이지라는 뜻!

                // 보스 테이블의 번호를 설정
                if (m_Stage_ID == 5)
                    m_Boss_ID = 1;
                else if (m_Stage_ID == 9)
                    m_Boss_ID = 2;
            }
        }
        
        Create_Map(m_Adventure_Stage_Data.Stage_Pattern_ID_List[m_Current_Stage_index_Count]); // 설정된 번호를 받아서 맵 생성!

        //m_Script_List = CSV_Manager.GetInstance().Get_Script_List(스크립트ID); // 대사 받아오기
        
        if (m_Stage_Time_Limit == MAP.NOT_SET) // 시간 설정
            m_Stage_Time_Limit = m_Adventure_Stage_Data.Stage_Time;


        // 비행기 미리 소환
        m_Airplane = Instantiate(m_Prefab_Airplane); // 인스턴스 생성
        m_Current_Map_Objects.Add(m_Airplane);
        m_Airplane.GetComponent<Airplane>().Set_Airdrop_Count(m_Adventure_Stage_Data.Number_Of_DropItem); // 드랍 개수 설정
        ++m_Current_Map_Objects_Count; // 카운트 증가
    }

    void Start()
    {
        // 페이드 인
        if (Fade_Slider.c_Fade_Slider != null)
            Fade_Slider.c_Fade_Slider.Start_Fade_Slider(2);
    }

    void Update()
    {
        Check_GameOver();

        Check_Airplane();
    }









    // =====================================================
    // ==================== 맵 관련 ========================

    void Create_Map(int stage_id) // 맵 생성 및 설정
    {
        // 터레인 생성
        Create_Terrain();

        // 오브젝트 스폰 위치 목록을 받아온다.
        CSV_Manager.GetInstance().Get_Object_Spawn_Position_List(ref m_Object_Spawn_Position_List, stage_id);

        m_Current_Map_Objects_Count = 0;



        // 위치에 맞게 오브젝트를 생성한다.
        for (int z = 0; z < 15; ++z)
        {
            for (int x = 0; x < 15; ++x)
            {
                if (m_Object_Spawn_Position_List[z].Spawn_Node[x] != 0)
                {
                    // x, z 좌표 설정
                    m_Object_Position.x = x * 2.0f;
                    m_Object_Position.z = 78.0f - (z * 2.0f);

                    switch (m_Object_Spawn_Position_List[z].Spawn_Node[x])
                    {
                        case OBJECT_TABLE_NUMBER.BOX: // 박스
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Box));
                            m_Object_Position.y = m_Prefab_Box.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ROCK: // 바위
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Rock));
                            m_Object_Position.y = m_Prefab_Rock.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.GRASS: // 부쉬
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Grass));
                            m_Object_Position.y = m_Prefab_Grass.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.GOBLIN: // 일반 고블린
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Goblin));
                            m_Object_Position.y = m_Prefab_Goblin.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.BOSS_GOBLIN: // 보스 고블린
                            //Instantiate(m_Prefab_Intro_Boss);

                            // 보스 데이터 읽어오기
                            CSV_Manager.GetInstance().Get_Adventure_Boss_Data(ref m_Adventure_Boss_Data, m_Boss_ID);

                            // 생성
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Goblin_Boss));
                            m_Object_Position.y = m_Prefab_Goblin_Boss.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.START_POINT:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Start_Point));
                            m_Object_Position.y = m_Prefab_Start_Point.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.NEXT_POINT:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Next_Portal));
                            m_Object_Position.y = m_Prefab_Next_Portal.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.END_POINT:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_End_Portal));
                            m_Object_Position.y = m_Prefab_End_Portal.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.CHARACTER_SPAWN:
                            //m_Current_Map_Objects.Add(Instantiate(m_Prefab_Character_Spawn));
                            //m_Object_Position.y = m_Prefab_Character_Spawn.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.BOSS_GIANT_1:

                            // 보스 데이터 읽어오기
                            CSV_Manager.GetInstance().Get_Adventure_Boss_Data(ref m_Adventure_Boss_Data, m_Boss_ID);
                            Big_Boss_Data_Allocation();
                            CSV_Manager.GetInstance().Get_Adventure_Big_Boss_AI_Data(ref m_Adv_Big_Boss_Normal_AI, ref m_Adv_Big_Boss_Angry_AI, ref m_Adv_Big_Boss_Groggy_AI);

                            // 생성
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Ork_Boss));
                            m_Object_Position.y = m_Prefab_Ork_Boss.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ITEM_BOMB:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Item_Bomb));
                            m_Object_Position.y = m_Prefab_Item_Bomb.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ITEM_SPEED:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Item_Speed));
                            m_Object_Position.y = m_Prefab_Item_Speed.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ITEM_FIRE:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Item_Fire));
                            m_Object_Position.y = m_Prefab_Item_Fire.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ITEM_KICK:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Item_Kick));
                            m_Object_Position.y = m_Prefab_Item_Kick.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.ITEM_THROW:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Item_Throw));
                            m_Object_Position.y = m_Prefab_Item_Throw.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.INFO_TRIGGER_BOMB:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Info_Trigger_Bomb));
                            m_Object_Position.y = m_Prefab_Info_Trigger_Bomb.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.INFO_TRIGGER_SPEED:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Info_Trigger_Speed));
                            m_Object_Position.y = m_Prefab_Info_Trigger_Speed.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.INFO_TRIGGER_FIRE:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Info_Trigger_Fire));
                            m_Object_Position.y = m_Prefab_Info_Trigger_Fire.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.INFO_TRIGGER_KICK:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Info_Trigger_Kick));
                            m_Object_Position.y = m_Prefab_Info_Trigger_Kick.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.INFO_TRIGGER_THROW:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Info_Trigger_Throw));
                            m_Object_Position.y = m_Prefab_Info_Trigger_Throw.transform.position.y;
                            break;

                        case OBJECT_TABLE_NUMBER.BOX_NONE_ITEM:
                            m_Current_Map_Objects.Add(Instantiate(m_Prefab_Box_None_Item));
                            m_Object_Position.y = m_Prefab_Box_None_Item.transform.position.y;
                            break;
                    }
                    // 생성한 객체 좌표 이동
                    m_Current_Map_Objects[m_Current_Map_Objects_Count].transform.position = m_Object_Position;

                    // 카운트 증가
                    ++m_Current_Map_Objects_Count;
                }
            }
        }
    }
    
    void Create_Terrain() // 터레인 생성
    {
        Instantiate(m_Forest_Theme_Terrain);
    }
    
    public void Next_Map_Load() // 다음 맵 불러오기
    {
        ++m_Current_Stage_index_Count; // 다음 맵 번호를 지정하고

        if (m_Adventure_Stage_Data.Stage_Pattern_ID_List[m_Current_Stage_index_Count] != 0) // 리스트에 맵 번호가 있으면 시작
        {
            m_is_Map_Changing = true; // 맵 변경 중이라고 알림
            
            MCL_init(); // MCL을 재설정한다.
            
            Destroy_Objects(); // 남아있는 오브젝트들을 제거한다.

            Create_Map(m_Adventure_Stage_Data.Stage_Pattern_ID_List[m_Current_Stage_index_Count]); // 새 맵을 생성!

            Invoke("Map_Changing_Over", 1.0f); // 1초 뒤 맵 변경 완료 알림
        }
    }
    
    void Map_Changing_Over() // 맵 전환이 끝났음을 알린다.
    {
        m_is_Map_Changing = false;
    }

    public bool Get_is_Map_Changing()
    {
        return m_is_Map_Changing;
    }









    // =====================================================
    // ==================== MCL 관련 =======================

    void MCL_init() // MCL 좌표 및 is_Blocked 초기화
    {
        m_Map_Coordinate_List.Clear();

        Map_Coordinate m_tmpCoordinate;

        for (int i = -1; i < m_Map_Size_X - 1; ++i)
        {
            for (int j = -1; j < m_Map_Size_Z - 1; ++j)
            {
                m_tmpCoordinate.x = i * 2;
                m_tmpCoordinate.z = j * 2 + 50;
                m_Map_Coordinate_List.Add(m_tmpCoordinate);
                m_MCL_is_Blocked_List.Add(false);
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

    public bool Get_is_init_MCL() // MCL이 초기화 되었는가를 반환
    {
        return m_is_init_MCL;
    }




    public void Update_MCL_isBlocked(int index, bool isBlocked) // isBlocked 갱신 메소드
    {
        if (index != -1)
        {
            m_MCL_is_Blocked_List[index] = isBlocked;
        }
    }
    



    public bool Get_MCL_index_is_Blocked(int index) // MCL 안의 해당 index가 막혀있는지를 반환해준다.
    {
        return m_MCL_is_Blocked_List[index];
    }
    



    public void Get_MCL_Coordinate(int index, ref float x, ref float z) // 인덱스에 따른 위치로 설정해준다.
    {
        x = m_Map_Coordinate_List[index].x;
        z = m_Map_Coordinate_List[index].z;
    }
    



    public int Find_Own_MCL_Index(float x, float z) // 받아온 좌표로 MCL 인덱스를 반환해준다.
    {
        Map_Coordinate m_tmpCoordinate;

        // 받아온 좌표에서 가장 가까운 좌표로 설정한다.
        int index_X = (int)x;
        int index_Z = (int)z;

        if (index_X % 2 == 1)
        {
            m_tmpCoordinate.x = index_X + 1;
        }
        else if (index_X % 2 == -1)
        {
            m_tmpCoordinate.x = index_X - 1;
        }
        else
            m_tmpCoordinate.x = index_X;


        if (index_Z % 2 == 1)
        {
            m_tmpCoordinate.z = index_Z + 1;
        }
        else if (index_Z % 2 == -1)
        {
            m_tmpCoordinate.z = index_Z - 1;
        }
        else
            m_tmpCoordinate.z = index_Z;


        // 속한 좌표의 인덱스를 뽑아서 반환한다.
        return m_Map_Coordinate_List.IndexOf(m_tmpCoordinate);
    }









    // =====================================================
    // =================== 시스템 관련 =====================

    public void Stage_Clear() // 스테이지 클리어시 호출
    {
        m_is_Stage_Clear = true; // 스테이지 클리어라고 알림!
        m_is_Pause = true; // 일시정지 시킨다.

        Condition_For_Getting_Stars(ref m_Adventure_Stage_Data.Adventure_Quest_ID_List); // 별 획득 조건 체크 및 저장

        // 현재 스테이지가 플레이 가능 최대 스테이지라면
        int tempMaxStage = PlayerPrefs.GetInt("Mode_Adventure_Playable_Max_Stage");

        if (PlayerPrefs.GetInt("Mode_Adventure_Current_Stage_ID") == tempMaxStage)
        {
            tempMaxStage += 1;

            // 최대 스테이지 범위 내에서 플레이가능 최대 스테이지 증가!
            if (tempMaxStage <= PlayerPrefs_Manager_Constants.MAX_STAGE_NUM)
            {
                PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", tempMaxStage);
            }
        }

        PlayerPrefs.Save();

        UI.Draw_StageClearPage(); // 클리어 화면 출력
    }
    
    void Condition_For_Getting_Stars(ref int[] list) // 별 획득 조건 관리
    {
        foreach (Adventure_Quest_Data QuestData in m_QuestList)
        {
            string tempString;
            for (int i = 0; i < 3; ++i)
            {
                if (QuestData.ID == list[i])
                {
                    if (QuestData.Quest_ID == 1) // 시간
                    {
                        if (UI.c_UI.Get_Left_Time() >= QuestData.Quest_Goal)
                        {
                            m_Star_Count += 1;
                            tempString = "Adventure_Stars_ID_" + list[i];
                            PlayerPrefs.SetInt(tempString, 1);
                        }
                    }
                    else if (QuestData.Quest_ID == 2) // 일반몹처치
                    {
                        if (m_Total_Monster_Count - m_Left_Monster_Count >= QuestData.Quest_Goal)
                        {
                            m_Star_Count += 1;
                            tempString = "Adventure_Stars_ID_" + list[i];
                            PlayerPrefs.SetInt(tempString, 1);
                        }
                    }
                    else if (QuestData.Quest_ID == 3) // 목표지점
                    {
                        if (m_is_Goal_In)
                        {
                            m_Star_Count += 1;
                            tempString = "Adventure_Stars_ID_" + list[i];
                            PlayerPrefs.SetInt(tempString, 1);
                        }
                    }
                    else if (QuestData.Quest_ID == 4) // 보스처치
                    {
                        if (m_is_Boss_Dead)
                        {
                            m_Star_Count += 1;
                            tempString = "Adventure_Stars_ID_" + list[i];
                            PlayerPrefs.SetInt(tempString, 1);
                        }
                    }
                }
            }
        }

        PlayerPrefs.Save();
    }

    public bool Get_is_Stage_Clear() // 스테이지가 클리어 되었는가를 반환
    {
        return m_is_Stage_Clear;
    }




    void Check_GameOver() // 게임오버인지 체크하여 설정
    {
        if (!PlayerMove.C_PM.Get_IsAlive()) // 죽어서 끝났거나,
        {
            m_Text.text = "Game Over";
            m_Game_Over = true;
        }

        else if (m_is_Stage_Clear) // 클리어해서 끝났거나!
        {
            m_Game_Over = true;
        }
    }
    
    public bool Get_Game_Over() // 게임오버인가를 반환
    {
        return m_Game_Over;
    }
    



    public void SetBossDead(bool b) // 보스가 죽었는지를 설정
    {
        m_is_Boss_Dead = b;
    }

    public bool GetBossDead() // 보스가 죽었는지를 반환
    {
        return m_is_Boss_Dead;
    }
    



    public void SetGoalIn(bool b) // 목표지점에 도달했는지를 설정
    {
        m_is_Goal_In = b;
    }




    void Summon_SuddenDeath_Glider() // 서든데스 고블린 소환
    {
        if (UI.c_UI.Get_Left_Time() <= m_Adventure_Stage_Data.SuddenDeath_Time)
        {
            for (int i = 0; i < m_Adventure_Stage_Data.Number_Of_GliderGoblin; ++i)
                Instantiate(m_SuddenDeath_JetGoblin).GetComponent<Boss_AI_JetGoblin>().Set_Bomb_info(m_Adventure_Stage_Data.GliderGoblin_Bomb, m_Adventure_Stage_Data.GliderGoblin_Fire);
        }
    }

    void Check_Airplane() // 비행기 체크(소환)
    {
        if (UI.c_UI.Get_Elapsed_Time() >= m_Adventure_Stage_Data.AirDrop_Time)
            m_Airplane.GetComponent<Airplane>().Dispatch_Airplane(); // 비행기 출발
    }




    public void Increase_Normal_Monster_Count() // 일반몹 개수 증가시키기
    {
        ++m_Total_Monster_Count;
        ++m_Left_Monster_Count;
    }

    public void Decrease_Normal_Monster_Count()
    {
        --m_Left_Monster_Count;
    }

    public int Get_Left_Normal_Monster_Count() // 남은 일반몹 수 반환
    {
        return m_Total_Monster_Count - m_Left_Monster_Count;
    }




    public void Init_Left_Time() // UI에 제한시간 설정
    {
        UI.c_UI.Set_Left_Time(m_Stage_Time_Limit);
    }




    public int Get_Star_Count() // 획득한 별 개수 반환
    {
        return m_Star_Count;
    }




    public void Destroy_Objects() // 씬전환 이전에 객체들을 제거한다.
    {
        // 테이블로 생성했던 오브젝트들을 제거
        foreach (GameObject g in m_Current_Map_Objects)
        {
            if (g != null)
                Destroy(g);
        }

        // 동적생성 폭탄들 제거
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach (GameObject b in bombs)
            Destroy(b);

        // 동적생성 화염들 제거
        GameObject[] flame = GameObject.FindGameObjectsWithTag("Flame");
        foreach (GameObject f in flame)
            Destroy(f);

        // 동적생성 화염잔해들 제거
        GameObject[] flame_remains = GameObject.FindGameObjectsWithTag("Flame_Remains");
        foreach (GameObject fr in flame_remains)
            Destroy(fr);

        // 불 붙은 부쉬의 파티클도 제거
        GameObject[] fBushs = GameObject.FindGameObjectsWithTag("Flame_Bush_Particle");
        foreach (GameObject fb in fBushs)
            Destroy(fb);

        // 동적생성 아이템들도 제거
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
        items = GameObject.FindGameObjectsWithTag("Airdrop_Item");
        foreach (GameObject i in items)
            Destroy(i);
    }










    // =====================================================
    // ================ 테이블 데이터 관련 =================

    public void GetQuestList(ref List<Adventure_Quest_Data> list) // 퀘스트 내용 리스트를 리턴해준다. (UI에 띄우기 위해)
    {
        list = m_QuestList;
    }

    public Adventure_Boss_Data Get_Adventure_Boss_Data() // 보스 데이터를 리턴해준다.
    {
        return m_Adventure_Boss_Data;
    }

    void Big_Boss_Data_Allocation() // 빅보스 데이터 메모리 할당작업
    {
        // Normal
        m_Adv_Big_Boss_Normal_AI.Boss_Speed_Value = new double[4];
        m_Adv_Big_Boss_Normal_AI.Skill_Time = new int[4];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Min = new int[2];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Max = new int[2];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Speed_Value = new int[2];
        m_Adv_Big_Boss_Normal_AI.First_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Normal_AI.First_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Normal_AI.First_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Normal_AI.Second_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Normal_AI.Second_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Normal_AI.Second_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Normal_AI.Third_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Normal_AI.Third_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Normal_AI.Third_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Normal_AI.Forth_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Normal_AI.Forth_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Normal_AI.Forth_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Normal_AI.Fifth_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Normal_AI.Fifth_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Normal_AI.Fifth_Turn_Link_Skill = new int[4];

        // ===============================================================




        // Angry
        m_Adv_Big_Boss_Angry_AI.Boss_Speed_Value = new double[4];
        m_Adv_Big_Boss_Angry_AI.Skill_Time = new int[4];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Min = new int[2];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Max = new int[2];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Speed_Value = new int[2];
        m_Adv_Big_Boss_Angry_AI.First_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Angry_AI.First_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Angry_AI.First_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Angry_AI.Second_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Angry_AI.Second_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Angry_AI.Second_Turn_Link_Skill = new int[4];
        m_Adv_Big_Boss_Angry_AI.Third_Turn_Skill_Percentage = new int[4];
        m_Adv_Big_Boss_Angry_AI.Third_Turn_Skill_Duration = new int[4];
        m_Adv_Big_Boss_Angry_AI.Third_Turn_Link_Skill = new int[4];

        // ===============================================================



        // Groggy
        m_Adv_Big_Boss_Groggy_AI.Boss_Speed_Value = new int[4];
    }
}
