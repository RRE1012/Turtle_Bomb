#include "stdafx.h"


int g_TotalSockets = 0;

DWORD io_size, key;

Socket_Info SocketInfoArray[WSA_MAXIMUM_WAIT_EVENTS];//유저들을 담을 소켓정보 구조체의 배열

WSAEVENT EventArray[WSA_MAXIMUM_WAIT_EVENTS];//WSAEVENT의 배열

BYTE MapInfo[15][15];//맵 정보 

TurtleBomb_Map g_TurtleMap; //맵 정보2

TurtleBomb_Pos char_info[4]; //4명의 캐릭터정보를 담아둘 캐릭터 정보 ->방 정보가 추가될 경우 2차원배열로 활용할 예정

void Refresh_Map(); //맵을 서버에서 갱신하기 위해 만든 함수 -> 계산도 추가할 예정

BYTE g_total_member = 0;//현재 접속자의 수

list<TurtleBomb_Bomb> bomb_list;//폭탄 터지는 시간을 계산하기 위해 만든 리스트 - 현재까지는 앞에서만 터트리지만, 좀 더 개선이 필요

void err_quit(char* msg); //에러 종료 및 출력 함수

void err_display(char* msg); //에러 출력 함수

BOOL AddSOCKETInfo(SOCKET sock); //접속자 소켓 정보 입력 함수

void RemoveSocketInfo(int nIndex);//접속자 중 종료 유저 정보 삭제 함수

void err_display(int errcode);//에러코드에 따른 에러 출력 함수

DWORD g_prevTime2; //GetTickCount()를 활용한 시간을 체크할 때 사용할 함수

void ArrayMap(); //맵 초기화 및 정렬 함수

void CalculateMap(int,int,byte);

int main(int argc, char* argv[]) {

	printf("-------------------------------------------------------------------------\n\nServer Start\n\n-------------------------------------------------------------------------\n\n");
	
	ArrayMap(); // 맵 초기화

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
		i = WSAWaitForMultipleEvents(g_TotalSockets, EventArray, FALSE, 0, FALSE);
		
		//타임아웃에 걸릴 경우 - 폭탄 시간은 계속 체크하고 있어야 하므로, 여기서도 시간체크를 실행시킨다.
		if (i == WSA_WAIT_FAILED || i==258 ||i==WAIT_TIMEOUT) {
			if (bomb_list.size() > 0) {
				list<TurtleBomb_Bomb>::iterator bomb = bomb_list.begin();
				for (; bomb != bomb_list.end(); ++bomb) {
					bomb->settime = bomb->settime + ((float)elapsedTime / 1000);
					//cout << "Timeout "<<bomb->settime << "  ";
					if (bomb->settime >= 2.0f) {
						int tempx = bomb->posx;
						int tempz = bomb->posz;
						g_TurtleMap.mapInfo[bomb->posx][bomb->posz] = MAP_NOTHING;
						CalculateMap(bomb->posx, bomb->posz, bomb->firepower);
						
						TurtleBomb_Explode temp_bomb = { 11,3, bomb->firepower,tempx ,tempz };
						g_TurtleMap.size = 227;
						for (int j = 0; j < g_TotalSockets; ++j) {
							
							//printf("폭발할 폭탄 전송!!!\n");
							
							if (SocketInfoArray[j].m_connected) {
								printf("Send size : %d\n",temp_bomb.size);

								retval = send(SocketInfoArray[j].sock, (char*)&temp_bomb, sizeof(TurtleBomb_Explode), 0);
								printf("Retval size : %d\n", retval);
								printf("Send size : %d\n", g_TurtleMap.size);

								retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap, sizeof(TurtleBomb_Map), 0);
								printf("Retval size : %d\n", retval);
							}

						}
						
						
						bomb_list.pop_front();
						//Refresh_Map();
						if (bomb_list.size() <= 0) {
							break;
						}
						break;
						
					}
				}
			}
			continue;
		}
		else {
			if (bomb_list.size() > 0) {
				list<TurtleBomb_Bomb>::iterator bomb = bomb_list.begin();
				for (; bomb != bomb_list.end(); ++bomb)
				{
					bomb->settime = bomb->settime + ((float)elapsedTime / 1000);
					//cout << "EventGet " << bomb->settime << "  ";
					if (bomb->settime >= 2.0f)
					{
						g_TurtleMap.mapInfo[bomb->posx][bomb->posz] = MAP_NOTHING;
						CalculateMap(bomb->posx, bomb->posz, bomb->firepower);
						int tempx = bomb->posx;
						int tempz = bomb->posz;
						TurtleBomb_Explode temp_bomb = { 11,3, bomb->firepower,tempx ,tempz };
						g_TurtleMap.size = 227;
						for (int j = 0; j < g_TotalSockets; ++j)
						{

							printf("폭발할 폭탄 전송!!!\n");
							if (SocketInfoArray[j].m_connected) {
								printf("Send size : %d\n", temp_bomb.size);
								retval = send(SocketInfoArray[j].sock, (char*)&temp_bomb, sizeof(TurtleBomb_Explode), 0);
								printf("Retval size : %d\n", retval);
								printf("Send size : %d\n", g_TurtleMap.size);

								retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap, sizeof(TurtleBomb_Map), 0);
								printf("Retval size : %d\n", retval);
							}
						}


						bomb_list.pop_front();
						//Refresh_Map();
						if (bomb_list.size() <= 0)
						{
							break;
						}
						break;
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
				
				TurtleBomb_ID tempid = {3,5,g_total_member};
				retval = send(client_sock, (char*)&tempid, sizeof(TurtleBomb_ID), 0); //ID 전송.
				printf("전송-%d번째 -ID : %d\n", i, g_total_member);

				g_total_member++; // 현재 접속자 수 체크
				
				retval = send(client_sock, (char*)&g_TurtleMap, sizeof(TurtleBomb_Map), 0); //초기화된 맵정보 클라이언트에게 전송
				printf("맵정보 전송 :%d바이트\n", retval);
				
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
						printf("%d바이트 수신 !!\n", retval);
						//c_buf[retval] = '\0';
						//ptr->buf[retval + ptr->remainbytes] = '\0';
						//ptr->recvbytes = ptr->recvbytes+retval;
						ptr->remainbytes = ptr->remainbytes + retval;
						//c_buf[ptr->remainbytes] = '\0';
					}




					if (ptr->remainbytes >= 11) 
					{
						switch (c_buf[1]) {
						case CASE_POS: //CharPos
							if (ptr->remainbytes >= 17) {
								TurtleBomb_Pos* pos = reinterpret_cast<TurtleBomb_Pos*>(c_buf);
								//필수 -
								BYTE tempid = pos->id;
								char_info[tempid].anistate = pos->anistate;
								char_info[tempid].is_alive = pos->is_alive;
								char_info[tempid].posx = pos->posx;
								char_info[tempid].rotY = pos->rotY;
								char_info[tempid].posz = pos->posz;
								//printf("1p포지션값  :x :%f, z:%f , roty:%f \n", char_info[0].posx, char_info[0].posz, char_info[0].rotY);
								//printf("2p포지션값  :x :%f, z:%f , roty:%f \n", char_info[1].posx, char_info[1].posz, char_info[1].rotY);
								//printf("3p포지션값  :x :%f, z:%f , roty:%f \n", char_info[2].posx, char_info[2].posz, char_info[2].rotY);
								//printf("4p포지션값  :x :%f, z:%f , roty:%f \n", char_info[3].posx, char_info[3].posz, char_info[3].rotY);
								ptr->remainbytes -= 17;

								memcpy(c_buf, ptr->buf + 17, ptr->remainbytes);
								memcpy(ptr->buf, c_buf, ptr->remainbytes);

								for (int j = 0; j < g_TotalSockets; ++j) {

									if (SocketInfoArray[j].m_connected) {
										printf("Send size : %d\n", char_info[tempid].size);
										char_info[tempid].size = 17;
										char_info[tempid].type = 1;
										retval = send(SocketInfoArray[j].sock, (char*)&char_info[tempid], sizeof(TurtleBomb_Pos), 0);
										printf("Retval size : %d\n", retval);
									}
								}
								

								break;
							}
							break;
						case CASE_BOMB:
							if (ptr->remainbytes >= 11) {
								TurtleBomb_Explode* b_pos = reinterpret_cast<TurtleBomb_Explode*>(c_buf);
								int tempx = b_pos->posx;
								int tempz = b_pos->posz;
								g_TurtleMap.mapInfo[tempz][tempx] = MAP_BOMB;
								//printf("폭탄포지션값  :x :%d, z:%d ,  \n", b_pos->posx, b_pos->posz);
								
								BYTE tempfire = b_pos->firepower;
								
								TurtleBomb_Bomb tempbomb = { sizeof(TurtleBomb_Bomb),CASE_BOMB,0,tempfire,tempx,tempz,0.0f };
								bomb_list.emplace_back(tempbomb);
								
								ptr->remainbytes -= 11;
								memcpy(ptr->buf, c_buf +11, ptr->remainbytes);
								memset(c_buf, 0, sizeof(c_buf));
 								memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
								//Refresh_Map();
								
								
								for (int j = 0; j < g_TotalSockets; ++j) {
									//폭탄을 받았으므로 갱신된 맵정보를 접속해있는 유저에게 전송
									if (SocketInfoArray[j].m_connected) {
										printf("Send size : %d\n", g_TurtleMap.size);

										retval = send(SocketInfoArray[j].sock, (char*)&g_TurtleMap, sizeof(TurtleBomb_Map), 0);
										printf("Retval size : %d\n", retval);
										//printf("Bomb가 추가된 맵정보값 전송!\n");
									}
								}
								break;
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
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, msg, MB_ICONERROR);
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
	if(g_TotalSockets==0)
		ptr->m_connected = false;
	else
		ptr->m_connected = true;

	EventArray[g_TotalSockets] = hEvent;
	++g_TotalSockets;
	printf("등록완료\n");
	return TRUE;
}
void RemoveSocketInfo(int nIndex) {
	Socket_Info* ptr = &SocketInfoArray[nIndex];
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










void ArrayMap() {


	for (int i = 0; i < 4; ++i) 
	{
		char_info[i].size = 17;
		char_info[i].type = 1;
		char_info[i].anistate = 0;
		char_info[i].is_alive = 0;

	}
	char_info[0].id = 0;
	char_info[1].id = 1;
	char_info[2].id = 2;
	char_info[3].id = 3;
	//char_info[0].hp = 10.0f;

	char_info[0].posx = 0.0f;
	char_info[0].posz = 0.0f;
	char_info[0].is_alive = true;
	char_info[0].rotY = 0.0f;
	//char_info[1].hp = 10.0f;
	char_info[1].posx = 28.0f;
	char_info[1].posz = 0.0f;
	char_info[1].is_alive = true;
	char_info[1].rotY = 0.0f;
	//char_info[2].hp = 10.0f;
	char_info[2].posx = 0.0f;
	char_info[2].posz = 28.0f;
	char_info[2].is_alive = true;
	char_info[2].rotY = 180.0f;
	//char_info[3].hp = 10.0f;
	char_info[3].posx = 28.0f;
	char_info[3].posz = 28.0f;
	char_info[3].is_alive = true;
	char_info[3].rotY = 180.0f;
	g_TurtleMap.size = 227;
	g_TurtleMap.type = 4;


	for (int x = 0; x < 15; ++x) {
		for (int y = 0; y < 15; ++y) {
			g_TurtleMap.mapInfo[x][y] = 2;
			MapInfo[x][y] = 2;

		}

	}

	for (int i = 2; i < 13; ++i) {
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[i][0] =MAP_BUSH;
			MapInfo[i][0] =MAP_BUSH;//'B';
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[i][0] = 2;
			MapInfo[i][0] = 2;//'N'
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[i][0] = 3;
			MapInfo[i][0] = 3;//'C'
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[i][0] = 4;
			MapInfo[i][0] = 4;//'R'
		}
		else
		{
			g_TurtleMap.mapInfo[i][0] = 5;
			MapInfo[i][0] = 5;//'I'
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[i][1] =MAP_BUSH;
			MapInfo[i][1] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[i][1] = 2;
			MapInfo[i][1] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[i][1] = 3;
			MapInfo[i][1] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[i][1] = 4;
			MapInfo[i][1] = 4;
		}
		else {
			g_TurtleMap.mapInfo[i][1] = 5;
			MapInfo[i][1] = 5;
		}
		
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[i][14] =MAP_BUSH;
			MapInfo[i][14] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[i][14] = 2;
			MapInfo[i][14] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[i][14] = 3;
			MapInfo[i][14] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[i][14] = 4;
			MapInfo[i][14] = 4;
		}
		else {
			g_TurtleMap.mapInfo[i][14] = 5;
			MapInfo[i][14] = 5;
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[i][13] =MAP_BUSH;
			MapInfo[i][13] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[i][13] = 2;
			MapInfo[i][13] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[i][13] = 3;
			MapInfo[i][13] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[i][13] = 4;
			MapInfo[i][13] = 4;
		}
		else {
			g_TurtleMap.mapInfo[i][13] = 5;
			MapInfo[i][13] = 5;
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[0][i] =MAP_BUSH;
			MapInfo[0][i] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[0][i] = 2;
			MapInfo[0][i] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[0][i] = 3;
			MapInfo[0][i] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[0][i] = 4;
			MapInfo[0][i] = 4;
		}
		else {
			g_TurtleMap.mapInfo[0][i] = 5;
			MapInfo[0][i] = 5;
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[1][i] =MAP_BUSH;
			MapInfo[1][i] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[1][i] = 2;
			MapInfo[1][i] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[1][i] = 3;
			MapInfo[1][i] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[1][i] = 4;
			MapInfo[1][i] = 4;
		}
		else {
			g_TurtleMap.mapInfo[1][i] = 5;
			MapInfo[1][i] = 5;
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[13][i] =MAP_BUSH;
			MapInfo[13][i] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[13][i] = 2;
			MapInfo[13][i] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[13][i] = 3;
			MapInfo[13][i] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[13][i] = 4;
			MapInfo[13][i] = 4;
		}
		else {
			g_TurtleMap.mapInfo[13][i] = 5;
			MapInfo[13][i] = 5;
		}
		if (rand() % 15 < 1) {
			g_TurtleMap.mapInfo[14][i] =MAP_BUSH;
			MapInfo[14][i] =MAP_BUSH;
		}
		else if (rand() % 15 < 5) {
			g_TurtleMap.mapInfo[14][i] = 2;
			MapInfo[14][i] = 2;
		}
		else if (rand() % 15 < 9) {
			g_TurtleMap.mapInfo[14][i] = 3;
			MapInfo[14][i] = 3;
		}
		else if (rand() % 15 < 13) {
			g_TurtleMap.mapInfo[14][i] = 4;
			MapInfo[14][i] = 4;
		}
		else {
			g_TurtleMap.mapInfo[14][i] = 5;
			MapInfo[14][i] = 5;
		}

		for (int j = 2; j < 13; ++j) {
			if (rand() % 15 < 1) {
				g_TurtleMap.mapInfo[i][j] =MAP_BUSH;
				MapInfo[i][j] =MAP_BUSH;
			}
			else if (rand() % 15 < 5) {
				g_TurtleMap.mapInfo[i][j] = 2;
				MapInfo[i][j] = 2;
			}
			else if (rand() % 15 < 9) {
				g_TurtleMap.mapInfo[i][j] = 3;
				MapInfo[i][j] = 3;
			}
			else if (rand() % 15 < 13) {
				g_TurtleMap.mapInfo[i][j] = 4;
				MapInfo[i][j] = 4;
			}

			else
			{
				g_TurtleMap.mapInfo[i][j] = 5;
				MapInfo[i][j] = 5;
			}
		}


	}

	for (int y = 0; y < 15; ++y) {
		for (int x = 0; x < 15; ++x) {
			if (g_TurtleMap.mapInfo[14 - y][x] == 1) {
				printf("●  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 2) {
				printf("N  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 3) {
				printf("C  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 4) {
				printf("R  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 5) {
				printf("I  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 6) {
				printf("B  ");
			}

		}
		printf("\n");
	}

}

void Refresh_Map() {
	system("cls");
	for (int y = 0; y < 15; ++y) {
		for (int x = 0; x < 15; ++x) {
			if (g_TurtleMap.mapInfo[14-y][x] == 1) {
				printf("●  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 2) {
				printf("N  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 3) {
				printf("C  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 4) {
				printf("R  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 5) {
				printf("I  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 6) {
				printf("B  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 7) {
				printf("F  ");
			}
			else if (g_TurtleMap.mapInfo[14 - y][x] == 8) {
				printf("S  ");
			}

		}
		printf("\n");
	}

}

void CalculateMap(int x, int z, byte f) {
	bool l_UpBlock = false;
	bool l_DownBlock = false;
	bool l_LeftBlock = false;
	bool l_RightBlock = false;

	BYTE tempMap[15][15];
	memcpy(tempMap, g_TurtleMap.mapInfo, sizeof(tempMap));
	tempMap[z][x] = MAP_NOTHING;
	for (byte b = 1; b <= f; ++b) {
		if (!l_DownBlock) {
			if (z - b < 0) {
				l_DownBlock = true;
			}
			else {
				if (tempMap[z - b][x] == MAP_BOMB ) {
					tempMap[z - b][x] = MAP_NOTHING;
					l_DownBlock = true;
				}
				else if (tempMap[z - b][x] == MAP_BOX) {
					int temp_rand = (rand() % 10);
					if(temp_rand<4)
						tempMap[z - b][x] = MAP_NOTHING;
					else if(temp_rand>=4&&temp_rand<=5)
						tempMap[z - b][x] = MAP_ITEM;
					else if (temp_rand >= 6 && temp_rand <= 7)
						tempMap[z - b][x] = MAP_ITEM_F;
					else if (temp_rand >= 8 && temp_rand <= 9)
						tempMap[z - b][x] = MAP_ITEM_S;

					l_DownBlock = true;
				}
				else if (tempMap[z - b][x] == MAP_ITEM || tempMap[z - b][x] == MAP_ITEM_F || tempMap[z - b][x] == MAP_ITEM_S || tempMap[z - b][x] == MAP_BUSH) {
					tempMap[z - b][x] = MAP_NOTHING;
				}
				else if (tempMap[z - b][x] == MAP_ROCK ) {
					l_DownBlock = true;
				}
			}
		}
		if (!l_UpBlock) {
			if (z + b > 14) {
				l_UpBlock = true;
			}
			else {
				if (tempMap[z + b][x] == MAP_BOMB ) {
					tempMap[z + b][x] = MAP_NOTHING;
					l_UpBlock = true;
				}
				else if (tempMap[z + b][x] == MAP_BOX) {
					int temp_rand = (rand() % 10);
					if (temp_rand<4)
						tempMap[z + b][x] = MAP_NOTHING;
					else if (temp_rand >= 4 && temp_rand <= 5)
						tempMap[z + b][x] = MAP_ITEM;
					else if (temp_rand >= 6 && temp_rand <= 7)
						tempMap[z + b][x] = MAP_ITEM_F;
					else if (temp_rand >= 8 && temp_rand <= 9)
						tempMap[z + b][x] = MAP_ITEM_S;

					l_UpBlock = true;
				}
				else if (tempMap[z + b][x] == MAP_ITEM || tempMap[z + b][x] == MAP_ITEM_F || tempMap[z + b][x] == MAP_ITEM_S || tempMap[z + b][x] == MAP_BUSH) {
					tempMap[z + b][x] = MAP_NOTHING;
				}
				else if (tempMap[z + b][x] == MAP_ROCK) {
					l_UpBlock = true;
				}
			}
		}
		if (!l_LeftBlock) {
			if (x - b < 0) {
				l_LeftBlock = true;
			}
			else {
				if (tempMap[z][x-b] == MAP_BOMB) {
					tempMap[z][x-b] = MAP_NOTHING;
					l_LeftBlock = true;
				}
				else if (tempMap[z][x - b] == MAP_BOX) {
					int temp_rand = (rand() % 10);
					if (temp_rand<4)
						tempMap[z][x - b] = MAP_NOTHING;
					else if (temp_rand >= 4 && temp_rand <= 5)
						tempMap[z][x - b] = MAP_ITEM;
					else if (temp_rand >= 6 && temp_rand <= 7)
						tempMap[z][x - b] = MAP_ITEM_F;
					else if (temp_rand >= 8 && temp_rand <= 9)
						tempMap[z][x - b] = MAP_ITEM_S;

					l_LeftBlock = true;
				}
				else if (tempMap[z][x-b] == MAP_ITEM || tempMap[z][x-b] == MAP_BUSH|| tempMap[z][x - b] == MAP_ITEM_F || tempMap[z][x - b] == MAP_ITEM_S) {
					tempMap[z][x-b] = MAP_NOTHING;
				}
				else if (tempMap[z][x-b] == MAP_ROCK) {
					l_LeftBlock = true;
				}
			}
		}
		if (!l_RightBlock) {
			if (x + b > 14) {
				l_RightBlock = true;
			}
			else {
				if (tempMap[z][x+b] == MAP_BOMB ) {
					tempMap[z][x+b] = MAP_NOTHING;
					l_RightBlock = true;
				}
				else if (tempMap[z][x + b] == MAP_BOX) {
					int temp_rand = (rand() % 10);
					if (temp_rand<4)
						tempMap[z][x + b] = MAP_NOTHING;
					else if (temp_rand >= 4 && temp_rand <= 5)
						tempMap[z][x + b] = MAP_ITEM;
					else if (temp_rand >= 6 && temp_rand <= 7)
						tempMap[z][x + b] = MAP_ITEM_F;
					else if (temp_rand >= 8 && temp_rand <= 9)
						tempMap[z][x + b] = MAP_ITEM_S;

					l_RightBlock = true;
				}
				else if (tempMap[z][x + b] == MAP_ITEM || tempMap[z][x + b] == MAP_BUSH || tempMap[z][x + b] == MAP_ITEM_F || tempMap[z][x + b] == MAP_ITEM_S) {
					tempMap[z][x+b] = MAP_NOTHING;
				}
				else if (tempMap[z][x+b] == MAP_ROCK) {
					l_RightBlock = true;
				}
			}
		}
		

	}
	g_TurtleMap.type = 4;
	memcpy( g_TurtleMap.mapInfo, tempMap, sizeof(tempMap));

}