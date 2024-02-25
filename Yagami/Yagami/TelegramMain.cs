using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;
using static Yagami.Inject;
using static Yagami.Functions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yagami
{
    public partial class TelegramMain : Form
    {

        public static string userID;
        private string _tokenBot;
        public TelegramMain()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            userID = "YOUR_USER_ID";
            _tokenBot = "YOUR_BOT_TOKEN";
            var client = new TelegramBotClient($"{_tokenBot}");
            client.StartReceiving(Update, Error);

            try
            {
                findProcess();
            }
            catch
            {

            }
            this.Close();
        }
        private async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var msg = update.Message;

            if (msg != null && msg.Chat.Id == (long)Convert.ToInt64(userID))
            {
                string result;

                if (msg.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(msg.Chat.Id, "Yagami v 1.0 - самый скрытый чит на CS:GO✅", replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Inject⚡"),
                            new KeyboardButton("Wallhack🔥"),
                            new KeyboardButton("Radarhack⚡"),
                            new KeyboardButton("Bunnyhop🚀"),
                            new KeyboardButton("Tasks kill💀"),
                            new KeyboardButton("Settings⚙️")
                        }
                    })
                    {

                    }) ;
                }
                else if (msg.Text == "Inject⚡")
                {
                    findProcess();

                    await botClient.SendTextMessageAsync(msg.Chat.Id, "Успешно✅");
                }
                else if (msg.Text == "Wallhack🔥")
                {
                    glowStatus = !glowStatus;
                    if (glowStatus)
                    {
                        newProcess(funcGlow);
                    }

                    result = glowStatus ? "enabled✅" : "disabled❌";

                    await botClient.SendTextMessageAsync(msg.Chat.Id, $"Wallhack {result}");
                }
                else if (msg.Text == "Radarhack⚡")
                {
                    radarStatus = !radarStatus;
                    if (radarStatus)
                    {
                        newProcess(funcRadar);
                    }

                    result = radarStatus ? "enabled✅" : "disabled❌";

                    await botClient.SendTextMessageAsync(msg.Chat.Id, $"Radarhack {result}");
                }
                else if (msg.Text == "Bunnyhop🚀")
                {
                    bhopStatus = !bhopStatus;
                    if (bhopStatus)
                    {
                        newProcess(funcBhop);
                    }

                    result = bhopStatus ? "enabled✅" : "disabled❌";

                    await botClient.SendTextMessageAsync(msg.Chat.Id, $"Bunnyhop {result}");
                }
                else if (msg.Text == "Tasks kill💀")
                {
                    tasksKill();

                    await botClient.SendTextMessageAsync(msg.Chat.Id, "Успешно✅");
                }
                else if (msg.Text == "Settings⚙️")
                {
                    await botClient.SendTextMessageAsync(msg.Chat.Id, $"Wallhack settings: \n" +
                                                                      $"whSetRed - red color \n" +
                                                                      $"whSetGreen - green color \n" +
                                                                      $"whSetBlue - blue color \n" +
                                                                      $"whSetPink - pink color \n" +
                                                                      $"whSetYellow - yellow color \n");
                }
                if (msg.Text.Contains("whSet"))
                {
                    glowColor = msg.Text;
                    await botClient.SendTextMessageAsync(msg.Chat.Id, "Color set");
                }
            }
        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
