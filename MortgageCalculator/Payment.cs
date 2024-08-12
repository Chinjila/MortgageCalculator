using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageCalculator
{
   
        public record struct Payment(int paymentNumber, decimal principalAmount, decimal interestAmount, decimal loanBalance, decimal payment, DateTime paymentDate)
        {
            public int PaymentNumber=> paymentNumber;
            public decimal PrincipalAmount => principalAmount;
            public decimal InterestAmount => interestAmount;
            public decimal LoanBalance => loanBalance;
            public decimal PaymentAmount => payment;
            public DateTime PaymentDate => paymentDate;
    }
    
}
