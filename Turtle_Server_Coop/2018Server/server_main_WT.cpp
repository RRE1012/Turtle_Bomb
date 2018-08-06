

#include "protocol.h"
#include <mysql.h>

HANDLE gh_iocp;

DWORD g_prevTime2; //GetTickCount()를 활용한 시간을 체크할 때 사용할 함수

TB_Room room;
array <Client, MAX_USER> g_clients;
map<BYTE, TB_Room> room_Map;
map<BYTE, InGameCalculator> gameRoom_Manager;
Map_TB g_TB_Map[3][4];
MYSQL* conn_ptr=NULL;
MYSQL_RES* result_sql=NULL;
MYSQL_ROW row=NULL;

list<MatchingSt> matching_list_2;
list<MatchingSt> matching_list_3;
list<MatchingSt> matching_list_4;

list<MatchingSt> playing_list;

list<TB_Room> room_page;

void SetMap(BYTE, BYTE, BYTE, TB_Map*);
void SetMapToValue(int, int);
void Throw_Calculate_Map(int x, int z, BYTE room_num, TB_ThrowBombRE* temppacket, BYTE direction);
void BoxPush_Calculate_Map(int x, int z, BYTE room_num, TB_BoxPushRE* temppacket, BYTE direction, TB_MapSetRE* tempp);
void Kick_CalculateMap(int x, int z, BYTE room_num, TB_KickBombRE* temppacket, BYTE direction, TB_MapSetRE* tempp);
void CopyRoomAtoB(TB_Room* a, TB_RoomInfo*b);
void Timer_thread();
void error_display(const char *msg, int err_no)
{
	WCHAR *lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	std::cout << msg;
	std::wcout << L"  에러" << lpMsgBuf << std::endl;
	LocalFree(lpMsgBuf);
	//while (true);
}

void ErrorDisplay(const char *location)
{
	error_display(location, WSAGetLastError());
}



void initialize()
{
	for (int i = 0; i < 3; ++i) {
		for (int j = 0; j < 4; ++j)
			SetMapToValue(i, j);
	}
	gh_iocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, 0, 0); // 의미없는 파라메터, 마지막은 알아서 쓰레드를 만들어준다.
	std::wcout.imbue(std::locale("korean"));

	WSADATA   wsadata;
	WSAStartup(MAKEWORD(2, 2), &wsadata);
	printf("Initialize Complete!!");
}

void StartRecv(int id)
{
	unsigned long r_flag = 0;
	ZeroMemory(&g_clients[id].m_rxover.m_over, sizeof(WSAOVERLAPPED));
	int ret = WSARecv(g_clients[id].m_s, &g_clients[id].m_rxover.m_wsabuf, 1,
		NULL, &r_flag, &g_clients[id].m_rxover.m_over, NULL);
	if (0 != ret) {
		int err_no = WSAGetLastError();
		if (WSA_IO_PENDING != err_no) error_display("Recv Error", err_no);
	}
}

void SendPacket(int id, void *ptr)
{
	//cout <<"ID : "<< id << endl;
	unsigned char *packet = reinterpret_cast<unsigned char *>(ptr);
	EXOVER *s_over = new EXOVER;
	s_over->is_recv = false;
	memcpy(s_over->m_iobuf, packet, packet[0]);
	s_over->m_wsabuf.buf = s_over->m_iobuf;
	if (s_over->m_iobuf[0] < 0) {
		if (s_over->m_iobuf[1] == CASE_MAP)
			s_over->m_wsabuf.len = 227;
		else
			s_over->m_wsabuf.len = 227;
	}
	else
		s_over->m_wsabuf.len = s_over->m_iobuf[0];
	ZeroMemory(&s_over->m_over, sizeof(WSAOVERLAPPED));
	int res = WSASend(g_clients[id].m_s, &s_over->m_wsabuf, 1, NULL, 0,
		&s_over->m_over, NULL);
	if (0 != res) {
		int err_no = WSAGetLastError();
		if (WSA_IO_PENDING != err_no) error_display("Send Error! ", err_no);
	}
}

void SendPutObjectPacket(int client, int object)
{
	sc_packet_put_player p;
	p.id = object;
	p.size = sizeof(p);
	p.type = SC_PUT_PLAYER;
	p.x = g_clients[object].m_x;
	p.y = g_clients[object].m_y;

	SendPacket(client, &p);
}

void SendRemovePacket(int client, int object)
{
	sc_packet_remove_player p;
	p.id = object;
	p.size = sizeof(p);
	p.type = SC_REMOVE_PLAYER;

	SendPacket(client, &p);
}

void ProcessPacket(int id, char *packet)
{

	TB_join * joininfo;
	unsigned char temproomid;
	unsigned char tempid;
	switch (packet[1])
	{
	case CASE_GAMEREADY:
	{
		TB_GAMEReady* tempt = reinterpret_cast<TB_GAMEReady*>(packet);
		temproomid = tempt->roomid;
		unsigned char tempid = tempt->myid;
		gameRoom_Manager[temproomid].ready_player[tempid] = true;
	}
		break;
	case CASE_BOSS:
	{
		TB_BossPos* tempb = reinterpret_cast<TB_BossPos*>(packet);
		temproomid = tempb->room_id;

		//gameRoom_Manager[temproomid].ingame_boss_Info.targetid = tempb->targetid;
		gameRoom_Manager[temproomid].ingame_boss_Info.anistate = tempb->anistate;
		gameRoom_Manager[temproomid].ingame_boss_Info.posx = tempb->posx;
		gameRoom_Manager[temproomid].ingame_boss_Info.posz = tempb->posz;
		gameRoom_Manager[temproomid].ingame_boss_Info.rotY = tempb->rotY;
		gameRoom_Manager[temproomid].Boss_Target_Change(tempb->targetid);
		TB_BossPos boss = { SIZEOF_TB_BossPos,CASE_BOSS,1,gameRoom_Manager[temproomid].ingame_boss_Info.targetid,gameRoom_Manager[temproomid].ingame_boss_Info.posx ,gameRoom_Manager[temproomid].ingame_boss_Info.posz,gameRoom_Manager[temproomid].ingame_boss_Info.rotY };
		for(int i=0;i<gameRoom_Manager[temproomid].howmany;++i)
			SendPacket(gameRoom_Manager[temproomid].idList[i], &boss);
	}

		break;
	case CASE_POS:
	{
		TB_CharPos* pos = reinterpret_cast<TB_CharPos*>(packet);
		bool tempbool = false;
		unsigned char tempid = pos->ingame_id;
		temproomid = pos->room_id;
		BYTE lastdeadID = 4;
		gameRoom_Manager[temproomid].ingame_Char_Info[tempid].posx = pos->posx;
		gameRoom_Manager[temproomid].ingame_Char_Info[tempid].posz = pos->posz;
		gameRoom_Manager[temproomid].ingame_Char_Info[tempid].rotY = pos->rotY;
		gameRoom_Manager[temproomid].ingame_Char_Info[tempid].is_alive = pos->is_alive;
		gameRoom_Manager[temproomid].ingame_Char_Info[tempid].anistate = pos->anistate;
		
		if (!gameRoom_Manager[temproomid].ingame_Char_Info[tempid].is_alive && !gameRoom_Manager[temproomid].IsGameOver()) {
			gameRoom_Manager[temproomid].PlayerDead(tempid);
			lastdeadID = tempid;
			tempbool = true;
			cout << "Dead!!!" << endl;
		}
		if (gameRoom_Manager[temproomid].deathcount == (room_Map[temproomid].people_count - 1)) {
			gameRoom_Manager[temproomid].SetGameOver();


		}
		if (tempbool) {
			TB_DEAD tempDead = { SIZEOF_TB_DEAD,CASE_DEAD,tempid };
			auto a = g_clients.begin();
			for (; a != g_clients.end(); a++) {
				if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
					cout << "Send You are Dead" << endl;
					
					SendPacket(a->id - 1, &tempDead);
				}

			}

			//retval = send(SocketInfoArray[j].sock, (char*)&tempd, sizeof(TB_DEAD), 0);

		}
		auto a = g_clients.begin();
		if (gameRoom_Manager[temproomid].IsGameOver() && gameRoom_Manager[temproomid].deathcount == (room_Map[temproomid].people_count - 1)) {
			unsigned char winnerid = gameRoom_Manager[temproomid].GetWinnerID();
			TB_GAMEEND gameover = { SIZEOF_TB_GAMEEND,CASE_GAMESET,winnerid,lastdeadID };
			//retval = send(SocketInfoArray[j].sock, (char*)&gameover, sizeof(TB_GAMEEND), 0);
			for (; a != g_clients.end(); a++) {
				if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
					cout << "Send GameOver" << endl;
					//if(gameRoom_Manager[temproomid].ingame_Char_Info[tempid].is_alive)
					SendPacket(a->id - 1, &gameover);
				}
			}
		}
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
				//cout << "Send Pos "<< gameRoom_Manager[temproomid].ingame_Char_Info[tempid].posx << "  To " <<a->id << endl;
				SendPacket(a->id - 1, &gameRoom_Manager[temproomid].ingame_Char_Info[tempid]);
			}

		}
		if (gameRoom_Manager[temproomid].IsGameOver())
			gameRoom_Manager[temproomid].deathcount = 0;


	}
	break;
	case CASE_BOMB:
	{
		TB_BombExplode* b_pos = reinterpret_cast<TB_BombExplode*>(packet);
		int tempx = b_pos->posx;
		int tempz = b_pos->posz;
		unsigned char roomid = b_pos->room_id;
		gameRoom_Manager[roomid].map.mapInfo[tempz][tempx] = MAP_BOMB;
		unsigned char tempfire = b_pos->firepower;
		unsigned char tempgameid = b_pos->game_id;
		//fireMap[roomid - 1][tempz][tempx] = tempfire;
		TB_BombPos tempbomb = { SIZEOF_TB_BombPos,CASE_BOMB,tempgameid,tempfire,b_pos->room_id,tempx,tempz,0.0f };
		gameRoom_Manager[roomid].bomb_Map.insert(pair<pair<int, int>, Bomb_TB>(make_pair(tempx, tempz), Bomb_TB(tempx, tempz, roomid, tempfire, tempgameid)));
		gameRoom_Manager[roomid].fireMap[tempz][tempx] = tempfire;
		TB_BombSetRE tBomb = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,tempfire,tempx,tempz };
		auto a = g_clients.begin();
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == roomid) {

				SendPacket(a->id - 1, &tBomb);
			}

		}
		//retval = send(SocketInfoArray[j].sock, (char*)&tB, sizeof(TB_MapSetRE), 0);
	}
	break;
	case CASE_ITEM_GET:
	{
		TB_ItemGet* tempitem = reinterpret_cast<TB_ItemGet*>(packet);
		temproomid = tempitem->room_id;
		unsigned char tempid = tempitem->ingame_id;

		unsigned char tempi = tempitem->item_type;
		printf("%d의 item type 획득\n", tempi);
		int tempx = tempitem->posx;
		int tempz = tempitem->posz;
		bool tempbool = gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] != MAP_NOTHING;
		printf("bool 초기화\n");
		if (tempbool) {
			gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] = MAP_NOTHING;
			TB_GetItem tempIRE = { SIZEOF_TB_GetItem,CASE_ITEM_GET,tempid,tempi };
			TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
			//retval = send(ptr->sock, (char*)&tempIRE, sizeof(TB_GetItem), 0);

			auto a = g_clients.begin();
			for (; a != g_clients.end(); a++) {
				if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
					SendPacket(a->id - 1, &tempIRE);
					SendPacket(a->id - 1, &tMap);
				}
			}
		}


	}
	break;
	case CASE_KICKBOMB:
	{
		TB_KickBomb* tK = reinterpret_cast<TB_KickBomb*>(packet);
		unsigned char tempdirc = tK->direction;
		unsigned char temproomid = tK->roomid;
		unsigned char tempid = tK->ingame_id;
		int tempx = tK->posx;
		int tempz = tK->posz;
		int ax = tempx;
		int az = tempz;
		switch (tempdirc) {
		case 1:
			ax = tempx + 1;
			break;
		case 2:
			ax = tempx - 1;
			break;
		case 3:
			az = tempz + 1;
			break;
		case 4:
			az = tempz - 1;
			break;
		}
		printf("발로까는캐릭터위치 %d,%d!!!\n", tempx, tempz);
		if (gameRoom_Manager[temproomid].bomb_Map.size() > 0) {
			auto bomb_b = gameRoom_Manager[temproomid].bomb_Map.find(pair<int, int>(ax, az));
			if (bomb_b != gameRoom_Manager[temproomid].bomb_Map.end()) {
				TB_KickBombRE tempKick = { SIZEOF_TB_ThrowBombRE,CASE_THROWBOMB,tempdirc,tempid,tempx,tempz };

				gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] = MAP_NOTHING;
				TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
				Kick_CalculateMap(tempx, tempz, temproomid, &tempKick, tempdirc, &tMap);
				Bomb_TB tempBomb = Bomb_TB(tempKick.posx_re, tempKick.posz_re, temproomid, bomb_b->second.firepower, tempid);
				tempBomb.time = bomb_b->second.time;
				tempBomb.ResetExplodeTime();
				tempBomb.is_kicked = true;

				gameRoom_Manager[temproomid].bomb_Map.insert(pair<pair<int, int>, Bomb_TB>(make_pair(tempKick.posx_re, tempKick.posz_re), tempBomb));
				gameRoom_Manager[temproomid].fireMap[tempz][tempx] = 0;
				gameRoom_Manager[temproomid].bomb_Map.erase(bomb_b);
				gameRoom_Manager[temproomid].ingame_Char_Info[tempid].anistate = TURTLE_ANI_KICK;//throw ani
				auto a = g_clients.begin();
				for (; a != g_clients.end(); a++) {
					if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
						if (tempKick.kick == 1) {

							SendPacket(a->id - 1, &tMap);
						}

						SendPacket(a->id - 1, &tempKick);
					}

				}
			}
		}
	}
	break;

	case CASE_KICKCOMPLETE:
	{
		TB_KickComplete* tK = reinterpret_cast<TB_KickComplete*>(packet);

		unsigned char temproomid = tK->roomid;

		int tempx = tK->posx;
		int tempz = tK->posz;
		TB_BombSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,MAP_BOMB,tempx,tempz };
		gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].is_kicked = false;
		gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].ResetTime();
		gameRoom_Manager[temproomid].fireMap[tempz][tempx] = gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].firepower;
		gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] = MAP_BOMB;
		auto a = g_clients.begin();
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
				SendPacket(a->id - 1, &tMap);

			}

		}


	}
	break;

	
	case CASE_THROWBOMB:
	{
		TB_ThrowBomb* tempt = reinterpret_cast<TB_ThrowBomb*>(packet);
		temproomid = tempt->roomid;
		unsigned char tempid = tempt->ingame_id;
		int tempx = tempt->posx;
		int tempz = tempt->posz;
		unsigned char tempdirect = tempt->direction;
		if (gameRoom_Manager[temproomid].bomb_Map.size() > 0) {
			auto bomb_b = gameRoom_Manager[temproomid].bomb_Map.find(make_pair(tempx, tempz));
			if (bomb_b != gameRoom_Manager[temproomid].bomb_Map.end()) {
				TB_ThrowBombRE tempThrow = { SIZEOF_TB_ThrowBombRE,CASE_THROWBOMB,tempdirect,tempid,tempx,tempz };
				Throw_Calculate_Map(tempx, tempz, temproomid, &tempThrow, tempdirect);
				gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] = MAP_NOTHING;
				gameRoom_Manager[temproomid].fireMap[tempz][tempx] = 0;

				Bomb_TB tempBomb = Bomb_TB(tempThrow.posx_re, tempThrow.posz_re, temproomid, bomb_b->second.firepower, tempid);
				tempBomb.time = bomb_b->second.time;
				tempBomb.ResetExplodeTime();
				tempBomb.is_throw = true;
				TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
				gameRoom_Manager[temproomid].bomb_Map.insert(pair<pair<int, int>, Bomb_TB>(make_pair(tempThrow.posx_re, tempThrow.posz_re), tempBomb));
				gameRoom_Manager[temproomid].bomb_Map.erase(bomb_b);
				auto a = g_clients.begin();
				for (; a != g_clients.end(); a++) {
					if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
						SendPacket(a->id - 1, &tMap);
						SendPacket(a->id - 1, &tempThrow);
					}

				}
			}
		}
	}
	break;
	case CASE_THROWCOMPLETE:
	{
		TB_ThrowComplete* tempt = reinterpret_cast<TB_ThrowComplete*>(packet);
		temproomid = tempt->roomid;
		int tempx = tempt->posx;
		int tempz = tempt->posz;
		gameRoom_Manager[temproomid].map.mapInfo[tempx][tempz] = MAP_BOMB;
		TB_BombSetRE tB = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,MAP_BOMB,tempx,tempz };
		gameRoom_Manager[temproomid].fireMap[tempz][tempx] = gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].firepower;
		gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].is_throw = false;
		gameRoom_Manager[temproomid].bomb_Map[pair<int, int>(tempx, tempz)].ResetTime();
		//fireMap[temproomid - 1][tempz][tempx] = bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].firepower;
		auto a = g_clients.begin();
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
				SendPacket(a->id - 1, &tB);

			}

		}







	}
	break;
	case CASE_BOXPUSH:
	{
		TB_BoxPush* tB = reinterpret_cast<TB_BoxPush*>(packet);

		unsigned char tempdirc = tB->direction;
		unsigned char temproomid = tB->roomid;
		unsigned char tempid = tB->ingame_id;
		int tempx = tB->posx;
		int tempz = tB->posz;


		TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
		TB_BoxPushRE tBox = { SIZEOF_TB_BoxPushRE, CASE_BOXPUSH,0,tempid };
		BoxPush_Calculate_Map(tempx, tempz, temproomid, &tBox, tempdirc, &tMap);
		printf("box받았다!!!\n");
		auto a = g_clients.begin();
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
				if (tBox.push == 1)
					SendPacket(a->id - 1, &tMap);
				SendPacket(a->id - 1, &tBox);

			}

		}

		//printf("보냈다 패킷!!!\n");



	}
	break;
	case CASE_BOXPUSHCOMPLETE:
	{
		TB_BoxPushComplete* tB = reinterpret_cast<TB_BoxPushComplete*>(packet);
		printf("boxcom받았다!!!\n");
		temproomid = tB->roomid;
		int tempx = tB->posx;
		int tempz = tB->posz;
		gameRoom_Manager[temproomid].map.mapInfo[tempz][tempx] = MAP_BOX;
		TB_MapSetRE tBd = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_BOX,tempx,tempz };

		auto a = g_clients.begin();
		for (; a != g_clients.end(); a++) {
			if (a->m_scene == 2 && a->m_isconnected&&a->roomNum == temproomid) {
				SendPacket(a->id - 1, &tBd);

			}

		}





	}
	break;
	case CASE_JOINROOM:
	{

		joininfo = reinterpret_cast<TB_join*>(packet);
		temproomid = joininfo->roomID;
		tempid = joininfo->id;
		if (temproomid != 0) {
			bool tempBool1 = room_Map[temproomid].people_count < room_Map[temproomid].people_max;
			bool tempBool2 = room_Map[temproomid].game_start != 1;
			bool tempBool3 = room_Map[temproomid].people_count >= 1;
			if (tempBool1&&tempBool2&&tempBool3) {

				g_clients[id].m_scene = 1;
				unsigned char tempcount = room_Map[temproomid].people_count;

				unsigned char tempguard = room_Map[temproomid].guardian_pos;
				for (unsigned char j = 0; j < 4; ++j) {
					if (room_Map[temproomid].people_inroom[j] == 0) {
						room_Map[temproomid].people_inroom[j] = joininfo->id;
						room_Map[temproomid].people_count = room_Map[temproomid].people_count + 1;
						g_clients[id].roomNum = temproomid;
						//tempcount = j + 1;
						tempcount = room_Map[temproomid].people_count;
						printf("%d가 %d번방에 들어감, %d+1번째 위치\n", joininfo->id, joininfo->roomID, j);
						break;
					}
				}


				g_clients[id].m_mVl.lock();
				auto a = g_clients.begin();
				for (; a != g_clients.end(); a++) {
					if (a->m_scene == 0 && a->m_isconnected) {
						cout << "Send Roominfo-Join" << endl;
						room_Map[temproomid].size = SIZEOF_TB_Room;
						room_Map[temproomid].type = CASE_ROOM;
						SendPacket(a->id - 1, &room_Map[temproomid]);
					}
					if (a->m_scene == 1 && a->m_isconnected &&a->roomNum == temproomid) {
						cout << "Send Roominfo2-Join" << endl;
						room_Map[temproomid].size = SIZEOF_TB_Room;
						room_Map[temproomid].type = CASE_ROOM;
						SendPacket(a->id - 1, &room_Map[temproomid]);
					}
				}
				TB_joinRE tempjoin = { SIZEOF_TB_joinRE,CASE_JOINROOM,1,temproomid,tempcount,tempguard };
				SendPacket(id, &tempjoin);
				g_clients[id].m_mVl.unlock();
			}
			else {
				TB_joinRE tempjoin = { SIZEOF_TB_joinRE,CASE_JOINROOM,0 };
				SendPacket(id, &tempjoin);
			}
		}
	}
	break;
	case CASE_CREATEROOM:
	{
		TB_create* createinfo = reinterpret_cast<TB_create*>(packet);
		temproomid = createinfo->roomid;
		TB_createRE re_createPacket = { SIZEOF_TB_createRE,CASE_CREATEROOM,0 };
		auto roomcreateinfo = room_Map.find(temproomid);
		if (room_Map[temproomid].made == 0) {
			//room_Map[temproomid]
			cout << "Create Room!!!" << endl;
			room_Map[temproomid].size = SIZEOF_TB_Room;
			room_Map[temproomid].type = CASE_ROOM;
			room_Map[temproomid].guardian_pos = 1;
			room_Map[temproomid].made = 1;
			room_Map[temproomid].people_count = 1;
			room_Map[temproomid].people_inroom[0] = createinfo->id;
			room_Map[temproomid].roomID = temproomid;
			room_Map[temproomid].people_max = 4;

			//ptr->roomID = room_Map[temproomid - 1].roomID;
			re_createPacket.can = 1;
			re_createPacket.roomid = room_Map[temproomid].roomID;
			g_clients[id].m_scene = 1;
			g_clients[id].is_guardian = 1;
			g_clients[id].roomNum = temproomid;
			SendPacket(id, &room_Map[temproomid]);
			SendPacket(id, &re_createPacket);
			g_clients[id].m_mVl.lock();
			auto a = g_clients.begin();
			TB_RoomInfo new_room;
			//CopyRoomAtoB(&room_Map[temproomid], &new_room);
			room_page.emplace_back(room_Map[temproomid]);

			for (; a != g_clients.end(); a++) {
				if (a->m_scene == 0 && a->m_isconnected) {
					cout << "Send Roominfo-Create" << endl;
					SendPacket(a->id - 1, &room_Map[temproomid]);
				}
			}

			g_clients[id].m_mVl.unlock();
			cout << (int)(createinfo->id) << "Create " << (int)temproomid << "Room" << endl;
		}
		else {
			cout << "Cannot Create Room!!!" << endl;
			re_createPacket.can = 0;
			SendPacket(id, &re_createPacket);
		}
	}
	break;
	case CASE_MATCH:
	{
		cout << "God Damn" << endl;
		TB_MatchingInfo* t_match = reinterpret_cast<TB_MatchingInfo*>(packet);
		unsigned char t_diff = t_match->difficulty;
		unsigned char t_man = t_match->prefer_man;
		MatchingSt tempstruct = { t_match->boss_type, t_match->map_type, t_diff, t_man };
		tempstruct.id[0] = id;
		for (int i = 1; i<4; ++i) {
			tempstruct.id[i] = -5;

		}
		bool m_inlist = false;
		if (t_man <= 2) {
			auto a = matching_list_2.begin();
			for (; a != matching_list_2.end(); a++) {
				for (int i = 0; i<2; ++i) {
					if (a->id[i] < 0) {
						a->id[i] = id;
						TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };
						m_inlist = true;

						SendPacket(id, &temp_Re);
						break;
					}
				}
			}
			if (!m_inlist) {
				matching_list_2.emplace_back(tempstruct);
				TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };


				SendPacket(id, &temp_Re);
				break;
			}
		}
		if (t_man == 3) {
			auto a = matching_list_3.begin();
			for (; a != matching_list_3.end(); a++) {
				for(int i=0;i<3;++i) {
					for (int i = 0; i<3; ++i) {
						if (a->id[i] < 0) {
							a->id[i] = id;
							TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };
							m_inlist = true;

							SendPacket(id, &temp_Re);
							break;
						}
					}
				}
			}
			if (!m_inlist) {
				matching_list_3.emplace_back(tempstruct);
				TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };


				SendPacket(id, &temp_Re);
				break;
			}
		}
		if (t_man == 4) {
			auto a = matching_list_4.begin();
			for (; a != matching_list_4.end(); a++) {
				for (int i = 0; i<4; ++i) {
					for (int i = 0; i<4; ++i) {
						if (a->id[i] < 0) {
							a->id[i] = id;
							TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };
							m_inlist = true;

							SendPacket(id, &temp_Re);
							break;
						}
					}
				}
			}
			if (!m_inlist) {
				matching_list_4.emplace_back(tempstruct);
				TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,1 };


				SendPacket(id, &temp_Re);
				break;
			}
		}
		TB_MatchingInfo_RE temp_Re = { 3,CASE_MATCH,0 };


		SendPacket(id, &temp_Re);
		
	}
	break;
	case CASE_READY:
	{
		TB_Ready* tempready = reinterpret_cast<TB_Ready*>(packet);
		unsigned char temproompos = tempready->pos_in_room;
		temproomid = tempready->room_num;
		bool b_isready = room_Map[temproomid].ready[temproompos - 1] == 1;
		if (!b_isready) {
			room_Map[temproomid].ready[temproompos - 1] = 1;
			//ptr->is_ready = 1;
		}
		else {
			room_Map[temproomid].ready[temproompos - 1] = 0;
			//ptr->is_ready = 0;
		}
		unsigned char tempReady = room_Map[temproomid].ready[temproompos - 1];
		TB_ReadyRE tempRE = { SIZEOF_TB_ReadyRE,CASE_READY,temproompos,tempReady,temproomid };
		SendPacket(id, &tempRE);
		for (int i = 0; i < 4; ++i) {
			if (room_Map[temproomid].people_inroom[i] != 0) {
				cout << (int)room_Map[temproomid].people_inroom[i] << "에게 보낸다." << endl;
				SendPacket((int)(room_Map[temproomid].people_inroom[i]) - 1, &tempRE);
			}
		}
	}

	break;
	case CASE_OUTROOM:
	{
		TB_RoomOut* tempRO = reinterpret_cast<TB_RoomOut*>(packet);
		temproomid = tempRO->roomID;
		unsigned char temproompos = tempRO->my_pos;
		if (temproompos != 0) {
			TB_RoomOutRE outRE = { SIZEOF_TB_RoomOutRE,CASE_OUTROOM,1 };
			room_Map[temproomid].people_count -= 1;
			room_Map[temproomid].people_inroom[temproompos - 1] = 0;

			if (room_Map[temproomid].people_count <= 0) {
				room_Map[temproomid].made = 0;
				room_Map[temproomid].game_start = 0;
				auto myroomState = room_Map.find(temproomid);
				room_Map.erase(myroomState);

				auto a = room_page.begin();
				for (; a != room_page.end();) {
					if (a->roomID == temproomid) {
						room_page.erase(a++);
						break;
					}
					else {
						a++;
					}
				}
			}
			if (g_clients[id].is_guardian == 1) {
				g_clients[id].is_guardian = 0; //이제 자유의 몸이야!
				for (int a = 0; a < 4; ++a) {
					if (room_Map[temproomid].people_inroom[a] != 0 && room_Map[temproomid].people_inroom[a] != g_clients[id].id) {
						room_Map[temproomid].guardian_pos = a + 1;
						g_clients[room_Map[temproomid].people_inroom[a] - 1].is_guardian = 1;
						break;
					}
				}

			}
			SendPacket(id, &outRE);
			if (tempRO->imwinner == 1) {
				g_clients[id].win_vs += 1;
				printf("%s win:%d %dXexp get\n", g_clients[id].stringID, g_clients[id].win_vs, room_Map[temproomid].people_max);
				int max = room_Map[temproomid].people_max;
				g_clients[id].exp = g_clients[id].exp + 100;
			}
			if (tempRO->imwinner == 2) {
				g_clients[id].lose_vs += 1;
				printf("%s lose:%d\n", g_clients[id].stringID, g_clients[id].lose_vs);
				g_clients[id].exp = g_clients[id].exp + 50;
			}
			g_clients[id].m_mVl.lock();
			auto a = g_clients.begin();
			g_clients[id].m_scene = 0;
			auto myroomState2 = room_Map.find(temproomid);
			bool tempbool = myroomState2 != room_Map.end();
			for (; a != g_clients.end(); a++) {
				if (a->m_scene == 0 && a->m_isconnected) {


					if (tempbool) {
						cout << "Send Roominfo-Out" << endl;
						room_Map[temproomid].size = SIZEOF_TB_Room;
						room_Map[temproomid].type = CASE_ROOM;

						room_Map[temproomid].roomID = temproomid;
						SendPacket(a->id - 1, &room_Map[temproomid]);
					}
					else {
						cout << "Send Roominfo-Out2" << endl;
						TB_Room empty_room = { SIZEOF_TB_Room,CASE_ROOM,temproomid,0,0,0,0 };
						SendPacket(a->id - 1, &empty_room);
					}

				}
				if (a->m_scene == 1 && a->m_isconnected &&a->roomNum == temproomid) {
					cout << "Send Roominfo2=Out" << endl;
					if (tempbool)
						SendPacket(a->id - 1, &room_Map[temproomid]);
					else {
						TB_Room empty_room = { SIZEOF_TB_Room,CASE_ROOM,temproomid,0,0,0,0 };
						SendPacket(a->id - 1, &empty_room);
					}
				}
			}

			g_clients[id].m_mVl.unlock();
		}
		else {
			g_clients[id].m_scene = 1;
		}
	}
	break;
	
	

	
	
	case CASE_STARTGAME:
	{
		TB_GameStart* startinfo = reinterpret_cast<TB_GameStart*>(packet);
		temproomid = startinfo->roomID;
		bool check_guard = (room_Map[temproomid].guardian_pos == startinfo->my_pos);
		int readycount = 0;
		TB_GameStartRE p_startGame = { SIZEOF_TB_GameStartRE,CASE_STARTGAME,0 };
		for (int i = 0; i < 4; ++i) {
			if (room_Map[temproomid].ready[i] == 1)
				readycount = readycount + 1;
		}
		bool check_ready = readycount + 1 == room_Map[temproomid].people_count;
		if (check_guard &&check_ready) {
			auto myroomState = gameRoom_Manager.find(temproomid);
			if (myroomState != gameRoom_Manager.end()) {
				gameRoom_Manager[temproomid].InitClass();
			}
			else {
				gameRoom_Manager.insert(make_pair(temproomid, InGameCalculator()));
			}

			p_startGame.startTB = 1;
			SetMap(room_Map[temproomid].map_mode, room_Map[temproomid].map_thema, temproomid, &gameRoom_Manager[temproomid].map);
			TB_Map tempmap = { SIZEOF_TB_MAP,CASE_MAP };
			memcpy(&tempmap, &gameRoom_Manager[temproomid].map, sizeof(Map_TB));
			room_Map[temproomid].game_start = 1;
			for (int i = 0; i < 4; ++i) {

				if (room_Map[temproomid].people_inroom[i] == 0) {
					cout << "Player Blank!!!" << endl;
					gameRoom_Manager[temproomid].PlayerBlank(i);
				}
				else {
					cout << "Player Change ID!!!" << endl;
					gameRoom_Manager[temproomid].ChangeID(i, room_Map[temproomid].people_inroom[i]);
				}
			}
			g_clients[id].m_mVl.lock();
			for (int i = 0; i < 4; ++i) {
				if (room_Map[temproomid].people_inroom[i] != 0) {
					cout << "Send Start\n" << endl;
					SendPacket((int)(room_Map[temproomid].people_inroom[i]) - 1, &gameRoom_Manager[temproomid].map);


				}
			}

			for (int i = 0; i < 4; ++i) {
				if (room_Map[temproomid].people_inroom[i] != 0) {
					g_clients[room_Map[temproomid].people_inroom[i] - 1].m_scene = 2;
					cout << "Send Map" << endl;
					SendPacket((int)(room_Map[temproomid].people_inroom[i]) - 1, &p_startGame);
				}
			}
			g_clients[id].m_mVl.unlock();
		}
		else {
			for (int i = 0; i < 4; ++i) {
				if (room_Map[temproomid].people_inroom[i] != 0) {
					//SendPacket((int)(room_Map[temproomid].people_inroom[i]) - 1, &gameRoom_Manager[temproomid].map);
					SendPacket((int)(room_Map[temproomid].people_inroom[i]) - 1, &p_startGame);
				}
			}
		}

	}

	break;
	case CASE_CONNECTSUCCESS:
	{
		TB_IDPW* idpwinfo = reinterpret_cast<TB_IDPW*>(packet);
		temproomid = idpwinfo->m_id;
		unsigned char login_type = idpwinfo->m_type;
		char tempID[20];
		char tempPW[20];

		memcpy(tempID, idpwinfo->id, sizeof(tempID));

		memcpy(tempPW, idpwinfo->pw, sizeof(tempPW));

		g_clients[id].m_mVl.lock();
		char query[255];
		int query_stat;
		conn_ptr = mysql_init(NULL);
		conn_ptr = mysql_real_connect(conn_ptr, DB_HOST, DB_USER, DB_PASS, DB_NAME, DB_PORT, (char*)NULL, 0);
		if (conn_ptr) {
			printf("Success\n");
		}
		else {
			printf("Fail To connect DB\n");
		}

		if (login_type == 1) {
			
			sprintf(query, "select * from UserTable where id='%s' and password='%s'", idpwinfo->id, idpwinfo->pw);
			printf("%s\n", query);
			if (mysql_query(conn_ptr, query))
			{
				printf("Fail To Login DB\n");
				TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,0 };
				SendPacket(id, &dbresult);
			}
			else {
				printf("Success Login\n");
				result_sql = mysql_store_result(conn_ptr);
				if (!result_sql) {
					printf("Fail To Login DB222\n");
					TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,0 };
				}
				else {
					TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,0 };
					unsigned int	fields = mysql_num_fields(result_sql);
					unsigned long* lengths = mysql_fetch_lengths(result_sql);
					MYSQL_ROW row2;
					printf("%d counts\n", fields);
					printf("%d lengths\n", lengths);

					while (row2 = mysql_fetch_row(result_sql)) {
						
						
						
						memcpy(dbresult.id_string, row2[0], 20);
						printf("%s and %s\n", dbresult.id_string, tempID);
						if (!strncmp(dbresult.id_string, tempID, sizeof(row2[0]))) {
							sscanf(row2[2], "%d", &dbresult.win);
							sscanf(row2[3], "%d", &dbresult.lose);
							sscanf(row2[4], "%d", &dbresult.tier);
							sscanf(row2[5], "%d", &dbresult.exp);
							sscanf(row2[6], "%d", &dbresult.exp_max);
							g_clients[id].win_vs = dbresult.win;
							g_clients[id].lose_vs = dbresult.lose;
							g_clients[id].tier = dbresult.tier;
							g_clients[id].exp = dbresult.exp;
							g_clients[id].exp_max = dbresult.exp_max;
							printf("ID :%s, win :%d, lose : %d!!\n", row2[0], dbresult.win, dbresult.lose);
							dbresult.connect = 1;
							break;
						}
						else {
							dbresult.connect = 0;
							break;
						}
						
						printf("Success Login1\n");
					}
					SendPacket(id, &dbresult);
					memcpy(g_clients[id].stringID, dbresult.id_string, 20);
				}
				printf("Success Login3333\n");
				mysql_free_result(result_sql);
			}

		}
		else if (login_type == 2) {

			printf("ID_Create\n");

			sprintf(query, "select EXISTS (select * from UserTable where id='%s' and password='%s') as success", tempID, tempPW);
			query_stat = mysql_query(conn_ptr, query);
			if (query_stat == 0) {

				printf("There is no ID-Create ID: %s\n", tempID);
				MYSQL_RES *result = mysql_store_result(conn_ptr);
				sprintf(query, "insert into UserTable values"
					"('%s', '%s', '0','0','0','0','50')", tempID, tempPW);
				query_stat = mysql_query(conn_ptr, query);
				if (query_stat == 0) {
					TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,2,*tempID,0,0,0,0,0 };
					memcpy(&dbresult.id_string, tempID, 20);
					SendPacket(id, &dbresult);
					g_clients[id].win_vs = 0;
					g_clients[id].lose_vs = 0;
					g_clients[id].tier = 0;
					g_clients[id].exp = 0;
					g_clients[id].exp_max = 50;
					memcpy(&g_clients[id].stringID, &dbresult.id_string, 20);
				}
				else {
					printf("Already Have%d\n", query_stat);
					TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,3 };


					SendPacket(id, &dbresult);
				}
				mysql_free_result(result);
			}
			else {
				printf("Fail%d\n", query_stat);
				TB_DBInfo_1 dbresult = { SIZEOF_TB_DBInfo_1,CASE_DB1,3 };


				SendPacket(id, &dbresult);
			}
		}
		
		mysql_close(conn_ptr);
		g_clients[id].m_mVl.unlock();

		//memcpy(

	}
	break;
	}



	sc_packet_pos pos_packet;

	pos_packet.id = id;
	pos_packet.size = sizeof(sc_packet_pos);
	pos_packet.type = SC_POS;

	
}

void DisconnectPlayer(int id)
{
	sc_packet_remove_player p;
	p.id = id;
	p.size = sizeof(p);
	p.type = SC_REMOVE_PLAYER;
	g_clients[id].m_mVl.lock();
	int query_stat;
	conn_ptr = mysql_init(NULL);
	conn_ptr = mysql_real_connect(conn_ptr, DB_HOST, DB_USER, DB_PASS, DB_NAME, DB_PORT, (char*)NULL, 0);
	if (conn_ptr) {
		printf("Success\n");
	}
	else {
		printf("Fail To connect DB\n");
	}
	char query[255];
	sprintf(query, "update UserTable set win_vs = %d,lose_vs=%d,tier=%d,exp=%d,expmax=%d where id='%s'", g_clients[id].win_vs, g_clients[id].lose_vs, g_clients[id].tier, g_clients[id].exp, g_clients[id].exp_max, g_clients[id].stringID);
	
	if (mysql_query(conn_ptr, query))//업데이트는 false가 성공
	{
		printf("Disconnect&Update DB FAIL\n");
	}
	else {
		
		printf("Disconnect&Update DB SUCCESS\n");
		g_clients[id].win_vs = 0;
		g_clients[id].lose_vs = 0;
		g_clients[id].tier = 0;
		g_clients[id].exp = 0;
		g_clients[id].exp_max = 0;
		ZeroMemory(&g_clients[id].stringID, sizeof(g_clients[id].stringID));
	}
	g_clients[id].m_mVl.unlock();
	g_clients[id].m_isconnected = false;
	g_clients[id].is_guardian = 0;
	g_clients[id].m_scene = 0;
	g_clients[id].roomNum = 0;
	
	for (int i = 0; i < MAX_USER; ++i)
	{
		if (!g_clients[i].m_isconnected)
			continue;
		if (i == id)
			continue;

		g_clients[i].m_mVl.lock();
		if (0 != g_clients[i].m_view_list.count(id))
		{
			g_clients[i].m_view_list.erase(id);
			g_clients[i].m_mVl.unlock();
			SendPacket(i, &p);
		}
		else
		{
			g_clients[i].m_mVl.unlock();
		}
	}

	closesocket(g_clients[id].m_s);
	g_clients[id].m_mVl.lock();
	g_clients[id].m_view_list.clear();
	g_clients[id].m_mVl.unlock();
	g_clients[id].m_isconnected = false;
}


void worker_thread()
{
	while (true)
	{
		unsigned long io_size;
		unsigned long long iocp_key; // 64 비트 integer , 우리가 64비트로 컴파일해서 64비트
		WSAOVERLAPPED *over;
		BOOL ret = GetQueuedCompletionStatus(gh_iocp, &io_size, &iocp_key, &over, INFINITE);
		int key = static_cast<int>(iocp_key);
		//cout << "WT::Network I/O with Client [" << key << "]\n";
		if (FALSE == ret) {
			cout << "Error in GQCS\n";
			DisconnectPlayer(key);
			continue;
		}
		if (0 == io_size) {
			DisconnectPlayer(key);
			continue;
		}

		EXOVER *p_over = reinterpret_cast<EXOVER *>(over);
		if (true == p_over->is_recv) {
			//cout << "WT:Packet From Client [" << key << "]\n";
			int work_size = io_size;
			char *wptr = p_over->m_iobuf;
			while (0 < work_size) {
				int p_size;
				if (0 != g_clients[key].m_packet_size)
					p_size = g_clients[key].m_packet_size;
				else {
					p_size = wptr[0];
					g_clients[key].m_packet_size = p_size;
				}
				int need_size = p_size - g_clients[key].m_prev_packet_size;
				if (need_size <= work_size) {
					memcpy(g_clients[key].m_packet
						+ g_clients[key].m_prev_packet_size, wptr, need_size);
					ProcessPacket(key, g_clients[key].m_packet);
					g_clients[key].m_prev_packet_size = 0;
					g_clients[key].m_packet_size = 0;
					work_size -= need_size;
					wptr += need_size;
				}
				else {
					memcpy(g_clients[key].m_packet + g_clients[key].m_prev_packet_size, wptr, work_size);
					g_clients[key].m_prev_packet_size += work_size;
					work_size = -work_size;
					wptr += work_size;
				}
			}
			StartRecv(key);
		}
		else {  // Send 후처리
				//cout << "WT:A packet was sent to Client[" << key << "]\n";
			delete p_over;
		}
	}
}

void accept_thread()   //새로 접속해 오는 클라이언트를 IOCP로 넘기는 역할
{
	SOCKET s = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, WSA_FLAG_OVERLAPPED);

	SOCKADDR_IN bind_addr;
	ZeroMemory(&bind_addr, sizeof(SOCKADDR_IN));
	bind_addr.sin_family = AF_INET;
	bind_addr.sin_port = htons(MY_SERVER_PORT);
	bind_addr.sin_addr.s_addr = INADDR_ANY;   // 0.0.0.0  아무대서나 오는 것을 다 받겠다.

	::bind(s, reinterpret_cast<sockaddr *>(&bind_addr), sizeof(bind_addr));
	listen(s, 1000);


	while (true)
	{
		SOCKADDR_IN c_addr;
		ZeroMemory(&c_addr, sizeof(SOCKADDR_IN));
		c_addr.sin_family = AF_INET;
		c_addr.sin_port = htons(MY_SERVER_PORT);
		c_addr.sin_addr.s_addr = INADDR_ANY;   // 0.0.0.0  아무대서나 오는 것을 다 받겠다.
		int addr_size = sizeof(sockaddr);

		SOCKET cs = WSAAccept(s, reinterpret_cast<sockaddr *>(&c_addr), &addr_size, NULL, NULL);
		if (INVALID_SOCKET == cs) {
			ErrorDisplay("In Accept Thread:WSAAccept()");
			continue;
		}

		cout << "New Client Connected!\n";
		int id = -1;
		for (int i = 0; i < MAX_USER; ++i) {
			if (false == g_clients[i].m_isconnected) {
				id = i;
				break;
			}
		}
		if (-1 == id) {
			cout << "MAX USER Exceeded\n";
			continue;
		}

		cout << "ID of new Client is [" << id << "]";
		g_clients[id].m_s = cs;
		g_clients[id].m_scene = 0; //로비로 들어갔다~
		g_clients[id].m_packet_size = 0;
		g_clients[id].m_prev_packet_size = 0;
		g_clients[id].m_view_list.clear();
		g_clients[id].m_x = SHOW_PLAYER_POS_X;
		g_clients[id].m_y = SHOW_PLAYER_POS_Y;
		g_clients[id].id = id + 1;
		CreateIoCompletionPort(reinterpret_cast<HANDLE>(cs), gh_iocp, id, 0);
		g_clients[id].m_isconnected = true;
		StartRecv(id);
		TB_Connect_Success s_success;

		s_success.size = SIZEOF_TB_Connect_Success;
		s_success.type = CASE_CONNECTSUCCESS;
		/*
		EXOVER *s_over = new EXOVER;
		unsigned char *packet = reinterpret_cast<unsigned char *>(&s_success);
		memcpy(s_over->m_iobuf, packet, packet[0]);
		s_over->m_wsabuf.len = s_over->m_iobuf[0];
		ZeroMemory(&s_over->m_over, sizeof(WSAOVERLAPPED));
		*/
		//int res = WSASend(cs, &s_over->m_wsabuf, 1, NULL, 0,&s_over->m_over, NULL);
		//SendPacket(id, &s_success);
		TB_ID s_id;
		s_id.id = id + 1;
		s_id.size = SIZEOF_TB_ID;
		s_id.type = CASE_ID;
		sc_packet_put_player p;
		p.id = id;
		p.size = sizeof(p);
		p.type = SC_PUT_PLAYER;
		p.x = g_clients[id].m_x;
		p.y = g_clients[id].m_y;

		SendPacket(id, &s_id);
		SendPacket(id, &s_success);
		// 나의 접속을 기존 플레이어들에게 알려준다.
		for (int i = 0; i < MAX_USER; ++i)
		{
			if (g_clients[i].m_scene == 0 && g_clients[i].m_isconnected)
			{

				if (i == id) continue;
				//내가 접속했다는 것을 알림 - 친구한테만 허용할 것인가? 아니면??
			}
		}

		// 나에게 이미 접속된 플레이어들의 정보를 알려준다.
		for (int i = 0; i < MAX_USER; ++i)
		{
			if (!g_clients[i].m_isconnected) continue;
			if (i == id) continue;



			//g_clients[id].m_mVl.lock();

			//g_clients[id].m_mVl.unlock();


		}
		//로비에서 방이 어떤게 있는지 페이지로 출력
		int tempcount = 0;
		list<TB_Room>::iterator room_t = room_page.begin();

		for (; room_t != room_page.end();) {
			if (tempcount >= 8) {
				break;
			}
			if (room_t->roomID != 0) {
				cout << "Send RRRoominfo\n" << endl;
				TB_Room roomList = { SIZEOF_TB_Room,CASE_ROOM,room_t->roomID,room_t->people_count,room_t->game_start,room_t->people_max,room_t->made,
					room_t->guardian_pos,room_t->people_inroom[0],room_t->people_inroom[1],room_t->people_inroom[2],room_t->people_inroom[3],room_t->roomstate ,room_t->map_thema ,room_t->map_mode,
					room_t->team_inroom[0],room_t->team_inroom[1],room_t->team_inroom[2],room_t->team_inroom[3],room_t->ready[0],room_t->ready[1],room_t->ready[2],room_t->ready[3] };


				SendPacket(id, &roomList);
				tempcount++;
				room_t++;
			}
			else {
				break;
			}
		}
		//SendPacket(id, &room_page);
	}
}

int main()
{
	vector <thread> w_threads;
	conn_ptr = mysql_init(NULL);

	initialize();
	for (int i = 0; i < 4; ++i) w_threads.push_back(thread{ worker_thread });

	thread a_thread{ accept_thread };
	thread timer_Thread{ Timer_thread };
	for (auto& th : w_threads) th.join();
	a_thread.join();
	timer_Thread.join();
}



void SetMap(unsigned char maptype, unsigned char mapnum, unsigned char room_num, TB_Map* map) {

	map->size = SIZEOF_TB_MAP;
	map->type = CASE_MAP;
	memcpy(map->mapInfo, &g_TB_Map[mapnum][maptype], sizeof(Map_TB));

	for (int i = 0; i < 15; ++i) {
		for (int j = 0; j < 15; ++j) {
			cout << map->mapInfo[i][j] << " ";
		}
		cout << endl;
	}

}

void SetMapToValue(int maptype, int mapnum) {
	if (maptype == 0 || maptype == 2) {
		ifstream in("Map1-1.txt");


		vector <string> v({ istream_iterator<string>(in),istream_iterator<string>() });

		in.close();
		vector<string> string_list;
		for (int i = 3 + (mapnum * 15); i < 18 + (mapnum * 15); ++i) {
			string wordlist;

			for (auto word : v[i]) {

				if (word == v[i].back()) {

				}
				if (word == ',' || word == '\0') {
					cout << wordlist << " ";
					string_list.emplace_back(wordlist);
					wordlist.clear();
				}
				else {
					//cout << word << " ";
					wordlist += word;
				}


			}
			cout << endl << endl;
			int x = 0;
			int z = 0;
			for (auto a : string_list) {


				if (x >= 5) {
					g_TB_Map[maptype][mapnum].mapTile[z][x - 5] = atoi(a.c_str());
				}
				if (x < 20)
					++x;
				if (x >= 20) {
					x = 0;
					z = z + 1;
				}
			}
		}
	}
	else if (maptype == 1) {
		ifstream in("Map2-1.txt");


		vector <string> v({ istream_iterator<string>(in),istream_iterator<string>() });

		in.close();
		vector<string> string_list;
		for (int i = 3 + (mapnum * 15); i < 18 + (mapnum * 15); ++i) {
			string wordlist;

			for (auto word : v[i]) {

				if (word == v[i].back()) {

				}
				if (word == ',' || word == '\0') {
					cout << wordlist << " ";
					string_list.emplace_back(wordlist);
					wordlist.clear();
				}
				else {
					//cout << word << " ";
					wordlist += word;
				}


			}
			cout << endl << endl;
			int x = 0;
			int z = 0;
			for (auto a : string_list) {


				if (x >= 5) {
					g_TB_Map[maptype][mapnum].mapTile[z][x - 5] = atoi(a.c_str());
				}
				if (x < 20)
					++x;
				if (x >= 20) {
					x = 0;
					z = z + 1;
				}
			}
		}

	}
}
void CopyRoomAtoB(TB_Room* a, TB_RoomInfo*b) {
	b->room_num = a->roomID;
	b->game_start = a->game_start;
	b->people_max = a->people_max;
	b->people_count = a->people_count;

}
void BoxPush_Calculate_Map(int x, int z, BYTE room_num, TB_BoxPushRE* temppacket, BYTE direction, TB_MapSetRE* tempp) {
	BYTE tempMap[15][15];
	memcpy(tempMap, gameRoom_Manager[room_num].map.mapInfo, sizeof(tempMap));
	//tempMap[z][x] = MAP_NOTHING;
	int tempx = x;
	int tempz = z;
	int startx = x;
	int startz = z;
	temppacket->push = 0;
	//direction에따라 어디로 차는지 알고 검색 1-우 2-좌 3-하 4-상
	switch (direction) {
	case 1:

		if (tempx > 14) {
			temppacket->push = 0;
			tempx = 14;
		}
		else if (tempMap[z][x + 1] == MAP_NOTHING || tempMap[z][x + 1] == MAP_ITEM || tempMap[z][x + 1] == MAP_ITEM_F || tempMap[z][x + 1] == MAP_ITEM_S) {
			temppacket->push = 0;
			tempx = x + 1;
		}
		else if (tempMap[z][x + 1] == MAP_BOX) {
			startx = x + 1;
			if (x + 2 > 14) {
				temppacket->push = 0;
				tempx = 14;
			}
			else if (tempMap[z][x + 2] == MAP_NOTHING || tempMap[z][x + 2] == MAP_ITEM || tempMap[z][x + 2] == MAP_ITEM_F || tempMap[z][x + 2] == MAP_ITEM_S) {
				temppacket->push = 1;
				tempx = x + 2;
			}
		}
		break;
	case 2:

		if (tempx < 0)
		{
			temppacket->push = 0;
			tempx = 0;
		}
		else if (tempMap[z][x - 1] == MAP_NOTHING || tempMap[z][x - 1] == MAP_ITEM || tempMap[z][x - 1] == MAP_ITEM_F || tempMap[z][x - 1] == MAP_ITEM_S) {
			temppacket->push = 0;
			tempx = x - 1;
		}
		else if (tempMap[z][x - 1] == MAP_BOX) {
			startx = x - 1;
			if (x - 2< 0) {
				temppacket->push = 0;
				tempx = 0;
			}
			else if (tempMap[z][x - 2] == MAP_NOTHING || tempMap[z][x - 2] == MAP_ITEM || tempMap[z][x - 2] == MAP_ITEM_F || tempMap[z][x - 2] == MAP_ITEM_S) {
				temppacket->push = 1;
				tempx = x - 2;
			}
		}
		break;
	case 3:

		if (tempz > 14) {
			temppacket->push = 0;
			tempz = 14;
		}
		else if (tempMap[z + 1][x] == MAP_NOTHING || tempMap[z + 1][x] == MAP_ITEM || tempMap[z + 1][x] == MAP_ITEM_F || tempMap[z + 1][x] == MAP_ITEM_S) {
			temppacket->push = 0;
			tempz = z;
		}
		else if (tempMap[z + 1][x] == MAP_BOX) {
			startz = z + 1;
			if (z + 2 > 14) {
				temppacket->push = 0;
				tempz = 14;
			}
			else if (tempMap[z + 2][x] == MAP_NOTHING || tempMap[z + 2][x] == MAP_ITEM || tempMap[z + 2][x] == MAP_ITEM_F || tempMap[z + 2][x] == MAP_ITEM_S) {
				temppacket->push = 1;
				tempz = z + 2;
			}
		}
		break;
	case 4:

		if (tempz < 0) {
			temppacket->push = 0;
			tempz = 0;
		}
		else if (tempMap[z - 1][x] == MAP_NOTHING || tempMap[z - 1][x] == MAP_ITEM || tempMap[z - 1][x] == MAP_ITEM_F || tempMap[z - 1][x] == MAP_ITEM_S) {
			temppacket->push = 0;
			tempz = z;
		}
		else if (tempMap[z - 1][x] == MAP_BOX) {
			startz = z - 1;
			if (z - 2 <0) {
				temppacket->push = 0;
				tempz = 0;
			}
			else if (tempMap[z - 2][x] == MAP_NOTHING || tempMap[z - 2][x] == MAP_ITEM || tempMap[z - 2][x] == MAP_ITEM_F || tempMap[z - 2][x] == MAP_ITEM_S) {
				temppacket->push = 1;
				tempz = z - 2;
			}
		}
		break;
	default:
		printf("Unknown Direction!!!!\n");
		break;
	}
	//throw관련 송신패킷구조체에 넣어줘야 한다.
	if (temppacket->push == 1)
		tempMap[startz][startx] = MAP_NOTHING;
	temppacket->posx = startx;
	temppacket->posz = startz;
	temppacket->posx_d = tempx;
	temppacket->posz_d = tempz;
	temppacket->direction = direction;
	tempp->posx = startx;
	tempp->posz = startz;

	memcpy(gameRoom_Manager[room_num].map.mapInfo, tempMap, sizeof(tempMap));

}
void Throw_Calculate_Map(int x, int z, BYTE room_num, TB_ThrowBombRE* temppacket, BYTE direction) {
	BYTE tempMap[15][15];
	memcpy(tempMap, gameRoom_Manager[room_num].map.mapInfo, sizeof(tempMap));
	tempMap[z][x] = MAP_NOTHING;
	int tempx = x;
	int tempz = z;
	//direction에따라 어디로 차는지 알고 검색 1-우 2-좌 3-하 4-상
	switch (direction) {
	case 1:
		tempx = x + 4;
		if (tempx > 14)
			tempx = 14;
		for (int i = tempx; i < 15; ++i) {
			if (tempMap[z][i] == MAP_NOTHING || tempMap[z][i] == MAP_ITEM || tempMap[z][i] == MAP_BUSH || tempMap[z][i] == MAP_ITEM_F || tempMap[z][i] == MAP_ITEM_S) {
				tempx = i;
				tempz = z;
				break;
			}
			if (i == 14) {
				tempx = i;
				tempz = z;
				break;
			}
		}
		break;
	case 2:
		tempx = x - 4;
		if (tempx < 0)
			tempx = 0;
		for (int i = tempx; i >= 0; --i) {
			if (tempMap[z][i] == MAP_NOTHING || tempMap[z][i] == MAP_ITEM || tempMap[z][i] == MAP_BUSH || tempMap[z][i] == MAP_ITEM_F || tempMap[z][i] == MAP_ITEM_S) {
				tempx = i;
				tempz = z;
				break;
			}
			if (i == 0) {
				tempx = i;
				tempz = z;
				break;
			}
		}
		break;
	case 3:
		tempz = z + 4;
		if (tempz > 14)
			tempz = 14;
		for (int i = tempz; i < 15; ++i) {
			if (tempMap[i][x] == MAP_NOTHING || tempMap[i][x] == MAP_ITEM || tempMap[i][x] == MAP_BUSH || tempMap[i][x] == MAP_ITEM_F || tempMap[i][x] == MAP_ITEM_S) {
				tempx = x;
				tempz = i;
				break;
			}
			if (i == 14) {
				tempx = x;
				tempz = i;
				break;
			}
		}
		break;
	case 4:
		tempz = z - 4;
		if (tempz < 0)
			tempz = 0;
		for (int i = z; i >= 0; --i) {
			if (tempMap[i][x] == MAP_NOTHING || tempMap[i][x] == MAP_ITEM || tempMap[i][x] == MAP_BUSH || tempMap[i][x] == MAP_ITEM_F || tempMap[i][x] == MAP_ITEM_S) {
				tempx = x;
				tempz = i;
				break;
			}
			if (i == 0) {
				tempx = x;
				tempz = i;
				break;
			}
		}
		break;
	default:
		printf("Unknown Direction!!!!\n");
		break;
	}
	//throw관련 송신패킷구조체에 넣어줘야 한다.
	temppacket->posx_re = tempx;
	temppacket->posz_re = tempz;
	temppacket->posx = x;
	temppacket->posz = z;
	temppacket->direction = direction;

	memcpy(gameRoom_Manager[room_num].map.mapInfo, tempMap, sizeof(tempMap));

}

void Kick_CalculateMap(int x, int z, BYTE room_num, TB_KickBombRE* temppacket, BYTE direction, TB_MapSetRE* tempp) {
	BYTE tempMap[15][15];
	memcpy(tempMap, gameRoom_Manager[room_num].map.mapInfo, sizeof(tempMap));
	tempMap[z][x] = MAP_NOTHING;
	int tempx = x;
	int tempz = z;
	int startx = x;
	int startz = z;
	temppacket->kick = 0;
	//direction에따라 어디로 차는지 알고 검색 1-우 2-좌 3-하 4-상
	switch (direction) {
	case 1:
		if (tempx > 14)
			tempx = 14;
		else if (tempMap[z][x + 1] == MAP_NOTHING || tempMap[z][x + 1] == MAP_ITEM || tempMap[z][x + 1] == MAP_ITEM_F || tempMap[z][x + 1] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempx = x + 1;
		}
		else if (tempMap[z][x + 1] == MAP_BOMB) {
			startx = x + 1;
			if (x + 2 > 14) {
				temppacket->kick = 0;
				tempx = 14;
			}
			else if (tempMap[z][x + 2] == MAP_NOTHING || tempMap[z][x + 2] == MAP_ITEM || tempMap[z][x + 2] == MAP_ITEM_F || tempMap[z][x + 2] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z][x + 2 + i] == MAP_ROCK || tempMap[z][x + 2 + i] == MAP_BOMB || tempMap[z][x + 2 + i] == MAP_BOX || x + 2 + i>14) {
						temppacket->kick = 1;
						tempx = x + 1 + i;
						break;
					}
				}
			}
		}
		break;
	case 2:
		if (tempx < 0)
			tempx = 0;
		else if (tempMap[z][x - 1] == MAP_NOTHING || tempMap[z][x - 1] == MAP_ITEM || tempMap[z][x - 1] == MAP_ITEM_F || tempMap[z][x - 1] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempx = x - 1;
		}
		else if (tempMap[z][x - 1] == MAP_BOMB) {
			startx = x - 1;
			if (x - 2 <0) {
				temppacket->kick = 0;
				tempx = 0;
			}
			else if (tempMap[z][x - 2] == MAP_NOTHING || tempMap[z][x - 2] == MAP_ITEM || tempMap[z][x - 2] == MAP_ITEM_F || tempMap[z][x - 2] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z][x - 2 - i] == MAP_ROCK || tempMap[z][x - 2 - i] == MAP_BOMB || tempMap[z][x - 2 - i] == MAP_BOX || x - 2 - i<0) {
						temppacket->kick = 1;
						tempx = x - 1 - i;
						break;
					}
				}
			}
		}
		break;
	case 3:
		if (tempz > 14)
			tempz = 14;
		else if (tempMap[z + 1][x] == MAP_NOTHING || tempMap[z + 1][x] == MAP_ITEM || tempMap[z + 1][x] == MAP_ITEM_F || tempMap[z + 1][x] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempz = z + 1;
		}
		else if (tempMap[z + 1][x] == MAP_BOMB) {
			startz = z + 1;
			if (z + 2 > 14) {
				temppacket->kick = 0;
				tempz = 14;
			}
			else if (tempMap[z + 2][x] == MAP_NOTHING || tempMap[z + 2][x] == MAP_ITEM || tempMap[z + 2][x] == MAP_ITEM_F || tempMap[z + 2][x] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z + 2 + i][x] == MAP_ROCK || tempMap[z + 2 + i][x] == MAP_BOMB || tempMap[z + 2 + i][x] == MAP_BOX || z + 2 + i>14) {
						temppacket->kick = 1;
						tempz = z + 1 + i;
						break;
					}
				}
			}
		}
		break;
	case 4:
		if (tempz < 0)
			tempz = 0;
		else if (tempMap[z - 1][x] == MAP_NOTHING || tempMap[z - 1][x] == MAP_ITEM || tempMap[z - 1][x] == MAP_ITEM_F || tempMap[z - 1][x] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempz = z - 1;
		}
		else if (tempMap[z - 1][x] == MAP_BOMB) {
			startz = z - 1;
			if (x - 2 <0) {
				temppacket->kick = 0;
				tempz = 0;
			}
			else if (tempMap[z - 2][x] == MAP_NOTHING || tempMap[z - 2][x] == MAP_ITEM || tempMap[z - 2][x] == MAP_ITEM_F || tempMap[z - 2][x] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z - 2 - i][x] == MAP_ROCK || tempMap[z - 2 - i][x] == MAP_BOMB || tempMap[z - 2 - i][x] == MAP_BOX || z - 2 - i<0) {
						temppacket->kick = 1;
						tempz = z - 1 - i;
						break;
					}
				}
			}
		}
		break;
	default:
		printf("Unknown Direction!!!!\n");
		break;



	}
	//kick관련 송신패킷구조체에 넣어줘야 한다.
	if (temppacket->kick == 1)
		tempMap[startz][startx] = MAP_NOTHING;
	temppacket->posx = startx;
	temppacket->posz = startz;
	temppacket->posx_re = tempx;
	temppacket->posz_re = tempz;
	temppacket->direction = direction;
	tempp->posx = startx;
	tempp->posz = startz;

	memcpy(gameRoom_Manager[room_num].map.mapInfo, tempMap, sizeof(tempMap));
}
void Timer_thread() {
	while (true)
	{
		Sleep(1);
		DWORD currTime = GetTickCount();
		DWORD elapsedTime = currTime - g_prevTime2;
		g_prevTime2 = currTime;
		auto b = matching_list_2.begin();
		for (; b != matching_list_2.end();) {
			//if(b== matching_list_2.begin())
				//cout << cos(45*3.1416 / 180) << endl;
				//cout << "Check Matching List\n" << endl;
			for (int i = 0; i < 2; ++i) {
				
				if (b->id[i] < 0) {
					b++;
					//cout << "Not Enough people\n" << endl;
					
				}
				else{
					if (i == 1) {
						char temp_id[4][20];
						cout << "Matching Success!!\n" << endl;
						for (int j = 0; j < b->prefer_man; ++j) {

							memcpy(&temp_id[j], &g_clients[b->id[j]].stringID, sizeof(g_clients[b->id[j]].stringID));

						}
						TB_GameStartRE_Cop temp_start = { SIZEOF_TB_GameStartRE_Cop,CASE_STARTGAME,1,2 };
						gameRoom_Manager[b->id[0]].InitClass();
						gameRoom_Manager[b->id[0]].idList[0] = b->id[0];
						gameRoom_Manager[b->id[0]].idList[1] = b->id[1];
						temp_start.roomid = b->id[0];

						memcpy(&temp_start.id1, &temp_id[0], sizeof(temp_id[0]));
						memcpy(&temp_start.id2, &temp_id[1], sizeof(temp_id[1]));
						ZeroMemory(&temp_start.id3, sizeof(temp_start.id3));
						ZeroMemory(&temp_start.id4, sizeof(temp_start.id4));
						cout << temp_id[0] << endl;
						for (int a = 0; a < b->prefer_man; ++a) {
							g_clients[b->id[a]].m_scene = 2;
							
							g_clients[b->id[a]].roomNum = b->id[0];
							SendPacket(b->id[a], &temp_start);
						}
						SetMap(room_Map[b->id[0]].map_mode, room_Map[b->id[0]].map_thema, b->id[0], &gameRoom_Manager[b->id[0]].map);
						TB_Map tempmap = { SIZEOF_TB_MAP,CASE_MAP };
						memcpy(&tempmap, &gameRoom_Manager[b->id[0]].map, sizeof(Map_TB));
						playing_list.emplace_back(*b);
						matching_list_2.erase(b++);
					
					}
					else {
						
						continue;
					}
					//playing_list.emplace_back(b);
					
				}
			}
			//SendPacket(b->id[i] - 1, &b);
			//시작하라고 지시
		}
		b = matching_list_3.begin();
		for (; b != matching_list_3.end(); ) {
			for (int i = 0; i < 3; ++i) {
				if (b->id[i] < 0) {
					b++;
					//break;
				}
				else {
					if (i == 2) {
						char temp_id[4][20];
						for (int j = 0; j <3; ++j) {

							memcpy(temp_id[j], &g_clients[b->id[i]].id, sizeof(g_clients[b->id[i]].id));

						}
						TB_GameStartRE_Cop temp_start = { SIZEOF_TB_GameStartRE_Cop,CASE_STARTGAME,1,3 };
						gameRoom_Manager[b->id[0]].InitClass();
						temp_start.roomid = b->id[0];
						memcpy(temp_start.id1, &temp_id[0], sizeof(temp_id[0]));
						memcpy(temp_start.id2, &temp_id[1], sizeof(temp_id[1]));
						memcpy(temp_start.id3, &temp_id[2], sizeof(temp_id[2]));
						ZeroMemory(&temp_start.id4, sizeof(temp_start.id4));
						SetMap(room_Map[b->id[0]].map_mode, room_Map[b->id[0]].map_thema, b->id[0], &gameRoom_Manager[b->id[0]].map);
						TB_Map tempmap = { SIZEOF_TB_MAP,CASE_MAP };
						memcpy(&tempmap, &gameRoom_Manager[b->id[0]].map, sizeof(Map_TB));
						room_Map[b->id[0]].game_start = 1;
						for (int a = 0; a < b->prefer_man; ++a) {
							SendPacket(b->id[a], &temp_start);
						}

						playing_list.emplace_back(*b);
						matching_list_3.erase(b++);
					}
					else
						continue;
					//

				}
			}
			//SendPacket(b->id[i] - 1, &b);
			//시작하라고 지시
		}
		b = matching_list_4.begin();
		for (; b != matching_list_4.end(); b++) {
			for (int i = 0; i < 4; ++i) {
				if (b->id[i] < 0) {
					b++;
				}
				else {
					if (i == 3) {
						char temp_id[4][20];
						for (int j = 0; j <4; ++j) {

							memcpy(temp_id[j], &g_clients[b->id[i]].id, sizeof(g_clients[b->id[i]].id));

						}
						TB_GameStartRE_Cop temp_start = { SIZEOF_TB_GameStartRE_Cop,CASE_STARTGAME,1,4 };
						gameRoom_Manager[b->id[0]].InitClass();
						temp_start.roomid = b->id[0];
						memcpy(temp_start.id1, &temp_id[0], sizeof(temp_id[0]));
						memcpy(temp_start.id2, &temp_id[1], sizeof(temp_id[1]));
						memcpy(temp_start.id3, &temp_id[2], sizeof(temp_id[2]));
						memcpy(temp_start.id4, &temp_id[3], sizeof(temp_id[3]));

						for (int a = 0; a < b->prefer_man; ++a) {
							SendPacket(b->id[a], &temp_start);
						}
						playing_list.emplace_back(*b);
						matching_list_4.erase(b++);
					}
					else
						continue;
					//playing_list.emplace_back(b);

				}
			}
			//SendPacket(b->id[i] - 1, &b);
			//시작하라고 지시
		}
		auto a = gameRoom_Manager.begin();
		
		for (; a != gameRoom_Manager.end(); a++)
		{
			if (!a->second.is_start) {
				int tempcount = 0;
				for (int i = 0; i < a->second.howmany; ++i) {
					if (a->second.ready_player[i])
						tempcount++;
				}
				if (tempcount == a->second.howmany) {
					cout << "ALL READY!!! START!!!" << endl;
					a->second.is_start = true;
				}
			}
			if (!a->second.IsGameOver())
			{
				
				
				
				if (a->second.is_start) {

					a->second.SetTime(elapsedTime);
					if (a->second.OneSec()) {
						float tempT = a->second.GetTime();
						TB_Time temp_t = { SIZEOF_TB_Time,CASE_TIME,tempT };
						for (int i = 0; i < 4; ++i) {
							//cout << "TimeSend" << endl;
							if (a->second.idList[i] >= 0) {
								//cout <<"Send Time "<<a->second.idList[i] << endl;
								SendPacket(a->second.idList[i], &temp_t);
							}
						}
					}
					if (a->second.bomb_Map.size() > 0)
					{
						auto b = a->second.bomb_Map.begin();
						for (; b != a->second.bomb_Map.end();)
						{
							if (b->second.GetTime())
							{
								int tempx = b->second.GetXZ().first;
								int tempz = b->second.GetXZ().second;
								unsigned char tfire = b->second.firepower;
								unsigned char tempid = b->second.game_id;
								a->second.CalculateMap(tempx, tempz, tfire, tempid);
								for (int i = 0; i < 4; ++i) {


									if (a->second.idList[i] != 0) {
										auto c = a->second.explode_List.begin();
										for (; c != a->second.explode_List.end(); c++) {
											//cout << (int)c->size<<"  "<<(int)c->type << endl;
											c->size = SIZEOF_TB_BombExplodeRE;
											c->type = CASE_BOMB_EX;
											TB_BombExplodeRE bomb = { SIZEOF_TB_BombExplodeRE,CASE_BOMB_EX,c->upfire,c->rightfire,c->downfire,c->leftfire,c->gameID,c->posx,c->posz };
											SendPacket(a->second.idList[i] - 1, &bomb);

										}
										//cout << "TimeMap" << endl;
										SendPacket(a->second.idList[i] - 1, &a->second.map);

									}

								}
								a->second.bomb_Map.erase(b++);
								a->second.explode_List.clear();
								//CalculateBomb();

							}
							else {
								b++;
							}
						}
					}
					if (a->second.boss_timestamp % 1500 == 0) {
						/*
						if (a->second.Boss_AI_Search()) {
							unsigned char templookid = a->second.Boss_AI_Search();
							TB_BossPos boss = { SIZEOF_TB_BossPos,CASE_BOSS,1,1,a->second.Boss_AI_Search(),a->second.ingame_boss_Info.posx ,a->second.ingame_boss_Info.posz,a->second.ingame_boss_Info.rotY };
							cout << "Target" << a->second.Boss_AI_Search() << endl;
							for (int i = 0; i < a->second.howmany; ++i)
								SendPacket(a->second.idList[i], &boss);
							a->second.boss_timestamp = (a->second.boss_timestamp + 1) % 200000;
						}
						*/
						a->second.boss_timestamp = (a->second.boss_timestamp + 1) % 200000;
					}
					else if (a->second.boss_timestamp % 1001 == 0) {
						a->second.ChaseAI();
						cout << "Boss Move" << endl;
						cout << "Target" << a->second.ingame_boss_Info.targetid << endl;
						TB_BossPos boss = { SIZEOF_TB_BossPos,CASE_BOSS,1,1,a->second.ingame_boss_Info.targetid,a->second.ingame_boss_Info.posx ,a->second.ingame_boss_Info.posz,a->second.ingame_boss_Info.rotY };
						for (int i = 0; i < a->second.howmany; ++i)
							SendPacket(a->second.idList[i], &boss);
						a->second.boss_timestamp = (a->second.boss_timestamp + 1) % 200000;
					}
					else {
						a->second.boss_timestamp = (a->second.boss_timestamp + 1) % 200000;
					}
					
				}
			}

		}
	}
}