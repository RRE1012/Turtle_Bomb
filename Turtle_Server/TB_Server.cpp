#include "TB_Server.h"



TB_Server::TB_Server()
{
	WSADATA wsa;
	//콘솔에서 한글 출력 안될 때 해결방법
	_wsetlocale(LC_ALL, L"korean");
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return;
	user_count = 0;
	Bind_Server();

}


TB_Server::~TB_Server()
{
	closesocket(accept_sock);
	WSACleanup();
}

void TB_Server::err_display(char *msg)
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

void TB_Server::err_quit(char* msg) {
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
void TB_Server::Bind_Server() {
	
	int retval=0;
	accept_sock = socket(AF_INET, SOCK_STREAM, 0);
	if (accept_sock == INVALID_SOCKET)
		err_quit("socket()");
	ZeroMemory(&server_addr, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	server_addr.sin_port = htons(TB_SERVER_PORT);
	::bind(accept_sock, reinterpret_cast<SOCKADDR*>(&server_addr), sizeof(server_addr));
	retval = listen(accept_sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
		err_quit("listen()");
}



void TB_Server::Receive_User() {
	while (1) {
		addrlen = sizeof(client_addr);
		client_sock = accept(accept_sock, (SOCKADDR*)&client_addr, &addrlen);
		if (client_sock == INVALID_SOCKET) {
			err_display("accept()");
			break;
		}
		user_count = user_count + 1;
		printf("\n[TCP 서버] 클라이언트 접속: IP 주소=%s, 포트 번호=%d\n", inet_ntoa(client_addr.sin_addr), ntohs(client_addr.sin_port));
		switch (user_count) {
		
		case 1:
			hThread = CreateThread(NULL, 0, this->Process_ServerP, (LPVOID)client_sock, 0, NULL);
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		default:
			break;

		}

	}

}

DWORD WINAPI TB_Server::Process_Server(LPVOID arg) {
	while (1) {


	}
	return 0;
}