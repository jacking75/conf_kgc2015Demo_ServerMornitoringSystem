// 이 코드는 구글링을 통해서 얻은 코드로 핵심 로직은 제가 만든 것이 아니다.
// 출처가 어디인지 기억이 나지 않아서 출처를 남기지 못했다 ;;;;;
// Windows 플랫폼에서만 사용할 수 있다
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

public static class 프로세스_상태_조사
{
  public static void CheckProcess(List<string> titleNameList, ref List<MsgAppServerStatus> statusList)
  {
    foreach(var titleName in titleNameList)
    {
      var pro = GetProcessByTitleName(titleName);
      if (pro != null)
      {
        //Console.WriteLine("{0} 타이틀 프로세스를 찾았음", titleName);
        ComputerStatus.GetStatus(pro, pro.ProcessName);

        statusList.Add(new MsgAppServerStatus()
        {
            AppTitleName = titleName,
            AppStatus = "Running",
            All_CPU_Percent = ComputerStatus.전체_CPU_사용량.ToString(),
            Process_CPU_Percent = ComputerStatus.프로세스_CPU_사용량.ToString(),
            App_Memory_MB = ComputerStatus.메모리_사용량.ToString(),
            Machine_Memory_Percent = ComputerStatus.머신_메모리_사용량_퍼센트.ToString(),
            Machine_Memory_GB = ComputerStatus.머신_총_메모리.ToString()
        });
      }
      else
      {
        //Console.WriteLine("{0} 타이틀 프로세스를 못 찾았음", titleName);

        statusList.Add(new MsgAppServerStatus()
        {
          AppTitleName = titleName,
          AppStatus = "Stop",
          All_CPU_Percent = "0",
          Process_CPU_Percent = "0",
          App_Memory_MB = "0",
          Machine_Memory_Percent = "0",
          Machine_Memory_GB = "0"
        });
      }
    }
  }

  public static Process GetProcessByTitleName(string titleName)
  {
    var allprocess = Process.GetProcesses();

    foreach (var pro in allprocess)
  	{
      //Console.WriteLine("GetProcessByTitleName - {0}. {1}", pro.MainWindowTitle, titleName);
      if (pro.MainWindowTitle == titleName)
  		{
  			return pro;
  		}
    }

    return null;
  }
}

public static class PerformanceInfo
{
    [System.Runtime.InteropServices.DllImport("psapi.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    public static extern bool GetPerformanceInfo([System.Runtime.InteropServices.Out] out PerformanceInformation PerformanceInformation, [System.Runtime.InteropServices.In] int Size);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct PerformanceInformation
    {
        public int Size;
        public IntPtr CommitTotal;
        public IntPtr CommitLimit;
        public IntPtr CommitPeak;
        public IntPtr PhysicalTotal;
        public IntPtr PhysicalAvailable;
        public IntPtr SystemCache;
        public IntPtr KernelTotal;
        public IntPtr KernelPaged;
        public IntPtr KernelNonPaged;
        public IntPtr PageSize;
        public int HandlesCount;
        public int ProcessCount;
        public int ThreadCount;
    }

    public static Int64 GetPhysicalAvailableMemoryInMiB()
    {
        PerformanceInformation pi = new PerformanceInformation();
        if (GetPerformanceInfo(out pi, System.Runtime.InteropServices.Marshal.SizeOf(pi)))
        {
            return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
        }
        else
        {
            return -1;
        }

    }

    public static Int64 GetTotalMemoryInMiB()
    {
        PerformanceInformation pi = new PerformanceInformation();
        if (GetPerformanceInfo(out pi, System.Runtime.InteropServices.Marshal.SizeOf(pi)))
        {
            return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
        }
        else
        {
            return -1;
        }

    }
}

public static class ComputerStatus
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetSystemTimes(out FILETIME lpIdleTime,
                out FILETIME lpKernelTime, out FILETIME lpUserTime);

    static FILETIME _prevSysKernel;
    static FILETIME _prevSysUser;
    static FILETIME _prevSysIdle;

    static TimeSpan _prevProcTotal;


    public static int 메모리_사용량 = 0;
    public static float 전체_CPU_사용량 = 0.0f;
    public static float 프로세스_CPU_사용량 = 0.0f;

    public static int 머신_메모리_사용량_퍼센트 = 0;
    public static long 머신_총_메모리= 0;


    public static void Init()
    {
        _prevProcTotal = TimeSpan.MinValue;
    }

    public static void GetStatus(Process process, string exename)
    {
        프로세스_CPU_사용량 = 0.0f;
        전체_CPU_사용량 = 0.0f;
        메모리_사용량 = 0;
        //Console.WriteLine("GetStatus - 1");

        if (process != null)
        {
            //Console.WriteLine("GetStatus - 2");
            프로세스_CPU_사용량 = GetUsage(process, out 전체_CPU_사용량);
            //Console.WriteLine("GetStatus - 3");
            var objMemory = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", exename);
            메모리_사용량 = (int)objMemory.NextValue() / 1000000;
        }

        //Console.WriteLine("GetStatus - 100");
        Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
        Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
        decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
        decimal percentOccupied = 100 - percentFree;

        머신_총_메모리 = (long)Math.Round(tot / (decimal)1024);
        머신_메모리_사용량_퍼센트 = (int)percentOccupied;

    }

    static float GetUsage(Process process, out float processorCpuUsage)
    {
        processorCpuUsage = 0.0f;

        float _processCpuUsage = 0.0f;
        FILETIME sysIdle, sysKernel, sysUser;

        TimeSpan procTime = process.TotalProcessorTime;

        if (!GetSystemTimes(out sysIdle, out sysKernel, out sysUser))
        {
            return 0.0f;
        }
        //Console.WriteLine("GetUsage - 1");
        if (_prevProcTotal != TimeSpan.MinValue)
        {
            //Console.WriteLine("GetUsage - 2");
            ulong sysKernelDiff = SubtractTimes(sysKernel, _prevSysKernel);
            //Console.WriteLine("GetUsage - 3");
            ulong sysUserDiff = SubtractTimes(sysUser, _prevSysUser);
            ulong sysIdleDiff = SubtractTimes(sysIdle, _prevSysIdle);
            //Console.WriteLine("GetUsage - 6");
            ulong sysTotal = sysKernelDiff + sysUserDiff;
            long kernelTotal = (long)(sysKernelDiff - sysIdleDiff);

            if (kernelTotal < 0)
            {
                kernelTotal = 0;
            }

            //Console.WriteLine("GetUsage - 11");
            processorCpuUsage = (float)((((ulong)kernelTotal + sysUserDiff) * 100.0) / sysTotal);
            //Console.WriteLine("GetUsage - 12");

            long procTotal = (procTime.Ticks - _prevProcTotal.Ticks);

            if (sysTotal > 0)
            {
                //Console.WriteLine("GetUsage - 21");
                _processCpuUsage = (short)((100.0 * procTotal) / sysTotal);
                //Console.WriteLine("GetUsage - 22");
            }
        }

        _prevProcTotal = procTime;
        _prevSysKernel = sysKernel;
        _prevSysUser = sysUser;
        _prevSysIdle = sysIdle;

        return _processCpuUsage;
    }

    static UInt64 SubtractTimes(FILETIME a, FILETIME b)
    {
        long hFTa = (((long)a.dwHighDateTime) << 32) + b.dwLowDateTime;
        long hFTb = (((long)a.dwHighDateTime) << 32) + b.dwLowDateTime;

        var a_datetime = DateTime.FromFileTime(hFTa);
        var b_datetime = DateTime.FromFileTime(hFTb);;
        return (ulong)((a_datetime - b_datetime).Ticks);
    }
}
