#pragma once
#include <iostream>
#include <vector>
#include <set>
#include <thread>
#include <mutex>
#include <queue>
#include <stack>
#include <list>
#include <thread>
#include <chrono>
#include <WinSock2.h>
#include <Windows.h>
#include <random>
#include <cmath>
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
#define CASE_JOINROOM 9
#define CASE_CREATEROOM 10
#define MAX_EVENT_SIZE 64

#define MAP_BOMB 1
#define MAP_NOTHING 2
#define MAP_BOX 3
#define MAP_ROCK 4
#define MAP_ITEM 5
#define MAP_BUSH 6
#define MAP_ITEM_F 7
#define MAP_ITEM_S 8


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



#pragma pack(push,1)
struct Pos {//type:1

	int id;
	float posx;
	float posz;
	float roty;
};





struct PosOfBOMB {//recv :type:2, send: type:3
	BYTE fire_power;
	int x;
	int y;
	
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
	BYTE roomID; //디폴트는 0. 안 들어갔다는 뜻
	BYTE is_guardian;//방장인지 아닌지 
	BYTE is_ready;
	//추가
	BYTE fire;
	BYTE bomb;
	BYTE speed;
};




struct TB_CharPos {//type:1
	BYTE size; //22
	BYTE type;
	BYTE ingame_id;
	BYTE anistate;
	BYTE is_alive;
	BYTE speed;
	BYTE fire;
	BYTE bomb;
	BYTE can_throw;
	BYTE can_kick;
	float posx;
	float posz;
	float rotY;

};

struct TB_BombPos { //type:2
	BYTE size;//16
	BYTE type;
	BYTE ingame_id;
	BYTE firepower; //화력
	int posx;
	int posz;
	float settime;
};

struct TB_BombExplode { //type:3
	BYTE size;//11
	BYTE type;
	BYTE firepower;
	int posx;
	int posz;

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
	BYTE size; //
	BYTE type;
	BYTE ingame_id;;
	BYTE item_posx; //if (g_TurtleMap.mapinfo[posz][posx] == type){g_TurtleMap.mapinfo[posz][posx] = MAP_NOTHING;charinfo[id].fire++; send(mapdata),send(charinfo[id])} 
	BYTE item_posz;
	BYTE item_type;

};
struct TB_GetItem{ //send : type 6 서버 전송-> 클라 수신
	BYTE size; //
	BYTE type;
	BYTE ingame_id;
	BYTE itemType; //타입에 따라 다른 문구가 출력된다
};


struct TB_UserInfo{ //유저정보 - type: 7
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

struct TB_Room { //방장 추가(완)
	BYTE size; //20
	BYTE type;//8
	BYTE roomID;
	BYTE people_count;
	BYTE game_start;
	BYTE people_max; //최대 인원 수
	BYTE made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
	BYTE guardian_pos; //배열에 넣을 때 -1할 것
	BYTE people_inroom[4];
	
	char password[8];
};

struct TB_join { //들어갈 때 보내는 패킷 9
	BYTE size;
	BYTE type;
	BYTE id;
	BYTE roomID;
	char password[8];
};
struct TB_joinRE { //방장 추가
	BYTE size;//9
	BYTE type;
	BYTE respond; //0이면 no, 1이면 yes
	BYTE yourpos; //1,2,3,4 중 하나
	BYTE guard_pos; //방장 위치
	BYTE people_inroom[4];


};
struct TB_create { //type:10
	BYTE size;
	BYTE type;
	BYTE id;
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
	BYTE type;
	BYTE roomID;
	BYTE id;

}; //받았을 경우 turtle_room.people_count-=1; turtle_waitroom.roodID = 0; 

#pragma pack(pop)