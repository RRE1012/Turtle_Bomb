#pragma once
#include <iostream>
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
#include <atlstr.h>
#include <windows.h>
#include <wininet.h>
#include <stdio.h>
#include<fstream>
#include<iterator>

#include <math.h>
#pragma warning(disable : 4996)
//#pragma comment(linker,"/entry:WinMainCRTStartup /subsystem:console")
#pragma comment(lib, "ws2_32")
using namespace std;

#define TB_SERVER_PORT 9000
#define MAX_BUFF_SIZE 4000

#define MAX_USER 4
#define MAX_NPC	50
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







#pragma pack(push,1)







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
	BYTE roomID; //디폴트는 0. 안 들어갔다는 뜻
	BYTE is_guardian; //방장인지 아닌지 
	BYTE is_ready;
	//추가
	BYTE fire;
	BYTE bomb;
	BYTE speed;
	BYTE pos_inRoom;
};



struct GameCharInfo {
	BYTE speed;
	BYTE fire;
	BYTE bomb;

};

struct TB_CharPos {//type:1
	BYTE size; //22
	BYTE type;
	BYTE ingame_id;
	BYTE anistate;
	BYTE is_alive;
	BYTE room_id;
	BYTE fire;
	BYTE bomb;
	BYTE can_throw;
	BYTE can_kick;
	float posx;
	float posz;
	float rotY;
};

struct TB_BombPos { //type:2
	BYTE size;//17
	BYTE type;
	BYTE ingame_id;
	BYTE firepower; //화력
					//BYTE throwing; //던져지고 있는지
					//BYTE kicking; //차여지고 있는지
	BYTE room_num;
	int posx;
	int posz;
	float settime;
};
struct TB_BombSetRE {
	BYTE size;//11
	BYTE type;//25
	BYTE f_power;
	int posx;
	int posz;
};
struct TB_MapSetRE {
	BYTE size;//11
	BYTE type;//21
	BYTE m_type;
	int posx;
	int posz;
};
struct TB_BombExplode { //type:3
	BYTE size;//13
	BYTE type;
	BYTE firepower;
	BYTE room_id;
	BYTE game_id;
	int posx;
	int posz;

};
struct TB_BombExplodeRE { //type:3
	BYTE size;//15
	BYTE type;
	BYTE upfire;
	BYTE rightfire;
	BYTE downfire;
	BYTE leftfire;
	BYTE gameID;
	int posx;
	int posz;

};

struct TB_ThrowBomb {
	BYTE size;//13
	BYTE type;//17
	BYTE roomid;
	BYTE ingame_id;
	BYTE direction;
	int posx;
	int posz;
};
struct TB_ThrowBombRE {
	BYTE size;//20
	BYTE type;//17
	BYTE direction;
	BYTE ingame_id;
	int posx;
	int posz;
	int posx_re;
	int posz_re;

};

struct TB_ThrowComplete {
	BYTE size;//11
	BYTE type;//19
	BYTE roomid;
	int posx;
	int posz;
};

struct TB_KickBomb {
	BYTE size;//13
	BYTE type;//18
	BYTE roomid;
	BYTE ingame_id;
	BYTE direction;
	int posx;
	int posz;
};
struct TB_KickComplete {
	BYTE size;//11
	BYTE type;//20
	BYTE roomid;
	int posx;
	int posz;
};
struct TB_KickBombRE {
	BYTE size;//21
	BYTE type;//18
	BYTE kick;
	BYTE ingame_id;
	BYTE direction;
	int posx;
	int posz;
	int posx_re;
	int posz_re;
};
struct TB_BoxPush {
	BYTE size;//13
	BYTE type;//22
	BYTE roomid; 
	BYTE ingame_id;
	BYTE direction;
	int posx;
	int posz;

};
struct TB_BoxPushRE {
	BYTE size;//21
	BYTE type;//22
	BYTE push; //0이면 안밀어, 1이면 밀어~!
	BYTE ingame_id;
	BYTE direction;
	int posx;
	int posz;
	int posx_d;
	int posz_d;

};

struct TB_BoxPushComplete {
	BYTE size;//11
	BYTE type;//19
	BYTE roomid;
	int posx;
	int posz;
};
struct TB_ReGame {

};
struct TB_Map { //type:4
	BYTE size;//227
	BYTE type;
	BYTE mapInfo[15][15];

};
struct TB_ID {//type:5
	BYTE size; //3
	BYTE type;
	BYTE id;  //0330 수정 int에서- BYTE로 수정
};


struct TB_ItemGet { //type:6
	BYTE size; //13
	BYTE type;//6
	BYTE room_id;
	BYTE ingame_id;
	BYTE item_type;
	int posx;
	int posz;
};

struct TB_GetItem { //send : type 6 서버 전송-> 클라 수신
	BYTE size; //4
	BYTE type;//6

	BYTE ingame_id;
	BYTE itemType; //타입에 따라 다른 문구가 출력된다 + 능력이 오른다.

};
struct TB_DEAD { //죽었을 때 알려주는 패킷
	BYTE size; //3
	BYTE type;//7
	BYTE game_id; //누가 죽었나!

};

struct TB_GAMEEND {
	BYTE size; //3
	BYTE type;//15
	BYTE winner_id; //누가 이겼나!
};


struct TB_UserInfo { //유저정보 - type: 7
	BYTE size; //9
	BYTE type;
	BYTE id; //인게임 id와는 다르다.
	BYTE roomID; //디폴트는 0. 안 들어갔다는 뜻
	BYTE is_guardian;//방장인지 아닌지 
	BYTE is_ready;
	//추가
	BYTE fire;
	BYTE bomb;
	BYTE speed;

};
//프로토콜이 아닌 구조체에 맵데이터를 넣은 방 구조체 작성

struct TB_Room { //방장 추가(완)
	BYTE size; //31
	BYTE type;//8
	BYTE roomID;
	BYTE people_count;
	BYTE game_start; //게임 시작 1
	BYTE people_max; //최대 인원 수
	BYTE made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
	BYTE guardian_pos; //배열에 넣을 때 -1할 것
	BYTE people_inroom[4];
	BYTE roomstate;  //팀전인가 개인전인가? 0-개인전 1-팀전
	BYTE map_thema;
	BYTE map_mode;
	BYTE team_inroom[4];
	BYTE ready[4];
	char password[8];
};
struct TB_Ready {
	BYTE size; //5
	BYTE type;//11
	BYTE room_num;
	BYTE pos_in_room;
	BYTE will_ready; // 0이면 레디하겠다고 보내온 것, 1이면 레디를 해제하겠다고 보내온 것.

};
struct TB_ReadyRE {
	BYTE size; //5
	BYTE type;//11
	BYTE pos_in_room;
	BYTE ready;//0이면 레디해제, 1이면 레디
	BYTE roomid;
};
struct TB_GameStart {
	BYTE size;//4
	BYTE type;//12
	BYTE roomID;
	BYTE my_pos;
};
struct TB_GameStartRE {
	BYTE size;
	BYTE type;
	BYTE startTB;//1이면 시작,0이면 땡
};

struct TB_Refresh { //type:?
	BYTE size;
	BYTE type;
	BYTE id;
	BYTE roomID;
};


struct TB_join { //들어갈 때 보내는 패킷 9
	BYTE size;
	BYTE type;//9
	BYTE id;
	BYTE roomID;
	char password[8];
};

struct TB_joinRE { //방장 추가
	BYTE size;//10
	BYTE type;
	BYTE respond; //0이면 no, 1이면 yes
	BYTE my_room_num;
	BYTE yourpos; //1,2,3,4 중 하나
	BYTE guard_pos; //방장 위치
	BYTE people_inroom[4];


};
struct TB_create { //type:10
	BYTE size;
	BYTE type;
	BYTE id;
	BYTE roomid;
	char password[8];
};

struct TB_createRE {
	BYTE size;
	BYTE type;
	BYTE can; //가능하면 1, 불가능하면 0
	BYTE roomid;
};

struct TB_GetOut {//클라 전송->서버 수신, 서버 전송할 경우 받은 클라는 강퇴 결과 출력
	BYTE size;
	BYTE type;//14
	BYTE roomID;
	BYTE position;

}; //받았을 경우 turtle_room.people_count-=1; turtle_waitroom.roodID = 0; 
struct TB_RoomOut {//방에서 나갈 때
	BYTE size;//4
	BYTE type;//13
	BYTE roomID;
	BYTE my_pos;

};
struct TB_GetOUTRE {
	BYTE size;
	BYTE type;//14
};
struct TB_RoomOutRE {
	BYTE size;
	BYTE type;
	BYTE can; //가능하면 1, 불가능하면 0

};

struct TB_RoomSetting {
	BYTE size;//6
	BYTE type;//15
	BYTE roomid;
	BYTE peoplemax; //인원수
	BYTE mapthema;
	BYTE mapnum;

};

struct TB_TeamSetting {
	BYTE size;//5
	BYTE type;//16
	BYTE roomid;
	BYTE pos_in_room;
	BYTE team;
};
struct TB_Time {
	BYTE size;//6
	BYTE type;//24
	float time;

};
struct TB_Room_Data { //방장 추가(완)

	BYTE roomID;
	BYTE people_count;
	BYTE game_start;
	BYTE people_max; //최대 인원 수
	BYTE made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
	BYTE guardian_pos; //배열에 넣을 때 -1할 것
	BYTE people_inroom[4];
	TB_Map map;
	char password[8];
};
struct Map_TB {

	BYTE mapTile[15][15];
};

#pragma pack(pop)

class InGameCalculator {
	bool id[4];
	float time;
	bool gameover;
public:
	int deathcount;
	InGameCalculator() {
		deathcount = 0;
		id[0] = true;
		id[1] = true;
		id[2] = true;
		id[3] = true;
		time = 180.0f;
		gameover = false;
	}
	~InGameCalculator() {}
	void InitClass() {
		deathcount = 0;
		gameover = false;
		id[0] = true;
		id[1] = true;
		id[2] = true;
		id[3] = true;
		time = 180.0f;
	}
	void PlayerDead(BYTE idd) {
		if (id[idd]) {
			id[idd] = false;
			deathcount++;
		}
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
		return ((int)(time * 10)%10)<1;
	}
	BYTE GetWinnerID() {
		for (BYTE i = 0; i < 4; ++i) {
			if (id[i])
				return i;
		}
		return 4; //DRAW를 뜻한다.
	}
};

template <class Iter,class Value>
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

class Bomb_TB {
public:
	int x, z;
	pair<int, int> xz;
	float time;
	bool is_throw;
	bool is_kicked;
	BYTE firepower;
	float explode_time;
	BYTE room_num;
	BYTE game_id;

	bool operator ==(const Bomb_TB& other) {
		return xz == other.xz;
	}

	Bomb_TB() {
		x = 0; z = 0; time = (float)GetTickCount() / 1000;
		explode_time = 2.0f;
		xz = make_pair(x, z);
		is_throw = false;
		is_kicked = false;
	}
	Bomb_TB(int a, int b) {
		x = a; z = b; time = (float)GetTickCount() / 1000; explode_time = 2.0f;
		xz = make_pair(a, b);
		is_throw = false;
		is_kicked = false;
	}
	Bomb_TB(int a, int b, BYTE r, BYTE f,BYTE g) {
		x = a; z = b; time = (float)GetTickCount() / 1000; explode_time = 2.0f;
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