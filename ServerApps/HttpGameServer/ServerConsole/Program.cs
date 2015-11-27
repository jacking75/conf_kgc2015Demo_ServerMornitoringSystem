using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Description;


namespace ServerConsole
{
    class Program
    {
        static bool IsServiceStart = true;

        static void Main(string[] args)
        {
            try
            {
                Console.Title = "HttpGameServer";
   
                // 멀티코어 JIT 유효화
                System.Runtime.ProfileOptimization.SetProfileRoot(Environment.CurrentDirectory);
                System.Runtime.ProfileOptimization.StartProfile("App.JIT.Profile");

                ServiceHost host = new ServiceHost(typeof(RESTfulAPILib.APIService));

                // WCF 설정 정보 출력
                foreach (ServiceEndpoint ep in host.Description.Endpoints)
                {
                    Console.WriteLine("바인딩 타입: {0}, Address:{1}", ep.Binding.ToString(), ep.Address.ToString());
                                        
                    var binding = ep.Binding as WebHttpBinding;
                   Console.WriteLine("최대버퍼:{0}, 최대버퍼풀:{1}, 최대받기크기:{2}",
                                binding.MaxBufferSize, binding.MaxBufferPoolSize, binding.MaxReceivedMessageSize);
                }

                // 서비스 시작 !!!
                host.Open();

                Console.WriteLine("WCF 호스트 생성 완료");
                Console.WriteLine("Running Server GC Mode: " + System.Runtime.GCSettings.IsServerGC);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            int workerThreads, completionPortThreads = 0;
            System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("- 최대 워커 스레드 수: {0}, 최대 IO 스레드 수:{1} --", workerThreads, completionPortThreads);

            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("- 최소 워커 스레드 수: {0}, 최소 IO 스레드 수:{1} --", workerThreads, completionPortThreads);

            System.Threading.ThreadPool.SetMinThreads(128, 128);
            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("- 최소 워커 스레드 수: {0}, 최소 IO 스레드 수:{1} --", workerThreads, completionPortThreads);

            Console.WriteLine("서비스 시작 시간: " + DateTime.Now.ToString());
            
            // 프로그램이 종료되지 않도록 대기
            while (IsServiceStart)
            {
                System.Threading.Thread.Sleep(128);
            }
        }

        static void 키보드입력조사(object userState)
        {
            while (true)
            {
                try
                {
                    var command = Console.ReadLine();

                    if (command == "exit")
                    {
                        Console.WriteLine("서버 종료 !!!");
                        IsServiceStart = false;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


    }
}
