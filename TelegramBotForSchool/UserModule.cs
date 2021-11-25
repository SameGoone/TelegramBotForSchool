using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotForSchool
{
    class UserModule
    {
        public event Action<UserModule> OnServeringDone;
        public event Action<UserModule, long> OnServeringReset;
        public long UserId { get; private set; }

        private ITelegramBotClient client;
        private StateController stateController;
        private CategoryCollection categoryCollection;

        private Category chosenCategoty;
        private Document chosenDocument;

        public UserModule(long userId, ITelegramBotClient client)
        {
            UserId = userId;
            this.client = client;
            try
            {
                categoryCollection = CategoryCompiler.Singleton.GetCategories();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось загрузить документы из файла. Будет использоваться набор документов по-умолчанию. Причина: " + ex.Message);
                categoryCollection = Tools.GetDefaultCategoryCollection();
            }
        }

        public async Task SayHello(long chatId)
        {
            await SendMessage(chatId, "Приветствую! Вы можете заказать выписку документа в этом боте. " +
                "Для начала, выберите интересующую Вас категорию.\r\nЧтобы начать сначала, в любой момент нажмите на /start.", markup: Tools.GetCategoriesButtons(categoryCollection));
        }

        public async void ProcessMessage(Message message)
        {
            if (message.Text == "/start")
            {
                OnServeringReset?.Invoke(this, message.Chat.Id);
            }
            else if (stateController != null)
            {
                if (stateController.CurrentState != State.FINISH)
                {
                    stateController.CompleteState(message.Text);

                    if (stateController.CurrentState != State.FINISH)
                    {
                        await SendMessage(message.Chat.Id, Tools.GetRequestText(stateController.CurrentState) + Tools.AdditionalInfo);
                    }
                    else
                    {
                        await SendMessage(message.Chat.Id, "Подтвердите создание заявки.\r\n" + Tools.ConvertResultToString(stateController.Result),
                                           Tools.GetConfirmationButtons());
                    }
                }
            }
        }

        private async Task SendMessage(long chatId, string text, IReplyMarkup markup = null)
        {
            await client.SendTextMessageAsync(chatId, text, replyMarkup: markup);
        }

        public async void ProcessCallback(CallbackQuery callbackQuery)
        {
            string callbackText;
            string callBackTag;
            Tools.GetTagAndTextFromCallbackData(callbackQuery.Data, out callbackText, out callBackTag);

            if (callBackTag == Tools.CategoryTag)
            {
                chosenCategoty = categoryCollection.GetCategoryWithName(callbackText);

                await EditMessage(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    "Теперь выберите конкретный документ." + Tools.AdditionalInfo,
                    markup: Tools.GetDocumentsButtons(chosenCategoty));

                stateController = null;
            }
            else if (callBackTag == Tools.DocumentTag && chosenCategoty != null)
            {
                if (chosenCategoty.TryGetDocumentWithName(callbackText, out chosenDocument))
                {
                    stateController = new StateController(chosenCategoty.Name, chosenDocument);

                    await EditMessage(
                        callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId,
                        Tools.GetRequestText(stateController.CurrentState) + Tools.AdditionalInfo);
                }
                else
                {
                    OnServeringReset?.Invoke(this, callbackQuery.Message.Chat.Id);
                }
            }
            else if (callBackTag == Tools.ConfirmingTag && stateController != null && stateController.CurrentState == State.FINISH)
            {
                if (Tools.IsConfirmed(callbackText))
                {
                    TrySendMailNotification();
                    WriteResultInSpreadsheet();

                    await EditMessage(callbackQuery.Message.Chat.Id, 
                        callbackQuery.Message.MessageId, 
                        $"Заявка создана.\r\nЧтобы создать новую заявку, нажмите на /start.");

                    OnServeringDone?.Invoke(this);
                }
                else
                {
                    await EditMessage(callbackQuery.Message.Chat.Id, 
                        callbackQuery.Message.MessageId, 
                        $"Создание заявки отменено");

                    OnServeringReset?.Invoke(this, callbackQuery.Message.Chat.Id);
                }
            }
        }

        private async Task EditMessage(long chatId, int messageId, string newText, InlineKeyboardMarkup markup = null)
        {
            try
            {
                await client.EditMessageTextAsync(chatId, messageId, newText, replyMarkup: markup);
            }
            catch { }
        }

        private void TrySendMailNotification()
        {
            try
            {
                SendEMailNotification();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалость отправить е-майл. Причина:" + ex.Message);
            }
        }

        private void SendEMailNotification()
        {
            string result = Tools.ConvertResultToString(stateController.Result);
            string headEmail = stateController.HeadEmail;
            if (headEmail == Tools.mainHeadEMail) // Если ответственный - Жукова В.С, отправляется только ей
            {
                EmailController.Singleton.SendNotification(headEmail, result);
            }
            else // иначе - отправляется ответственному и Жуковой В.С
            {
                EmailController.Singleton.SendNotification(headEmail, result);
                EmailController.Singleton.SendNotification(Tools.mainHeadEMail, result);
            }
        }

        private void WriteResultInSpreadsheet()
        {
            try
            {
                var spreadsheetValues = SpreadsheetsController.Singleton.ReadSheetValues();
                var result = stateController.Result;
                var newRow = Tools.ConvertResultToSpreadsheetRow(result, UserId);
                spreadsheetValues.Add(newRow);
                SpreadsheetsController.Singleton.WriteSheetValues(spreadsheetValues);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось записать результаты в таблицу. Причина:" + ex.Message);
            }
        }
    }
}
