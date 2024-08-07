using MortgageCalculator;
namespace MortgageCalculatorTest
{

    [TestClass]
    public class MortgageTest
    {
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void Construct_Mortgage_With_OriginationDate_In_The_Past_Should_ThrowException()
        {
            //arrange
            Mortgage m;
            //act
            m = new Mortgage(
                DateTime.Now.AddDays(-3),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            //assert
        }

        [TestMethod()]
        public void New_15Years_Mortgage_Should_Have_180_Payments()
        {
            //arrange - instantiate a new Mortgage with 15 Year
            //act - no need to act for this one
            //assert
            Mortgage m;

            m = new Mortgage(
                DateTime.Now.AddDays(5),
                MortgageDuration.FifteenYears,
                200000M,
                6.5M
                );
            Assert.AreEqual(180, m.Payments.Count);
        }

        [TestMethod()]
        public void New_30Years_Mortgage_Should_Have_360_Payments()
        {
            Mortgage m;
            //act
            m = new Mortgage(
                DateTime.Now.AddDays(5),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            Assert.AreEqual(360, m.Payments.Count);
        }
        [TestMethod()]
        public void Thirty_Years_Mortgage_Payments_Should_AddUp_To_Loan_Amount()
        {
            Mortgage m;
            //act
            m = new Mortgage(
                DateTime.Now.AddDays(5),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            Assert.AreEqual(Math.Round(m.OriginalPrincipalAmount),
                Math.Round(m.Payments.Sum(p => p.PrincipalAmount)));
        }
        [TestMethod()]
        public void Test_For_Principal_Over_Interest()
        {
            Mortgage m;
            //act
            m = new Mortgage(
                DateTime.Now.AddDays(5),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            Assert.AreEqual(233, m.WhichPaymentHasMorePincipalThanInterest().PaymentNumber);
        }

        [TestMethod()]
        public void Test_GetRemainingBalanceOnDate()
        {
            Mortgage m;
            //act
            m = new Mortgage(
                new DateTime(2050, 1, 1),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            Assert.AreEqual(169205.85M, m.GetRemainingBalanceOnDate(new DateTime(2060, 1, 1)));
        }

        [TestMethod()]
        public void Test_SortPrincipalAsPercentage()
        {
            //arrange
            Mortgage m;
            m = new Mortgage(
                new DateTime(2050, 1, 1),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );
            //act
            var payments = m.SortPrincipalAsPercentage();
            int stop = 75;
            Payment payment1=default , payment2=default;
            int loopCounter = 0;
            foreach (var item in payments)
            {
                if (loopCounter == stop - 1) { payment1 = item; }
                if (loopCounter == stop) { payment2 = item; }
                loopCounter++;
                //assert
               
            }
            Assert.IsTrue(payment1.PrincipalAmount / payment1.PaymentAmount < payment2.PrincipalAmount / payment2.PaymentAmount);

        }


        [TestMethod()]
        public void Test_GetYearlyAmort()
        {
            Mortgage m;
            //act
            m = new Mortgage(
                new DateTime(2050, 1, 1),
                MortgageDuration.ThirtyYears,
                200000M,
                6.5M
                );

            var result = m.GetYearlyAmortization();
            Assert.AreEqual(31, result.Count());
        }

        [TestMethod()]
        [DataRow(200000.00,360,6.5,1264.14)]
        [DataRow(100000.00, 180, 3, 690.58)]
        [DataRow(100000.00, 180, 6, 843.86)]
        public void VerifyPaymentAmount(double loanAmount,int durationInMonth,double interestRate,double expected)
        {
            //arrange
            Mortgage m;
            m = new Mortgage(
                new DateTime(2050, 1, 1),
                Convert.ToDecimal(loanAmount),
                durationInMonth,
                Convert.ToDecimal(interestRate)
                );
            //act

            Assert.AreEqual(Convert.ToDecimal(expected), m.Payments[0].PaymentAmount);

        }


    }
}