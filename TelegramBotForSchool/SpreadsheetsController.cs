using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramBotForSchool
{
    class SpreadsheetsController
    {
        public static SpreadsheetsController Singleton { get; private set; }
        public string SheetURL
        {
            get
            {
                return "https://docs.google.com/spreadsheets/d/" + spreadsheetId;
            }
        }

        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        SheetsService service;
        private const string spreadsheetId = "1NA6ptlvTQjPjgnPYmtQKrxe3x3fTIWQNRYHZiiZvAHA";
        private const string GoogleCredentialsFileName = "google-credentials.json";
        private const string range = "Лист1!A2:I";

        public SpreadsheetsController()
        {
            if (Singleton == null)
                Singleton = this;

            InitializeSheetsService();
        }

        private void InitializeSheetsService()
        {
            using (var stream = new FileStream(GoogleCredentialsFileName, FileMode.Open, FileAccess.Read))
            {
                var serviceInitializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(Scopes)
                };
                service = new SheetsService(serviceInitializer);
            }
        }

        public IList<IList<object>> ReadSheetValues()
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values == null)
                values = new List<IList<object>>();

            return values;
        }

        public void WriteSheetValues(IList<IList<object>> values)
        {
            ValueRange body = new ValueRange();
            body.Values = values;
            var update = service.Spreadsheets.Values.Update(body, spreadsheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var result = update.Execute();
            Console.WriteLine($"{result.UpdatedCells} cells updated.");
        }
    }
}
