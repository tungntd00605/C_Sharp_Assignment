using System;
using System.Collections.Generic;
using ConsoleApp3.model;
using MySql.Data.MySqlClient;
using SpringHeroBank.entity;
using SpringHeroBank.utility;

namespace SpringHeroBank.model
{
    public class TransactionModel
    {
        public bool Save(Transaction transaction)
        {
            // đảm bảo rằng đã kết nối đến db thành công.
            DbConnection.Instance().OpenConnection();
            
            var sqlQuery = "insert into `transactions` " +
                           "(`id`, `type`, `amount`, `content`, `senderAccountNumber`, `receiverAccountNumber`, `status`) values " +
                           "(@id, @type, @amount, @content, @senderAccountNumber, @receiverAccountNumber, @status)";
            var cmd = new MySqlCommand(sqlQuery, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@id", transaction.Id);
            cmd.Parameters.AddWithValue("@type", transaction.Type);
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@content", transaction.Content);
            cmd.Parameters.AddWithValue("@senderAccountNumber", transaction.SenderAccountNumber);
            cmd.Parameters.AddWithValue("@receiverAccountNumber", transaction.ReceiverAccountNumber);
            cmd.Parameters.AddWithValue("@status", transaction.Status);
            var result = cmd.ExecuteNonQuery();
            return result == 1;
        }

        public List<Transaction> GetTransactionHistory7Lastest(Account account)
        {
            List<Transaction> transactionList = new List<Transaction>();
            DbConnection.Instance().OpenConnection();
            var sqlQuery = "SELECT * FROM `transactions` WHERE (`senderAccountNumber` = @loggedInAccountNumber" +
                           " OR `receiverAccountNumber` = @loggedInAccountNumber) AND DATEDIFF(CURDATE(), `createdAt`) <=7" +
                           " ORDER BY `createdAt`";
            var cmd = new MySqlCommand(sqlQuery, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@loggedInAccountNumber", account.AccountNumber);
            var transReader = cmd.ExecuteReader();
            while (transReader.Read())
            {
                var id = transReader.GetString("id");
                var createdAt = transReader.GetString("createdAt");
                var updatedAt = transReader.GetString("updatedAt");
                var type = transReader.GetInt32("type");
                var amount = transReader.GetDecimal("amount");
                var content = transReader.GetString("content");
                var senderAccountNumber = transReader.GetString("senderAccountNumber");
                var receiverAccountNumber = transReader.GetString("receiverAccountNumber");
                var status = transReader.GetInt32("status");
                transactionList.Add(new Transaction(id, createdAt, updatedAt, (Transaction.TransactionType) type,
                    amount, content, senderAccountNumber, receiverAccountNumber, (Transaction.ActiveStatus) status));
            }
            transReader.Close();
            if (transactionList.Count > 0)
            {
                return transactionList;
            }

            return null;
        }
        
        public List<Transaction> GetAllTransactionHistory(Account account)
        {
            List<Transaction> transactionList = new List<Transaction>();
            DbConnection.Instance().OpenConnection();
            var sqlQuery = "SELECT * FROM `transactions` WHERE `senderAccountNumber` = @loggedInAccountNumber" +
                           " OR `receiverAccountNumber` = @loggedInAccountNumber ORDER BY `createdAt`";
            var cmd = new MySqlCommand(sqlQuery, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@loggedInAccountNumber", account.AccountNumber);
            var transReader = cmd.ExecuteReader();
            while (transReader.Read())
            {
                var id = transReader.GetString("id");
                var createdAt = transReader.GetString("createdAt");
                var updatedAt = transReader.GetString("updatedAt");
                var type = transReader.GetInt32("type");
                var amount = transReader.GetDecimal("amount");
                var content = transReader.GetString("content");
                var senderAccountNumber = transReader.GetString("senderAccountNumber");
                var receiverAccountNumber = transReader.GetString("receiverAccountNumber");
                var status = transReader.GetInt32("status");
                transactionList.Add(new Transaction(id, createdAt, updatedAt, (Transaction.TransactionType) type,
                    amount, content, senderAccountNumber, receiverAccountNumber, (Transaction.ActiveStatus) status));
            }
            transReader.Close();
            if (transactionList.Count > 0)
            {
                return transactionList;
            }

            return null;
        }
        
        public List<Transaction> GetTransactionBetweenTimeSpan(Account account, DateTime startTime, DateTime endTime)
        {
            List<Transaction> transactionList = new List<Transaction>();
            
            DbConnection.Instance().OpenConnection();
            var sqlQuery = "SELECT * FROM `transactions` WHERE (`senderAccountNumber` = @loggedInAccountNumber" +
                           " OR `receiverAccountNumber` = @loggedInAccountNumber) AND `createdAt` between @sTime and @eTime" +
                           " ORDER BY `createdAt`";
            var cmd = new MySqlCommand(sqlQuery, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@loggedInAccountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("sTime", startTime.ToString("yyyy-MM-dd 00:00:00"));
            cmd.Parameters.AddWithValue("eTime", endTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            var transReader = cmd.ExecuteReader();
            while (transReader.Read())
            {
                var id = transReader.GetString("id");
                var createdAt = transReader.GetString("createdAt");
                var updatedAt = transReader.GetString("updatedAt");
                var type = transReader.GetInt32("type");
                var amount = transReader.GetDecimal("amount");
                var content = transReader.GetString("content");
                var senderAccountNumber = transReader.GetString("senderAccountNumber");
                var receiverAccountNumber = transReader.GetString("receiverAccountNumber");
                var status = transReader.GetInt32("status");
                transactionList.Add(new Transaction(id, createdAt, updatedAt, (Transaction.TransactionType) type,
                    amount, content, senderAccountNumber, receiverAccountNumber, (Transaction.ActiveStatus) status));
            }
            transReader.Close();
            if (transactionList.Count > 0)
            {
                return transactionList;
            }

            return null;
        }
    }
}