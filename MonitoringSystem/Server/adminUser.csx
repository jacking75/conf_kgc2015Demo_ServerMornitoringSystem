// 관리자 객체
public static class AdminUser
{
  static System.Threading.SpinLock LockObject = new System.Threading.SpinLock();
  static string UserID = "";
  static string AuthToken = "";
  static string ReqAgentIP = "";
  static string ReqCommand = "";
  static string ReqCommandTargetAsppServer = "";

  // 코드로 계정 정보를 저장한다
  static Dictionary<string, string> adminPasswordDic = new Dictionary<string, string>();

  public static void Init()
  {
    adminPasswordDic.Add("jacking", "123qwe");
  }

  // 관리자인지 확인한다
  static bool CheckAdmin(string userID, string password)
  {
    if (adminPasswordDic.ContainsKey(userID) == false)
    {
      return false;
    }

    if(adminPasswordDic[userID] != password)
    {
      return false;
    }

    return true;
  }

  // 앞에 다른 유저가 로그인 했었다면 강제로 지우고 로그인 한다.
  public static Tuple<bool,string> SetUser(string id, string password, string authToken)
  {
    try
    {
      Lock();

      bool isResult = false;
      string returnValue = DefineVar.RET_FAIL;

      Global.logger.Debug(string.Format("현재 로그인된 관리자:{0}, 요청한 유저:{1}", UserID, id));
      if(CheckAdmin(id, password))
      {
        Logout();
        isResult = true;

        UserID = id;
        AuthToken = authToken;
        returnValue = AuthToken;
      }
      else
      {
        returnValue = "Failed: 패스워드가 틀림";
      }

      return Tuple.Create(isResult, returnValue);
    }
    finally
    {
      UnLock();
    }
  }

  public static string GetUserID()
  {
    Lock();
    var userId = UserID;
    UnLock();

    return userId;
  }

  static void Logout()
  {
    UserID = "";
    AuthToken = "";
    ReqAgentIP = "";
    ReqCommand = "";
  }

  // 관리자 명령을 저장
  public static string SetCommand(string userId, string authToken, string agentIP, string cmd, string appServer)
  {
    //Global.logger.Error("[SetCommand] 현재 관리자 인증을 하지 않고 있음");
    Lock();
    var result = "Failed";

    if(ReqCommand != "" )
    {
      Global.logger.Error(string.Format("[SetCommand] 이미 관리자 요청이 있음. cmd:{0}", ReqCommand));
    }
    else if( UserID != userId )
    {
      Global.logger.Error(string.Format("[SetCommand] 관리자가 아님. 현재 관리자:{0}, 요청 관리자:{1}", UserID, userId));
    }
    else if(AuthToken != authToken)
    {
      Global.logger.Error(string.Format("[SetCommand] AuthToken 이 틀림"));
    }
    else
    {
      ReqAgentIP = agentIP;
      ReqCommand = cmd;
      ReqCommandTargetAsppServer = appServer;
      result = "Ok";

      Global.logger.Info(string.Format("관리자가 명령 요청. agent:{0}, cmd:{1}", ReqAgentIP, ReqCommand));
    }

    UnLock();
    return result;
  }

  public static bool TakeCommand(string agentIP, ref string cmd, ref string appServer)
  {
    Lock();
    bool success = false;

    if(agentIP == ReqAgentIP && ReqCommand != "")
    {
      cmd = ReqCommand;
      appServer = ReqCommandTargetAsppServer;

      ReqAgentIP = "";
      ReqCommand = "";
      ReqCommandTargetAsppServer = "";
      success = true;

      Global.logger.Debug(string.Format("Agnet:{0}가 명령:{1}을 가져감", agentIP, cmd));
    }

    UnLock();
    return success;
  }


  // 일부러 락을 느슨하게 구현. 락이 빈번하지 않을 것이기 때문...
  public static void Lock()
  {
    bool gotLock = false;
    LockObject.Enter(ref gotLock);
  }

  public static void UnLock()
  {
    LockObject.Exit();
  }
}
