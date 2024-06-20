using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBackupMysql
{
    internal class BackupTest
    {
        string _connectionString = "Server=192.168.13.6;port=3306;Database=polaritonlifedb4;user=root;pwd=root;SslMode=None;charset=utf8mb4;";
        string _connectionString2 = "Server=192.168.13.6;port=3306;Database=polaritonlifedb3;user=root;pwd=root;SslMode=None;charset=utf8mb4;";
        internal Action<object, ExportProgressArgs> ExportProgressAct;
        internal Action<object, ImportProgressArgs> ImportProgressAct;
        public void Backup(string file)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
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

        private void Mb_ExportProgressChanged(object sender, ExportProgressArgs e)
        {
            ExportProgressAct?.Invoke(sender, e);
        }

        public void Restore(string file)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString2))
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

        private void Mb_ImportProgressChanged(object sender, ImportProgressArgs e)
        {
            ImportProgressAct?.Invoke(sender, e);
        }
    }
}
