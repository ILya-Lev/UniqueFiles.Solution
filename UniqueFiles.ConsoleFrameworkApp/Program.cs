using System;
using System.Collections.Generic;
using UniqueFiles.BL;
using UniqueFiles.BL.Transactions;

namespace UniqueFiles.ConsoleFrameworkApp
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
            var bckpName = BackupDirectoryManager.DefaultBackUpSubFolder;
            Console.Write($@"
usage
{_helpToken} - to show user manual
{_preserveToken} <root> <backup> - preserves empty folders, moves  duplicates in <root> into <backup>
{_preserveToken} <root>  - preserves empty folders, moves  duplicates in <root> into <root>\{bckpName}
{_eraseToken} <root> <backup> - erases empty folders, moves  duplicates in <root> into <backup>
{_eraseToken} <root> - erases empty folders, moves  duplicates in <root> into <root>\{bckpName}
");
        }

        private static ITransaction CreateTransaction(string mode, string root, string backup)
        {
            Func<string, string> trimMess = s => s?.TrimEnd('\\', '/');
            return _transactionFactory[mode](trimMess(root), trimMess(backup));
        }
    }
}
