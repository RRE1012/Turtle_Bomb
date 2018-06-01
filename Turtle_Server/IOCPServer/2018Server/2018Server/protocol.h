#pragma once
#define WIN32_LEAN_AND_MEAN  
#define INITGUID

#include <WinSock2.h>
#include <windows.h>   // include important windows stuff

#pragma comment(lib, "ws2_32.lib")
#include <thread>
#include <vector>
#include <array>
#include <iostream>
#include <unordered_set>
#include <mutex>

#include <vector>
#include <set>
#include <thread>
#include <mutex>
#include <queue>
#include <stack>
#include <set>
#include <map>

#include <list>
#include <thread>
#include <string>
#include <chrono>
#include <WinSock2.h>
#include <Windows.h>
#include <random>
#include <cmath>

#include <windows.h>
#include <wininet.h>
#include <stdio.h>
#include<fstream>
#include<iterator>

#include <math.h>
using namespace std;
#define MAX_BUFF_SIZE   4000 // 작으면 성능이 떨어진다.
#define MAX_PACKET_SIZE  255

#define BOARD_WIDTH   400
#define BOARD_HEIGHT  400

#define SHOW_PLAYER_POS_X 10
#define SHOW_PLAYER_POS_Y 10

#define VIEW_RADIUS   3

#define MAX_USER 500

#define NPC_START  1000
#define NUM_OF_NPC  10000

#define MY_SERVER_PORT  9000 // 포트는 서버/클라 동일해야함.

#define MAX_STR_SIZE  100

#define CS_UP    1
#define CS_DOWN  2
#define CS_LEFT  3
#define CS_RIGHT    4
#define CS_CHAT      5

#define SC_POS           1
#define SC_PUT_PLAYER    2
#define SC_REMOVE_PLAYER 3
#define SC_CHAT      4

//TBServer 
#define MAX_USER_INROOM 4
#define MAX_NPC   50
#define CASE_POS 1
#define CASE_BOMB 2
#define CASE_BOMB_EX 3
#define CASE_MAP 4
#define CASE_ID 5
#define CASE_ITEM_GET 6
#define CASE_DEAD 7
#define CASE_ROOM 8
#define CASE_GAMESET 15
#define CASE_JOINROOM 9
#define CASE_CREATEROOM 10
#define CASE_READY 11
#define CASE_STARTGAME 12
#define CASE_OUTROOM 13
#define CASE_FORCEOUTROOM 14
#define CASE_ROOMSETTING 15
#define CASE_TEAMSETTING 16
#define CASE_THROWBOMB 17
#define CASE_KICKBOMB 18
#define CASE_THROWCOMPLETE 19
#define CASE_KICKCOMPLETE 20
#define CASE_MAPSET 21
#define CASE_BOXPUSH 22
#define CASE_BOXPUSHCOMPLETE 23
#define CASE_TIME 24
#define CASE_BOMBSET 25

#define SIZEOF_TB_CharPos 22
#define SIZEOF_TB_BombPos 17
#define SIZEOF_TB_BombExplode 13
#define SIZEOF_TB_BombExplodeRE 15
#define SIZEOF_TB_MAP 227
#define SIZEOF_TB_ID 3
#define SIZEOF_TB_ItemGet 13
#define SIZEOF_TB_GetItem 4
#define SIZEOF_TB_DEAD 3
#define SIZEOF_TB_GAMEEND 3
#define SIZEOF_TB_Room 31
#define SIZEOF_TB_join 12
#define SIZEOF_TB_joinRE 10
#define SIZEOF_TB_create  12
#define SIZEOF_TB_createRE  4
#define SIZEOF_TB_GameStart 4
#define SIZEOF_TB_GameStartRE 3
#define SIZEOF_CASE_READY 5
#define SIZEOF_TB_ReadyRE 5
#define SIZEOF_TB_RoomOut 4
#define SIZEOF_TB_RoomOutRE 3
#define SIZEOF_TB_GetOut  4
#define SIZEOF_TB_GetOutRE  2
#define SIZEOF_TB_RoomSetting 6
#define SIZEOF_TB_TeamSetting 5
#define SIZEOF_TB_ThrowBomb 13
#define SIZEOF_TB_ThrowBombRE 20
#define SIZEOF_TB_ThrowComplete 11
#define SIZEOF_TB_MapSetRE 11
#define SIZEOF_TB_BoxPush 13
#define SIZEOF_TB_BoxPushComplete 11
#define SIZEOF_TB_BoxPushRE 21
#define SIZEOF_TB_Time 6
#define MAX_EVENT_SIZE 64


#define MAP_NOTHING 0
#define MAP_BOX 1
#define MAP_ROCK 2

#define MAP_BUSH 3
#define MAP_ITEM 11
#define MAP_BOMB 5
#define MAP_ITEM_F 13
#define MAP_ITEM_S 12
#define MAP_FIREBUSH 10
#define MAP_KICKITEM 14
#define MAP_THROWITEM 15
#define MAP_CHAR 7
#define MAP_ENEMY 8
#define MAP_BOSS 9


#define BASIC_POSX_CHAR1 0
#define BASIC_POSZ_CHAR1 0
#define BASIC_POSX_CHAR2 0
#define BASIC_POSZ_CHAR2 0
#define BASIC_POSX_CHAR3 0
#define BASIC_POSZ_CHAR3 0
#define BASIC_POSX_CHAR4 0
#define BASIC_POSZ_CHAR4 0

#define TURTLE_ANI_IDLE 0
#define TURTLE_ANI_WALK 1
#define TURTLE_ANI_HIDE 2
#define TURTLE_ANI_DEAD 3
#define TURTLE_ANI_KICK 4
#define TURTLE_ANI_PUSH 5

#pragma pack (push, 1)

struct cs_packet_up {
	unsigned char size;
	unsigned char type;
};

struct cs_packet_down {
	unsigned char size;
	unsigned char type;
};

struct cs_packet_left {
	unsigned char size;
	unsigned char type;
};

struct cs_packet_right {
	unsigned char size;
	unsigned char type;
};

struct cs_packet_chat {
	unsigned char size;
	unsigned char type;
	wchar_t message[MAX_STR_SIZE];
};

struct sc_packet_pos {
	unsigned char size;
	unsigned char type;
	unsigned short id;
	unsigned short x;
	unsigned short y;
};

struct sc_packet_put_player {
	unsigned char size;
	unsigned char type;
	unsigned short id;
	unsigned short x;
	unsigned short y;
};
struct sc_packet_remove_player {
	unsigned char size;
	unsigned char type;
	short id;
};

struct sc_packet_chat {
	unsigned char size;
	unsigned char type;
	short id;
	wchar_t message[MAX_STR_SIZE];
};





struct Socket_Info {
	SOCKET sock;
	bool m_connected;
	bool m_getpacket;
	char buf[MAX_BUFF_SIZE];

	int type;
	int id;
	int recvbytes;

	int sendbytes;
	int remainbytes;
	unsigned char roomID; //디폴트는 0. 안 들어갔다는 뜻
	unsigned char is_guardian; //방장인지 아닌지 
	unsigned char is_ready;
	//추가
	unsigned char fire;
	unsigned char bomb;
	unsigned char speed;
	unsigned char pos_inRoom;
};



struct GameCharInfo {
	unsigned char speed;
	unsigned char fire;
	unsigned char bomb;

};

struct TB_CharPos {//type:1
	unsigned char size; //22
	unsigned char type;
	unsigned char ingame_id;
	unsigned char anistate;
	unsigned char is_alive;
	unsigned char room_id;
	unsigned char fire;
	unsigned char bomb;
	unsigned char can_throw;
	unsigned char can_kick;
	float posx;
	float posz;
	float rotY;
};

struct TB_BombPos { //type:2
	unsigned char size;//17
	unsigned char type;
	unsigned char ingame_id;
	unsigned char firepower; //화력
							 //unsigned char throwing; //던져지고 있는지
							 //unsigned char kicking; //차여지고 있는지
	unsigned char room_num;
	int posx;
	int posz;
	float settime;
};
struct TB_BombSetRE {
	unsigned char size;//11
	unsigned char type;//25
	unsigned char f_power;
	int posx;
	int posz;
};
struct TB_MapSetRE {
	unsigned char size;//11
	unsigned char type;//21
	unsigned char m_type;
	int posx;
	int posz;
};
struct TB_BombExplode { //type:3
	unsigned char size;//13
	unsigned char type;
	unsigned char firepower;
	unsigned char room_id;
	unsigned char game_id;
	int posx;
	int posz;

};
struct TB_BombExplodeRE { //type:3
	unsigned char size;//15
	unsigned char type;
	unsigned char upfire;
	unsigned char rightfire;
	unsigned char downfire;
	unsigned char leftfire;
	unsigned char gameID;
	int posx;
	int posz;

};

struct TB_ThrowBomb {
	unsigned char size;//13
	unsigned char type;//17
	unsigned char roomid;
	unsigned char ingame_id;
	unsigned char direction;
	int posx;
	int posz;
};
struct TB_ThrowBombRE {
	unsigned char size;//20
	unsigned char type;//17
	unsigned char direction;
	unsigned char ingame_id;
	int posx;
	int posz;
	int posx_re;
	int posz_re;

};

struct TB_ThrowComplete {
	unsigned char size;//11
	unsigned char type;//19
	unsigned char roomid;
	int posx;
	int posz;
};

struct TB_KickBomb {
	unsigned char size;//13
	unsigned char type;//18
	unsigned char roomid;
	unsigned char ingame_id;
	unsigned char direction;
	int posx;
	int posz;
};
struct TB_KickComplete {
	unsigned char size;//11
	unsigned char type;//20
	unsigned char roomid;
	int posx;
	int posz;
};
struct TB_KickBombRE {
	unsigned char size;//21
	unsigned char type;//18
	unsigned char kick;
	unsigned char ingame_id;
	unsigned char direction;
	int posx;
	int posz;
	int posx_re;
	int posz_re;
};
struct TB_BoxPush {
	unsigned char size;//13
	unsigned char type;//22
	unsigned char roomid;
	unsigned char ingame_id;
	unsigned char direction;
	int posx;
	int posz;

};
struct TB_BoxPushRE {
	unsigned char size;//21
	unsigned char type;//22
	unsigned char push; //0이면 안밀어, 1이면 밀어~!
	unsigned char ingame_id;
	unsigned char direction;
	int posx;
	int posz;
	int posx_d;
	int posz_d;

};

struct TB_BoxPushComplete {
	unsigned char size;//11
	unsigned char type;//19
	unsigned char roomid;
	int posx;
	int posz;
};
struct TB_ReGame {

};
struct TB_Map { //type:4
	unsigned char size;//227
	unsigned char type;
	unsigned char mapInfo[15][15];

};
struct TB_ID {//type:5
	unsigned char size; //3
	unsigned char type;
	unsigned char id;  //0330 수정 int에서- BYTE로 수정
};


struct TB_ItemGet { //type:6
	unsigned char size; //13
	unsigned char type;//6
	unsigned char room_id;
	unsigned char ingame_id;
	unsigned char item_type;
	int posx;
	int posz;
};

struct TB_GetItem { //send : type 6 서버 전송-> 클라 수신
	unsigned char size; //4
	unsigned char type;//6

	unsigned char ingame_id;
	unsigned char itemType; //타입에 따라 다른 문구가 출력된다 + 능력이 오른다.

};
struct TB_DEAD { //죽었을 때 알려주는 패킷
	unsigned char size; //3
	unsigned char type;//7
	unsigned char game_id; //누가 죽었나!

};

struct TB_GAMEEND {
	unsigned char size; //3
	unsigned char type;//15
	unsigned char winner_id; //누가 이겼나!
};


struct TB_UserInfo { //유저정보 - type: 7
	unsigned char size; //9
	unsigned char type;
	unsigned char id; //인게임 id와는 다르다.
	unsigned char roomID; //디폴트는 0. 안 들어갔다는 뜻
	unsigned char is_guardian;//방장인지 아닌지 
	unsigned char is_ready;
	//추가
	unsigned char fire;
	unsigned char bomb;
	unsigned char speed;

};
//프로토콜이 아닌 구조체에 맵데이터를 넣은 방 구조체 작성

struct TB_Room { //방장 추가(완)
	unsigned char size; //31
	unsigned char type;//8
	unsigned char roomID;
	unsigned char people_count;
	unsigned char game_start; //게임 시작 1
	unsigned char people_max; //최대 인원 수
	unsigned char made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
	unsigned char guardian_pos; //배열에 넣을 때 -1할 것
	unsigned char people_inroom[4];
	unsigned char roomstate;  //팀전인가 개인전인가? 0-개인전 1-팀전
	unsigned char map_thema;
	unsigned char map_mode;
	unsigned char team_inroom[4];
	unsigned char ready[4];
	char password[8];
};

struct TB_RoomInfo {
	unsigned char room_num;
	unsigned char people_count;
	unsigned char people_max;
	unsigned char game_start;
};
//없는 방입니다.
//게임중입니다. 
struct TB_Ready {
	unsigned char size; //5
	unsigned char type;//11
	unsigned char room_num;
	unsigned char pos_in_room;
	unsigned char will_ready; // 0이면 레디하겠다고 보내온 것, 1이면 레디를 해제하겠다고 보내온 것.

};
struct TB_ReadyRE {
	unsigned char size; //5
	unsigned char type;//11
	unsigned char pos_in_room;
	unsigned char ready;//0이면 레디해제, 1이면 레디
	unsigned char roomid;
};
struct TB_GameStart {
	unsigned char size;//4
	unsigned char type;//12
	unsigned char roomID;
	unsigned char my_pos;
};
struct TB_GameStartRE {
	unsigned char size;
	unsigned char type;
	unsigned char startTB;//1이면 시작,0이면 땡
};

struct TB_Refresh { //type:?
	unsigned char size;
	unsigned char type;
	unsigned char id;
	unsigned char roomID;
};


struct TB_join { //들어갈 때 보내는 패킷 9
	unsigned char size;
	unsigned char type;//9
	unsigned char id;
	unsigned char roomID;
	char password[8];
};

struct TB_joinRE { //방장 추가
	unsigned char size;//10
	unsigned char type;
	unsigned char respond; //0이면 no, 1이면 yes
	unsigned char my_room_num;
	unsigned char yourpos; //1,2,3,4 중 하나
	unsigned char guard_pos; //방장 위치
	unsigned char people_inroom[4];


};
struct TB_create { //type:10
	unsigned char size;
	unsigned char type;
	unsigned char id;
	unsigned char roomid;
	char password[8];
};

struct TB_createRE {
	unsigned char size;
	unsigned char type;
	unsigned char can; //가능하면 1, 불가능하면 0
	unsigned char roomid;
};

struct TB_GetOut {//클라 전송->서버 수신, 서버 전송할 경우 받은 클라는 강퇴 결과 출력
	unsigned char size;
	unsigned char type;//14
	unsigned char roomID;
	unsigned char position;

}; //받았을 경우 turtle_room.people_count-=1; turtle_waitroom.roodID = 0; 
struct TB_RoomOut {//방에서 나갈 때
	unsigned char size;//4
	unsigned char type;//13
	unsigned char roomID;
	unsigned char my_pos;

};
struct TB_GetOUTRE {
	unsigned char size;
	unsigned char type;//14
};
struct TB_RoomOutRE {
	unsigned char size;
	unsigned char type;
	unsigned char can; //가능하면 1, 불가능하면 0

};

struct TB_RoomSetting {
	unsigned char size;//6
	unsigned char type;//15
	unsigned char roomid;
	unsigned char peoplemax; //인원수
	unsigned char mapthema;
	unsigned char mapnum;

};

struct TB_TeamSetting {
	unsigned char size;//5
	unsigned char type;//16
	unsigned char roomid;
	unsigned char pos_in_room;
	unsigned char team;
};
struct TB_Time {
	unsigned char size;//6
	unsigned char type;//24
	float time;

};
struct TB_Room_Data { //방장 추가(완)

	unsigned char roomID;
	unsigned char people_count;
	unsigned char game_start;
	unsigned char people_max; //최대 인원 수
	unsigned char made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
	unsigned char guardian_pos; //배열에 넣을 때 -1할 것
	unsigned char people_inroom[4];
	TB_Map map;
	char password[8];
};
struct Map_TB {

	unsigned char mapTile[15][15];
};

#pragma pack(pop)
class Bomb_TB {
public:

	pair<int, int> xz;
	float time;
	bool is_throw;
	bool is_kicked;
	unsigned char firepower;
	float explode_time;
	unsigned char room_num;
	unsigned char game_id;

	bool operator ==(const Bomb_TB& other) {
		return xz == other.xz;
	}

	Bomb_TB() {
		time = (float)GetTickCount() / 1000;
		explode_time = 2.0f;
		xz = make_pair(0, 0);
		is_throw = false;
		is_kicked = false;
	}
	Bomb_TB(int a, int b) {
		time = (float)GetTickCount() / 1000; explode_time = 2.0f;
		xz = make_pair(a, b);
		is_throw = false;
		is_kicked = false;
	}
	Bomb_TB(int a, int b, unsigned char r, unsigned char f, unsigned char g) {
		time = (float)GetTickCount() / 1000; explode_time = 2.0f;
		room_num = r;
		firepower = f;
		game_id = g;
		xz = make_pair(a, b);
		is_throw = false;
		is_kicked = false;
	}
	bool GetTime() {
		float temptime = (float)GetTickCount() / 1000;
		return (temptime - time >= explode_time) && !is_throw && !is_kicked;
	}
	void ResetExplodeTime() {
		float temptime = (float)GetTickCount() / 1000;
		temptime = temptime - time;
		explode_time = explode_time - temptime;
	}
	void ResetTime() {
		time = (float)GetTickCount() / 1000;

	}
	pair<int, int> GetXZ() {
		return xz;
	}

};
class InGameCalculator {
	bool id[4];
	float time;

	bool gameover;


public:
	map<pair<int, int>, Bomb_TB> bomb_Map;
	vector<TB_BombExplodeRE> explode_List;
	unsigned char idList[4];
	TB_Map map;
	TB_CharPos ingame_Char_Info[4];
	unsigned char fireMap[15][15];
	int deathcount;
	InGameCalculator() {
		explode_List.clear();
		deathcount = 0;
		id[0] = true;
		id[1] = true;
		id[2] = true;
		id[3] = true;
		time = 180.0f;
		gameover = false;
		map.size = SIZEOF_TB_MAP;
		map.type = CASE_MAP;
		for (int i = 0; i < 4; ++i) {
			ingame_Char_Info[i].size = SIZEOF_TB_CharPos;
			ingame_Char_Info[i].type = CASE_POS;
			idList[i] = 0;
		}
		for (int i = 0; i < 15; ++i) {
			for (int j = 0; j < 15; ++j) {
				fireMap[i][j] = 0;
			}
		}
		ingame_Char_Info[0].ingame_id = 0;
		ingame_Char_Info[1].ingame_id = 1;
		ingame_Char_Info[2].ingame_id = 2;
		ingame_Char_Info[3].ingame_id = 3;
		ingame_Char_Info[0].posx = 0.0f;
		ingame_Char_Info[0].posz = 0.0f;
		ingame_Char_Info[0].is_alive = true;
		ingame_Char_Info[0].rotY = 0.0f;
		//char_info[1].hp = 10.0f;
		ingame_Char_Info[1].posx = 28.0f;
		ingame_Char_Info[1].posz = 0.0f;
		ingame_Char_Info[1].is_alive = true;
		ingame_Char_Info[1].rotY = 0.0f;
		//char_info[2].hp = 10.0f;
		ingame_Char_Info[2].posx = 0.0f;
		ingame_Char_Info[2].posz = 28.0f;
		ingame_Char_Info[2].is_alive = true;
		ingame_Char_Info[2].rotY = 180.0f;
		//char_info[3].hp = 10.0f;
		ingame_Char_Info[3].posx = 28.0f;
		ingame_Char_Info[3].posz = 28.0f;
		ingame_Char_Info[3].is_alive = true;
		ingame_Char_Info[3].rotY = 180.0f;
	}
	~InGameCalculator() {}
	void InitClass() {
		map.size = SIZEOF_TB_MAP;
		map.type = CASE_MAP;
		deathcount = 0;
		gameover = false;
		explode_List.clear();
		id[0] = true;
		id[1] = true;
		id[2] = true;
		id[3] = true;
		time = 180.0f;
		for (int i = 0; i < 4; ++i) {
			ingame_Char_Info[i].size = SIZEOF_TB_CharPos;
			ingame_Char_Info[i].type = CASE_POS;
			idList[i] = 0;
		}
		for (int i = 0; i < 15; ++i) {
			for (int j = 0; j < 15; ++j) {
				fireMap[i][j] = 0;
			}
		}
		ingame_Char_Info[0].ingame_id = 0;
		ingame_Char_Info[1].ingame_id = 1;
		ingame_Char_Info[2].ingame_id = 2;
		ingame_Char_Info[3].ingame_id = 3;
		ingame_Char_Info[0].posx = 0.0f;
		ingame_Char_Info[0].posz = 0.0f;
		ingame_Char_Info[0].is_alive = true;
		ingame_Char_Info[0].rotY = 0.0f;
		//char_info[1].hp = 10.0f;
		ingame_Char_Info[1].posx = 28.0f;
		ingame_Char_Info[1].posz = 0.0f;
		ingame_Char_Info[1].is_alive = true;
		ingame_Char_Info[1].rotY = 0.0f;
		//char_info[2].hp = 10.0f;
		ingame_Char_Info[2].posx = 0.0f;
		ingame_Char_Info[2].posz = 28.0f;
		ingame_Char_Info[2].is_alive = true;
		ingame_Char_Info[2].rotY = 180.0f;
		//char_info[3].hp = 10.0f;
		ingame_Char_Info[3].posx = 28.0f;
		ingame_Char_Info[3].posz = 28.0f;
		ingame_Char_Info[3].is_alive = true;
		ingame_Char_Info[3].rotY = 180.0f;
	}
	void PlayerDead(unsigned char idd) {
		if (id[idd]) {
			id[idd] = false;
			deathcount++;
		}
	}
	void ChangeID(int place, unsigned char id_p) {
		
		idList[place] = id_p;

	}
	void PlayerBlank(int id_p) {
		id[id_p] = false;
	}
	void SetGameOver() {
		gameover = true;
	}

	bool IsGameOver() {
		return gameover;
	}
	float GetTime() {
		return time;
	}
	void SetTime(DWORD a) {
		float temp = ((float)a) / 1000.0f;
		time = time - temp;
	}
	bool OneSec() {
		return ((int)(time * 10) % 10)<1;
	}
	unsigned char GetWinnerID() {
		for (unsigned char i = 0; i < 4; ++i) {
			if (id[i])
				return i;
		}
		return 4; //DRAW를 뜻한다.
	}
	void CalculateMap(int x, int z, unsigned char f,unsigned char id_player) {
		bool l_UpBlock = false;
		bool l_DownBlock = false;
		bool l_LeftBlock = false;
		bool l_RightBlock = false;
		unsigned char uf = f;
		unsigned char df = f;
		unsigned char lf = f;
		unsigned char rf = f;
		unsigned char tempMap[15][15];
		memcpy(tempMap, map.mapInfo, sizeof(tempMap));
		tempMap[z][x] = MAP_NOTHING;
		for (unsigned char b = 1; b <= f; ++b) {
			if (!l_DownBlock) {
				if (z - b < 0) {
					l_DownBlock = true;
					df = b - 1;
				}
				else {
					if (tempMap[z - b][x] == MAP_BOMB) {
						tempMap[z - b][x] = MAP_NOTHING;
						memcpy(map.mapInfo, tempMap, sizeof(tempMap));

						//*tempB = true;
						//CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
						if (bomb_Map[make_pair(x, z - b)].firepower != 0) {

							CalculateMap(x, z- b, fireMap[z - b][x], bomb_Map[make_pair(x, z - b)].game_id);
							auto a = bomb_Map.find(pair<int, int>(x, z - b));
							bomb_Map.erase(a);
						}
						memcpy(tempMap, map.mapInfo, sizeof(tempMap));
						l_DownBlock = true;
						df = b;
					}
					else if (tempMap[z - b][x] == MAP_BOX) {
						cout << "Box!!!" << endl;
						int temp_rand = (rand() % 14);
						if (temp_rand < 4)
							tempMap[z - b][x] = MAP_NOTHING;
						else if (temp_rand >= 4 && temp_rand <= 5)
							tempMap[z - b][x] = MAP_ITEM;
						else if (temp_rand >= 6 && temp_rand <= 7)
							tempMap[z - b][x] = MAP_ITEM_F;
						else if (temp_rand >= 8 && temp_rand <= 9)
							tempMap[z - b][x] = MAP_ITEM_S;
						else if (temp_rand >= 10 && temp_rand <= 11)
							tempMap[z - b][x] = MAP_KICKITEM;
						else if (temp_rand >= 12 && temp_rand <= 13)
							tempMap[z - b][x] = MAP_THROWITEM;

						l_DownBlock = true;
						df = b - 1;
					}
					else if (tempMap[z - b][x] == MAP_ITEM || tempMap[z - b][x] == MAP_ITEM_F || tempMap[z - b][x] == MAP_ITEM_S || tempMap[z - b][x] == MAP_KICKITEM || tempMap[z - b][x] == MAP_THROWITEM) {
						tempMap[z - b][x] = MAP_NOTHING;
					}
					else if (tempMap[z - b][x] == MAP_BUSH || tempMap[z - b][x] == MAP_FIREBUSH) {

					}
					else if (tempMap[z - b][x] == MAP_ROCK) {
						l_DownBlock = true;
						df = b - 1;
					}
				}
			}
			if (!l_UpBlock) {
				if (z + b > 14) {
					l_UpBlock = true;
					uf = b - 1;
				}
				else {
					if (tempMap[z + b][x] == MAP_BOMB) {
						tempMap[z + b][x] = MAP_NOTHING;

						l_UpBlock = true;
						memcpy(map.mapInfo, tempMap, sizeof(tempMap));
						if (bomb_Map[make_pair(x, z + b)].firepower != 0) {
							
							CalculateMap(x, z + b, fireMap[z + b][x], bomb_Map[make_pair(x, z + b)].game_id);
							auto a = bomb_Map.find(pair<int, int>(x, z+b));
							bomb_Map.erase(a);
						}
						

						//*tempB = true;
						//CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
						memcpy(tempMap, map.mapInfo, sizeof(tempMap));
						uf = b;
					}
					else if (tempMap[z + b][x] == MAP_BOX) {
						cout << "Box!!!" << endl;
						int temp_rand = (rand() % 14);
						if (temp_rand<4)
							tempMap[z + b][x] = MAP_NOTHING;
						else if (temp_rand >= 4 && temp_rand <= 5)
							tempMap[z + b][x] = MAP_ITEM;
						else if (temp_rand >= 6 && temp_rand <= 7)
							tempMap[z + b][x] = MAP_ITEM_F;
						else if (temp_rand >= 8 && temp_rand <= 9)
							tempMap[z + b][x] = MAP_ITEM_S;
						else if (temp_rand >= 10 && temp_rand <= 11)
							tempMap[z + b][x] = MAP_KICKITEM;
						else if (temp_rand >= 12 && temp_rand <= 13)
							tempMap[z + b][x] = MAP_THROWITEM;
						l_UpBlock = true;
						uf = b - 1;
					}
					else if (tempMap[z + b][x] == MAP_ITEM || tempMap[z + b][x] == MAP_ITEM_F || tempMap[z + b][x] == MAP_ITEM_S || tempMap[z + b][x] == MAP_KICKITEM || tempMap[z + b][x] == MAP_THROWITEM) {
						tempMap[z + b][x] = MAP_NOTHING;
					}
					else if (tempMap[z + b][x] == MAP_ROCK) {
						l_UpBlock = true;
						uf = b - 1;
					}
				}
			}
			if (!l_LeftBlock) {
				if (x - b < 0) {
					l_LeftBlock = true;
					lf = b - 1;
				}
				else {
					if (tempMap[z][x - b] == MAP_BOMB) {
						tempMap[z][x - b] = MAP_NOTHING;
						l_LeftBlock = true;
						memcpy(map.mapInfo, tempMap, sizeof(tempMap));
						//*tempB = true;
						//CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
						if (bomb_Map[make_pair(x-b, z)].firepower != 0) {

							CalculateMap(x-b, z, fireMap[z][x-b], bomb_Map[make_pair(x-b, z)].game_id);
							auto a = bomb_Map.find(pair<int,int>(x - b, z));
							bomb_Map.erase(a);
						}
						memcpy(tempMap, map.mapInfo, sizeof(tempMap));
						lf = b;
					}
					else if (tempMap[z][x - b] == MAP_BOX) {
						cout << "Box!!!" << endl;
						int temp_rand = (rand() % 14);
						if (temp_rand<4)
							tempMap[z][x - b] = MAP_NOTHING;
						else if (temp_rand >= 4 && temp_rand <= 5)
							tempMap[z][x - b] = MAP_ITEM;
						else if (temp_rand >= 6 && temp_rand <= 7)
							tempMap[z][x - b] = MAP_ITEM_F;
						else if (temp_rand >= 8 && temp_rand <= 9)
							tempMap[z][x - b] = MAP_ITEM_S;
						else if (temp_rand >= 10 && temp_rand <= 11)
							tempMap[z][x - b] = MAP_KICKITEM;
						else if (temp_rand >= 12 && temp_rand <= 13)
							tempMap[z][x - b] = MAP_THROWITEM;
						l_LeftBlock = true;
						lf = b - 1;
					}
					else if (tempMap[z][x - b] == MAP_ITEM || tempMap[z][x - b] == MAP_KICKITEM || tempMap[z][x - b] == MAP_THROWITEM || tempMap[z][x - b] == MAP_ITEM_F || tempMap[z][x - b] == MAP_ITEM_S) {
						tempMap[z][x - b] = MAP_NOTHING;
					}
					else if (tempMap[z][x - b] == MAP_ROCK) {
						l_LeftBlock = true;
						lf = b - 1;
					}
				}
			}
			if (!l_RightBlock) {
				if (x + b > 14) {
					l_RightBlock = true;
					rf = b - 1;
				}
				else {
					if (tempMap[z][x + b] == MAP_BOMB) {
						tempMap[z][x + b] = MAP_NOTHING;
						l_RightBlock = true;
						memcpy(map.mapInfo, tempMap, sizeof(tempMap));
						//*tempB = true;
						//CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
						if (bomb_Map[make_pair(x + b, z)].firepower != 0) {

							CalculateMap(x + b, z, fireMap[z][x + b], bomb_Map[make_pair(x + b, z)].game_id);
							auto a = bomb_Map.find(pair<int, int>(x+b, z ));
							bomb_Map.erase(a);
						}
						memcpy(tempMap, map.mapInfo, sizeof(tempMap));
						rf = b;
					}
					else if (tempMap[z][x + b] == MAP_BOX) {
						cout << "Box!!!" << endl;
						int temp_rand = (rand() % 14);
						if (temp_rand<4)
							tempMap[z][x + b] = MAP_NOTHING;
						else if (temp_rand >= 4 && temp_rand <= 5)
							tempMap[z][x + b] = MAP_ITEM;
						else if (temp_rand >= 6 && temp_rand <= 7)
							tempMap[z][x + b] = MAP_ITEM_F;
						else if (temp_rand >= 8 && temp_rand <= 9)
							tempMap[z][x + b] = MAP_ITEM_S;
						else if (temp_rand >= 10 && temp_rand <= 11)
							tempMap[z][x + b] = MAP_KICKITEM;
						else if (temp_rand >= 12 && temp_rand <= 13)
							tempMap[z][x + b] = MAP_THROWITEM;
						l_RightBlock = true;
						rf = b - 1;
					}
					else if (tempMap[z][x + b] == MAP_ITEM || tempMap[z][x + b] == MAP_KICKITEM || tempMap[z][x + b] == MAP_THROWITEM || tempMap[z][x + b] == MAP_ITEM_F || tempMap[z][x + b] == MAP_ITEM_S) {
						tempMap[z][x + b] = MAP_NOTHING;
					}
					else if (tempMap[z][x + b] == MAP_ROCK) {
						l_RightBlock = true;
						rf = b - 1;
					}
				}
			}


		}
		TB_BombExplodeRE bomb = { SIZEOF_TB_BombExplodeRE,CASE_BOMB_EX,uf,rf,df,lf,id_player,x,z };
		explode_List.emplace_back(bomb);
		//explode_List
		fireMap[z][x] = 0;
		tempMap[z][x] = MAP_NOTHING;
		memcpy(map.mapInfo, tempMap, sizeof(tempMap));
	}
};

template <class Iter, class Value>
Iter myFind(Iter a, Iter b, Value val) {
	Iter p = a;
	while (a != b) {
		if (*a == val)
			return a;
		else
			++a;
	}
	return b;

}


struct EXOVER {
	WSAOVERLAPPED m_over;
	char m_iobuf[MAX_BUFF_SIZE];
	WSABUF m_wsabuf;
	bool is_recv;
};

class Client {
public:
	SOCKET m_s;
	bool m_isconnected;
	unsigned char is_guardian;
	int m_x;
	int m_y;
	int m_scene; // 0 - 로비, 1- 방, 2- 게임중
	EXOVER m_rxover;
	int m_packet_size;  // 지금 조립하고 있는 패킷의 크기
	int   m_prev_packet_size; // 지난번 recv에서 완성되지 않아서 저장해 놓은 패킷의 앞부분의 크기
	char m_packet[MAX_PACKET_SIZE];
	int id;
	int roomNum;
	// set<int> view_list; // 삽입/삭제가 자유로워야 한다. + 시간복잡도 상 가장 효율적인것이 set이다. (list는 삽입/삭제가 빠르지만 검색 성능이 느리다.)
	// multiset 은 중복가능 이기때문에 사용하지 않아야한다.
	unordered_set<int> m_view_list; // 정렬이 딱히 필요하지 않기 때문에 비정렬셋으로 성능 향상.
	mutex m_mVl; // 뷰리스트를 보호하기위해

	Client()
	{
		m_isconnected = false;
		m_x = 4;
		m_y = 4;

		ZeroMemory(&m_rxover.m_over, sizeof(WSAOVERLAPPED));
		m_rxover.m_wsabuf.buf = m_rxover.m_iobuf;
		m_rxover.m_wsabuf.len = sizeof(m_rxover.m_wsabuf.buf);
		m_rxover.is_recv = true;
		m_prev_packet_size = 0;
	}
};
