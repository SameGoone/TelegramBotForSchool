using System;

namespace TelegramBotForSchool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CategoryCompiler compiler = new CategoryCompiler();
            SpreadsheetsController spreadsheetsController = new SpreadsheetsController();
            EmailController emailController = new EmailController();
            Tg_Bot bot = new Tg_Bot();
        }
    }
}
