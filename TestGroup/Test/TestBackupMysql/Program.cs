namespace TestBackupMysql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var file = ".\\backup\\20240619174935.sql";//$".\\{DateTime.Now:yyyyMMddHHmmssfff}.sql";
            var fileDir = ".\\backup\\";
            var zipfile = ".\\20240619174935.zip";
            var test = new BackupTest();
            var zipText = new CompressTest();
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


            zipText.PackProcessAct = (o, e) =>
            {
                Console.WriteLine($"Pack:{e.PercentComplete}");
            };

            zipText.UnpackProcessAct = (o, e) =>
            {
                Console.WriteLine($"Pack:{e.PercentComplete}");
            };

            test.Backup(file);

            Console.WriteLine($"{file} backup completed!");

            zipText.Compress(zipfile, file);
            //zipText.FastZip(zipfile, fileDir);

            Console.WriteLine($"{file} compress completed!");


            zipText.Unpack(zipfile, ".\\");
            Console.WriteLine($"{file} unpack completed!");

            test.Restore(file);
            Console.WriteLine($"{file} import completed!");

            Console.ReadLine();
        }
    }
}
