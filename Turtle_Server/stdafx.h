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
#define MAX_BUFF_SIZE 2000

#define MAX_USER 4
#define MAX_NPC	50
#define CASE_POS 1
#define CASE_BOMB 2
#define CASE_BOMB_EX 3
#define BUFSIZE 512

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
	char buf[MAX_BUFF_SIZE + 1];

	int type;
	int id;
	int recvbytes;

	int sendbytes;
	int remainbytes;
};


struct TurtleBomb_ID {//type:0
	BYTE size; //6
	BYTE type;
	int id;
};

struct TurtleBomb_Pos {//type:1
	BYTE size; //17
	BYTE type;
	BYTE id;
	BYTE anistate;
	BYTE is_alive;
	float posx;
	float posz;
	float rotY;

};

struct TurtleBomb_Bomb { //type:2
	BYTE size;//16
	BYTE type;
	BYTE id;
	BYTE firepower; //ȭ��
	int posx;
	int posz;
	float settime;
};

struct TurtleBomb_Explode { //type:3
	BYTE size;//11
	BYTE type;
	BYTE firepower;
	int posx;
	int posz;

};
struct TurtleBomb_Map { //type:4
	BYTE size;//227
	BYTE type;
	BYTE mapInfo[15][15];

};


#pragma pack(pop)