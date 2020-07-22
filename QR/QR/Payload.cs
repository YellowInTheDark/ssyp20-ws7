using System;
using System.Collections.Generic;
using System.Text;

namespace QR
{
    class Payload
    {
        public static string UsePayload(int number)
        {
            string payload = string.Empty;
            switch (number)
            {
                case 1:
                    payload = Skype();
                    break;
                case 2:
                    payload = URL();
                    break;
                case 3:
                    payload = MailTo();
                    break;
                case 4:
                    payload = PhoneCall();
                    break;
                case 5:
                    payload = SMS();
                    break;
                case 6:
                    payload = Discord();
                    break;
            }

            return payload;
        }
        public static string Skype()
        {
            Console.WriteLine("Write Skype username to call");
            string input = Console.ReadLine();
            return $"skype:{input}?call";
        }

        public static string URL()
        {
            Console.WriteLine("Write url");
            string input = Console.ReadLine();
            return (input.StartsWith("http") ? input : "http://" + input);
        }
        public static string MailTo()
        {
            Console.WriteLine("Write e-mail");
            string input = Console.ReadLine();
            return $"mailto:{input}";
        }

        public static string PhoneCall()
        {
            Console.WriteLine("Write phone number (+79123456789)");
            string input = Console.ReadLine();
            return $"tel:{input}";
        }

        public static string SMS()
        {
            Console.WriteLine("Write phone number (+79123456789)");
            string input = Console.ReadLine();
            return $"sms:{input}";
        }
        public static string Discord()
        {
            Console.WriteLine("Write invitation URL");
            string input = Console.ReadLine();
            return (input.StartsWith("https://discord.gg/") ? input : "https://discord.gg/" + input);
        }
    }
}
