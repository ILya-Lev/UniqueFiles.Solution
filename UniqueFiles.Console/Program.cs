using System;
using System.Collections.Generic;
using UniqueFiles.BL;
using UniqueFiles.BL.Transactions;

namespace UniqueFiles.ConsoleApp
{
    class Program
    {
        private static readonly string _helpToken = "h";
        private static readonly string _preserveToken = "p";
        private static readonly string _eraseToken = "e";

        private static readonly string _invalidFormat = $"Invalid input format. Type '{_helpToken}' to see manual";

        private static readonly Dictionary<string, Func<string, string, ITransaction>> _transactionFactory = new Dictionary<string, Func<string, string, ITransaction>>
        {
            [_preserveToken] = (root, bckp) => new DuplicateCleaningPreserveFoldersTransaction(root, bckp),
            [_eraseToken] = (root, bckp) => new DuplicateCleaningTransaction(root, bckp)
        };


        static void Main(string[] args)
        {
            try
            {
                var transaction = ParseInputArguments(args);
                transaction?.Execute();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static ITransaction ParseInputArguments(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    {
                        if (args[0] != _helpToken)
                            throw new Exception(_invalidFormat);

                        PrintUserManual();
                        break;
                    }
                case 2:
                    {
                        if (args[0] != _preserveToken && args[0] != _eraseToken)
                            throw new Exception(_invalidFormat);

                        return CreateTransaction(args[0], args[1], null);
                    }
                case 3:
                    {
                        if (args[0] != _preserveToken && args[0] != _eraseToken)
                            throw new Exception(_invalidFormat);

                        return CreateTransaction(args[0], args[1], args[2]);
                    }
                default:
                    throw new Exception(_invalidFormat);
            }

            return null;
        }

        private static void PrintUserManual()
        {
            Console.Write(@"
usage
h - to show user manual
p <root> <backup> - preserves empty folders, moves  duplicates in <root> into <backup>
p <root>  - preserves empty folders, moves  duplicates in <root> into <root>\bkp
e <root> <backup> - erases empty folders, moves  duplicates in <root> into <backup>
e <root> - erases empty folders, moves  duplicates in <root> into <root>\bkp
");
        }

        private static ITransaction CreateTransaction(string mode, string root, string backup)
        {
            return _transactionFactory[mode](root?.TrimEnd('\\'), backup?.TrimEnd('\\'));
        }
    }
}
