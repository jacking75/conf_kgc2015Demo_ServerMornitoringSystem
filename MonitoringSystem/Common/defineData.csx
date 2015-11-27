// Server와 Agent에서 사용할 데이터 정의

public class AppServerCofig
{
	public string TitleName; 	// 서버 프로세스 이름(한 머신에서 중복 불가)
	public string FullPath;		// 서버 프로세스를 실행할 수 있는 파일이 있는 디렉토리 path
	public string ExeFile;		// 서버 프로세스 실행 파일 이름
	public bool IsAgentToHttp; // true이면 agent와 http로 통신. false이면 tcp 통신을 한다.
	public List<string> UpdateS3Files;// 패치로 다운로드 받을 파일(AWS 오브젝트 이름)

	public string ToString(int index)
	{
		var outPut = string.Format("== AppServer Index: {0}, TitleName: {1} =={2}", index, TitleName, Environment.NewLine);
		outPut += string.Format("Agent와 http 통신을 한다: {0}", IsAgentToHttp, Environment.NewLine);
		outPut += string.Format("FullPath: {0}{1}", FullPath, Environment.NewLine);
		outPut += string.Format("ExeFile: {0}{1}", ExeFile, Environment.NewLine);

		foreach(var s3object in UpdateS3Files)
		{
			outPut += string.Format("S3: {0}{1}", s3object, Environment.NewLine);
		}

		outPut.TrimEnd(Environment.NewLine.ToCharArray());
		return outPut;
	}
}



// 변수 선언 클래스
public static class DefineVar
{
	public const string RET_OK = "Ok";
	public const string RET_FAIL = "Failed";
}


// 중요:
// Nancy(Server에서 사용하는 웹 프레임워크)에서 json을 객체로 자동매핑하는 클래스는
// 멤버가 꼭 property로 만들어야 한다.

class TestForm
{
	// 꼭 속성으로 만들어야 한다!!!!
	public string User { get; set; }
	public string Pass { get; set; }
}

// 애플리케이션 서버 상태 메시지
public class MsgAppServerStatus
{
		public string AgentIP;
    public string AppTitleName { get; set; }
    public string AppStatus { get; set; }
    public string Process_CPU_Percent { get; set; }
    public string All_CPU_Percent { get; set; }

    public string App_Memory_MB { get; set; }
    public string App_Memory_Percent { get; set; }
    public string Machine_Memory_Percent { get; set; }
    public string Machine_Memory_GB { get; set; }
}



///////////////////////////////////////////////////////////////////
////// 패킷 정의
///////////////////////////////////////////////////////////////////

// Agent가 앱서버 상태 통보
class PacketNotifyAppServerStatus
{
	public string AgentIP { get; set; }
	public List<MsgAppServerStatus> AppStatusList { get; set; }
}


//  관리자 로그인 요청
class PacketRequestAdminLogIn
{
	public string UserID { get; set; }
	public string Password { get; set; }
}

class PacketResponseAdminLogIn
{
	public string Result { get; set; }
	public string AuthToken { get; set; }
}


// 관리자 명령 요청
class PacketRequestAdminCommand
{
	public string UserID { get; set; }
	public string AuthToken { get; set; }
	public string AgentIP { get; set; }
	public string Command { get; set; }
	public string AppServer { get; set; }
}

class PacketResponseAdminCommand
{
	public string AgentIP { get; set; }
	public string Command { get; set; }
	public string AppServer { get; set; }
	public string Result { get; set; }
}

// 클라이언트가 Agent의 상태를 요청한다
class PacketRequestAllAgentStatus
{
	public string IP { get; set; }
}

class PacketResponseRefresh
{
	public string AdminUserID { get; set; }
	public int Count { get; set; }
	public List<string> AppServerList { get; set; }
}


// (Agent가)관리서버로부터 관리자의 요청 명령어를 가져온다.
class PacketRequestTakeAdminCommand
{
	public string AgentIP { get; set; }
}

class PacketResponseTakeAdminCommand
{
	public string Command { get; set; }
	public string AppServer { get; set; }
}
