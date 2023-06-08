using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotMethodsETC;

namespace BotTest
{
    public class Program
    {
        public static void Main()
        {
            TelegramBotStarter t = new TelegramBotStarter();
            t.Start();
            Console.ReadKey();
        }
    }
}
