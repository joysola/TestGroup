
namespace TestBackupMysql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            //var file = ".\\backup\\20240619174935.sql";//$".\\{DateTime.Now:yyyyMMddHHmmssfff}.sql";
            var file2 = $".\\backup\\{DateTime.Now:yyyyMMddHHmmssfff}.pl";
            var fileDir = ".\\backup\\";
            var zipfile = ".\\20240619174935.zip";
            var test = new BackupTest();
            var zipText = new CompressTest();
            var password = "pl@123456";
            test.ExportProgressAct = (o, e) =>
            {
                Console.WriteLine($"{e.CurrentTableName}");
                //Console.WriteLine($"{e.CurrentRowIndexInAllTables}/{e.TotalRowsInAllTables}");
                Console.WriteLine($"{Math.Round(1.0 * e.CurrentRowIndexInAllTables / e.TotalRowsInAllTables * 100.0, 2, MidpointRounding.AwayFromZero)}");
            };

            test.ImportProgressAct = (o, e) =>
            {
                Console.WriteLine(e.PercentageCompleted);
                // Console.WriteLine($"{e.CurrentBytes}/{e.TotalBytes}");
            };


            zipText.PackProcessAct = (o, e) =>
            {
                Console.WriteLine($"Pack:{e.PercentComplete}");
            };

            zipText.UnpackProcessAct = (o, e) =>
            {
                Console.WriteLine($"Pack:{e.PercentComplete}");
            };

            test.BackupAES(file2, password);

            Console.WriteLine($"{file2} backup completed!");

            // zipText.Compress(zipfile, file);
            //zipText.FastZip(zipfile, fileDir);

            //Console.WriteLine($"{file} compress completed!");


            //zipText.Unpack(zipfile, ".\\");
            //Console.WriteLine($"{file} unpack completed!");

            test.RestoreAES(file2, password);
            Console.WriteLine($"{file2} import completed!");

            Console.ReadLine();
        }
    }
}
