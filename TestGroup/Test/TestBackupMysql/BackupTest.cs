using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBackupMysql
{
    internal class BackupTest
    {
        string _connectionStringExport = "Server=192.168.13.6;port=3306;Database=polaritonlifedb3;user=root;pwd=root;SslMode=None;charset=utf8mb4;";
        string _connectionStringImport = "Server=192.168.13.6;port=3306;Database=polaritonlifedb;user=root;pwd=root;SslMode=None;charset=utf8mb4;";
        internal Action<object, ExportProgressArgs> ExportProgressAct;
        internal Action<object, ImportProgressArgs> ImportProgressAct;
        public void Backup(string file)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionStringExport))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        mb.ExportProgressChanged += Mb_ExportProgressChanged;
                        mb.ExportInfo.IntervalForProgressReport = 1000;
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(file);
                        conn.Close();
                    }
                }
            }
        }

        public void BackupAES(string file, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionStringExport))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            mb.ExportProgressChanged += Mb_ExportProgressChanged;
                            mb.ExportInfo.IntervalForProgressReport = 1000;
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToMemoryStream(ms);
                            conn.Close();
                            //ms.Flush();
                            byte[] ba = ms.ToArray();

                            // 1st Compress the file data
                            // The size is 50%-70% smaller
                            var sw = new Stopwatch();
                            sw.Start();
                            ba = CompressHelper.CompressData2(ba);
                            sw.Stop();
                            Console.WriteLine($"压缩：{sw.ElapsedMilliseconds}");
                            // 2nd Encrypt the file data
                            sw.Restart();
                            ba = AESHelper.AES_Encrypt(ba, password);
                            Console.WriteLine($"加密：{sw.ElapsedMilliseconds}");
                            sw.Stop();


                            // 3rd Write the file data to disk
                            File.WriteAllBytes(file, ba);
                            mb.ExportProgressChanged -= Mb_ExportProgressChanged;
                        }
                    }
                }
            }
        }


        private void Mb_ExportProgressChanged(object sender, ExportProgressArgs e)
        {
            ExportProgressAct?.Invoke(sender, e);
        }

        public void Restore(string file)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionStringImport))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        mb.ImportProgressChanged += Mb_ImportProgressChanged;
                        mb.ImportInfo.IntervalForProgressReport = 1000;
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(file);
                        conn.Close();
                    }
                }
            }
        }

        public void RestoreAES(string file, string password)
        {
            // 1st Read the file bytes
            byte[] ba = File.ReadAllBytes(file);
            var sw = new Stopwatch();
            sw.Start();
            // 2nd Decrypt the file data
            ba = AESHelper.AES_Decrypt(ba, password);
            sw.Stop();
            Console.WriteLine($"解密：{sw.ElapsedMilliseconds}");

            sw.Restart();
            // 3rd Decompress the file data
            ba = CompressHelper.DecompressData2(ba);
            sw.Stop();
            Console.WriteLine($"解压：{sw.ElapsedMilliseconds}");


            using (MemoryStream ms = new MemoryStream(ba))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionStringImport))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            mb.ImportInfo.IntervalForProgressReport = 1000;
                            mb.ImportProgressChanged += Mb_ImportProgressChanged;
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ImportFromMemoryStream(ms);
                            conn.Close();
                            mb.ImportProgressChanged -= Mb_ImportProgressChanged;
                        }
                    }
                }
            }
        }

        private void Mb_ImportProgressChanged(object sender, ImportProgressArgs e)
        {
            ImportProgressAct?.Invoke(sender, e);
        }
    }
}
