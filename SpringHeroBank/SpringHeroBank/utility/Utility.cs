using System;

namespace SpringHeroBank.utility
{
    public class Utility
    {
        // Đảm bảo người dùng nhập số.
        public static decimal GetUnsignDecimalNumber()
        {
            decimal choice;
            while (true)
            {
                try
                {
                    var strChoice = Console.ReadLine();
                    choice = Decimal.Parse(strChoice);
                    if (choice <= 0)
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please enter an unsign number.");
                }
            }

            return choice;
        }

        public static int GetInt32Number()
        {
            var choice = 0;
            while (true)
            {
                try
                {
                    var strChoice = Console.ReadLine();
                    choice = Int32.Parse(strChoice);
                    break;
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please enter a number.");
                }
            }

            return choice;
        }
        
        public static DateTime GetDateTime()
        {
            DateTime inputtedDate;
            while (true)
            {
                try
                {
                    Console.Write("Enter day: ");
                    int day = int.Parse(Console.ReadLine());
                    if (day < 1 || day > 31)
                    {
                        throw new Exception();
                    }
                    Console.Write("Enter month: ");
                    int month = int.Parse(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        throw new Exception();
                    }
                    Console.Write("Enter Year: ");
                    int year = int.Parse(Console.ReadLine());
                    if (year < 1)
                    {
                        throw new Exception();
                    }

                    inputtedDate = new DateTime(year, month, day);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please enter the correct date time");
                }
            }
            // Console.ReadLine();

            return inputtedDate;
        }
    }
}