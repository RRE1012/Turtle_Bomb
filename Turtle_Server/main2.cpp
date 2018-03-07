#pragma once

#include "stdafx.h"
#include "TB_Server.h"

int g_TotalSockets = 0;

Socket_Info *SocketInfoArray[WSA_MAXIMUM_WAIT_EVENTS];
WSAEVENT EventArray[WSA_MAXIMUM_WAIT_EVENTS];
DWORD io_size, key;
list<Bomb_Pos> bomb_list;
int MapInfo[15][15];
unsigned char socket_buf[MAX_BUFF_SIZE+1];
//int MapInfo[225];
DWORD WINAPI Bomb_Count_Thread(LPVOID arg);
Packet_Char char_info[4]; //4명의 캐릭터정보를 담아두자
int bomb_count1;
int bomb_count2;
bool bomb_Map[15][15]; //폭탄 on/off
Bomb_Pos bombs[225];
int g_total_member = 0;
void err_quit(char* msg);
void err_display(char* msg);
BOOL AddSOCKETInfo(SOCKET sock);
void RemoveSocketInfo(int nIndex);
void err_display(int errcode);
DWORD g_prevTime2;
list<PosOfBOMB> g_bomb_explode;
int map = 3;
void ArrayMap();
CRITICAL_SECTION g_cs;
int main(int argc, char* argv[]) {
	
	printf("sizeof DWORD : %d\n", sizeof(BYTE));
	ArrayMap();
	HANDLE hThread;
	hThread = CreateThread(NULL, 0, Bomb_Count_Thread, NULL, 0, NULL);
	int retval;
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return 0;
	SOCKET listen_sock = socket(AF_INET,SOCK_STREAM,0);
	if (listen_sock == INVALID_SOCKET)
		err_quit("socket()");
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serveraddr.sin_port = htons(TB_SERVER_PORT);
	retval = ::bind(listen_sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr));
	if(retval==SOCKET_ERROR)
		err_quit("bind()");
	retval = listen(listen_sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
		err_quit("listen()");
	//소켓 정보 추가&WSAEventSelect
	AddSOCKETInfo(listen_sock);
	retval = WSAEventSelect(listen_sock, EventArray[g_TotalSockets - 1],FD_ACCEPT|FD_CLOSE);
	if (retval == SOCKET_ERROR)
		err_quit("WSAEventSelect()");

	WSANETWORKEVENTS m_NetworkEvents;
	SOCKET client_sock;
	SOCKADDR_IN clientaddr;
	int i, addrlen;
	
	InitializeCriticalSection(&g_cs);
	while (1) {
		//이벤트 객체 관찰하기
		i = WSAWaitForMultipleEvents(g_TotalSockets, EventArray, FALSE, WSA_INFINITE, FALSE);
		if (i == WSA_WAIT_FAILED)
			continue;
		i -= WSA_WAIT_EVENT_0;
		//구체적인 네트워크 이벤트 알아내기
		retval = WSAEnumNetworkEvents(SocketInfoArray[i]->sock, EventArray[i], &m_NetworkEvents);
		if (retval == SOCKET_ERROR)
			continue;
		//FD_ACCEPT 이벤트 처리
		if (m_NetworkEvents.lNetworkEvents&FD_ACCEPT) {
			if (m_NetworkEvents.iErrorCode[FD_ACCEPT_BIT] != 0) {
				err_display(m_NetworkEvents.iErrorCode[FD_ACCEPT_BIT]);
				continue;
			}
			addrlen = sizeof(clientaddr);
			client_sock = accept(SocketInfoArray[i]->sock, (SOCKADDR*)&clientaddr, &addrlen);
			if (client_sock == INVALID_SOCKET) {
				err_display("accept()");
				continue;
			}
			printf("[TCP 서버] 클라이언트 접속 : IP 주소 =%s, 포트번호=%d\n",inet_ntoa(clientaddr.sin_addr),ntohs(clientaddr.sin_port));
			retval = send(client_sock, (char*)&g_total_member, sizeof(int), 0);
			printf("전송-%d번째 -ID : %d\n", i,g_total_member);

			g_total_member++;
			//retval = send(client_sock, (char*)&map, sizeof(int), 0);
			retval = send(client_sock, (char*)&MapInfo, sizeof(MapInfo), 0);
			printf("맵정보 전송 :%d바이트\n", retval);
			

			if (g_TotalSockets >= WSA_MAXIMUM_WAIT_EVENTS) {
				printf("[오류] 더 이상 접속을 받아들일 수 없습니다!!!!!\n");
				closesocket(client_sock);
				continue;
			}
			
			if (retval == SOCKET_ERROR) {
				if (WSAGetLastError() != WSAEWOULDBLOCK) {
					err_display("send()");
					RemoveSocketInfo(i);
				}
				continue;
			}
			//
			//소켓 정보 추가
			AddSOCKETInfo(client_sock);
			
			retval = WSAEventSelect(client_sock, EventArray[g_TotalSockets - 1], FD_READ | FD_WRITE | FD_CLOSE);
			if (retval == SOCKET_ERROR)
				err_quit("WSAEventSelect()-client");

		}
		if (m_NetworkEvents.lNetworkEvents&FD_READ || m_NetworkEvents.lNetworkEvents&FD_WRITE) {
			if (m_NetworkEvents.lNetworkEvents&FD_READ &&m_NetworkEvents.iErrorCode[FD_READ_BIT] != 0) {
				err_display(m_NetworkEvents.iErrorCode[FD_READ_BIT]);
				continue;
			}
			if (m_NetworkEvents.lNetworkEvents&FD_WRITE &&m_NetworkEvents.iErrorCode[FD_WRITE_BIT] != 0) {
				err_display(m_NetworkEvents.iErrorCode[FD_WRITE_BIT]);
				continue;
			}
			
			Socket_Info* ptr = SocketInfoArray[i];
			float m_turtle1_posx;
			float m_turtle1_posz;
			float m_turtle1_roty;
			int m_temp_id=0;
			Bomb_Pos m_tempbombinfo;
			if (ptr->recvbytes == 0) {
				//데이터 받기
				retval = recv(ptr->sock, (char*)&ptr->buf, sizeof(ptr->buf), 0);
				char* c_buf = ptr->buf;
				
				if (retval == SOCKET_ERROR) {
					printf("수신 오류 !!\n");
					continue;
				}
				else {
					memcpy(c_buf + ptr->remainbytes, ptr->buf, retval);
					printf("%d바이트 수신 !!\n", retval);
				}
				c_buf[retval] = '\0';
				//받은 데이터 출력
				//for(int b=0;b<MAX_BUFF_SIZE+1;++b)
				//	printf("%d  ", ptr->buf[b]);
				/*
				switch (ptr->type) {
				case 1:
					recv(ptr->sock, (char*)&m_temp_id, sizeof(int), 0);
					recv(ptr->sock, (char*)&m_turtle1_posx, sizeof(float), 0);

					recv(ptr->sock, (char*)&m_turtle1_posz, sizeof(float), 0);
					recv(ptr->sock, (char*)&m_turtle1_roty, sizeof(float), 0);
					
					char_info[m_temp_id].x = m_turtle1_posx;
					char_info[m_temp_id].z = m_turtle1_posz;
					char_info[m_temp_id].rotY = m_turtle1_roty;
					
					
					printf("캐릭터 정보를 받았다!\n");
					break;
				case 2:
					int x;
					int y;
					printf("what?\n");
					recv(ptr->sock, (char*)&x, sizeof(int), 0);
					recv(ptr->sock, (char*)&y, sizeof(int), 0);
					//bomb_Map[x][y] = true;
					MapInfo[x][y] = 1;
					m_tempbombinfo = { x, y, true, 0.0f };
					bomb_list.emplace_back(m_tempbombinfo);
					printf("폭탄 정보를 받았다!\n");
					break;
				case 3:
					int x2;
					int y2;
					recv(ptr->sock, (char*)&x2, sizeof(int), 0);
					recv(ptr->sock, (char*)&y2, sizeof(int), 0);
					bomb_Map[x2][y2] = false;

					break;
				}
				


				if (retval == SOCKET_ERROR) {
					if (WSAGetLastError() != WSAEWOULDBLOCK) {
						err_display("recv()");
						RemoveSocketInfo(i);
					}
					continue;
				}
				*/
				//ptr->recvbytes = ptr->recvbytes+retval;
				ptr->remainbytes = ptr->remainbytes + retval;
				
				ptr->buf[retval] = '\0';
				c_buf[ptr->remainbytes] = '\0';
				if (ptr->remainbytes >= 9) {
					switch (c_buf[0]) {
					case CASE_POS: //CharPos
						if (ptr->remainbytes >= 18) {
							Pos* pos = reinterpret_cast<Pos*>(c_buf);
							char_info[pos->id].x = pos->posx;
							char_info[pos->id].rotY = pos->roty;
							char_info[pos->id].z = pos->posz;
							printf("포지션값 받음! :x :%f, z:%f , roty:%f \n", pos->posx, pos->posz, char_info[pos->id].rotY);
							ptr->remainbytes -= 18;
							
							memcpy(ptr->buf, c_buf + 18, ptr->remainbytes);
							memset(c_buf, 0, sizeof(c_buf));
							memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							ptr->m_getpacket = true;
							
							break;
						}
						break;
					case CASE_BOMB:
						if (ptr->remainbytes >= 9) {
							PosOfBOMB* b_pos = reinterpret_cast<PosOfBOMB*>(c_buf);
							MapInfo[b_pos->x][b_pos->y] = MAP_BOMB;
							printf("Bomb값 받음!\n");
							ptr->remainbytes -= 9;
							memcpy(ptr->buf, c_buf + 9, ptr->remainbytes);
							memset(c_buf, 0, sizeof(c_buf));
							memcpy(c_buf, ptr->buf, sizeof(ptr->buf));
							ptr->m_getpacket = true;
							break;
						}
						break;


					default:
						break;
					}
				}

				addrlen = sizeof(clientaddr);
				
				getpeername(ptr->sock, (SOCKADDR*)&clientaddr, &addrlen);
			//	printf("[TCP/%s:%d] TurtlePosx,z : %f,%f, \n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port), m_turtle1_posx, m_turtle1_posz);
				
			}
			if (ptr->m_getpacket) {
				//데이터 보내기
				int temptype = 1;
				
				retval = send(ptr->sock, (char*)&temptype, sizeof(int), 0);
				//for (int player = 0; player < g_total_member; ++player) {
					for (int j = 0; j < 4; ++j) {
						retval = send(ptr->sock, (char*)&char_info[j], sizeof(Packet_Char), 0);
						printf("%d바이트 보냈다!!!\n", retval);
					}
				//}
				printf("보냈다!!!\n");
				int temptype2 = 2;
				retval = send(ptr->sock, (char*)&temptype2, sizeof(int), 0);
				retval = send(ptr->sock, (char*)&MapInfo, sizeof(MapInfo), 0);
				printf("%d바이트 맵정보를 보냈다!!!\n", retval);
				if (retval == SOCKET_ERROR) {
					if (WSAGetLastError() != WSAEWOULDBLOCK) {
						err_display("send()");
						RemoveSocketInfo(i);
					}
					continue;
				}
				
				ptr->m_getpacket = false;
			
			}
			//제거할 폭탄이 있다면
			if(g_bomb_explode.size()>0){
				int m_temptype = 3;

				list<PosOfBOMB>::iterator b = g_bomb_explode.begin();

				for (; b != g_bomb_explode.end(); ++b) {
					send(ptr->sock, (char*)&m_temptype, sizeof(int), 0);
					send(ptr->sock, (char*)&b, sizeof(PosOfBOMB), 0);

				}

				//ㄴretval = send(ptr->sock, (char*)&char_info[j], sizeof(Packet_Char), 0);
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
	DeleteCriticalSection(&g_cs);
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
	Socket_Info* ptr = new Socket_Info;
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
	ptr->m_getpacket = false;
	ptr->sock = sock;
	ptr->recvbytes = 0;
	ptr->sendbytes = 0;
	ptr->m_connected = true;
	SocketInfoArray[g_TotalSockets] = ptr;
	EventArray[g_TotalSockets] = hEvent;
	++g_TotalSockets;
	printf("등록완료\n");
	return TRUE;
}
void RemoveSocketInfo(int nIndex) {
	Socket_Info* ptr = SocketInfoArray[nIndex];

	SOCKADDR_IN clientaddr;
	int addrlen = sizeof(clientaddr);
	getpeername(ptr->sock, (SOCKADDR*)&clientaddr, &addrlen);
	printf("TCP서버 클라이언트 종료:IP 주소=%s,포트번호 = %d\n", inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));
	closesocket(ptr->sock);
	delete ptr;
	WSACloseEvent(EventArray[nIndex]);
	if (nIndex != (g_TotalSockets - 1)) {
		SocketInfoArray[nIndex] = SocketInfoArray[g_TotalSockets - 1];
		EventArray[nIndex] = EventArray[g_TotalSockets - 1];

	}
	--g_TotalSockets;

}


DWORD WINAPI Bomb_Count_Thread(LPVOID arg) {

	float m_testtime = 0.0f;
	while (1) {
		DWORD currTime = GetTickCount();
		DWORD elapsedTime = currTime - g_prevTime2;
		
		g_prevTime2 = currTime;
		
		list<Bomb_Pos>::iterator k = bomb_list.begin();
		if (bomb_list.size()>0) {
			EnterCriticalSection(&g_cs);
			for (; k != bomb_list.end(); ++k) {

				if (k->is_set) {
					k->settime = k->settime + ((float)elapsedTime / 1000);
				}
				else {
					k = bomb_list.erase(k);
				}
				if (k->settime >= 3.0f) {
					MapInfo[k->posx][k->posz] = 2;
					//bomb_Map[k->posx][k->posz] = false;
					k->is_set = false;
					k->settime = 0.0f;
					PosOfBOMB pos = {k->posx,k->posz};
					g_bomb_explode.emplace_back(pos);
					printf("폭탄제거\n");
				}

			}
			
		}
		else {
			m_testtime = m_testtime + ((float)elapsedTime / 1000);
			//printf("m_testtime = %f\n", m_testtime);
			if (m_testtime > 2.0f) {
				printf("폭탄없음\n");
				m_testtime = 0.0f;

			}
		}
		LeaveCriticalSection(&g_cs);
		/*
		for (int j = 0; j < 225; ++j) {
			if (bombs[j].is_set) {
				bombs[j].settime = bombs[j].settime + ((float)elapsedTime / 1000);

			}
			if (bombs[j].settime >= 2.0f) {
				//폭탄을 폭파시키라고 명령

				//다시 false로
				bombs[j].is_set = false;
				bombs[j].settime = 0.0f;
			}

		}
		*/
	}
	return 0;
	
}







void ArrayMap() {

	char_info[0].hp = 10.0f;
	char_info[0].x = 0.0f;
	char_info[0].z =0.0f;
	char_info[0].is_alive = true;
	char_info[0].rotY = 0.0f;
	char_info[1].hp = 10.0f;
	char_info[1].x = 28.0f;
	char_info[1].z = 0.0f;
	char_info[1].is_alive = true;
	char_info[1].rotY = 0.0f;
	char_info[2].hp = 10.0f;
	char_info[2].x = 0.0f;
	char_info[2].z = 28.0f;
	char_info[2].is_alive = true;
	char_info[2].rotY = 180.0f;
	char_info[3].hp = 10.0f;
	char_info[3].x = 28.0f;
	char_info[3].z = 28.0f;
	char_info[3].is_alive = true;
	char_info[3].rotY = 180.0f;



	for (int x = 0; x < 15; ++x) {
		for (int y = 0; y < 15; ++y) {
			MapInfo[x][y] = 2;
		}

	}

	for (int i = 2; i < 13; ++i) {
		if (rand() % 15 < 1) {
			MapInfo[i][0] = 1;//'B';
		}
		else if (rand() % 15 < 5) {
			MapInfo[i][0] = 2;//'N'
		}
		else if (rand() % 15 < 9) {
			MapInfo[i][0] = 3;//'C'
		}
		else if (rand() % 15 < 13) {
			MapInfo[i][0] = 4;//'R'
		}
		else
			MapInfo[i][0] = 5;//'I'

		if (rand() % 15 < 1) {
			MapInfo[i][1] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[i][1] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[i][1] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[i][1] = 4;
		}
		else
			MapInfo[i][1] = 5;

		if (rand() % 15 < 1) {
			MapInfo[i][1] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[i][1] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[i][1] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[i][1] = 4;
		}
		else
			MapInfo[i][1] = 5;

		if (rand() % 15 < 1) {
			MapInfo[i][14] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[i][14] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[i][14] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[i][14] = 4;
		}
		else
			MapInfo[i][14] = 5;
		if (rand() % 15 < 1) {
			MapInfo[i][13] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[i][13] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[i][13] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[i][13] = 4;
		}
		else
			MapInfo[i][13] = 5;

		if (rand() % 15 < 1) {
			MapInfo[0][i] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[0][i] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[0][i] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[0][i] = 4;
		}
		else
			MapInfo[0][i] = 5;

		if (rand() % 15 < 1) {
			MapInfo[1][i] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[1][i] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[1][i] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[1][i] = 4;
		}
		else
			MapInfo[1][i] = 5;

		if (rand() % 15 < 1) {
			MapInfo[13][i] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[13][i] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[13][i] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[13][i] = 4;
		}
		else
			MapInfo[13][i] = 5;
		if (rand() % 15 < 1) {
			MapInfo[14][i] = 1;
		}
		else if (rand() % 15 < 5) {
			MapInfo[14][i] = 2;
		}
		else if (rand() % 15 < 9) {
			MapInfo[14][i] = 3;
		}
		else if (rand() % 15 < 13) {
			MapInfo[14][i] = 4;
		}
		else
			MapInfo[14][i] = 5;


		for (int j = 2; j < 13; ++j) {
			if (rand() % 15 < 1) {
				MapInfo[i][j] = 1;
			}
			else if (rand() % 15 < 5) {
				MapInfo[i][j] = 2;
			}
			else if (rand() % 15 < 9) {
				MapInfo[i][j] = 3;
			}
			else if (rand() % 15 < 13) {
				MapInfo[i][j] = 4;
			}

			else
				MapInfo[i][j] = 5;

		}


	}

	for (int x = 0; x < 15; ++x) {
		for (int y = 0; y < 15; ++y) {
			if (MapInfo[x][y] == 1) {
				printf("B  ");
			}
			else if (MapInfo[x][y] == 2) {
				printf("N  ");
			}
			else if (MapInfo[x][y] == 3) {
				printf("C  ");
			}
			else if (MapInfo[x][y] == 4) {
				printf("R  ");
			}
			else if (MapInfo[x][y] == 5) {
				printf("I  ");
			}


		}
		printf("\n");
	}

}




