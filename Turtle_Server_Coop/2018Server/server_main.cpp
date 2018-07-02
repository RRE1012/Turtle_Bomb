#define WIN32_LEAN_AND_MEAN
#define INITGUID

#include <winsock2.h>
#include <Windows.h>
#include <thread>
#include <vector>
#include <array>
#include "protocol.h"
#include <iostream>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

HANDLE g_hIOCP;

struct EXOVERLAPPED
{
	WSAOVERLAPPED m_over;
	char m_iobuf[MAX_BUFF_SIZE];
	WSABUF m_WSABUF;
	bool is_recv;
};

class Client
{
public:
	SOCKET m_Socket;
	bool m_isConnected;
	int m_X;
	int m_Y;

	EXOVERLAPPED m_Recv_exover;
	int m_prev_packet_size; // 이전 recv에서 완성되지 않고 쌓인 패킷 크기
	int m_curr_packet_size; // 지금 조립하는 패킷 크기
	char m_packet[MAX_PACKET_SIZE];

	Client()
	{
		m_isConnected = false;
		m_X = 4;
		m_Y = 4;

		ZeroMemory(&m_Recv_exover.m_over, sizeof(WSAOVERLAPPED));
		m_Recv_exover.m_WSABUF.buf = m_Recv_exover.m_iobuf;
		m_Recv_exover.m_WSABUF.len = sizeof(m_Recv_exover.m_WSABUF.buf);
		m_Recv_exover.is_recv = true;
	};
	~Client() {};
};

array<Client, MAX_USER> g_Clients;

void Initialize()
{
	g_hIOCP = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, 0, 0);
}

void StartRecv(int id)
{
	unsigned long r_flag = 0;
	
	ZeroMemory(&g_Clients[id].m_Recv_exover.m_over, sizeof(WSAOVERLAPPED));

	WSARecv(g_Clients[id].m_Socket, &g_Clients[id].m_Recv_exover.m_WSABUF, 1, NULL, &r_flag, &g_Clients[id].m_Recv_exover.m_over, NULL);
}

void ProcessPacket(int id, char* packet)
{
	int x = g_Clients[id].m_X;
	int y = g_Clients[id].m_Y;

	switch (packet[1])
	{
	case CS_UP:
		if (y > 0)
			--y;
		break;

	case CS_DOWN:
		if (y < BOARD_HEIGHT - 1)
			++y;
		break;

	case CS_LEFT:
		if (x > 0)
			--x;
		break;

	case CS_RIGHT:
		if (x < BOARD_WIDTH - 1)
			++x;
		break;

	default:
		cout << "Unknown Packet Type from Client [" << id << "]\n";
		return;
	}

	g_Clients[id].m_X = x;
	g_Clients[id].m_Y = y;

	sc_packet_pos pos_packet;

	pos_packet.id = id;
	pos_packet.size = sizeof(sc_packet_pos);
	pos_packet.type = SC_POS;
	pos_packet.x = x;
	pos_packet.y = y;

	EXOVERLAPPED* send_over = new EXOVERLAPPED;
	send_over->is_recv = false;
	memcpy(send_over->m_iobuf, &pos_packet, pos_packet.size);
	send_over->m_over, 0;
	send_over->m_WSABUF.buf = send_over->m_iobuf;
	send_over->m_WSABUF.len = send_over->m_iobuf[0];

	WSASend(g_Clients[id].m_Socket, &send_over->m_WSABUF, 1, NULL, 0, &send_over->m_over, NULL);
}

void Worker_Thread()
{
	while (true)
	{
		unsigned long io_size;
		unsigned long long iocp_key; // x64 = long long, x86 = long
		WSAOVERLAPPED* over; // WSAOVERLAPPED 구조체는 winsock2.h 에 있다.
		BOOL ret = GetQueuedCompletionStatus(g_hIOCP, &io_size, &iocp_key, &over, INFINITE);
		
		int key = static_cast<int> (iocp_key);

		// Send / Recv 처리를 한다.
		
		if (ret == FALSE) // 오류처리
		{
			cout << "ERROR in GQCS\n";
			continue;
		}
		
		if (io_size == 0) // 접속 종료 처리
		{
			closesocket(g_Clients[key].m_Socket);
			g_Clients[key].m_isConnected = false;
			continue;
		}

		EXOVERLAPPED* p_Overlapped = reinterpret_cast<EXOVERLAPPED*>(&over);
		
		if (p_Overlapped->is_recv == true) // recv 처리
		{
			int work_size = io_size;
			char* wptr = p_Overlapped->m_iobuf;

			while (work_size > 0)
			{
				int p_size;

				if (g_Clients[key].m_curr_packet_size != 0)
					p_size = g_Clients[key].m_curr_packet_size;
				else
				{
					p_size = wptr[0];
					g_Clients[key].m_prev_packet_size = p_size;
				}

				int need_size = p_size - g_Clients[key].m_prev_packet_size;

				// 패킷 처리
				if (need_size <= work_size)
				{
					memcpy(g_Clients[key].m_packet + g_Clients[key].m_prev_packet_size, wptr, need_size);
					ProcessPacket(key, g_Clients[key].m_packet);
					g_Clients[key].m_prev_packet_size = 0;
					g_Clients[key].m_curr_packet_size = 0;
					work_size -= need_size;
				}

				// 패킷을 처리할 수 없어서 저장
				else
				{
					memcpy(g_Clients[key].m_packet + g_Clients[key].m_prev_packet_size, wptr, work_size);
					g_Clients[key].m_prev_packet_size += work_size;
					work_size = -work_size;
					wptr += work_size;
				}
			}
			
			StartRecv(key);
		}

		else // send 후처리
		{
			delete p_Overlapped;
		}
	}
}

void Accept_Thread()
{
	SOCKET s = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, WSA_FLAG_OVERLAPPED); // 마지막 인자에 주의!
	SOCKADDR_IN bind_Addr;
	ZeroMemory(&bind_Addr, sizeof(SOCKADDR_IN));
	bind_Addr.sin_family = AF_INET;
	bind_Addr.sin_port = htons(MY_SERVER_PORT);
	bind_Addr.sin_addr.s_addr = INADDR_ANY;

	::bind(s, reinterpret_cast<sockaddr*>(&bind_Addr), sizeof(bind_Addr));
	listen(s, 1000);

	while (true)
	{
		SOCKADDR_IN recv_Addr;
		ZeroMemory(&recv_Addr, sizeof(SOCKADDR_IN));
		recv_Addr.sin_family = AF_INET;
		recv_Addr.sin_port = htons(MY_SERVER_PORT);
		recv_Addr.sin_addr.s_addr = INADDR_ANY;
		int addr_size = sizeof(sockaddr);

		// WSAAccept()를 호출하여 클라이언트 소켓을 받는다.
		SOCKET client_socket = WSAAccept(s, reinterpret_cast<sockaddr*>(&recv_Addr), &addr_size, NULL, NULL);

		int id = -1;

		for (int i = 0; i < MAX_USER; ++i)
		{
			if (g_Clients[i].m_isConnected == false)
			{
				id = i;
				break;
			}
		}
		
		if (id == -1)
		{
			cout << "MAX_USER Exceeded\n";
			continue;
		}

		// 받아온 소켓을 IOCP에 등록한다.
		CreateIoCompletionPort(reinterpret_cast<HANDLE>(client_socket), g_hIOCP, id, 0);
		g_Clients[id].m_isConnected = true;

		StartRecv(id);
	}
}

void main()
{
	vector<thread> w_threads;
	
	Initialize();
	
	for (int i = 0; i < 4; ++i) w_threads.push_back(thread{ Worker_Thread, i });
	thread a_thread{ Accept_Thread };

	for (auto& th : w_threads) th.join();
	a_thread.join();
}