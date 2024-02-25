using System.Threading;
using System;
using static Yagami.Offsets;
using System.Diagnostics;

namespace Yagami
{
    public class Inject
    {
        public static Memory mem;
        public static int clientdll, enginedll;
        public static void findProcess()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    try
                    {
                        Process prc = Process.GetProcessesByName("csgo")[0];
                        mem = new Memory("csgo");

                        foreach (ProcessModule mdl in prc.Modules)
                        {
                            switch (mdl.ModuleName)
                            {
                                case "client.dll":
                                    clientdll = (int)mdl.BaseAddress;
                                    break;
                                case "engine.dll":
                                    enginedll = (int)mdl.BaseAddress;
                                    break;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }).Start();
        }
        public static void newProcess(Action action)
        {
            Thread thr = new Thread(() =>
            {
                while (true)
                {
                    if (gameStatus())
                    {
                        action();
                    }
                }
            });
            thr.Priority = ThreadPriority.Highest;
            thr.Start();
        }
        public static bool gameStatus()
        {
            int clientState = mem.Read<int>(enginedll + dwClientState);

            bool isPlay = mem.Read<int>(clientState + dwClientState_State) >= 5 && !mem.Read<bool>(
                clientState + dwClientState_IsHLTV);

            return isPlay;
        }
    }
}
