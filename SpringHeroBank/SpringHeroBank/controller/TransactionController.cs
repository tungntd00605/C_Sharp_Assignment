using System;
using System.Collections.Generic;
using System.Text;
using SpringHeroBank.entity;
using SpringHeroBank.model;
using SpringHeroBank.utility;

namespace SpringHeroBank.controller
{
    public class TransactionController
    {
        private TransactionModel model = new TransactionModel();
        private List<Transaction> transList = new List<Transaction>();

        public void DisplayTransaction7LastestDays()
        {
            transList = model.GetTransactionHistory7Lastest(Program.currentLoggedIn);
            WriteTransactionHistory(transList);
        }
        
        public void DisplayAllTransaction()
        {
            transList = model.GetAllTransactionHistory(Program.currentLoggedIn);
            WriteTransactionHistory(transList);
        }

        public void DisplayTransactionBetweenTimeSpan()
        {
            Console.WriteLine("Please enter the start searching time:");
            var startTime = Utility.GetDateTime();
            if (startTime > DateTime.Now)
            {
                Console.WriteLine("Error: The searching time can not be set in a future day");
                return;
            }
            DateTime endTime;
            Console.WriteLine("Press 'Y' to manual set the end searching time." +
                              " Press any other button will choose the current time as the end searching time...");
            if (Console.ReadKey().Key != ConsoleKey.Y)
            {
                endTime = DateTime.Now;
            }
            else
            {
                endTime = Utility.GetDateTime();
            }
            // Check the valid input time
            if (endTime > DateTime.Now)
            {
                Console.WriteLine("Error: The searching time can not be set in a future day");
                return;
            }

            if (startTime > endTime)
            {
                Console.WriteLine("Error: Start searching time can not set after the end searching time.");
                return;
            } 
            Console.WriteLine("Begin Time: {0} - End Time: {1}", startTime.ToString("dd-MM-yyyy"), endTime.ToString("dd-MM-yyyy"));
            transList = model.GetTransactionBetweenTimeSpan(Program.currentLoggedIn, startTime, endTime);
            WriteTransactionHistory(transList);
        }

        public void WriteTransactionHistory(List<Transaction> transactionList)
        {
            if (transactionList.Count > 0)
            {              
                Console.Out.Flush();
                Console.Clear();
                
                //
                StringBuilder printString = new StringBuilder();
                printString.AppendFormat("> Found {0} match result", transactionList.Count);
                printString.AppendFormat("\n|{0,25}|{1,36}|{2,16}|{3,30}|{4,26}|{5,15}|{6,30}|", "Created Time", "Transaction ID", "Transaction Type", "Sender account number", "Content", "Amount", "Receiver account Number");
                printString.AppendFormat("\n|{0,25}|{1,36}|{2,16}|{3,30}|{4,26}|{5,15}|{6,30}|",
                    "-------------------------", "------------------------------------", "---------------",
                    "------------------------------", "--------------------------", "---------------",
                    "------------------------------");
                foreach (var t in transactionList)
                {
                    if (t.Type == Transaction.TransactionType.DEPOSIT || t.Type == Transaction.TransactionType.WITHDRAW  || t.Type == Transaction.TransactionType.TRANSFER )
                    {
                       printString.AppendFormat("\n|{0,25}|{1,36}|{2,16}|{3,30}|{4,26}|{5,15}|{6,30}|", t.CreatedAt, t.Id, t.Type == Transaction.TransactionType.TRANSFER ? "TRANSFER" : (t.Type == Transaction.TransactionType.DEPOSIT ? "DEPOSIT" : "WITHDRAW"), t.SenderAccountNumber, t.Content, t.Amount.ToString(), t.ReceiverAccountNumber);
                    }
                }
                printString.AppendFormat("\n|{0,25}|{1,36}|{2,16}|{3,30}|{4,26}|{5,15}|{6,30}|",
                    "-------------------------", "------------------------------------", "---------------",
                    "------------------------------", "--------------------------", "---------------",
                    "------------------------------");
                Console.WriteLine(printString);
                
                // Ask whether the customer want to print transaction log
                Console.WriteLine("Press 'Y' if you want to export this transaction log into a txt file. Press other buttons to cancel this action...");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    try
                    {
                        System.IO.File.WriteAllText(@"e:\log\" + DateTime.Now.ToString("dd-mm-yyyy_HH-mm-ss") + "_TransactionLog.txt", printString.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    Console.WriteLine("Successfully created transaction log file");
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Can not find any transactions match with searching condition");
                Console.ReadLine();
            }
        }
    }
}