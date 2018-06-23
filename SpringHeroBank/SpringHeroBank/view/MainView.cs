using System;
using System.Collections.Generic;
using SpringHeroBank.controller;
using SpringHeroBank.entity;
using SpringHeroBank.utility;

namespace SpringHeroBank.view
{
    public class MainView
    {
        private static AccountController accController = new AccountController();
        private static TransactionController transController = new TransactionController();

        public static void GenerateMenu()
        {
            while (true)
            {
                if (Program.currentLoggedIn == null)
                {
                    GenerateGeneralMenu();
                }
                else
                {
                    GenerateCustomerMenu();
                }
            }
        }

        private static void GenerateTransactionMenu()
        {
            while (true)
            {
                Console.WriteLine("---------- TRANSACTION HISTORY MENU ----------");
                Console.WriteLine("1. History transaction of the 7 lastest day.");
                Console.WriteLine("2. All history transaction.");
                Console.WriteLine("3. Find history transaction in a timespan (dd/mm/yyyy - dd/mm/yyyy)");
                Console.WriteLine("4. Back to account menu");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3|4): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        transController.DisplayTransaction7LastestDays();
                        break;
                    case 2:
                        transController.DisplayAllTransaction();
                        break;
                    case 3:
                        transController.DisplayTransactionBetweenTimeSpan();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void GenerateCustomerMenu()
        {
            while (true)
            {
                Console.WriteLine("---------SPRING HERO BANK---------");
                Console.WriteLine("Welcome back: " + Program.currentLoggedIn.FullName);
                Console.WriteLine("1. Balance.");
                Console.WriteLine("2. Withdraw.");
                Console.WriteLine("3. Deposit.");
                Console.WriteLine("4. Transfer.");
                Console.WriteLine("5. Check history transactions");
                Console.WriteLine("6. Exit.");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3|4|5|6): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        accController.CheckBalance();
                        break;
                    case 2:
                        accController.Withdraw();
                        break;
                    case 3:
                        accController.Deposit();
                        break;
                    case 4:
                        accController.Transfer();
                        break;
                    case 5:
                        GenerateTransactionMenu();
                        break;
                    case 6:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void GenerateGeneralMenu()
        {
            while (true)
            {
                Console.WriteLine("---------WELCOME TO SPRING HERO BANK---------");
                Console.WriteLine("1. Register for free.");
                Console.WriteLine("2. Login.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        accController.Register();
                        break;
                    case 2:
                        if (accController.DoLogin())
                        {
                            Console.WriteLine("Login success.");
                        }

                        break;
                    case 3:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (Program.currentLoggedIn != null)
                {
                    break;
                }
            }
        }
    }
}