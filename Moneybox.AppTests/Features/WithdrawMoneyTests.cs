using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Features.Tests
{
    [TestClass()]
    public class WithdrawMoneyTests
    {
        [TestMethod()]
        public void ExecuteNegativeAmountTest()
        {
            var fromGuid = Guid.NewGuid();
            var toGuid = Guid.NewGuid();

            var fromAcct = new Account();
            fromAcct.Id = fromGuid;
            fromAcct.Balance = 1000;

            var toAcct = new Account();
            toAcct.Id = toGuid;
            toAcct.PaidIn = 0;

            decimal amountToWithdraw = -20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);
            accountRepo.Setup(m => m.GetAccountById(toGuid)).Returns(toAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            try
            {
                withdraw.Execute(fromGuid, toGuid, amountToWithdraw);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Negative or 0 withdrawals not allowed", ex.Message);
            }
        }

        [TestMethod()]
        public void ExecuteInsufficientFundsTest()
        {
            var fromGuid = Guid.NewGuid();
            var toGuid = Guid.NewGuid();

            var fromAcct = new Account();
            fromAcct.Id = fromGuid;
            fromAcct.Balance = 10;

            decimal amountToWithdraw = 20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            try
            {
                withdraw.Execute(fromGuid, toGuid, amountToWithdraw);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Insufficient funds to make withdrawal", ex.Message);
            }
        }

        [TestMethod()]
        public void ExecutePayInLimitTest()
        {
            var fromGuid = Guid.NewGuid();
            var toGuid = Guid.NewGuid();

            var fromAcct = new Account();
            fromAcct.Id = fromGuid;
            fromAcct.Balance = 1000;

            var toAcct = new Account();
            toAcct.Id = toGuid;
            toAcct.PaidIn = 5000;

            decimal amountToWithdraw = 20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);
            accountRepo.Setup(m => m.GetAccountById(toGuid)).Returns(toAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            try
            {
                withdraw.Execute(fromGuid, toGuid, amountToWithdraw);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Account pay in limit reached", ex.Message);
            }
        }

        [TestMethod()]
        public void ExecuteSuccessfulWithdrawTest()
        {
            var fromGuid = Guid.NewGuid();
            var toGuid = Guid.NewGuid();

            var fromAcct = new Account();
            fromAcct.Id = fromGuid;
            fromAcct.Balance = 1000;
            var toAcct = new Account();
            toAcct.Id = toGuid;
            toAcct.Balance = 0;

            decimal amountToWithdraw = 20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);
            accountRepo.Setup(m => m.GetAccountById(toGuid)).Returns(toAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            withdraw.Execute(fromGuid, toGuid, amountToWithdraw);

            Assert.AreEqual(fromAcct.Balance, 980);
            Assert.AreEqual(toAcct.Balance, amountToWithdraw);
        }
    }
}