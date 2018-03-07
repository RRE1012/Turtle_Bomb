#pragma once
#include "stdafx.h"
class TB_Server
{
public:
	TB_Server();
	~TB_Server();
	void err_quit(char*);
	void err_display(char*);

	void Bind_Server();
	void Receive_User();
	static DWORD WINAPI Process_ServerP(LPVOID param) {
		TB_Server* objs = new TB_Server();
		return objs->Process_Server(param);
	};

	DWORD WINAPI Process_Server(LPVOID);
	
	
	//void Destory_Sockets();
private:
	SOCKET start_sock;
	SOCKET accept_sock;
	SOCKET client_sock;
	int addrlen;
	SOCKADDR_IN server_addr;
	SOCKADDR_IN client_addr;
	HANDLE hThread;
	int user_count;
};

