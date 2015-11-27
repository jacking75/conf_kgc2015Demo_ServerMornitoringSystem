#load "..\Common\defineData.csx"
#load "configuration.csx"
#load "computerStatus.csx"
#load "cron_sendAppStatus.csx"
#load "cron_processCommand.csx"
#load "aws_s3_service.csx"

// 실행 명령어.
//scriptcs agent.csx -- -loglevel debug // 로그 레벨을 디버그 레벨로 한다
//scriptcs agent.csx -- -loglevel info  // 로그 레벨을 정보 레벨로 한다
//scriptcs agent.csx -- -loglevel error // 로그 레벨을 에러 레벨로 한다


var logger = Require<Logger>();

var dumper = Require<ObjectDumper>()
    .Compact()
    .Build();


public static class Global
{
  public static Logger logger;
  public static ObjectDumper dumper;
}
Global.logger = logger;
Global.dumper = dumper;


logger.Info("스크립트 시작 ~");


bool IsWating = true;
main();

void main()
{
	ConfigManager.Load();

  설정_정보_출력();

	System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(AppServer들의_상태를_관리서버에_통보한다), null);
	System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(관리자_명령을_처리한다), null);

	logger.Info("Ctrl+C 키를 누를 때까지 종료하지 않고 대기한다...");

	while (IsWating)
	{
	    System.Threading.Thread.Sleep(128);
	}
}

void 설정_정보_출력()
{
  ConfigManager.WriteLog();
  WriteLogS3Config();
}
