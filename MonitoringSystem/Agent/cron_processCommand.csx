
// 주기적으로 관리자의 요청을 서버에서 가져와서 실행한다.

void 관리자_명령을_처리한다(object userState)
{
	while(true)
	{
		System.Threading.Thread.Sleep(2000);
		Console.WriteLine("관리자_명령을_처리한다. {0}", DateTime.Now);

		var request = 관리서버에서_명령을_가져온다();
		ProcessReqCommand(request);
	}
}

PacketResponseTakeAdminCommand 관리서버에서_명령을_가져온다()
{
	try
	{
		var packet = new PacketRequestTakeAdminCommand();
		packet.AgentIP = ConfigManager.AgentIP;

		var request = new Request();
		var httpResult = request.PostJson<PacketRequestTakeAdminCommand>(ConfigManager.관리서버에서_관리자_요청_가져오기url, packet);
		if (httpResult.IsSuccessStatusCode)
    {
			var response = JsonConvert.DeserializeObject<PacketResponseTakeAdminCommand>(httpResult.Content.ReadAsStringAsync().Result);
			logger.Debug("관리서버에서_명령을_가져온다. " + dumper.Dump(response));
			return response;
    }
		else
		{
			Console.WriteLine("실패 관리서버에서_명령을_가져온다");
			return null;
		}
	}
	catch(Exception ex)
	{
		logger.Error("[관리서버에서_명령을_가져온다] " + ex.Message);
		return null;
	}
}

// 관리자의 요청을 처리한다
void ProcessReqCommand(PacketResponseTakeAdminCommand request)
{
	if(request == null || request.Command == "")
	{
		return;
	}

	switch(request.Command)
	{
			case "START_SERVER":
				{
					var result = 프로세스_실행하기(request.AppServer);
				}
				break;
			case "STOP_SERVER":
				{
					var result = 프로세스_종료하기(request.AppServer);
				}
				break;
			case "PATCH_SERVER":
					{
						logger.Error("PATCH_SERVER 아직 미구현");
					}
					break;
			default:
				{
					Console.WriteLine("알수 없는 명령어. {0}", request.Command);
				}
				break;
	}
}

string 프로세스_실행하기(string titleName)
{
    var porc = 프로세스_상태_조사.GetProcessByTitleName(titleName);
		if(porc != null)
		{
			return "이미 프로세스 실행 중";
		}

		var exeString = ConfigManager.GetAppServerExeCommand(titleName);
		if(string.IsNullOrEmpty(exeString))
		{
			return string.Format("{0}-{1} 타이틀의 앱서버는 관리하지 않음", titleName, exeString);
		}

		logger.Debug("[프로세스_실행하기] " + exeString);
		using (var appServerProcess = Process.Start(exeString))
    {
        if (appServerProcess == null)
        {
            return string.Format("{0}-{1} 타이틀의 앱서버는 실행 실패", titleName, exeString);
        }
		}

    return DefineVar.RET_OK;
}


string 프로세스_종료하기(string titleName)
{
    var appServerProcess =  프로세스_상태_조사.GetProcessByTitleName(titleName);
    if (appServerProcess != null)
    {
			logger.Info(string.Format("[프로세스_종료하기] {0} 타이틀의 종료 시도", titleName));
      appServerProcess.CloseMainWindow();
    }
		else
		{
			var error = string.Format("[프로세스_종료하기] {0} 타이틀의 앱서버는 관리하지 않음", titleName);
			logger.Error(error);
			return error;
		}

		appServerProcess =  프로세스_상태_조사.GetProcessByTitleName(titleName);
		if (appServerProcess != null)
    {
			var error = string.Format("[프로세스_종료하기] {0} 타이틀의 앱서버가 종료되지 않음 -_-", titleName);
			logger.Error(error);
      return error;
    }
		else
		{
			logger.Info(string.Format("[프로세스_종료하기] {0} 타이틀의 종료 성공", titleName));
		}

    return DefineVar.RET_OK;
}

bool 서버_리소스_업데이트하기(string titleName)
{
	var appServer = ConfigManager.GetAppServer(titleName);
	if(appServer == null)
	{
		logger.Error("[서버_리소스_업데이트하기] " + titleName + " 은 없음");
		return false;
	}

	foreach(var s3File in appServer.UpdateS3Files)
	{
		S3_FileDownload(s3File, appServer.FullPath);
	}

	logger.Info("[서버_리소스_업데이트하기] " + titleName + " 성공");
	return true;
}
