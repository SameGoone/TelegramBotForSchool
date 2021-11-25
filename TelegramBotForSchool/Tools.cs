using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotForSchool
{
    class Tools
    {
        public const string AdditionalInfo = "\r\nЧтобы вернуться в самое начало, нажмите на /start.";
        public const int UserIdColumnNumber = 8;
        public static string mainHeadEMail;
        private static string email1;
        private static string email2;
        private static string fileName = "head-emails.txt";

        public const string CategoryTag = "category";
        public const string DocumentTag = "clarification";
        public const string ConfirmingTag = "confirming";
        private static readonly string[] ConfirmingCallbacks = new string[] { "Создать", "Отменить" };

        private const string textAndTagSeparator = "|||";

        static Tools()
        {
            ReadEmailsFromFile();
        }

        private static void ReadEmailsFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    mainHeadEMail = sr.ReadLine();
                    email1 = sr.ReadLine();
                    email2 = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Category category1 = new Category("Справка", new List<Document> 
        { 
            new Document("2 НДФЛ", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V, State.YEAR, State.NUMBER_OF_COPIES }, email1),
            new Document("В соц. службы", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V }, email1),
            new Document("На визу", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V }, email2),
            new Document("О работе в ОО", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V, State.YEAR, State.NUMBER_OF_COPIES }, email2),
            new Document("В ПФР по выслуге лет", new List<State> { State.FIO, State.POSITION }, email2)
        }); 
        
        private static Category category2 = new Category("Копия ТК/Форма СТД-Р", new List<Document>
        {
            new Document("Заверенная копия ТК", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V, State.NUMBER_OF_COPIES }, email2),
            new Document("Форма СТД-Р (если ЭТК)", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V, State.NUMBER_OF_COPIES }, email1)
        }); 
        
        private static Category category3 = new Category("Выписка из ТК", new List<Document>
        {
            new Document("По форме", new List<State> { State.FIO, State.POSITION, State.PREDOSTAVLENIE_V, State.NUMBER_OF_COPIES }, email2)
        });

        private static Category category4 = new Category("Расчётный листок по З/П", new List<Document>
        {
            new Document("Расчётный листок", new List<State> { State.FIO, State.POSITION }, email1),
            new Document("Расшифровка З/П", new List<State> { State.FIO, State.POSITION }, mainHeadEMail)
        });

        private static CategoryCollection CategoryCollection = new CategoryCollection(new List<Category> { category1, category2, category3, category4 });

        public static CategoryCollection GetDefaultCategoryCollection()
        {
            return CategoryCollection;
        }

        public static InlineKeyboardMarkup GetCategoriesButtons(CategoryCollection categoryCollection)
        {
            return GetButtons(categoryCollection.GetCategoriesNames(), CategoryTag);
        }

        public static InlineKeyboardMarkup GetDocumentsButtons(Category category)
        {
            return GetButtons(category.GetDocumentsNames(), DocumentTag);
        }

        public static InlineKeyboardMarkup GetConfirmationButtons()
        {
            return GetButtons(ConfirmingCallbacks, ConfirmingTag);
        }

        private static InlineKeyboardMarkup GetButtons(string[] buttonTexts, string tag)
        {
            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();
            keyboard.Add(new List<InlineKeyboardButton>());
            foreach (string text in buttonTexts)
            {
                keyboard.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(text, $"{text}{textAndTagSeparator}{tag}") });
            }
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(keyboard);
            return inlineKeyboardMarkup;
        }

        public static void GetTagAndTextFromCallbackData(string callbackData, out string text, out string tag)
        {
            string[] textAndTag = callbackData.Split(new string[] { textAndTagSeparator }, StringSplitOptions.None);
            text = textAndTag[0];
            tag = textAndTag[1];
        }

        public static bool IsConfirmed(string callbackText)
        {
            if (callbackText == ConfirmingCallbacks[0])
                return true;
            else
                return false;
        }

        public static string GetRequestText(State state)
        {
            string requestText = "";
            switch (state)
            {
                case State.FIO:
                    requestText = "Необходимо узнать о вас некоторую информацию, чтобы мы могли составить необходимые вам документы!\r\nНапишите, пожалуйста, ваши ФИО.";
                    break;
                case State.POSITION:
                    requestText = "Укажите, пожалуйста, вашу должность.";
                    break;
                case State.PREDOSTAVLENIE_V:
                    requestText = "Для предоставления в какое учреждение вам необходим данный документ?";
                    break;
                case State.YEAR:
                    requestText = "За какой календарный год работы вам необходимо составить данный документ?";
                    break;
                case State.NUMBER_OF_COPIES:
                    requestText = "Сколько экземпляров вам понадобится?";
                    break;
                case State.ADDITIONAL_INFO:
                    requestText = "Почти готово! Сейчас вы можете написать свои пожелания или комментарии для специалистов школы, которые будут готовить необходимые вам документы.\r\nЕсли их нет, то отправьте любой символ.";
                    break;
            }
            return requestText;
        }

        public static string GetStateName(State state)
        {
            string name = "";
            switch (state)
            {
                case State.CATEGORY:
                    name = "Тип документа";
                    break;
                case State.CLARIFICATION:
                    name = "Документ";
                    break;
                case State.FIO:
                    name = "ФИО";
                    break;
                case State.POSITION:
                    name = "Должность";
                    break;
                case State.PREDOSTAVLENIE_V:
                    name = "Для предоставления в";
                    break;
                case State.YEAR:
                    name = "За год";
                    break;
                case State.NUMBER_OF_COPIES:
                    name = "Количество копий";
                    break;
                case State.ADDITIONAL_INFO:
                    name = "Доп. информация";
                    break;
            }
            return name;
        }

        public static IList<object> ConvertResultToSpreadsheetRow(List<StateValue> result, long userId)
        {
            List<object> row = new List<object>();
            foreach (StateValue value in result)
            {
                AddInfoInRow(value.ColumnNumber, value.Value, row);
            }
            AddInfoInRow(UserIdColumnNumber, userId.ToString(), row);
            return row;
        }

        private static void AddInfoInRow(int columnIndex, string info, List<object> row)
        {
            while (columnIndex >= row.Count)
            {
                row.Add("");
            }
            row[columnIndex] = info;
        }

        public static string ConvertResultToString(List<StateValue> result)
        {
            StringBuilder resultInfo = new StringBuilder();
            foreach (StateValue value in result)
            {
                resultInfo.Append($"{value.Name}: {value.Value}\r\n");
            }
            return resultInfo.ToString();
        }
    }
}
