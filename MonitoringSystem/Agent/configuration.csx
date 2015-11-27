// agent에서 사용할 설정 관리 클래스
public static class ConfigManager
{
	// agent의 IP 주소
	public static string AgentIP = "12.2.8.3";

	// agent에서 서버에 http 통신을 할 때의 url
	// http://localhost:8888 주소는 상황에 맞게 변경해야 한다
	public static string 관리서버에서_관리자_요청_가져오기url = "http://localhost:8888/json/RequestTakeAdminCommand";
	public static string 관리서버에_App서버_상태_통보하기url = "http://localhost:8888/json/appServerStatus";

	// agent에서 관리하는 서버애클리케이션 정보를 저장
	public static List<AppServerCofig> AppServerList = new List<AppServerCofig>();


	public static void Load()
	{
		var server1 = new AppServerCofig();
		server1.TitleName = "HttpGameServer";
		server1.FullPath = @"E:\Dev\Github_SSMSScriptCS\trunk\ServerApps\HttpGameServer\Bin";
		server1.ExeFile = "HttpGameServer.exe";
		server1.IsAgentToHttp = true;
		server1.UpdateS3Files = new List<string>();
		server1.UpdateS3Files.Add("none.dll");
		AppServerList.Add(server1);

		/*
		var server2 = new AppServerCofig();
		server2.TitleName = "TCPGameServer";
		server2.FullPath = @"D:\kgcDemo\AppServers";
		server2.ExeFile = "TCPGameServer.exe";
		server2.IsAgentToHttp = false;
		server2.UpdateS3Files = new List<string>();
		server2.UpdateS3Files.Add("none.dll");
		AppServerList.Add(server2);
		*/		
	}

	public static void WriteLog()
	{
		Global.logger.Info("Agent IP: " + ConfigManager.AgentIP);
	  Global.logger.Info("관리서버에서 관리자 요청 가져오기 url: " + ConfigManager.관리서버에서_관리자_요청_가져오기url);
	  Global.logger.Info("관리서버에 App서버 상태 통보하기 url: " + ConfigManager.관리서버에_App서버_상태_통보하기url);

		var count = 0;
		foreach(var server in AppServerList)
		{
			++count;
			Global.logger.Info(server.ToString(count));
		}
	}

	// 서버애플리케이션이 있는 '디렉토리패스+실행파일 이름'의 문자열을 반환
	public static string GetAppServerExeCommand(string appServerTitleName)
	{
		foreach(var app in AppServerList)
		{
			if(app.TitleName == appServerTitleName)
			{
				return app.FullPath + "\\" + app.ExeFile;
			}
		}

		return "";
	}

	public static AppServerCofig GetAppServer(string appServerTitleName)
	{
		foreach(var app in AppServerList)
		{
			if(app.TitleName == appServerTitleName)
			{
				return app;
			}
		}

		return null;
	}
}
