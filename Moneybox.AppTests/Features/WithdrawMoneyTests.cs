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

            decimal amountToWithdraw = -20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            try
            {
                withdraw.Execute(fromGuid, amountToWithdraw);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Negative or 0 transfers not allowed", ex.Message);
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
                withdraw.Execute(fromGuid, amountToWithdraw);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Insufficient funds to make withdrawal", ex.Message);
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

            decimal amountToWithdraw = 20;

            var accountRepo = new Mock<IAccountRepository>();
            accountRepo.Setup(m => m.GetAccountById(fromGuid)).Returns(fromAcct);

            var notification = new Mock<INotificationService>();

            var withdraw = new WithdrawMoney(accountRepo.Object, notification.Object);

            withdraw.Execute(fromGuid, amountToWithdraw);

            Assert.AreEqual(fromAcct.Balance, 980);
        }
    }
}