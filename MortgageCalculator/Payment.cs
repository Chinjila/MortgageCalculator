using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageCalculator
{
   
        public record struct Payment(int paymentNumber, decimal rincipalAmount, decimal interestAmount, decimal loanBalance, decimal payment, DateTime paymentDate)
        {
         
    }
    
}
