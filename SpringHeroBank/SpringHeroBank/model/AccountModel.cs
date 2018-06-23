using System;
using ConsoleApp3.model;
using MySql.Data.MySqlClient;
using SpringHeroBank.entity;
using SpringHeroBank.error;
using SpringHeroBank.utility;

namespace SpringHeroBank.model
{
    public class AccountModel
    {
        public Boolean Save(Account account)
        {
            DbConnection.Instance().OpenConnection(); // đảm bảo rằng đã kết nối đến db thành công.
            var salt = Hash.RandomString(7); // sinh ra chuỗi muối random.
            account.Salt = salt; // đưa muối vào thuộc tính của account để lưu vào database.
            // mã hoá password của người dùng kèm theo muối, set thuộc tính password mới.
            account.Password = Hash.GenerateSaltedSHA1(account.Password, account.Salt);
            var sqlQuery = "insert into `accounts` " +
                           "(`username`, `password`, `accountNumber`, `identityCard`, `balance`, `phone`, `email`, `fullName`, `salt`) values" +
                           "(@username, @password, @accountNumber, @identityCard, @balance, @phone, @email, @fullName, @salt)";
            var cmd = new MySqlCommand(sqlQuery, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", account.Username);
            cmd.Parameters.AddWithValue("@password", account.Password);
            cmd.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("@identityCard", account.IdentityCard);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@phone", account.Phone);
            cmd.Parameters.AddWithValue("@email", account.Email);
            cmd.Parameters.AddWithValue("@fullName", account.FullName);
            cmd.Parameters.AddWithValue("@salt", account.Salt);
            var result = cmd.ExecuteNonQuery();
            DbConnection.Instance().CloseConnection();
            return result == 1;
        }

        public decimal GetCurrentBalanceByAccountNumber(string accountNumber)
        {
            DbConnection.Instance().OpenConnection(); // đảm bảo rằng đã kết nối đến db thành công.
            decimal currentBalance;
            var queryBalance = "select balance from `accounts` where accountNumber = @accountNumber and status = 1";
            MySqlCommand queryBalanceCommand = new MySqlCommand(queryBalance, DbConnection.Instance().Connection);
            queryBalanceCommand.Parameters.AddWithValue("@accountNumber", accountNumber);
            var balanceReader = queryBalanceCommand.ExecuteReader();
            // Không tìm thấy tài khoản tương ứng, throw lỗi.
            if (!balanceReader.Read())
            {
                // Không tồn tại bản ghi tương ứng, lập tức rollback transaction, trả về false.
                // Hàm dừng tại đây.
                balanceReader.Close();
                throw new SpringHeroTransactionException("This account's balance is not valid");
            }

            // Đảm bảo sẽ có bản ghi.
            currentBalance = balanceReader.GetDecimal("balance");
            balanceReader.Close();
            return currentBalance;
        }

        public bool UpdateBalance(Account account, Transaction historyTransaction)
        {
            DbConnection.Instance().OpenConnection(); // đảm bảo rằng đã kết nối đến db thành công.
            var transaction = DbConnection.Instance().Connection.BeginTransaction(); // Khởi tạo transaction.

            try
            {
                /**
                 * 1. Lấy thông tin số dư mới nhất của tài khoản.
                 * 2. Kiểm tra kiểu transaction. Chỉ chấp nhận deposit và withdraw.
                 *     2.1. Kiểm tra số tiền rút nếu kiểu transaction là withdraw.                 
                 * 3. Update số dư vào tài khoản.
                 *     3.1. Tính toán lại số tiền trong tài khoản.
                 *     3.2. Update số tiền vào database.
                 * 4. Lưu thông tin transaction vào bảng transaction.
                 */

                // 1. Lấy thông tin số dư mới nhất của tài khoản.
//                kiem tra tinh trang cua tai khoan
                var currentBalance = GetCurrentBalanceByAccountNumber(account.AccountNumber);

                // 2. Kiểm tra kiểu transaction. Chỉ chấp nhận deposit và withdraw. 
                if (historyTransaction.Type != Transaction.TransactionType.DEPOSIT
                    && historyTransaction.Type != Transaction.TransactionType.WITHDRAW)
                {
                    throw new SpringHeroTransactionException("Invalid transaction type!");
                }

                // 2.1. Kiểm tra số tiền rút nếu kiểu transaction là withdraw.
                if (historyTransaction.Type == Transaction.TransactionType.WITHDRAW &&
                    historyTransaction.Amount > currentBalance)
                {
                    throw new SpringHeroTransactionException("You dont have enough money to do this transaction!");
                }

                // 3. Update số dư vào tài khoản.
                // 3.1. Tính toán lại số tiền trong tài khoản.
                if (historyTransaction.Type == Transaction.TransactionType.DEPOSIT)
                {
                    currentBalance += historyTransaction.Amount;
                }
                else
                {
                    currentBalance -= historyTransaction.Amount;
                }

                // 3.2. Update số dư vào database.
                var updateAccountResult = 0;
                var queryUpdateAccountBalance =
                    "update `accounts` set balance = @balance where username = @username and status = 1";
                var cmdUpdateAccountBalance =
                    new MySqlCommand(queryUpdateAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@username", account.Username);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@balance", currentBalance);
                updateAccountResult = cmdUpdateAccountBalance.ExecuteNonQuery();

                // 4. Lưu thông tin transaction vào bảng transaction.
                var transModel = new TransactionModel();
                if (updateAccountResult == 1 && transModel.Save(historyTransaction))
                {
                    transaction.Commit();
                    return true;
                }
            }
            catch (SpringHeroTransactionException e)
            {
                transaction.Rollback();
                return false;
            }

            DbConnection.Instance().CloseConnection();
            return false;
        }

        public bool UpdateTransferBalance(Account account, Transaction historyTransaction, string receiverAccountNumber)
        {
            DbConnection.Instance().OpenConnection(); // đảm bảo rằng đã kết nối đến db thành công.
            var transaction = DbConnection.Instance().Connection.BeginTransaction(); // Khởi tạo transaction.
            try
            {
                /**
                 * 1. Lấy thông tin số dư mới nhất của tài khoản ng gửi.
                 * 2. Kiểm tra kiểu transaction có phải TRANSFER hay không và kiểm tra số tiền rút.             
                 * 3. Update số dư vào tài khoản ng gửi.
                 *     3.1. Tính toán lại số tiền trong tài khoản.
                 *     3.2. Update số tiền vào database.
                 * 4. Lưu thông tin transaction vào bảng transaction.
                 */

                // 1. Lấy thông tin số dư mới nhất của tài khoản ng gửi.
//                kiem tra tinh trang cua tai khoan
                var currentSenderBalance = GetCurrentBalanceByAccountNumber(account.AccountNumber);
                
                // 2. Kiểm tra kiểu transaction có phải TRANSFER hay không và kiểm tra số tiền rút.    
                if (historyTransaction.Type != Transaction.TransactionType.TRANSFER)
                {
                    throw new SpringHeroTransactionException("Invalid transaction type");
                }else if (historyTransaction.Amount > currentSenderBalance)
                {
                    throw new SpringHeroTransactionException("You dont have enough money to do this transaction!");
                }
                
                // 3. Update số dư vào tài khoản ng gửi.
                // 3.1. Tính toán lại số tiền trong tài khoản.
                currentSenderBalance -= historyTransaction.Amount;

                // 3.2. Update số dư tài khoảng ng gửi vào database.
                var updateSenderAccountResult = 0;
                var querySenderUpdateAccountBalance =
                    "update `accounts` set balance = @balance where username = @username and status = 1";
                var cmdUpdateSenderAccountBalance =
                    new MySqlCommand(querySenderUpdateAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateSenderAccountBalance.Parameters.AddWithValue("@username", account.Username);
                cmdUpdateSenderAccountBalance.Parameters.AddWithValue("@balance", currentSenderBalance);
                updateSenderAccountResult = cmdUpdateSenderAccountBalance.ExecuteNonQuery();
                
                // 4. Lấy thông tin số dư mới nhất của tài khoản ng nhận.
                var currentReceiverBalance = GetCurrentBalanceByAccountNumber(receiverAccountNumber);
                // 4.1 Update số dư vào tài khoản ng nhận.
                // Tính toán lại số tiền trong tài khoản.
                currentReceiverBalance += historyTransaction.Amount;
                
                // Update số dư tài khoảng ng nhận vào database.
                var updateReciverAccountResult = 0;
                var queryUpdateReceiverAccountBalance =
                    "update `accounts` set balance = @balance where accountNumber = @accountNumber and status = 1";
                var cmdUpdateReceiverAccountBalance =
                    new MySqlCommand(queryUpdateReceiverAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateReceiverAccountBalance.Parameters.AddWithValue("@accountNumber", receiverAccountNumber);
                cmdUpdateReceiverAccountBalance.Parameters.AddWithValue("@balance", currentReceiverBalance);
                updateReciverAccountResult = cmdUpdateReceiverAccountBalance.ExecuteNonQuery();

                // 5. Lưu thông tin transaction vào bảng transaction.
                var transModel = new TransactionModel();
                if (updateSenderAccountResult == 1 && updateReciverAccountResult == 1 && transModel.Save(historyTransaction))
                {
                    transaction.Commit();
                    return true;
                }
            }
            catch (SpringHeroTransactionException e)
            {
                transaction.Rollback();
                return false;
            }

            DbConnection.Instance().CloseConnection();
            return false;
        }

        public Account GetAccountByAccountNumber(string accNumber)
        {
            DbConnection.Instance().OpenConnection();
            var queryString = "select * from  `accounts` where accountNumber = @accountNumber and status = 1";
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@accountNumber", accNumber);
            var reader = cmd.ExecuteReader();
            Account account = null;
            if (reader.Read())
            {
                var _username = reader.GetString("username");
                var password = reader.GetString("password");
                var salt = reader.GetString("salt");
                var accountNumber = reader.GetString("accountNumber");
                var identityCard = reader.GetString("identityCard");
                var balance = reader.GetDecimal("balance");
                var phone = reader.GetString("phone");
                var email = reader.GetString("email");
                var fullName = reader.GetString("fullName");
                var createdAt = reader.GetString("createdAt");
                var updatedAt = reader.GetString("updatedAt");
                var status = reader.GetInt32("status");
                account = new Account(_username, password, salt, accountNumber, identityCard, balance, phone, email,
                    fullName, createdAt, updatedAt, (Account.ActiveStatus) status);
            }

            DbConnection.Instance().CloseConnection();
            return account;
        }
        
        public Account GetAccountByUserName(string username)
        {
            DbConnection.Instance().OpenConnection();
            var queryString = "select * from  `accounts` where username = @username and status = 1";
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
            Account account = null;
            if (reader.Read())
            {
                var _username = reader.GetString("username");
                var password = reader.GetString("password");
                var salt = reader.GetString("salt");
                var accountNumber = reader.GetString("accountNumber");
                var identityCard = reader.GetString("identityCard");
                var balance = reader.GetDecimal("balance");
                var phone = reader.GetString("phone");
                var email = reader.GetString("email");
                var fullName = reader.GetString("fullName");
                var createdAt = reader.GetString("createdAt");
                var updatedAt = reader.GetString("updatedAt");
                var status = reader.GetInt32("status");
                account = new Account(_username, password, salt, accountNumber, identityCard, balance, phone, email,
                    fullName, createdAt, updatedAt, (Account.ActiveStatus) status);
            }

            DbConnection.Instance().CloseConnection();
            return account;
        }
    }
}