using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageCalculator
{
    public class Mortgage
    {
        public DateTime OriginationDate;
        public decimal OriginalPrincipalAmount;
        public decimal CurrentPrincipal;
        public decimal OriginalInterestRate;

        public int LoanDuration { get; }

        public MortgageType MortgageType;
        public List<Payment> Payments;

        public Payment CalculateMonthlyPayment(DateTime maturityDate)
        { throw new NotImplementedException(); }
        public Payment CalculateMonthlyPayment(int numberOfPayments)
        { throw new NotImplementedException(); }

        public List<Payment> GetAmortizationSchedule(DateTime maturityDate)
        { throw new NotImplementedException(); }
        public List<Payment> GetAmortizationSchedule(int numberOfPayments)
        { throw new NotImplementedException(); }
        
        //create a constructor that accept the origination date and an enum
        // for 15 or 30 year, throw exception if origination < today - in the past
        public Mortgage(DateTime mortgageOriginationDate, MortgageDuration duration, Decimal originalLoanAmount, Decimal originalInterestRateInPercentage):
        this(mortgageOriginationDate, originalLoanAmount, (int)duration, originalInterestRateInPercentage)
        {
            ;
        }
        public Mortgage(DateTime mortgageOriginationDate,  Decimal originalLoanAmount, int durationInMonth, Decimal originalInterestRateInPercentage)
        {
            if (mortgageOriginationDate < System.DateTime.Now.AddDays(-3))
            {
                throw new Exception("Mortgage can not be originated in the past.");
            }
            if (originalLoanAmount <= 0 | originalInterestRateInPercentage <= 0) throw new ArgumentOutOfRangeException();
            // right click solution -> Add -> New Project -> search for MSTest Project
            // Name the project MortgageLibTest
            // delete UnitTest1.cs

            // right click on MortgageLibTest, select Add -> Project Reference so we can
            // instantiate the Mortgage class.
            // 
            // Right click on MortgageLibTest, add->class
            // make sure to call the test class file [classUnderTest]test.cs
            // referece the content of
            // https://github.com/Chinjila/ClassProjects/blob/51cd7400fbdac472d6bc2380dcbea2043203340b/MortgageLibTest/MortgageTest.cs

            // top menu-> Test -> Run All Tests
            Payments = new List<Payment>();
            this.OriginalPrincipalAmount = originalLoanAmount;
            this.OriginalInterestRate = originalInterestRateInPercentage;
            this.LoanDuration = durationInMonth;
            this.OriginationDate = mortgageOriginationDate;
            var payment = this.calculateMonthlyPayment(durationInMonth);
            var loanBalance = this.OriginalPrincipalAmount;

            for (int i = 0; i < durationInMonth; i++)
            {
                var paymentNumber = i + 1;
                var interestAmount = loanBalance * this.OriginalInterestRate / 1200;
                var principalAmount = payment - interestAmount;
                if (paymentNumber == durationInMonth)
                {
                    payment = loanBalance;
                    principalAmount = loanBalance;
                }
                loanBalance -= principalAmount;

                Payments.Add(
                    new Payment(paymentNumber,
                                principalAmount,
                                interestAmount,
                                loanBalance,
                                payment,
                                OriginationDate.AddMonths(paymentNumber)));
                //added payment constructor parameter to include payment date
            }

        }

        public Decimal GetRemainingBalanceOnDate(DateTime onDate)
        {
            if (onDate <= OriginationDate | onDate > OriginationDate.AddMonths((int)LoanDuration))
            { throw new ArgumentOutOfRangeException("Date must be with in the range of the Mortgage."); }
            // use DateTime.AddMonth to LoanOriginationDate to figure out payment date
            return Math.Round(
                Payments.Where(p => p.PaymentDate > onDate).First().LoanBalance, 2);
        }

        public Payment WhichPaymentHasMorePincipalThanInterest()
        {
            return Payments.Where(p => p.PrincipalAmount > p.InterestAmount).First();
            // return the payment which applies more principal than interest
            // please add "PaymentDate" property to Payment.cs
        }
        public Decimal GetTotalInterestPaidOnDate(DateTime onDate) //this return total interest paid on given date
        {
            if (onDate <= OriginationDate | onDate > OriginationDate.AddMonths((int)LoanDuration))
            { throw new ArgumentOutOfRangeException("Date must be with in the range of the Mortgage."); }
            return Math.Round(Payments.Where(p => p.PaymentDate < onDate).Sum(p => p.InterestAmount), 2);
        }
        public IEnumerable<Payment> SortPrincipalAsPercentage()
        {
            return Payments.OrderBy(p => p.PrincipalAmount / p.PaymentAmount).Select(p => p);
        }
        public IEnumerable<YearlyPayment> GetYearlyAmortization() //this return total interest paid on given date
        {
            var result = Payments.GroupBy(p => p.PaymentDate.Year).Select(
                g => new YearlyPayment(g.Key, g.Sum(p => p.PrincipalAmount), g.Sum(p => p.InterestAmount)));


            return result;

        }




        internal Decimal calculateMonthlyPayment(int numberOfPayment)
        {
            double r = (double)(this.OriginalInterestRate / 1200);
            double p = (double)OriginalPrincipalAmount;
            double monthlyPayment = r * p * (Math.Pow(1 + r, numberOfPayment)) / (Math.Pow(1 + r, numberOfPayment) - 1);

            return (decimal)Math.Round(monthlyPayment, 2);
        }
    }

    

    public class YearlyPayment(int year, decimal totalPrincipal, decimal totalInterest)
    {
        public int Year => year;
        public decimal TotalPrincipal => totalPrincipal;
        public decimal TotalInterest => totalInterest;




        public override bool Equals(object? obj)
        {
            return obj is YearlyPayment other &&
                   Year == other.Year &&
                   TotalPrincipal == other.TotalPrincipal &&
                   TotalInterest == other.TotalInterest;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, TotalPrincipal, TotalInterest);
        }
    }
}
