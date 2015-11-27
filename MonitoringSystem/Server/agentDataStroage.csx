using System.Collections.Concurrent;

// agnet들의 현재 정보를 저장한다.
// Nacny의 문제가라고 생각하는데 일반적인 객체를 컨테이너에 저장하면 Nancy 초기화에 실패가 발생한다
// 객체를 컨테이너에 저장하고 싶은 경우라면 객체를 json화 시켜서 문자열로 저장해야 한다.
public static class AgentDataStroage
{
  // key: Agent IP, value:agent가 보낸 서버들 상태
  static ConcurrentDictionary<string, string> AgentStatusDic = new ConcurrentDictionary<string, string>();

  // key: Agent IP, value:agent 정보를 받은 시간
  static ConcurrentDictionary<string, DateTime> AgentUpdateTimeDic = new ConcurrentDictionary<string, DateTime>();


  public static void Init()
  {
    //Global.logger.Debug("AgentDataStroage.Init");
    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Loop), null);
  }

  // 주기적으로 해야하는 여기에 정의한다
  static void Loop(object userState)
  {
  	while(true)
  	{
  		System.Threading.Thread.Sleep(1500);
  		Global.logger.Debug("called AgentDataStroage.Loop");

      상태_이상_Agent를_Agnet리스트에서_제거한다_inLoop();
    }
  }

  static void 상태_이상_Agent를_Agnet리스트에서_제거한다_inLoop()
  {
    var deleteAgentList = new List<string>();
    var curTime = DateTime.Now;

    // Agent 업데이트 시간이 30초가 넘은 것은 해당 Agent에 이상이 있다고 보고 리스트에서 삭제한다
    foreach(var data in AgentUpdateTimeDic)
    {
      var diffTime = curTime - data.Value;
      if(diffTime.TotalSeconds > 30 )
      {
        deleteAgentList.Add(data.Key);
      }
    }

    foreach(var key in deleteAgentList)
    {
      RemoveData(key);
      RemoveTime(key);
    }
  }

  public static bool AddUpdate(string key, string jsonData)
  {
    string oldData;

  	if (AgentStatusDic.TryGetValue(key, out oldData) == false)
  	{
  		AgentStatusDic.TryAdd(key, jsonData);
      Global.logger.Info(string.Format("AddUpdate - Add. {0}-{1}", key, jsonData));
    }
  	else
  	{
  		AgentStatusDic.TryUpdate(key, jsonData, oldData);
      Global.logger.Info(string.Format("AddUpdate - Update. {0}-{1}", key, jsonData));
  	}

    UpdateTime(key);

    return true;
  }

  public static List<MsgAppServerStatus> GetData(string key)
  {
  	string  agentData;
  	if (AgentStatusDic.TryGetValue(key, out agentData))
  	{
      var agentList = JsonConvert.DeserializeObject<List<MsgAppServerStatus>>(agentData);
  		return agentList;
  	}

  	return null;
  }

  static void RemoveData(string key)
  {
  	string agentData;
  	AgentStatusDic.TryRemove(key, out agentData);

    RemoveTime(key);
  }


  public static bool UpdateTime(string key)
  {
    DateTime oldDateTime;

  	if (AgentUpdateTimeDic.TryGetValue(key, out oldDateTime) == false)
  	{
  		AgentUpdateTimeDic.TryAdd(key, DateTime.Now);
    }
  	else
  	{
  		AgentUpdateTimeDic.TryUpdate(key, DateTime.Now, oldDateTime);
    }

    return true;
  }

  static void RemoveTime(string key)
  {
  	DateTime data;
  	AgentUpdateTimeDic.TryRemove(key, out data);
  }



  public static List<string> GetAllData()
  {
    var allAgentList = new List<string>();

  	foreach(var data in AgentStatusDic)
    {
      allAgentList.Add(data.Value);
    }

  	return allAgentList;
  }
  
}
