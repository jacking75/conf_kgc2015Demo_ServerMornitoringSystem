// 주기적으로 애플리케이션 서버의 상태를 서버에 통보한다

void AppServer들의_상태를_관리서버에_통보한다(object userState)
{
	while(true)
	{
		System.Threading.Thread.Sleep(3000);
		//Console.WriteLine("AppServer들의_상태를_관리서버에_통보한다. {0}", DateTime.Now);

		try
		{
			var statusList = new List<MsgAppServerStatus>();
			AppServer들의_상태를_조사한다(ref statusList);
			AppServer들의_상태를_통보한다(statusList);
		}
		catch(Exception ex)
		{
			logger.Error("[AppServer들의_상태를_관리서버에_통보한다] " + ex.Message);
		}
	}
}

void AppServer들의_상태를_조사한다(ref List<MsgAppServerStatus> statusList)
{
	var titleNameList = new List<string>();
	foreach(var app in ConfigManager.AppServerList)
	{
		titleNameList.Add(app.TitleName);
	}

	프로세스_상태_조사.CheckProcess(titleNameList, ref statusList);

	for(int i = 0; i < statusList.Count; ++i)
	{
			statusList[i].AgentIP = ConfigManager.AgentIP;
	}
	/*
	foreach(var status in statusList)
	{
		if(status.AppExcute == "Y")
		{
			Console.WriteLine("{0} 타이틀의 프로세스 실행 중", status.AppTitleName);
			Console.WriteLine("- 프로세스 CPU 사용량: {0}", status.Process_CPU_Useage);
			Console.WriteLine("- 프로세스 메모리 사용량: {0} MB", status.App_Memory_UseageMB);
		}
		else
		{
				Console.WriteLine("{0} 타이틀의 프로세스를 찾지 못했음", status.AppTitleName);
		}
	}
	*/
}

void AppServer들의_상태를_통보한다(List<MsgAppServerStatus> statusList)
{
	var packet = new PacketNotifyAppServerStatus();
	packet.AgentIP = ConfigManager.AgentIP;
	packet.AppStatusList = statusList;

	var request = new Request();
	var response = request.PostJson(ConfigManager.관리서버에_App서버_상태_통보하기url, packet);
	//Console.WriteLine(ConfigManager.관리서버에_App서버_상태_통보하기url);
	//Console.WriteLine("Response status code: " + response.StatusCode);
}
