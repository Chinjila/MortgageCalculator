using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageCalculator
{
    public enum MortgageType
    {
        FixRate,
        AdjustableRate
    }

    // Create enum for 15 Year Mortgage and 30 Year Mortgage
    public enum MortgageDuration
    {
        FifteenYears = 180,
        ThirtyYears = 360
    }
}
