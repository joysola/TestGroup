namespace TestBackupMysql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var file = $".\\{DateTime.Now:yyyyMMddHHmmss}.sql";
            var test = new BackupTest();
            test.ExportProgressAct = (o, e) =>
            {
                Console.WriteLine($"{e.CurrentTableName}");
                Console.WriteLine($"{e.CurrentRowIndexInAllTables}/{e.TotalRowsInAllTables}");
            };

            test.ImportProgressAct = (o, e) =>
            {
                Console.WriteLine(e.PercentageCompleted);
                Console.WriteLine($"{e.CurrentBytes}/{e.TotalBytes}");
            };
            test.Backup(file);

            Console.WriteLine($"{file} backup completed!");

            test.Restore(file);
            Console.WriteLine($"{file} import completed!");

            Console.ReadLine();
        }
    }
}
