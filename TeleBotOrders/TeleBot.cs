using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace TeleBotOrders
{
    internal class TeleBot
    {
        public string Token { get; set; }
        private ITelegramBotClient _bot;
        public TeleBot() 
        {
            Token = "5773151578:AAH2GEcv7Ey3LKcnM_Z0lJpmH0NiXf1Dttk";
            _bot = new TelegramBotClient(Token);

        }
        public void Start() 
        {
            Console.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);
            Debug.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
        public void Stop()
        {
            
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ex = JsonSerializer.Serialize(exception);
            Console.WriteLine(ex);
            Debug.WriteLine(ex);
        }

    }
}
