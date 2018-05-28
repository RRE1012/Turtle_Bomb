#include "stdafx.h"


int g_TotalSockets = 0;

DWORD io_size, key;

Socket_Info SocketInfoArray[WSA_MAXIMUM_WAIT_EVENTS];//유저들을 담을 소켓정보 구조체의 배열

WSAEVENT EventArray[WSA_MAXIMUM_WAIT_EVENTS];//WSAEVENT의 배열

CString GetIpAddress();
Map_TB g_TB_Map[3][4]; //맵 정보2
TB_Map g_TurtleMap_room[20]; //맵 정보2
BYTE fireMap[20][15][15];
BYTE dfMap[20][15][15];
BYTE ufMap[20][15][15];
BYTE lfMap[20][15][15];
BYTE rfMap[20][15][15];
//string real_ip();
TB_CharPos char_info[4]; //4명의 캐릭터정보를 담아둘 캐릭터 정보 ->방 정보가 추가될 경우 2차원배열로 활용할 예정
TB_CharPos ingame_Char_Info[20][4]; //4명의 캐릭터정보를 담아둘 캐릭터 정보 ->방 정보가 추가될 경우 2차원배열로 활용할 예정

InGameCalculator ingamestate[20];
TB_Room room[20];
void Refresh_Map(); //맵을 서버에서 갱신하기 위해 만든 함수 -> 계산도 추가할 예정
void SetMapToValue(int, int);
BYTE g_total_member = 1;//현재 접속자의 수
void SetGameRoomInit(BYTE);
//list<Bomb_TB> bomb_List;
vector<TB_BombExplodeRE> explode_List;
map<pair<int,int>,Bomb_TB> bomb_Map[20];
void err_quit(char* msg); //에러 종료 및 출력 함수

void err_display(char* msg); //에러 출력 함수

BOOL AddSOCKETInfo(SOCKET sock); //접속자 소켓 정보 입력 함수

void RemoveSocketInfo(int nIndex);//접속자 중 종료 유저 정보 삭제 함수

void err_display(int errcode);//에러코드에 따른 에러 출력 함수

DWORD g_prevTime2; //GetTickCount()를 활용한 시간을 체크할 때 사용할 함수

void ArrayMap(); //맵 초기화 및 정렬 함수
void ReGame(BYTE);
void CalculateMap(int, int, byte, byte, TB_BombExplodeRE*);
void CalculateMap_Simple(int, int, byte, byte);

void Throw_Calculate_Map(int, int, BYTE, TB_ThrowBombRE*, BYTE);
void Kick_CalculateMap(int x, int z, BYTE , TB_KickBombRE* , BYTE, TB_MapSetRE*);
void BoxPush_Calculate_Map(int, int, BYTE, TB_BoxPushRE*, BYTE, TB_MapSetRE*);
void SetMap(BYTE,BYTE,BYTE);

int main(int argc, char* argv[]) {

	printf("-------------------------------------------------------------------------\n\n%d Server Start\n\n-------------------------------------------------------------------------\n\n", sizeof(char));
	
	ArrayMap(); // 맵 초기화
	for (int i = 0; i < 3; ++i) {
		for (int j = 0; j < 4; ++j)
			SetMapToValue(i, j);
	}
	int retval; //recv, 및 send 등 몇바이트를 받았는가 나타내는 지역변수

	WSADATA wsa; //윈속데이터 변수
	
	//기본적인 wsastartup부터 ~ 
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return 0;
	SOCKET listen_sock = socket(AF_INET, SOCK_STREAM, 0);
	if (listen_sock == INVALID_SOCKET)
		err_quit("socket()");
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serveraddr.sin_port = htons(TB_SERVER_PORT);
	CString MyIPadress = GetIpAddress();
	char * tmpch;
	char* myipadd = new char[MyIPadress.GetLength()];
	//strcpy(myipadd, CT2A(MyIPadress)); ﻿
	int ipLen= WideCharToMultiByte(CP_ACP, 0, MyIPadress, -1, NULL, 0, NULL, NULL);
	tmpch = new char[ipLen + 1];
	WideCharToMultiByte(CP_ACP, 0, MyIPadress, -1, tmpch, ipLen, NULL, NULL);


	
	//cout << real_ip() << endl;

	cout << MyIPadress.GetLength()<<"  "<<ipLen<<endl;

     cout <<"My IP :"<< (inet_addr(tmpch)) << endl;
	//delete myipadd;
	retval = ::bind(listen_sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr));
	if (retval == SOCKET_ERROR)
		err_quit("bind()");
	retval = listen(listen_sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
		err_quit("listen()");
	//~Listen 까지

	//소켓 정보 추가(WSAEventSelect를 위한 소켓 정보 추가)&WSAEventSelect
	AddSOCKETInfo(listen_sock);
	retval = WSAEventSelect(listen_sock, EventArray[g_TotalSockets - 1], FD_ACCEPT | FD_CLOSE);
	if (retval == SOCKET_ERROR)
		err_quit("WSAEventSelect()");

	WSANETWORKEVENTS m_NetworkEvents;
	SOCKET client_sock;
	SOCKADDR_IN clientaddr;
	int i, addrlen;


	while (1) { //Loop문
				//이벤트 객체 관찰하기
		DWORD currTime = GetTickCount();
		DWORD elapsedTime = currTime - g_prevTime2;
		g_prevTime2 = currTime;
		//i = WSAWaitForMultipleEvents(g_TotalSockets, EventArray, FALSE, WSA_INFINITE, FALSE);
		i = WSAWaitForMultipleEvents(g_TotalSockets, EventArray, FALSE, 1, FALSE);

		//타임아웃에 걸릴 경우 - 폭탄 시간은 계속 체크하고 있어야 하므로, 여기서도 시간체크를 실행시킨다.
		if (i == WSA_WAIT_FAILED || i == 258 || i == WAIT_TIMEOUT) {

			for (int a = 0; a < 20; ++a) {
				if (room[a].game_start == 1 && !ingamestate[a].IsGameOver()) {
					ingamestate[a].SetTime(elapsedTime);
					if (ingamestate[a].OneSec()) {
						float tempT = ingamestate[a].GetTime();
						TB_Time temp_t = { SIZEOF_TB_Time,CASE_TIME,tempT };
						for (int j = 0; j < g_TotalSockets; ++j)
						{
							if (SocketInfoArray[j].m_connected) {
								if (SocketInfoArray[j].roomID == BYTE(a + 1)) {
									//printf("시간정보 전송 %f\n", ingamestate[a].GetTime());
									retval = send(SocketInfoArray[j].sock, (char*)&temp_t, sizeof(TB_Time), 0);
								}
							}
						}
					}
					if (bomb_Map[a].size() > 0) {
						map<pair<int, int>, Bomb_TB>::iterator bomb = bomb_Map[a].begin();
						for (; bomb != bomb_Map[a].end(); ++bomb)
						{
							if (bomb->second.GetTime()) {
								BYTE temproom = bomb->second.room_num;
								int tempx = bomb->second.xz.first;
								int tempz = bomb->second.xz.second;
								BYTE tempfire = bomb->second.firepower;
								g_TurtleMap_room[temproom - 1].mapInfo[tempz][tempx] = MAP_NOTHING;
								TB_BombExplodeRE temp_Bomb = { SIZEOF_TB_BombExplodeRE,CASE_BOMB_EX,0,0,0,0,bomb->second.game_id };
								CalculateMap(tempx, tempz, tempfire, temproom, &temp_Bomb);
								fireMap[temproom - 1][tempz][tempx] = 0;
								g_TurtleMap_room[temproom - 1].size = SIZEOF_TB_MAP;
								for (int j = 0; j < g_TotalSockets; ++j)
								{

									//printf("폭발할 폭탄 전송!!!\n");
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproom) {
											//printf("Send size : %d\n", temp_bomb.size);
											retval = send(SocketInfoArray[j].sock, (char*)&temp_Bomb, sizeof(TB_BombExplodeRE), 0);
											if (explode_List.size() > 0) {
												for(int i=0;i<explode_List.size();++i)
													retval = send(SocketInfoArray[j].sock, (char*)&explode_List[i], sizeof(TB_BombExplodeRE), 0);

											}
											//printf("Retval size : %d\n", retval);
											//printf("Send size : %d\n", g_TurtleMap.size);

											retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap_room[temproom - 1], sizeof(TB_Map), 0);
											//printf("Retval size : %d\n", retval);
										}
									}
								}
								explode_List.clear();
								bomb_Map[a].erase(bomb++);
								if (bomb_Map[a].size() <= 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			continue;
		}
		else {
			for (int a = 0; a < 20; ++a) {
				if (room[a].game_start == 1 && !ingamestate[a].IsGameOver()) {
					ingamestate[a].SetTime(elapsedTime);
					if (ingamestate[a].OneSec()) {
						float tempT = ingamestate[a].GetTime();
						TB_Time temp_t = { SIZEOF_TB_Time,CASE_TIME,tempT };
						for (int j = 0; j < g_TotalSockets; ++j)
						{
							if (SocketInfoArray[j].m_connected) {
								if (SocketInfoArray[j].roomID == BYTE(a + 1)) {
									//printf("시간정보 전송 %f\n", ingamestate[a].GetTime());
									retval = send(SocketInfoArray[j].sock, (char*)&temp_t, sizeof(TB_Time), 0);
								}
							}
						}
					}
					if (bomb_Map[a].size() > 0) {
						map<pair<int, int>, Bomb_TB>::iterator bomb = bomb_Map[a].begin();
						for (; bomb != bomb_Map[a].end(); ++bomb)
						{
							if (bomb->second.GetTime()) {
								BYTE temproom = bomb->second.room_num;
								int tempx = bomb->second.xz.first;
								int tempz = bomb->second.xz.second;
								BYTE tempfire = bomb->second.firepower;
								g_TurtleMap_room[temproom - 1].mapInfo[tempz][tempx] = MAP_NOTHING;
								TB_BombExplodeRE temp_Bomb = { SIZEOF_TB_BombExplodeRE,CASE_BOMB_EX,0,0,0,0,bomb->second.game_id };
								CalculateMap(tempx, tempz, tempfire, temproom, &temp_Bomb);
								fireMap[temproom - 1][tempz][tempx] = 0;
								g_TurtleMap_room[temproom - 1].size = SIZEOF_TB_MAP;
								for (int j = 0; j < g_TotalSockets; ++j)
								{

									//printf("폭발할 폭탄 전송!!!\n");
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproom) {
											//printf("Send size : %d\n", temp_bomb.size);
											retval = send(SocketInfoArray[j].sock, (char*)&temp_Bomb, sizeof(TB_BombExplodeRE), 0);
											for (int i = 0; i<explode_List.size(); ++i)
												retval = send(SocketInfoArray[j].sock, (char*)&explode_List[i], sizeof(TB_BombExplodeRE), 0);
											//printf("Retval size : %d\n", retval);
											//printf("Send size : %d\n", g_TurtleMap.size);

											retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap_room[temproom - 1], sizeof(TB_Map), 0);
											//printf("Retval size : %d\n", retval);
										}
									}
								}
								explode_List.clear();
								bomb_Map[a].erase(bomb++);
								if (bomb_Map[a].size() <= 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			
			
			i -= WSA_WAIT_EVENT_0;
			//구체적인 네트워크 이벤트 알아내기
			retval = WSAEnumNetworkEvents(SocketInfoArray[i].sock, EventArray[i], &m_NetworkEvents);
			if (retval == SOCKET_ERROR)
				continue;
			//FD_ACCEPT 이벤트 처리
			if (m_NetworkEvents.lNetworkEvents&FD_ACCEPT)
			{
				if (m_NetworkEvents.iErrorCode[FD_ACCEPT_BIT] != 0)
				{
					err_display(m_NetworkEvents.iErrorCode[FD_ACCEPT_BIT]);
					continue;
				}
				addrlen = sizeof(clientaddr);
				client_sock = accept(SocketInfoArray[i].sock, (SOCKADDR*)&clientaddr, &addrlen);
				if (client_sock == INVALID_SOCKET)
				{
					err_display("accept()");
					continue;
				}
				printf("[TCP 서버] 클라이언트 접속 : IP 주소 =%s, 포트번호=%d\n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));

				TB_ID tempid = { SIZEOF_TB_ID,CASE_ID,g_total_member };
				retval = send(client_sock, (char*)&tempid, sizeof(TB_ID), 0); //ID 전송.
				printf("전송-%d번째 -ID : %d\n", i, g_total_member);

				// 현재 접속자 수 체크

				retval = send(client_sock, (char*)&room, sizeof(room), 0); //초기화된 맵정보 클라이언트에게 전송
				printf("방정보 전송 :%d바이트\n", retval);
				//retval = send(client_sock, (char*)&g_TurtleMap, sizeof(TB_Map), 0); //초기화된 맵정보 클라이언트에게 전송
				//printf("맵정보 전송 :%d바이트\n", retval);

				if (g_TotalSockets >= WSA_MAXIMUM_WAIT_EVENTS)  //접속자가 서버의 최대일 경우
				{
					printf("[오류] 더 이상 접속을 받아들일 수 없습니다!!!!!\n");
					closesocket(client_sock);
					continue;
				}

				if (retval == SOCKET_ERROR) //send 오류 시
				{
					if (WSAGetLastError() != WSAEWOULDBLOCK)
					{
						err_display("send()");
						RemoveSocketInfo(i);
					}
					continue;
				}

				//소켓 정보 추가
				AddSOCKETInfo(client_sock);
				g_total_member++;
				retval = WSAEventSelect(client_sock, EventArray[g_TotalSockets - 1], FD_READ | FD_WRITE | FD_CLOSE);
				if (retval == SOCKET_ERROR)
					err_quit("WSAEventSelect()-client");

			}
			//클라이언트 요청이 읽기나 쓰기일 경우
			if (m_NetworkEvents.lNetworkEvents&FD_READ || m_NetworkEvents.lNetworkEvents&FD_WRITE)
			{
				if (m_NetworkEvents.lNetworkEvents&FD_READ &&m_NetworkEvents.iErrorCode[FD_READ_BIT] != 0)
				{
					err_display(m_NetworkEvents.iErrorCode[FD_READ_BIT]);
					continue;
				}
				if (m_NetworkEvents.lNetworkEvents&FD_WRITE &&m_NetworkEvents.iErrorCode[FD_WRITE_BIT] != 0)
				{
					err_display(m_NetworkEvents.iErrorCode[FD_WRITE_BIT]);
					continue;
				}

				Socket_Info* ptr = &SocketInfoArray[i];

				int m_temp_id = 0;

				if (ptr->recvbytes == 0)
				{
					//데이터 받기
					char recv_buf[MAX_BUFF_SIZE];
					retval = recv(ptr->sock, (char*)recv_buf, sizeof(recv_buf), 0);
					char* c_buf = recv_buf;

					if (retval == SOCKET_ERROR)
					{
						err_display("recv()");
						printf("수신 오류 !!\n");
						continue;
					}
					else
					{
						memcpy(ptr->buf + ptr->remainbytes, c_buf, retval);
						//printf("%d바이트 수신 !!\n", retval);
						//c_buf[retval] = '\0';
						//ptr->buf[retval + ptr->remainbytes] = '\0';
						//ptr->recvbytes = ptr->recvbytes+retval;
						ptr->remainbytes = ptr->remainbytes + retval;
						//c_buf[ptr->remainbytes] = '\0';
					}




					if (ptr->remainbytes >= 4)
					{
						switch (c_buf[1]) {
						case CASE_POS: //CharPos
							if (ptr->remainbytes >= SIZEOF_TB_CharPos) {
								TB_CharPos* pos = reinterpret_cast<TB_CharPos*>(c_buf);
								//필수 -
								bool tempbool = false;
								BYTE tempid = pos->ingame_id;
								BYTE temproom = pos->room_id;
								/*
								char_info[tempid].anistate = pos->anistate;
								char_info[tempid].is_alive = pos->is_alive;
								char_info[tempid].posx = pos->posx;
								char_info[tempid].rotY = pos->rotY;
								char_info[tempid].posz = pos->posz;
								*/

								ingame_Char_Info[temproom - 1][tempid].anistate = pos->anistate;
								ingame_Char_Info[temproom - 1][tempid].is_alive = pos->is_alive;
								ingame_Char_Info[temproom - 1][tempid].posx = pos->posx;
								ingame_Char_Info[temproom - 1][tempid].rotY = pos->rotY;
								ingame_Char_Info[temproom - 1][tempid].posz = pos->posz;
								if (!ingame_Char_Info[temproom - 1][tempid].is_alive && !ingamestate[temproom - 1].IsGameOver()) {
									ingamestate[temproom - 1].PlayerDead(tempid);
									tempbool = true;
									printf("%d\n", ingamestate[temproom - 1].deathcount);
								}

								//몇명이 죽었는가 테스트
								
								if (ingamestate[temproom - 1].deathcount == (room[temproom - 1].people_count - 1)) {
									ingamestate[temproom - 1].SetGameOver();
									
									printf("GameOver!!!!%d\n", ingamestate[temproom - 1].deathcount);
								}
								
								//printf("1p포지션값  :x :%f, z:%f , roty:%f \n", char_info[0].posx, char_info[0].posz, char_info[0].rotY);
								//printf("2p포지션값  :x :%f, z:%f , roty:%f \n", char_info[1].posx, char_info[1].posz, char_info[1].rotY);
								//printf("3p포지션값  :x :%f, z:%f , roty:%f \n", char_info[2].posx, char_info[2].posz, char_info[2].rotY);
								//printf("4p포지션값  :x :%f, z:%f , roty:%f \n", char_info[3].posx, char_info[3].posz, char_info[3].rotY);
								ptr->remainbytes -= SIZEOF_TB_CharPos;

								memcpy(c_buf, ptr->buf + SIZEOF_TB_CharPos, ptr->remainbytes);
								memcpy(ptr->buf, c_buf, ptr->remainbytes);

								for (int j = 0; j < g_TotalSockets; ++j) {

									if (SocketInfoArray[j].m_connected) {
										//printf("Send size : %d\n", char_info[tempid].size);
										if (SocketInfoArray[j].roomID == temproom) {
											ingame_Char_Info[temproom - 1][tempid].size = SIZEOF_TB_CharPos;
											ingame_Char_Info[temproom - 1][tempid].type = CASE_POS;
											ingame_Char_Info[temproom - 1][tempid].anistate = 1;
											retval = send(SocketInfoArray[j].sock, (char*)&ingame_Char_Info[temproom - 1][tempid], sizeof(TB_CharPos), 0);
											if (ingamestate[temproom - 1].IsGameOver()&& ingamestate[temproom - 1].deathcount == (room[temproom - 1].people_count - 1)) {

												BYTE winnerid = ingamestate[temproom - 1].GetWinnerID();
												TB_GAMEEND gameover = { SIZEOF_TB_GAMEEND,CASE_GAMESET,winnerid };
												retval = send(SocketInfoArray[j].sock, (char*)&gameover, sizeof(TB_GAMEEND), 0);
												

											}
											if (tempbool) {
												TB_DEAD tempd = { SIZEOF_TB_DEAD,CASE_DEAD,tempid };
												//retval = send(SocketInfoArray[j].sock, (char*)&tempd, sizeof(TB_DEAD), 0);
												retval = send(SocketInfoArray[j].sock, (char*)&tempd, sizeof(TB_DEAD), 0);
											}
										}
										//printf("Retval size : %d\n", retval);
									}
								}
								if (ingamestate[temproom - 1].IsGameOver())
									ingamestate[temproom - 1].deathcount = 0;


								break;
							}
							break;
						case CASE_BOMB:
							if (ptr->remainbytes >= SIZEOF_TB_BombExplode) {
								TB_BombExplode* b_pos = reinterpret_cast<TB_BombExplode*>(c_buf);
								int tempx = b_pos->posx;
								int tempz = b_pos->posz;
								BYTE roomid = b_pos->room_id;
								g_TurtleMap_room[roomid - 1].mapInfo[tempz][tempx] = MAP_BOMB;
								printf("폭탄포지션값  :x :%d, z:%d ,  \n", b_pos->posx, b_pos->posz);

								BYTE tempfire = b_pos->firepower;
								BYTE tempgameid = b_pos->game_id;
								fireMap[roomid - 1][tempz][tempx] = tempfire;
								TB_BombPos tempbomb = { SIZEOF_TB_BombPos,CASE_BOMB,tempgameid,tempfire,b_pos->room_id,tempx,tempz,0.0f };
								
								bomb_Map[roomid - 1].insert(pair<pair<int,int>,Bomb_TB>(make_pair(tempx, tempz), Bomb_TB(tempx, tempz, roomid, tempfire, tempgameid)));
								ptr->remainbytes -= SIZEOF_TB_BombExplode;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_BombExplode, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
								//Refresh_Map();
								TB_BombSetRE tB = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,tempfire,tempx,tempz };


								for (int j = 0; j < g_TotalSockets; ++j) {
									//폭탄을 받았으므로 갱신된 맵정보를 접속해있는 유저에게 전송
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == roomid) {
											printf("Send size : %d\n", g_TurtleMap_room[roomid - 1].size);

											retval = send(SocketInfoArray[j].sock, (char*)&tB, sizeof(TB_MapSetRE), 0);
											printf("Retval size : %d\n", retval);
											printf("Bomb가 추가된 맵정보값 전송!\n");
										}
									}
								}
								break;
							}
							break;
						case CASE_JOINROOM:
							if (ptr->remainbytes >= SIZEOF_TB_join) {
								TB_join* joininfo = reinterpret_cast<TB_join*>(c_buf);
								byte temproomid = joininfo->roomID;//방id 변수
								printf("ID:%d\n", temproomid);

								if (temproomid != 0) {
									bool bool_a = room[temproomid - 1].people_count < room[temproomid - 1].people_max;
									bool bool_b = room[temproomid - 1].game_start != 1;
									bool bool_c = room[temproomid - 1].made == 1;
									if (bool_a&&bool_b&&bool_c) {
										BYTE tempcount = room[temproomid - 1].people_count + 1;
										BYTE tempguard = room[temproomid - 1].guardian_pos;
										
										room[temproomid - 1].people_count += 1;
										for (BYTE j = 0; j < 4; ++j) {
											if (room[temproomid - 1].people_inroom[j] == 0) {
												room[temproomid - 1].people_inroom[j] = joininfo->id;
												ptr->pos_inRoom = j + 1;
												tempcount = j + 1;
												printf("%d가 %d번방에 들어감, %d+1번째 위치\n", joininfo->id, joininfo->roomID, j);
												break;
											}
										}
										ptr->roomID = temproomid;
										ptr->is_guardian = 0;
										TB_joinRE tempjoin = { SIZEOF_TB_joinRE,CASE_JOINROOM,1,temproomid,tempcount,tempguard };
										for (int j = 0; j < 4; ++j)
											tempjoin.people_inroom[j] = room[temproomid - 1].people_inroom[j];
										for (int j = 0; j < g_TotalSockets; ++j) {
											if (SocketInfoArray[j].m_connected) {
												if (SocketInfoArray[j].roomID == 0) {
													//printf("Send size : %d\n", g_TurtleMap.size);
													retval = send(SocketInfoArray[j].sock, (char*)&room, sizeof(room), 0); //방정보 전송
												}
												if (SocketInfoArray[j].roomID == temproomid) {
													//printf("Send size : %d\n", g_TurtleMap.size);

													retval = send(SocketInfoArray[j].sock, (char*)&room[temproomid - 1], sizeof(TB_Room), 0); //방에 들어있는 친구들에게도 전송
												}

												//retval = send(SocketInfoArray[j].sock, (char*)&tempjoin, sizeof(tempjoin), 0);

											}

										}

										retval = send(ptr->sock, (char*)&tempjoin, sizeof(tempjoin), 0);
										printf("send yes!! %d\n", retval);
									}
									else {
										TB_joinRE tempjoin = { SIZEOF_TB_joinRE,CASE_JOINROOM,0 };
										retval = send(ptr->sock, (char*)&tempjoin, sizeof(tempjoin), 0);
										printf("send no! %d\n", retval);
									}
									ptr->remainbytes -= SIZEOF_TB_join;
									memcpy(ptr->buf, c_buf + SIZEOF_TB_join, ptr->remainbytes);
									memset(c_buf, 0, sizeof(c_buf));
									memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
								}
							}
							break;
						case CASE_CREATEROOM:
							if (ptr->remainbytes >= SIZEOF_TB_create) {
								TB_create* createinfo = reinterpret_cast<TB_create*>(c_buf);
								TB_createRE tempa = { SIZEOF_TB_createRE,CASE_CREATEROOM,0 };
								/*for (int a = 0; a < 20; ++a) {
									if (room[a].made == 0) {
										tempa.can = 1;
										tempa.roomid = room[a].roomID;
										room[a].guardian_pos = 1;
										room[a].made = 1;
										room[a].people_count = 1;
										room[a].people_inroom[0] = createinfo->id;
										ptr->is_guardian = 1;
										ptr->roomID = room[a].roomID;
										
										//printf("%d Created No.%d Room!!\n", room[a].people_inroom[0],ptr->roomID);
										break;
									}
								}*/

								for (auto roominfo : room) {
									if (roominfo.made == 1)
										printf("%d(Made) ", roominfo.roomID);
								}
								BYTE temproomid = createinfo->roomid;
								if (room[temproomid - 1].made == 0) {
									room[temproomid - 1].guardian_pos = 1;
									room[temproomid - 1].made = 1;
									room[temproomid - 1].people_count = 1;
									room[temproomid - 1].people_inroom[0] = createinfo->id;
									ptr->is_guardian = 1;
									ptr->roomID = room[temproomid - 1].roomID;
									tempa.can = 1;
									tempa.roomid = room[temproomid - 1].roomID;
								}
								retval = send(ptr->sock, (char*)&tempa, sizeof(TB_createRE), 0);
								for (int j = 0; j < g_TotalSockets; ++j) {
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == 0) {
											//printf("Send size : %d\n", g_TurtleMap.size);
											retval = send(SocketInfoArray[j].sock, (char*)&room, sizeof(room), 0); //초기화된 맵정보 클라이언트에게 전송
										}
									}
								}
								ptr->remainbytes -= SIZEOF_TB_create;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_create, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_READY:
							if (ptr->remainbytes >=SIZEOF_CASE_READY) {
								TB_Ready* tempready = reinterpret_cast<TB_Ready*>(c_buf);
								byte temproompos = tempready->pos_in_room;
								byte temproomid = tempready->room_num;
								bool b_isready = room[temproomid - 1].ready[temproompos - 1] == 1;
								if (!b_isready) {
									room[temproomid - 1].ready[temproompos - 1] = 1;
									ptr->is_ready = 1;
								}
								else {
									room[temproomid - 1].ready[temproompos - 1] = 0;
									ptr->is_ready = 0;
								}
								BYTE tempReady = room[temproomid - 1].ready[temproompos - 1];
								TB_ReadyRE tempRE = { SIZEOF_TB_ReadyRE,CASE_READY,temproompos,tempReady,temproomid };
								for (int j = 0; j < g_TotalSockets; ++j) {
									//폭탄을 받았으므로 갱신된 맵정보를 접속해있는 유저에게 전송
									if (SocketInfoArray[j].m_connected) {
										printf("Connected\n");
										if (SocketInfoArray[j].roomID == temproomid) {
											
											

											retval = send(SocketInfoArray[j].sock, (char*)&tempRE, sizeof(TB_ReadyRE), 0);
											printf("Retval size : %d\n", retval);


										}
									}
								}


								ptr->remainbytes -= SIZEOF_CASE_READY;
								memcpy(ptr->buf, c_buf + SIZEOF_CASE_READY, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_STARTGAME:
							if (ptr->remainbytes >= SIZEOF_TB_GameStart) {

								TB_GameStart* startinfo = reinterpret_cast<TB_GameStart*>(c_buf);

								byte temproomid = startinfo->roomID;
								printf("Get Start Data from No.%d Room\n", startinfo->roomID);
								bool check_guard = (room[temproomid - 1].guardian_pos == startinfo->my_pos);
								bool survivalgame = room[temproomid - 1].roomstate == 0;
								int teamaCount = 0;
								for (int t = 0; t < 4; ++t) {
									if (room[temproomid - 1].team_inroom[t] == 0)
										teamaCount++;
								}
								bool teamGame;
								//room[temproomid-1].people_max<4
								if (room[temproomid - 1].people_count <= 2)
									teamGame = (1 == teamaCount) && room[temproomid - 1].roomstate == 1;
								else if (room[temproomid - 1].people_count == 3)
									teamGame = (1 == teamaCount || 2 == teamaCount) && room[temproomid - 1].roomstate == 1;
								else
									teamGame = 2 == teamaCount&& room[temproomid - 1].roomstate == 1;
								printf("Start Check guardian_pos : %d == %d?\n", room[temproomid - 1].guardian_pos, startinfo->my_pos);
								//bool check_all_ready= 전원 준비상태인가
								int readycount = 0;
								for (int i = 0; i < 4; ++i) {
									if (room[temproomid - 1].ready[i] == 1)
										readycount = readycount + 1;
								}
								teamGame = readycount + 1 == room[temproomid - 1].people_count;
								if (check_guard && (teamGame)) {
									if (ingamestate[temproomid - 1].IsGameOver()) {
										ingamestate[temproomid - 1].InitClass();
										SetGameRoomInit(temproomid);
									}
									ReGame(temproomid);
									SetMap(room[temproomid - 1].map_mode, room[temproomid - 1].map_thema, temproomid);
									room[temproomid - 1].game_start = 1;
									for (int i = 0; i < 4; ++i) {
										printf("%d는 없는 유저\n",i);
										if (room[temproomid - 1].people_inroom[i] == 0)
											ingamestate[temproomid - 1].PlayerBlank(i);
									}
									//ingamestate[temproomid - 1]
									printf("True\n");
									for (int j = 0; j < g_TotalSockets; ++j) {
										//폭탄을 받았으므로 갱신된 맵정보를 접속해있는 유저에게 전송
										if (SocketInfoArray[j].m_connected) {
											printf("Connected\n");
											if (SocketInfoArray[j].roomID == temproomid) {
												printf("Connected\n");
												
												g_TurtleMap_room[temproomid - 1].size = SIZEOF_TB_MAP;
												retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap_room[temproomid - 1], sizeof(TB_Map), 0); //초기화된 맵정보 클라이언트에게 전송
												
												printf("Retval size : %d\n", retval);
												
												printf("맵정보 전송 :%d바이트\n", retval);

											}
										}
									}
									for (int j = 0; j < g_TotalSockets; ++j) {
										//폭탄을 받았으므로 갱신된 맵정보를 접속해있는 유저에게 전송
										if (SocketInfoArray[j].m_connected) {
											printf("Connected\n");
											if (SocketInfoArray[j].roomID == temproomid) {
												printf("Connected\n");
												TB_GameStartRE tempRE = { SIZEOF_TB_GameStartRE,CASE_STARTGAME,1 };
												retval = send(SocketInfoArray[j].sock, (char*)&tempRE, sizeof(TB_GameStartRE), 0);
											}
										}
									}
								}
								else {
									TB_GameStartRE tempRE = { SIZEOF_TB_GameStartRE,CASE_STARTGAME,0 };
									retval = send(ptr->sock, (char*)&tempRE, sizeof(TB_GameStartRE), 0);
									printf("Rejected size : %d\n", retval);
								}
								ptr->remainbytes -= SIZEOF_TB_GameStart;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_GameStart, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_OUTROOM:
							if (ptr->remainbytes >= SIZEOF_TB_RoomOut) {
								TB_RoomOut* tempRO = reinterpret_cast<TB_RoomOut*>(c_buf);
								byte temproomid = tempRO->roomID;
								byte temproompos = tempRO->my_pos;
								TB_RoomOutRE tempRE = { SIZEOF_TB_RoomOutRE,CASE_OUTROOM,1 };
								room[temproomid - 1].people_count -= 1;
								room[temproomid - 1].people_inroom[temproompos - 1] = 0;

								if (room[temproomid - 1].people_count <= 0) {
									room[temproomid - 1].made = 0;
									room[temproomid - 1].game_start = 0;
								}
								if (ptr->is_guardian == 1) {
									ptr->is_guardian = 0; //이제 자유의 몸이야!
									for (int a = 0; a < 4; ++a) {
										if (room[temproomid - 1].people_inroom[a] != 0 && room[temproomid - 1].people_inroom[a] != ptr->id) {
											room[temproomid - 1].guardian_pos = a + 1;

											break;
										}
									}

								}


								for (int j = 0; j < g_TotalSockets; ++j) {
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("Send size : %d\n", g_TurtleMap.size);

											//for (int t = 0; t < 4; ++t) {
											BYTE tempt = room[temproomid - 1].guardian_pos;
											if (SocketInfoArray[j].id == room[temproomid - 1].people_inroom[tempt - 1]) {
												printf("Guardian Change\n");
												SocketInfoArray[j].is_guardian = 1;
											}

											//

											//자기한테 보내는 것을 방지해야 할 것
											retval = send(SocketInfoArray[j].sock, (char*)&room[temproomid - 1], sizeof(TB_Room), 0); //초기화된 맵정보 클라이언트에게 전송

										}

										if (SocketInfoArray[j].roomID == 0) {
											retval = send(SocketInfoArray[j].sock, (char*)&room, sizeof(room), 0); //초기화된 맵정보 클라이언트에게 전송
										}
									}

								}
								ptr->roomID = 0;
								retval = send(ptr->sock, (char*)&tempRE, sizeof(TB_RoomOutRE), 0);
								retval = send(ptr->sock, (char*)&room, sizeof(room), 0);
								ptr->remainbytes -= SIZEOF_TB_RoomOut;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_RoomOut, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_FORCEOUTROOM:
							if (ptr->remainbytes >= SIZEOF_TB_GetOut) {
								TB_GetOut* tempFO = reinterpret_cast<TB_GetOut*>(c_buf);
								byte temproomid = tempFO->roomID;
								byte temproompos = tempFO->position;
								printf("강퇴 요청자 :ID:%d\n", ptr->id);
								room[temproomid - 1].people_count -= 1;
								byte tempid = room[temproomid - 1].people_inroom[temproompos - 1];
								room[temproomid - 1].people_inroom[temproompos - 1] = 0;

								TB_GetOUTRE tempRE = { SIZEOF_TB_GetOutRE,CASE_FORCEOUTROOM };
								for (int j = 0; j < g_TotalSockets; ++j) {
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].id == tempid) {
											//강퇴당했다!
											printf("%d가 강퇴당함!!\n", SocketInfoArray[j].id);
											retval = send(SocketInfoArray[j].sock, (char*)&tempRE, sizeof(TB_GetOUTRE), 0);
											retval = send(SocketInfoArray[j].sock, (char*)&room, sizeof(room), 0);
										}
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("Send size : %d\n", g_TurtleMap.size);
											retval = send(SocketInfoArray[j].sock, (char*)&room[temproomid - 1], sizeof(TB_Room), 0); //초기화된 맵정보 클라이언트에게 전송
										}


									}

								}
								ptr->remainbytes -= SIZEOF_TB_GetOut;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_GetOut, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_ROOMSETTING:
							if (ptr->remainbytes >= SIZEOF_TB_RoomSetting) {
								TB_RoomSetting* temproom = reinterpret_cast<TB_RoomSetting*>(c_buf);
								BYTE temproomid = temproom->roomid;
								BYTE temproomstate = temproom->peoplemax;
								BYTE tempmapthema = temproom->mapthema;
								BYTE tempmaptype = temproom->mapnum;
								room[temproomid - 1].people_max = temproomstate;
								room[temproomid - 1].map_mode = tempmaptype;
								room[temproomid - 1].map_thema = tempmapthema;
								for (int j = 0; j < g_TotalSockets; ++j) {
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("Send size : %d\n", g_TurtleMap.size);
											retval = send(SocketInfoArray[j].sock, (char*)&room[temproomid - 1], sizeof(TB_Room), 0); //초기화된 맵정보 클라이언트에게 전송
										}
										if (SocketInfoArray[j].roomID == 0) {
											//printf("Send size : %d\n", g_TurtleMap.size);
											retval = send(SocketInfoArray[j].sock, (char*)&room, sizeof(room), 0); //초기화된 맵정보 클라이언트에게 전송
										}

									}

								}

								ptr->remainbytes -= SIZEOF_TB_RoomSetting;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_RoomSetting, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_TEAMSETTING:
							if (ptr->remainbytes >= SIZEOF_TB_TeamSetting) {
								TB_TeamSetting* tempt = reinterpret_cast<TB_TeamSetting*>(c_buf);
								BYTE temproomid = tempt->roomid;
								BYTE temppos = tempt->pos_in_room;
								BYTE tempteam = tempt->team;
								room[temproomid - 1].team_inroom[temppos] = tempteam;
								for (int j = 0; j < g_TotalSockets; ++j) {
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("Send size : %d\n", g_TurtleMap.size);
											retval = send(SocketInfoArray[j].sock, (char*)&room[temproomid - 1], sizeof(TB_Room), 0); //초기화된 맵정보 클라이언트에게 전송
										}


									}

								}
								ptr->remainbytes -= SIZEOF_TB_TeamSetting;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_TeamSetting, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_ITEM_GET:
							if (ptr->remainbytes >= SIZEOF_TB_ItemGet) {
								TB_ItemGet* tempitem = reinterpret_cast<TB_ItemGet*>(c_buf);
								BYTE temproomid = tempitem->room_id;
								BYTE tempid = tempitem->ingame_id;
								
								BYTE tempi = tempitem->item_type;
								printf("%d의 item type 획득\n", tempi);
								int tempx = tempitem->posx;
								int tempz = tempitem->posz;
								bool tempbool = g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] != MAP_NOTHING;
								printf("bool 초기화\n");
								if (tempbool) {
									g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] = MAP_NOTHING;
									TB_GetItem tempIRE = { SIZEOF_TB_GetItem,CASE_ITEM_GET,tempid,tempi };
									TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
									//retval = send(ptr->sock, (char*)&tempIRE, sizeof(TB_GetItem), 0);
									printf("임시 구조체 생성\n");
									for (int j = 0; j < g_TotalSockets; ++j) {
										if (SocketInfoArray[j].m_connected) {

											if (SocketInfoArray[j].roomID == temproomid) {
												retval = send(SocketInfoArray[j].sock, (char*)&tempIRE, sizeof(TB_GetItem), 0);
												printf("연결된 친구들 검색 - 아이템 정보 올림\n");
												retval = send(SocketInfoArray[j].sock, (char*)&tMap, sizeof(TB_MapSetRE), 0);
												printf("연결된 친구들 검색 - 맵 정보 올림\n");
											}



										}

									}
								}

								ptr->remainbytes -= SIZEOF_TB_ItemGet;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_ItemGet, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_THROWBOMB:
							if (ptr->remainbytes >= SIZEOF_TB_ThrowBomb) {

								TB_ThrowBomb* tempt = reinterpret_cast<TB_ThrowBomb*>(c_buf);
								BYTE temproomid = tempt->roomid;
								BYTE tempid = tempt->ingame_id;
								int tempx = tempt->posx;
								int tempz = tempt->posz;
								BYTE tempdirect = tempt->direction;
								//printf("던진다 폭탄!!! x:%d , z:%d \n",tempx,tempz);
								
								if (bomb_Map[temproomid-1].size() > 0) {
									auto bomb_b = bomb_Map[temproomid - 1].find(make_pair(tempx, tempz));
									if (bomb_b != bomb_Map[temproomid - 1].end()) {
										TB_ThrowBombRE tempThrow = { SIZEOF_TB_ThrowBombRE,CASE_THROWBOMB,tempdirect,tempid,tempx,tempz };
										Throw_Calculate_Map(tempx, tempz, temproomid, &tempThrow, tempdirect);
										g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] = MAP_NOTHING;
										Bomb_TB tempBomb = Bomb_TB(tempThrow.posx_re, tempThrow.posz_re, temproomid, bomb_b->second.firepower, tempid);
										tempBomb.time = bomb_b->second.time;
										tempBomb.ResetExplodeTime();
										tempBomb.is_throw = true;
										TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
										bomb_Map[temproomid - 1].insert(pair<pair<int, int>, Bomb_TB>(make_pair(tempThrow.posx_re, tempThrow.posz_re), tempBomb));
										bomb_Map[temproomid - 1].erase(bomb_b);
										
										for (int j = 0; j < g_TotalSockets; ++j)
										{
											if (SocketInfoArray[j].m_connected) {
												if (SocketInfoArray[j].roomID == temproomid) {
													printf("보냈다 패킷!!!\n");

													retval = send(SocketInfoArray[j].sock, (char*)&tMap, sizeof(TB_MapSetRE), 0);
													retval = send(SocketInfoArray[j].sock, (char*)&ingame_Char_Info[temproomid - 1][tempid], sizeof(TB_CharPos), 0);
													retval = send(SocketInfoArray[j].sock, (char*)&tempThrow, sizeof(TB_ThrowBombRE), 0);
												}
											}
										}
										//bomb_b->second.xz
									}
									//bomb_b->second.xz 
								}
								



								ptr->remainbytes -= SIZEOF_TB_ThrowBomb;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_ThrowBomb, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_THROWCOMPLETE:
							if (ptr->remainbytes >= SIZEOF_TB_ThrowComplete) {
								TB_ThrowComplete* tempt = reinterpret_cast<TB_ThrowComplete*>(c_buf);
								BYTE temproomid = tempt->roomid;
								int tempx = tempt->posx;
								int tempz = tempt->posz;
								g_TurtleMap_room[temproomid - 1].mapInfo[tempx][tempz] = MAP_BOMB;
								TB_BombSetRE tB = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,MAP_BOMB,tempx,tempz };
								bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].is_throw = false;
								bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].ResetTime();
								fireMap[temproomid - 1][tempz][tempx] = bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].firepower;
								
								for (int j = 0; j < g_TotalSockets; ++j)
								{
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {


											retval = send(SocketInfoArray[j].sock, (char*)&tB, sizeof(TB_BombSetRE), 0);

										}
									}
								}

								ptr->remainbytes -= SIZEOF_TB_ThrowComplete;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_ThrowComplete, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						
						case CASE_BOXPUSH:
							if (ptr->remainbytes >= SIZEOF_TB_BoxPush) {
								TB_BoxPush* tB = reinterpret_cast<TB_BoxPush*>(c_buf);

								BYTE tempdirc = tB->direction;
								BYTE temproomid = tB->roomid;
								BYTE tempid = tB->ingame_id;
								int tempx = tB->posx;
								int tempz = tB->posz;


								TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
								TB_BoxPushRE tBox = { SIZEOF_TB_BoxPushRE, CASE_BOXPUSH,0,tempid };
								BoxPush_Calculate_Map(tempx, tempz, temproomid, &tBox, tempdirc, &tMap);
								printf("box받았다!!!\n");

								for (int j = 0; j < g_TotalSockets; ++j)
								{


									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("보냈다 패킷!!!\n");
											if (tBox.push == 1)
												retval = send(SocketInfoArray[j].sock, (char*)&tMap, sizeof(TB_MapSetRE), 0);
											retval = send(SocketInfoArray[j].sock, (char*)&ingame_Char_Info[temproomid - 1][tempid], sizeof(TB_CharPos), 0);
											retval = send(SocketInfoArray[j].sock, (char*)&tBox, sizeof(TB_BoxPushRE), 0);
										}
									}
								}
								ptr->remainbytes -= SIZEOF_TB_BoxPush;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_BoxPush, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_BOXPUSHCOMPLETE:
							if (ptr->remainbytes >= SIZEOF_TB_BoxPushComplete) {
								TB_BoxPushComplete* tB = reinterpret_cast<TB_BoxPushComplete*>(c_buf);
								printf("boxcom받았다!!!\n");
								BYTE temproomid = tB->roomid;
								int tempx = tB->posx;
								int tempz = tB->posz;
								g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] = MAP_BOX;
								TB_MapSetRE tBd = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_BOX,tempx,tempz };
								for (int j = 0; j < g_TotalSockets; ++j)
								{
									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {


											retval = send(SocketInfoArray[j].sock, (char*)&tBd, sizeof(TB_MapSetRE), 0);

										}
									}
								}

								ptr->remainbytes -= SIZEOF_TB_BoxPushComplete;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_BoxPushComplete, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						case CASE_KICKBOMB:
							if (ptr->remainbytes >= SIZEOF_TB_BoxPush) {
								TB_KickBomb* tK = reinterpret_cast<TB_KickBomb*>(c_buf);
								BYTE tempdirc = tK->direction;
								BYTE temproomid = tK->roomid;
								BYTE tempid = tK->ingame_id;
								int tempx = tK->posx;
								int tempz = tK->posz;
								int ax=tempx;
								int az=tempz;
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
								printf("발로까는캐릭터위치 %d,%d!!!\n",tempx,tempz);
								if (bomb_Map[temproomid - 1].size() > 0) {
									auto bomb_b = bomb_Map[temproomid - 1].find(pair<int, int>(ax, az));
									if (bomb_b != bomb_Map[temproomid - 1].end()) {
										TB_KickBombRE tempKick = { SIZEOF_TB_ThrowBombRE,CASE_THROWBOMB,tempdirc,tempid,tempx,tempz };
										
										g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] = MAP_NOTHING;
										TB_MapSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_MAPSET,MAP_NOTHING,tempx,tempz };
										Kick_CalculateMap(tempx, tempz, temproomid, &tempKick, tempdirc, &tMap);
										Bomb_TB tempBomb = Bomb_TB(tempKick.posx_re, tempKick.posz_re, temproomid, bomb_b->second.firepower, tempid);
										tempBomb.time = bomb_b->second.time;
										tempBomb.ResetExplodeTime();
										tempBomb.is_kicked = true;
										
										bomb_Map[temproomid - 1].insert(pair<pair<int, int>, Bomb_TB>(make_pair(tempKick.posx_re, tempKick.posz_re), tempBomb));
										
										bomb_Map[temproomid - 1].erase(bomb_b);
										ingame_Char_Info[temproomid - 1][tempid].anistate = TURTLE_ANI_KICK;//throw ani
										for (int j = 0; j < g_TotalSockets; ++j)
										{
											if (SocketInfoArray[j].m_connected) {
												if (SocketInfoArray[j].roomID == temproomid) {
													printf("보냈다 발패킷!!!\n");
													if (tempKick.kick == 1) {
														printf("보냈다 참발패킷!!!\n");
														retval = send(SocketInfoArray[j].sock, (char*)&tMap, sizeof(TB_MapSetRE), 0);
													}
													retval = send(SocketInfoArray[j].sock, (char*)&ingame_Char_Info[temproomid - 1][tempid], sizeof(TB_CharPos), 0);
													retval = send(SocketInfoArray[j].sock, (char*)&tempKick, sizeof(TB_KickBombRE), 0);
												}
											}
										}
										//bomb_b->second.xz
									}
								}

								
								//bomb_Map[temproomid-1][pair<int,int>(ax,az)]
								
								
								
								ptr->remainbytes -= SIZEOF_TB_BoxPush;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_BoxPush, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							
							break;

						case CASE_KICKCOMPLETE:
							if (ptr->remainbytes >= SIZEOF_TB_ThrowComplete) {
								TB_KickComplete* tK = reinterpret_cast<TB_KickComplete*>(c_buf);
								
								BYTE temproomid = tK->roomid;
								
								int tempx = tK->posx;
								int tempz = tK->posz;
								TB_BombSetRE tMap = { SIZEOF_TB_MapSetRE,CASE_BOMBSET,MAP_BOMB,tempx,tempz };
								bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].is_kicked = false;
								bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].ResetTime();

								g_TurtleMap_room[temproomid - 1].mapInfo[tempz][tempx] = MAP_BOMB;
								fireMap[temproomid - 1][tempz][tempx] = bomb_Map[temproomid - 1][pair<int, int>(tempx, tempz)].firepower;
								for (int j = 0; j < g_TotalSockets; ++j)
								{


									if (SocketInfoArray[j].m_connected) {
										if (SocketInfoArray[j].roomID == temproomid) {
											//printf("보냈다 패킷!!!\n");
											
											retval = send(SocketInfoArray[j].sock, (char*)&tMap, sizeof(TB_MapSetRE), 0);
											
										}
									}
								}
								ptr->remainbytes -= SIZEOF_TB_ThrowComplete;
								memcpy(ptr->buf, c_buf + SIZEOF_TB_ThrowComplete, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							}
							break;
						default:
							printf("현재 버퍼 첫 바이트값 : %d\n", c_buf[0]);
							break;
						}
					}

					addrlen = sizeof(clientaddr);

					getpeername(ptr->sock, (SOCKADDR*)&clientaddr, &addrlen);

				}


			}
			//FD_CLOSE 이벤트 처리
			if (m_NetworkEvents.lNetworkEvents&FD_CLOSE) {
				if (m_NetworkEvents.iErrorCode[FD_CLOSE_BIT] != 0) {
					err_display(m_NetworkEvents.iErrorCode[FD_CLOSE_BIT]);
				}
				RemoveSocketInfo(i);
			}
		}

	}


	WSACleanup();
	return 0;

}



void err_display(char *msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s", msg, (char *)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

void err_quit(char* msg) {
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, (LPCWSTR)msg, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(1);
}
void err_display(int errcode) {
	LPVOID lpMsgBuf;
	FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL, WSAGetLastError(), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[오류]%s", (char*)lpMsgBuf);
	LocalFree(lpMsgBuf);
}
BOOL AddSOCKETInfo(SOCKET sock) {
	Socket_Info* ptr = &SocketInfoArray[g_TotalSockets];
	if (ptr == NULL) {
		printf("Not enough Memory!!!\n");
		return FALSE;
	}
	WSAEVENT hEvent = WSACreateEvent();
	if (hEvent == WSA_INVALID_EVENT) {
		err_display("WSACreateEvent()");
		return FALSE;
	}


	ZeroMemory(ptr->buf, sizeof(ptr->buf));
	//ZeroMemory(ptr->c_buf, sizeof(ptr->c_buf));
	ptr->id = g_total_member;
	ptr->m_getpacket = false;
	ptr->sock = sock;
	ptr->recvbytes = 0;
	ptr->remainbytes = 0;
	ptr->sendbytes = 0;

	if (g_TotalSockets == 0)
		ptr->m_connected = false;
	else
		ptr->m_connected = true;
	ptr->roomID = 0;
	ptr->bomb = 2;
	ptr->fire = 2;
	ptr->speed = 2;
	ptr->is_guardian = 0;
	ptr->is_ready = 0;

	EventArray[g_TotalSockets] = hEvent;
	++g_TotalSockets;
	printf("등록완료\n");
	return TRUE;
}
void RemoveSocketInfo(int nIndex) {
	
	Socket_Info* ptr = &SocketInfoArray[nIndex];
	if (ptr->roomID != 0) {
		room[ptr->roomID - 1].people_count--;
		room[ptr->roomID - 1].people_inroom[ptr->pos_inRoom - 1] = 0;
		room[ptr->roomID - 1].ready[ptr->pos_inRoom - 1] = 0;
		room[ptr->roomID - 1].team_inroom[ptr->pos_inRoom - 1] = 0;

		if (ptr->is_guardian == 1&&room[ptr->roomID - 1].people_count > 0) {
			
			for (int a = 0; a < 4; ++a) {
				if (room[ptr->roomID - 1].people_inroom[a] != 0 && room[ptr->roomID - 1].people_inroom[a] != ptr->id) {
					
					room[ptr->roomID - 1].guardian_pos = a + 1;
					for (int j = 0; j < g_TotalSockets; ++j)
					{
						if (SocketInfoArray[j].m_connected) {
							if (SocketInfoArray[j].roomID == ptr->roomID) {
								send(SocketInfoArray[j].sock, (char*)&room[ptr->roomID - 1], sizeof(TB_Room), 0);
							}
						}
					}
					break;
				}
			}
		
		}
		else if (room[ptr->roomID - 1].people_count > 0) {
			for (int j = 0; j < g_TotalSockets; ++j)
			{
				if (SocketInfoArray[j].m_connected) {
					if (SocketInfoArray[j].roomID == ptr->roomID) {
						send(SocketInfoArray[j].sock, (char*)&room[ptr->roomID - 1], sizeof(TB_Room), 0);
					}
				}
			}
			
		}
		if (room[ptr->roomID - 1].people_count <= 0) {
			room[ptr->roomID - 1].made = 0;
			room[ptr->roomID - 1].people_count = 0;
			room[ptr->roomID - 1].game_start = 0;
			room[ptr->roomID - 1].guardian_pos = 0;
			room[ptr->roomID - 1].map_thema = 0;
			room[ptr->roomID - 1].map_mode = 0;
			room[ptr->roomID - 1].people_max = 4;
			
			for (int i = 0; i < 4; ++i) {
				room[ptr->roomID - 1].team_inroom[i] = 0;
				room[ptr->roomID - 1].people_inroom[i] = 0;
				room[ptr->roomID - 1].ready[i] = 0;
			}
		}
	}
	ptr->roomID = 0;
	ptr->recvbytes = 0;
	ptr->remainbytes = 0;
	ptr->sendbytes = 0;
	ptr->pos_inRoom = 0;
	ptr->is_guardian = 0;
	ptr->m_connected = false;
	SOCKADDR_IN clientaddr;
	int addrlen = sizeof(clientaddr);
	getpeername(ptr->sock, (SOCKADDR*)&clientaddr, &addrlen);
	printf("TCP서버 클라이언트 종료:IP 주소=%s,포트번호 = %d\n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));
	closesocket(ptr->sock);

	WSACloseEvent(EventArray[nIndex]);
	if (nIndex != (g_TotalSockets - 1)) {
		SocketInfoArray[nIndex] = SocketInfoArray[g_TotalSockets - 1];
		EventArray[nIndex] = EventArray[g_TotalSockets - 1];
	}
	--g_TotalSockets;

}







void SetGameRoomInit(BYTE j) {
	ingame_Char_Info[j][0].ingame_id = 0;
	ingame_Char_Info[j][1].ingame_id = 1;
	ingame_Char_Info[j][2].ingame_id = 2;
	ingame_Char_Info[j][3].ingame_id = 3;
	//char_info[0].hp = 10.0f;


	ingame_Char_Info[j][0].posx = 0.0f;
	ingame_Char_Info[j][0].posz = 0.0f;
	ingame_Char_Info[j][0].is_alive = true;
	ingame_Char_Info[j][0].rotY = 0.0f;
	//char_info[1].hp = 10.0f;
	ingame_Char_Info[j][1].posx = 28.0f;
	ingame_Char_Info[j][1].posz = 0.0f;
	ingame_Char_Info[j][1].is_alive = true;
	ingame_Char_Info[j][1].rotY = 0.0f;
	//char_info[2].hp = 10.0f;
	ingame_Char_Info[j][2].posx = 0.0f;
	ingame_Char_Info[j][2].posz = 28.0f;
	ingame_Char_Info[j][2].is_alive = true;
	ingame_Char_Info[j][2].rotY = 180.0f;
	//char_info[3].hp = 10.0f;
	ingame_Char_Info[j][3].posx = 28.0f;
	ingame_Char_Info[j][3].posz = 28.0f;
	ingame_Char_Info[j][3].is_alive = true;
	ingame_Char_Info[j][3].rotY = 180.0f;
}



void CalculateMap_Simple(int x, int z, byte f, byte room_num) {
	bool l_UpBlock = false;
	bool l_DownBlock = false;
	bool l_LeftBlock = false;
	bool l_RightBlock = false;
	BYTE uf = f;
	BYTE df = f;
	BYTE lf = f;
	BYTE rf = f;
	
	
	
	
	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap_room[room_num - 1].mapInfo, sizeof(tempMap));
	tempMap[z][x] = MAP_NOTHING;
	for (byte b = 1; b <= f; ++b) {
		if (!l_DownBlock) {
			if (z - b < 0) {
				l_DownBlock = true;
				dfMap[room_num - 1][z - b][x] = b;
			}
			else {
				if (tempMap[z - b][x] == MAP_BOMB) {
					tempMap[z - b][x] = MAP_NOTHING;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
					l_DownBlock = true;
					dfMap[room_num - 1][z][x] = b-1;

				}
				else if (tempMap[z - b][x] == MAP_BOX) {
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
					dfMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z - b][x] == MAP_ITEM || tempMap[z - b][x] == MAP_ITEM_F || tempMap[z - b][x] == MAP_ITEM_S) {
					tempMap[z - b][x] = MAP_NOTHING;
				}
				else if (tempMap[z - b][x] == MAP_BUSH || tempMap[z - b][x] == MAP_FIREBUSH) {

				}
				else if (tempMap[z - b][x] == MAP_ROCK) {
					l_DownBlock = true;
					dfMap[room_num - 1][z][x] = b-1;
				}
			}
		}
		if (!l_UpBlock) {
			if (z + b > 14) {
				l_UpBlock = true;
				ufMap[room_num - 1][z][x] = b-1;
			}
			else {
				if (tempMap[z + b][x] == MAP_BOMB) {
					tempMap[z + b][x] = MAP_NOTHING;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x, z + b, fireMap[room_num - 1][z + b][x], room_num);
					l_UpBlock = true;
					ufMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z + b][x] == MAP_BOX) {
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
					ufMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z + b][x] == MAP_ITEM || tempMap[z + b][x] == MAP_ITEM_F || tempMap[z + b][x] == MAP_ITEM_S) {
					tempMap[z + b][x] = MAP_NOTHING;
				}
				else if (tempMap[z + b][x] == MAP_ROCK) {
					l_UpBlock = true;
					ufMap[room_num - 1][z][x] = b-1;
				}
			}
		}
		if (!l_LeftBlock) {
			if (x - b < 0) {
				l_LeftBlock = true;
				lfMap[room_num - 1][z][x] = b-1;
			}
			else {
				if (tempMap[z][x - b] == MAP_BOMB) {

					tempMap[z][x - b] = MAP_NOTHING;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x-b, z, fireMap[room_num - 1][z ][x-b], room_num);
					l_LeftBlock = true;
					lfMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z][x - b] == MAP_BOX) {
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
					lfMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z][x - b] == MAP_ITEM || tempMap[z][x - b] == MAP_ITEM_F || tempMap[z][x - b] == MAP_ITEM_S) {
					tempMap[z][x - b] = MAP_NOTHING;
				}
				else if (tempMap[z][x - b] == MAP_ROCK) {
					l_LeftBlock = true;
					lfMap[room_num - 1][z][x] = b-1;
				}
			}
		}
		if (!l_RightBlock) {
			if (x + b > 14) {
				l_RightBlock = true;
				rfMap[room_num - 1][z][x] = b-1;
			}
			else {
				if (tempMap[z][x + b] == MAP_BOMB) {
					tempMap[z][x + b] = MAP_NOTHING;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x + b, z, fireMap[room_num - 1][z][x + b], room_num);
					l_RightBlock = true;
					rfMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z][x + b] == MAP_BOX) {
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
					rfMap[room_num - 1][z][x] = b-1;
				}
				else if (tempMap[z][x + b] == MAP_ITEM || tempMap[z][x + b] == MAP_ITEM_F || tempMap[z][x + b] == MAP_ITEM_S) {
					tempMap[z][x + b] = MAP_NOTHING;
				}
				else if (tempMap[z][x + b] == MAP_ROCK) {
					l_RightBlock = true;
					rfMap[room_num - 1][z][x ] = b-1;
				}
			}
		}


	}

	g_TurtleMap_room[room_num - 1].type = CASE_MAP;
	BYTE gID = bomb_Map[room_num - 1][pair<int, int>(x, z)].game_id;
	TB_BombExplodeRE tempBomb = { SIZEOF_TB_BombExplodeRE,CASE_BOMB_EX,ufMap[room_num - 1][z][x],rfMap[room_num - 1][z][x],dfMap[room_num - 1][z][x],lfMap[room_num - 1][z][x],gID,x,z };
	explode_List.emplace_back(tempBomb);
	auto bomb =  bomb_Map[room_num - 1].find(pair<int, int>(x, z));
	bomb_Map[room_num - 1].erase(bomb);
	fireMap[room_num - 1][z][x] = 0;
	
	

	memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
}
void CalculateMap(int x, int z, byte f, byte room_num, TB_BombExplodeRE* temppacket) {
	bool l_UpBlock = false;
	bool l_DownBlock = false;
	bool l_LeftBlock = false;
	bool l_RightBlock = false;
	BYTE uf = f;
	BYTE df = f;
	BYTE lf = f;
	BYTE rf = f;

	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap_room[room_num - 1].mapInfo, sizeof(tempMap));
	tempMap[z][x] = MAP_NOTHING;
	for (byte b = 1; b <= f; ++b) {
		if (!l_DownBlock) {
			if (z - b < 0) {
				l_DownBlock = true;
				df = b-1;
			}
			else {
				if (tempMap[z - b][x] == MAP_BOMB) {
					tempMap[z - b][x] = MAP_NOTHING;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));

					
					CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
					
					l_DownBlock = true;
					df = b-1;
				}
				else if (tempMap[z - b][x] == MAP_BOX) {
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
					df = b-1;
				}
				else if (tempMap[z - b][x] == MAP_ITEM || tempMap[z - b][x] == MAP_ITEM_F || tempMap[z - b][x] == MAP_ITEM_S || tempMap[z - b][x] == MAP_KICKITEM || tempMap[z - b][x] == MAP_THROWITEM) {
					tempMap[z - b][x] = MAP_NOTHING;
				}
				else if (tempMap[z - b][x] == MAP_BUSH || tempMap[z - b][x] == MAP_FIREBUSH) {
					
				}
				else if (tempMap[z - b][x] == MAP_ROCK) {
					l_DownBlock = true;
					df = b-1;
				}
			}
		}
		if (!l_UpBlock) {
			if (z + b > 14) {
				l_UpBlock = true;
				uf = b-1;
			}
			else {
				if (tempMap[z + b][x] == MAP_BOMB) {
					tempMap[z + b][x] = MAP_NOTHING;
					
					l_UpBlock = true;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
					uf = b-1;
				}
				else if (tempMap[z + b][x] == MAP_BOX) {
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
					uf = b-1;
				}
				else if (tempMap[z + b][x] == MAP_ITEM || tempMap[z + b][x] == MAP_ITEM_F || tempMap[z + b][x] == MAP_ITEM_S || tempMap[z + b][x] == MAP_KICKITEM || tempMap[z + b][x] == MAP_THROWITEM) {
					tempMap[z + b][x] = MAP_NOTHING;
				}
				else if (tempMap[z + b][x] == MAP_ROCK) {
					l_UpBlock = true;
					uf = b-1;
				}
			}
		}
		if (!l_LeftBlock) {
			if (x - b < 0) {
				l_LeftBlock = true;
				lf = b-1;
			}
			else {
				if (tempMap[z][x - b] == MAP_BOMB) {
					tempMap[z][x - b] = MAP_NOTHING;
					l_LeftBlock = true;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
					lf = b-1;
				}
				else if (tempMap[z][x - b] == MAP_BOX) {
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
						tempMap[z][x-b] = MAP_KICKITEM;
					else if (temp_rand >= 12 && temp_rand <= 13)
						tempMap[z][x-b] = MAP_THROWITEM;
					l_LeftBlock = true;
					lf = b-1;
				}
				else if (tempMap[z][x - b] == MAP_ITEM  || tempMap[z][x - b] == MAP_KICKITEM || tempMap[z][x - b] == MAP_THROWITEM || tempMap[z][x - b] == MAP_ITEM_F || tempMap[z][x - b] == MAP_ITEM_S) {
					tempMap[z][x - b] = MAP_NOTHING;
				}
				else if (tempMap[z][x - b] == MAP_ROCK) {
					l_LeftBlock = true;
					lf = b-1;
				}
			}
		}
		if (!l_RightBlock) {
			if (x + b > 14) {
				l_RightBlock = true;
				rf = b-1;
			}
			else {
				if (tempMap[z][x + b] == MAP_BOMB) {
					tempMap[z][x + b] = MAP_NOTHING;
					l_RightBlock = true;
					memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
					CalculateMap_Simple(x, z - b, fireMap[room_num - 1][z - b][x], room_num);
					rf = b-1;
				}
				else if (tempMap[z][x + b] == MAP_BOX) {
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
						tempMap[z][x+b] = MAP_KICKITEM;
					else if (temp_rand >= 12 && temp_rand <= 13)
						tempMap[z][x+b] = MAP_THROWITEM;
					l_RightBlock = true;
					rf = b-1;
				}
				else if (tempMap[z][x + b] == MAP_ITEM || tempMap[z][x + b] == MAP_KICKITEM || tempMap[z][x + b] == MAP_THROWITEM || tempMap[z][x + b] == MAP_ITEM_F || tempMap[z][x + b] == MAP_ITEM_S) {
					tempMap[z][x + b] = MAP_NOTHING;
				}
				else if (tempMap[z][x + b] == MAP_ROCK) {
					l_RightBlock = true;
					rf = b-1;
				}
			}
		}


	}
	g_TurtleMap_room[room_num - 1].type = CASE_MAP;

	fireMap[room_num - 1][z][x] = 0;
	temppacket->size = SIZEOF_TB_BombExplodeRE;
	temppacket->type = CASE_BOMB_EX;
	temppacket->upfire = uf;
	temppacket->downfire = df;
	temppacket->rightfire = rf;
	temppacket->leftfire = lf;
	temppacket->posx = x;
	temppacket->posz = z;


	memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));

}

void Kick_CalculateMap(int x, int z, BYTE room_num, TB_KickBombRE* temppacket, BYTE direction, TB_MapSetRE* tempp) {
	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap_room[room_num - 1].mapInfo, sizeof(tempMap));
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
					if (tempMap[z][x + 2 + i] == MAP_ROCK || tempMap[z][x + 2 + i] == MAP_BOMB || tempMap[z][x + 2 + i] == MAP_BOX || x+2+i>14) {
						temppacket->kick = 1;
						tempx = x + 1 + i;
						break;
					}
				}
			}
		}
		break;
	case 2:
		if (tempx < 0 )
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
		else if (tempMap[z+1][x] == MAP_NOTHING || tempMap[z+1][x] == MAP_ITEM || tempMap[z+1][x] == MAP_ITEM_F || tempMap[z+1][x] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempz = z + 1;
		}
		else if (tempMap[z+1][x] == MAP_BOMB) {
			startz = z + 1;
			if (z + 2 > 14) {
				temppacket->kick = 0;
				tempz= 14;
			}
			else if (tempMap[z+2][x] == MAP_NOTHING || tempMap[z+2][x] == MAP_ITEM || tempMap[z+2][x] == MAP_ITEM_F || tempMap[z+2][x] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z + 2 + i][x] == MAP_ROCK || tempMap[z+2+i][x] == MAP_BOMB || tempMap[z+2+i][x] == MAP_BOX || z+ 2 + i>14) {
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
		else if (tempMap[z-1][x] == MAP_NOTHING || tempMap[z-1][x] == MAP_ITEM || tempMap[z-1][x] == MAP_ITEM_F || tempMap[z-1][x] == MAP_ITEM_S) {
			temppacket->kick = 0;
			tempz = z - 1;
		}
		else if (tempMap[z-1][x] == MAP_BOMB) {
			startz = z - 1;
			if (x - 2 <0) {
				temppacket->kick = 0;
				tempz = 0;
			}
			else if (tempMap[z-2][x] == MAP_NOTHING || tempMap[z-2][x] == MAP_ITEM || tempMap[z-2][x] == MAP_ITEM_F || tempMap[z-2][x] == MAP_ITEM_S) {
				for (int i = 1; i < 14; ++i) {
					if (tempMap[z - 2 - i][x] == MAP_ROCK || tempMap[z-2-i][x] == MAP_BOMB || tempMap[z-2-i][x] == MAP_BOX || z - 2 - i<0) {
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

	memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));
}

void Throw_Calculate_Map(int x, int z, BYTE room_num, TB_ThrowBombRE* temppacket, BYTE direction) {
	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap_room[room_num - 1].mapInfo, sizeof(tempMap));
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

	memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));

}


void BoxPush_Calculate_Map(int x, int z, BYTE room_num, TB_BoxPushRE* temppacket, BYTE direction, TB_MapSetRE* tempp) {
	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap_room[room_num - 1].mapInfo, sizeof(tempMap));
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

	memcpy(g_TurtleMap_room[room_num - 1].mapInfo, tempMap, sizeof(tempMap));

}
void SetMapToValue(int maptype,int mapnum) {
	if (maptype == 0 || maptype == 2) {
		ifstream in("Map1-1.csv");


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
		ifstream in("Map2-1.csv");


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
void SetMap(BYTE maptype, BYTE mapnum,BYTE room_num) {
	
	
	memcpy(&g_TurtleMap_room[room_num - 1].mapInfo, &g_TB_Map[mapnum][maptype], sizeof(g_TB_Map[mapnum][maptype]));
	
			

}

void ReGame(BYTE roomnum) {
	for(int i=0;i<4;++i)
		room[roomnum - 1].ready[i] = 0;
	ingame_Char_Info[roomnum - 1][0].posx = 0.0f;
	ingame_Char_Info[roomnum - 1][0].posz = 0.0f;
	ingame_Char_Info[roomnum - 1][0].is_alive = true;
	ingame_Char_Info[roomnum - 1][0].rotY = 0.0f;
	//char_info[1].hp = 10.0f;
	ingame_Char_Info[roomnum - 1][1].posx = 28.0f;
	ingame_Char_Info[roomnum - 1][1].posz = 0.0f;
	ingame_Char_Info[roomnum - 1][1].is_alive = true;
	ingame_Char_Info[roomnum - 1][1].rotY = 0.0f;
	//char_info[2].hp = 10.0f;
	ingame_Char_Info[roomnum - 1][2].posx = 0.0f;
	ingame_Char_Info[roomnum - 1][2].posz = 28.0f;
	ingame_Char_Info[roomnum - 1][2].is_alive = true;
	ingame_Char_Info[roomnum - 1][2].rotY = 180.0f;
	//char_info[3].hp = 10.0f;
	ingame_Char_Info[roomnum - 1][3].posx = 28.0f;
	ingame_Char_Info[roomnum - 1][3].posz = 28.0f;
	ingame_Char_Info[roomnum - 1][3].is_alive = true;
	ingame_Char_Info[roomnum - 1][3].rotY = 180.0f;
	for (int i = 0; i < 4; ++i) {
		ingame_Char_Info[roomnum - 1][i].is_alive = true;
		ingame_Char_Info[roomnum - 1][i].can_kick = false;
		ingame_Char_Info[roomnum - 1][i].can_throw = false;
		ingame_Char_Info[roomnum - 1][i].ingame_id = i;
		ingame_Char_Info[roomnum - 1][i].anistate = 0;
		ingame_Char_Info[roomnum - 1][i].is_alive = 0;
		ingame_Char_Info[roomnum - 1][i].can_kick = 0;
		ingame_Char_Info[roomnum - 1][i].can_throw = 0;
		ingame_Char_Info[roomnum - 1][i].bomb = 2;
		ingame_Char_Info[roomnum - 1][i].fire = 2;


		
	}
}
void ArrayMap() {

	for (int j = 0; j < 20; ++j) {
		g_TurtleMap_room[j].size = SIZEOF_TB_MAP;
		g_TurtleMap_room[j].type = CASE_MAP;
		room[j].game_start = 0;
		room[j].size = SIZEOF_TB_Room;
		room[j].type = CASE_ROOM;
		room[j].made = 0;
		room[j].people_count = 0;
		room[j].people_max = 4;
		room[j].roomID = j + 1;
		room[j].roomstate = 0;
		room[j].guardian_pos = 0;
		room[j].map_mode = 0;
		room[j].map_thema = 0;
		for (int z = 0; z < 15; ++z) {
			for (int x = 0; x < 15; ++x) {
				fireMap[j][z][x] = 0;
				dfMap[j][z][x]=0;
				ufMap[j][z][x]=0;
				lfMap[j][z][x]=0;
			    rfMap[j][z][x]=0;
			}
		}
		for (int i = 0; i < 4; ++i)
		{
			room[j].ready[i] = 0;

			room[j].people_inroom[i] = 0;
			ingame_Char_Info[j][i].size = SIZEOF_TB_CharPos;
			ingame_Char_Info[j][i].type = CASE_POS;
			ingame_Char_Info[j][i].anistate = 0;
			ingame_Char_Info[j][i].is_alive = 0;
			ingame_Char_Info[j][i].can_kick = 0;
			ingame_Char_Info[j][i].can_throw = 0;
			ingame_Char_Info[j][i].bomb = 2;
			ingame_Char_Info[j][i].fire = 2;
			//ingame_Char_Info[j][i].speed = 2;
			

			//char_info[i].speed = 2;

		}

		ingame_Char_Info[j][0].ingame_id = 0;
		ingame_Char_Info[j][1].ingame_id = 1;
		ingame_Char_Info[j][2].ingame_id = 2;
		ingame_Char_Info[j][3].ingame_id = 3;
		//char_info[0].hp = 10.0f;


		ingame_Char_Info[j][0].posx = 0.0f;
		ingame_Char_Info[j][0].posz = 0.0f;
		ingame_Char_Info[j][0].is_alive = true;
		ingame_Char_Info[j][0].rotY = 0.0f;
		//char_info[1].hp = 10.0f;
		ingame_Char_Info[j][1].posx = 28.0f;
		ingame_Char_Info[j][1].posz = 0.0f;
		ingame_Char_Info[j][1].is_alive = true;
		ingame_Char_Info[j][1].rotY = 0.0f;
		//char_info[2].hp = 10.0f;
		ingame_Char_Info[j][2].posx = 0.0f;
		ingame_Char_Info[j][2].posz = 28.0f;
		ingame_Char_Info[j][2].is_alive = true;
		ingame_Char_Info[j][2].rotY = 180.0f;
		//char_info[3].hp = 10.0f;
		ingame_Char_Info[j][3].posx = 28.0f;
		ingame_Char_Info[j][3].posz = 28.0f;
		ingame_Char_Info[j][3].is_alive = true;
		ingame_Char_Info[j][3].rotY = 180.0f;
	}
	/*
	room[0].made = 1;
	room[2].made = 1;
	room[10].made = 1;
	room[0].people_inroom[2] = 25;
	room[0].guardian_pos = 3;
	*/
	


	
	

}
CString GetIpAddress()
{
	WORD wVersionRequested;
	WSADATA wsaData;
	char name[255];
	PHOSTENT hostinfo;
	CString strIpAddress = _T("");
	wVersionRequested = MAKEWORD(2, 2);

	if (WSAStartup(wVersionRequested, &wsaData) == 0)
	{
		cout << "Get IP";
		if (gethostname(name, sizeof(name)) == 0)
		{
			cout << "Get IP2";
			if ((hostinfo = gethostbyname(name)) != NULL) {
				strIpAddress = inet_ntoa(*(struct in_addr *)*hostinfo->h_addr_list);
				cout << "Get IP3";
			}
		}
		WSACleanup();
	}
	return strIpAddress;
}



void Refresh_Map() {
	system("cls");


}
/*
string real_ip() {

	HINTERNET net = InternetOpenA("IP retriever",
		INTERNET_OPEN_TYPE_PRECONFIG,
		NULL,
		NULL,
		0);

	HINTERNET conn = InternetOpenUrlA(net,
		"http://myexternalip.com/raw",
		NULL,
		0,
		INTERNET_FLAG_RELOAD,
		0);

	char buffer[4096];
	DWORD read;

	InternetReadFile(conn, buffer, sizeof(buffer) / sizeof(buffer[0]), &read);
	InternetCloseHandle(net);

	return std::string(buffer, read);
}*/