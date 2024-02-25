using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using static Yagami.Inject;
using static Yagami.Offsets;

namespace Yagami
{
    public class Functions
    {

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys ArrowKeys);

        public static bool glowStatus, bhopStatus, radarStatus;

        public static string[] tasks = {"taskmgr.exe", "Analyzer.exe", "ExecutedProgramsList.exe", "\"SandeLLo CHECKER.exe\"", "BrowserDownloadsView.exe",
                "Everything.exe", "LastActivityView.exe", "m9snoi.exe", "USBDeview.exe"};
        public static string glowColor;
        public static void funcGlow()
        {
            Thread.Sleep(1);

            if (glowStatus)
            {
                int localPlayer = mem.Read<int>(clientdll + dwLocalPlayer);
                int ourteam = mem.Read<int>(localPlayer + m_iTeamNum);

                for (byte i = 0; i < 64; i++)
                {
                    int entityList = mem.Read<int>(clientdll + dwEntityList + i * 0x10);
                    int otherTeam = mem.Read<int>(entityList + m_iTeamNum);
                    int enemyHP = mem.Read<int>(entityList + m_iHealth);

                    if (entityList != 0 && otherTeam != 0 && otherTeam != ourteam)
                    {
                        int glowID = mem.Read<int>(entityList + m_iGlowIndex);

                        switch (glowColor)
                        {
                            case "whSetRed":
                                drawPlayer(glowID, 255, 0, 0);
                                break;
                            case "whSetGreen":
                                drawPlayer(glowID, 0, 255, 0);
                                break;
                            case "whSetBlue":
                                drawPlayer(glowID, 0, 0, 255);
                                break;
                            case "whSetYellow":
                                drawPlayer(glowID, 255, 255, 0);
                                break;
                            case "whSetPink":
                                drawPlayer(glowID, 255, 0, 255);
                                break;
                            default:
                                drawPlayer(glowID, 255, 0, 0);
                                break;
                        }
                    }
                }
            }
        }
        public static void funcRadar()
        {
            Thread.Sleep(1);

            if (radarStatus)
            {
                for (int i = 0; i < 64; i++)
                {
                    int entityList = mem.Read<int>(clientdll + dwEntityList + i * 0x10);
                    mem.Write<int>(entityList + m_bSpotted, 1);
                }
            }
        }
        public static void funcBhop()
        {
            Thread.Sleep(1);

            if (bhopStatus)
            {
                int localPlayer = mem.Read<int>(clientdll + dwLocalPlayer);
                int flag = mem.Read<int>(localPlayer + m_fFlags);

                if (GetAsyncKeyState(Keys.Space) < 0 && flag == 257 || flag == 263)
                {
                    mem.Write(clientdll + dwForceJump, 5);
                    Thread.Sleep(1);
                    mem.Write(clientdll + dwForceJump, 4);
                }
            }
        }
        public static void drawPlayer(int glowID, int red, int green, int blue)
        {
            int glowObject = mem.Read<int>(clientdll + dwGlowObjectManager);

            mem.Write(glowObject + (glowID * 0x38) + 0x8, red / 100f);
            mem.Write(glowObject + (glowID * 0x38) + 0xC, green / 100f);
            mem.Write(glowObject + (glowID * 0x38) + 0x10, blue / 100f);
            mem.Write(glowObject + (glowID * 0x38) + 0x14, 255 / 100f);
            mem.Write(glowObject + (glowID * 0x38) + 0x28, true);
            mem.Write(glowObject + (glowID * 0x38) + 0x29, false);
        }
        public static void tasksKill()
        {
            Process process = new Process();

            foreach (string s in tasks)
            {
                try
                {
                    process.StartInfo.FileName = "taskkill";
                    process.StartInfo.Arguments = string.Format("/IM {0} /T /F", s);
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                }
                catch
                {

                }
            }
        }
    }
}
