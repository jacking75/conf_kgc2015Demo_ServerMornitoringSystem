#load "..\Common\defineData.csx"
#load "..\Common\util.csx"
#load "agentDataStroage.csx"
#load "adminUser.csx"


// 실행 명령어.
//scriptcs server.csx -- -loglevel debug // 로그 레벨을 디버그 레벨로 한다
//scriptcs server.csx -- -loglevel info  // 로그 레벨을 정보 레벨로 한다
//scriptcs server.csx -- -loglevel error // 로그 레벨을 에러 레벨로 한다

// 객체 덤퍼와 로거 초기하
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

// agent 데이터 저장소와 관리자 객체 초기화
AgentDataStroage.Init();
AdminUser.Init();

// Nancy 호스팅 시작
Require<NancyPack>().Host();



public class IndexModule : NancyModule
{
    public IndexModule()
    {
        Get["/"] = _ =>	View["index"];

        // 테스트용 핸들러
        Post["/json/test"] = _ => {
            var form = this.Bind<TestForm>();
            var jsoFormat = Newtonsoft.Json.JsonConvert.SerializeObject(form);
            Console.WriteLine("/json/test. Req json: {0}", jsoFormat);
    				return Response.AsJson(form);
          };


        // Agent가 보낸 appServer 상태
        Post["/json/appServerStatus"] = _ => {
            var packet = this.Bind<PacketNotifyAppServerStatus>();
            var jsonFormat = Newtonsoft.Json.JsonConvert.SerializeObject(packet.AppStatusList);
            AgentDataStroage.AddUpdate(packet.AgentIP, jsonFormat);
						return "Ok";
          };

        // Agent가 관리자의 요청을 가져간다
        Post["/json/RequestTakeAdminCommand"] = _ => {
            try
            {
              var packet = this.Bind<PacketRequestTakeAdminCommand>();
              var takeCommand = "";
              string appServer = "";
              AdminUser.TakeCommand(packet.AgentIP, ref takeCommand, ref appServer);

              var response = new PacketResponseTakeAdminCommand();
              response.Command = takeCommand;
              response.AppServer = appServer;
              return Response.AsJson(response);
            }
            catch(Exception ex)
            {
              return ex.Message;
            }
          };


        // 클라이언트가 관리자 로그인 요청
        Post["/json/RequestAdminLogin"] = _ => {
            try
            {
              Global.logger.Debug("/json/RequestAdminLogin start");
              var packet = this.Bind<PacketRequestAdminLogIn>();
              Console.WriteLine("/json/RequestAdminLogin. req:{0}",packet.UserID);

              var authToken = Util.GenerateSecureString(4,4,4);
              var result = AdminUser.SetUser(packet.UserID, packet.Password, authToken);

              var response = new PacketResponseAdminLogIn();
              if(result.Item1)
              {
                  response.Result = DefineVar.RET_OK;
                  response.AuthToken = result.Item2;
              }
              else
              {
                response.Result = result.Item2;
              }

              Global.logger.Debug("/json/RequestAdminLogin end. result: " + response.Result);
              return Response.AsJson(response);
            }
            catch(Exception ex)
            {
              return ex.Message;
            }
          };

        // 클라이언트의 Agent 들의 상태 갱신 요청
        Post["/json/RequestRefresh"] = _ => {
            try
            {
              var allServerList = AgentDataStroage.GetAllData();

              var response = new PacketResponseRefresh();
              response.AdminUserID = AdminUser.GetUserID();
              response.Count = allServerList.Count;
              response.AppServerList = allServerList;

              return Response.AsJson(response);
            }
            catch(Exception ex)
            {
              return ex.Message;
            }
          };

        // 관리자 명령 요청
        Post["/json/RequestCommand"] = _ => {
            try
            {
              var packet = this.Bind<PacketRequestAdminCommand>();
              Global.logger.Debug(Global.dumper.Dump(packet));

              var result = AdminUser.SetCommand(packet.UserID, packet.AuthToken, packet.AgentIP, packet.Command, packet.AppServer);

              var response = new PacketResponseAdminCommand()
              {
                AgentIP = packet.AgentIP,
                Command = packet.Command,
                AppServer = packet.AppServer,
                Result = result,
              };
      				return Response.AsJson(response);
            }
            catch(Exception ex)
            {
              return new PacketResponseAdminCommand() { Result =ex.Message };
            }
          };
	}
}
