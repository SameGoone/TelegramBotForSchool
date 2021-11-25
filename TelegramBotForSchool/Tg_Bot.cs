using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using System.IO;

namespace TelegramBotForSchool
{
    public enum State
    {
        CATEGORY = 0,
        CLARIFICATION = 1,
        FIO = 2,
        POSITION = 3,
        PREDOSTAVLENIE_V = 4,
        YEAR = 5,
        NUMBER_OF_COPIES = 6,
        ADDITIONAL_INFO = 7,
        FINISH
    }

    internal class Tg_Bot
    {
        private string token;
        private string configFileName = "bot-token.txt";
        private ITelegramBotClient client;
        private Dictionary<long, UserModule> serveringUsers = new Dictionary<long, UserModule>();

        public Tg_Bot()
        {
            ReadConfigsFromFile();
            client = new TelegramBotClient(token);
            Console.WriteLine("Bot initialized");
            var cts = new CancellationTokenSource();

            client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            Console.WriteLine($"Start listening for bot");

            while (true)
                Console.ReadLine();

            cts.Cancel();
        }

        private void ReadConfigsFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(configFileName))
                {
                    token = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
            {
                await ServeMessage(update.Message);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await ServeCallbackQuery(update.CallbackQuery);
            }
        }

        private async Task ServeMessage(Message message)
        {
            OutputMessageInfo(message);

            var userId = message.From.Id;
            if (serveringUsers.ContainsKey(userId))
            {
                serveringUsers[userId].ProcessMessage(message);
            }
            else
            {
                await StartUserServeringAsync(userId, message.Chat.Id);
            }
        }

        private async Task ServeCallbackQuery(CallbackQuery callbackQuery)
        {
            OutputCallbackInfo(callbackQuery);

            var userId = callbackQuery.From.Id;
            if (serveringUsers.ContainsKey(userId))
            {
                serveringUsers[userId].ProcessCallback(callbackQuery);
            }
            else
            {
                await StartUserServeringAsync(userId, callbackQuery.Message.Chat.Id);
            }
        }

        private async Task StartUserServeringAsync(long userId, long chatId)
        {
            UserModule newUser = new UserModule(userId, client);
            newUser.OnServeringReset += User_OnServeringReset;
            newUser.OnServeringDone += User_OnServeringDone;
            serveringUsers.Add(userId, newUser);
            await newUser.SayHello(chatId);
        }

        private async void User_OnServeringReset(UserModule sender, long chatId)
        {
            serveringUsers.Remove(sender.UserId);
            await StartUserServeringAsync(sender.UserId, chatId);
        }

        private void User_OnServeringDone(UserModule sender)
        {
            serveringUsers.Remove(sender.UserId);
        }

        private void OutputCallbackInfo(CallbackQuery callbackQuery)
        {
            Console.WriteLine($"Пользователь {callbackQuery.Message.Chat.FirstName} {callbackQuery.Message.Chat.LastName} нажал кнопку {callbackQuery.Data}");
        }

        private void OutputMessageInfo(Message message)
        {
            Console.WriteLine($"Пользователь {message.From.FirstName} {message.From.LastName} прислал сообщение: {message.Text}");
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
