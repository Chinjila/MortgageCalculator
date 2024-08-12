using MortgageCalculator;
using Spectre.Console;
using System.CommandLine;
using System.ComponentModel.Design;
namespace MortgageCommandLine
{
    internal class Program
    {
        static bool isBatch=false;
        static async Task Main(string[] args)
        {

            var rootCommand = new RootCommand("Mortgage Calculator Console App");

            var interactiveCommand = new Command("interactive", "Run calculator interactively.");
            
            var batchCommand = new Command("batch", "Run calculator in batch mode.");
            var loanOption = new Option<decimal?>(
                  name: "--loan",
                  description: "the loan amount requested.",
                  getDefaultValue: () => 200000m);
            loanOption.AddAlias("-l");
            var interestOption = new Option<decimal?>(
                  name: "--interest",
                  description: "the interest rate express in percentage, ex. 5.5 or 6.2",
                  getDefaultValue: () => 6m);
            interestOption.AddAlias("-i");
            var durationOption = new Option<int?>(
                  name: "--duration",
                  description: "loan duration specified in years. default to 15",
                  getDefaultValue: () => 15
                  );
            durationOption.AddAlias("-d");

            batchCommand.Add(loanOption);
            batchCommand.Add(interestOption);
            batchCommand.Add(durationOption);


            rootCommand.AddCommand(interactiveCommand);
            rootCommand.AddCommand(batchCommand);

            batchCommand.SetHandler((loanOption, interestOption, durationOption)
                => { RunBatch(loanOption!.Value, interestOption!.Value, durationOption!.Value); },
                loanOption,
                interestOption,
                durationOption)
                ;
            interactiveCommand.SetHandler(() => RunInteractive());
            await rootCommand.InvokeAsync(args);

            return;
        }

        private static void RunBatch(decimal loanOption, decimal interestOption, int durationOption)
        {
            isBatch = true;
            PrintTable(loanOption, interestOption, durationOption);
        }

        private static void RunInteractive()
        {
            decimal loanAmount = AnsiConsole.Ask<decimal>("How much is the [green]loan amount:[/]?", 100000);
            decimal interest = AnsiConsole.Ask<decimal>("What is the [green]annualized interest rate[/] in percentage?", 5);
            int durationInYear = AnsiConsole.Ask<int>("How many [green]years[/] is the loan for?", 10);
            PrintTable(loanAmount, interest, durationInYear);
        }

        private static void PrintTable(decimal loanAmount, decimal interest, int durationInYear)
        {
            Mortgage m = new Mortgage(mortgageOriginationDate: DateTime.Now,
                                                  originalLoanAmount: loanAmount,
                                                  durationInMonth: durationInYear * 12,
                                                  originalInterestRateInPercentage: interest);
            // Create a table
            if (isBatch)
            {
                foreach (var p in m.Payments)
                {
                    Console.WriteLine($"{p.PaymentNumber.ToString("n0")}," +
                        $"{p.PaymentAmount.ToString("F2")}," +
                        $"{p.PrincipalAmount.ToString("F2")}," +
                        $"{p.InterestAmount.ToString("F2")}," +
                        $"{p.paymentDate.ToShortDateString()}" +
                        $",{p.LoanBalance.ToString("F2")}");
                }

            }
            else
            {
                int currentPage = 0;
                int pageSize = 20;
                int recordNumber = m.Payments.Count;
                int numberOfPages = recordNumber % pageSize == 0 ? recordNumber / pageSize - 1 : recordNumber / pageSize;
                char cmd = ' ';
                do
                {
                    switch (char.ToUpper(cmd))
                    {
                        case 'F':
                            currentPage = 0; break;
                        case 'N':
                            currentPage = Math.Min(numberOfPages, currentPage + 1); break;
                        case 'P':
                            currentPage = Math.Max(0, currentPage - 1); break;
                        case 'L':
                            currentPage = numberOfPages; break;
                        default:
                            currentPage = 0;
                            break;
                    }
                    var table = PrepareTable(m, currentPage, pageSize);
                    AnsiConsole.Write(table);
                    cmd = AnsiConsole.Ask<char>("[red]Q[/]uit, [red]F[/]irst Page,[red]P[/]revious Page,[red]N[/]ext Page,[red]L[/]ast Page",'Q');
                } while (!('Q' == Char.ToUpper(cmd)));

            }

        }

        private static Table PrepareTable(Mortgage m,int pageNumber, int pageSize)
        {
            var table = new Table();

            // Add some columns
           
            table.AddColumn(new TableColumn("[red]Payment Date[/]").Centered());
            table.AddColumn(new TableColumn("[red]Principle applied[/]").Centered());
            table.AddColumn(new TableColumn("[red]Interest[/]").Centered());
            table.AddColumn(new TableColumn("[red]Principle remain[/]").Centered());
            table.AddColumn(new TableColumn("[red]Interest Paid[/]").Centered());
            decimal runningInterestTotal = 0;
            foreach (var p in m.Payments.Skip(pageNumber * pageSize).Take(pageSize)) {
                runningInterestTotal += p.InterestAmount;
                table.AddRow(new Markup(p.paymentDate.ToShortDateString()),
                    new Markup(p.PrincipalAmount.ToString("c")),
                    new Markup(p.InterestAmount.ToString("c")),
                    new Markup(p.LoanBalance.ToString("c")),
                    new Markup(runningInterestTotal.ToString("c")));
            }
            // Add some rows
            
            

            // Render the table to the console
            return table;
        }
    }
}
