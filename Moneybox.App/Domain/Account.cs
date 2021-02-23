using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void VerifyWithdrawalPermitted(decimal withdrawalAmount)
        {
            // Do not allow negative withdrawals at all.
            if (withdrawalAmount <= 0)
            {
                throw new Exception("Negative or 0 transactions not allowed");
            }

            // Check account has enough balance.
            var fromBalance = Balance - withdrawalAmount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to complete transaction");
            }
        }

        public void VerifyPayInPermitted(decimal payInAmount)
        {
            // Do not allow negative pay ins at all.
            if (payInAmount <= 0)
            {
                throw new Exception("Negative or 0 transactions not allowed");
            }

            // Check account has enough balance.
            var paidIn = PaidIn + payInAmount;
            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }

        /// <summary>
        /// This will check and send either notification for this account
        /// on any transaction in any direction: probably what we want, speak with
        /// product owner.
        /// </summary>
        public void CheckAndSendNotifications(INotificationService notificationService)
        {
            if (Balance < 500m)
            {
                notificationService.NotifyFundsLow(User.Email);
            }

            if (Account.PayInLimit - PaidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(User.Email);
            }
        }
    }
}
