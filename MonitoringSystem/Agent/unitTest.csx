#load "..\ScriptCsCommon\defineData.csx"
#load "configuration.csx"
#load "computerStatus.csx"
#load "cron_sendAppStatus.csx"
#load "cron_processCommand.csx"
#load "aws_service.csx"

// 개발 시에 사용할 유닛테스트용 코드 모음

//scriptcs unittest.csx -- -loglevel debug
var logger = Require<Logger>();

var dumper = Require<ObjectDumper>()
    .Compact()
    .Build();


logger.Info("테스트 스크립트 시작 ~");

// 테스트
테스트_S3_다운로드();
//TestPrintAppServerInfo();
//TestComputerStatusInfo();
//프로세스_실행_테스트();


//////////////// 테스트 모듈 ///////////////////////
void 테스트_S3_다운로드()
{
  S3_FileDownload("ㅇㅇ", @"D:\tmp");
}

void 프로세스_실행_테스트()
{
	ConfigManager.Load();

	var result = 프로세스_실행하기("LoginServer");
  Console.WriteLine(result);
}



void TestComputerStatusInfo()
{
	var titleNameList = new List<string>();
	titleNameList.Add("명령 프롬프트");
	titleNameList.Add("Agent - Microsoft Visual Studio");

	var statusList = new List<MsgAppServerStatus>();

	프로세스_상태_조사.CheckProcess(titleNameList, ref statusList);

	foreach(var status in statusList)
	{
		if(status.AppStatus == "Running")
		{
			Console.WriteLine("{0} 타이틀의 프로세스 실행 중", status.AppTitleName);
			Console.WriteLine("- 프로세스 CPU 사용량: {0}", status.Process_CPU_Percent);
			Console.WriteLine("- 프로세스 메모리 사용량: {0} MB", status.App_Memory_MB);
		}
		else
		{
				Console.WriteLine("{0} 타이틀의 프로세스를 찾지 못했음", status.AppTitleName);
		}
	}
}

void TestPrintAppServerInfo()
{
	var index = 0;

	foreach(var appServer in ConfigManager.AppServerList)
	{
		appServer.ConsoleWrite(index);
		++index;
	}
}
