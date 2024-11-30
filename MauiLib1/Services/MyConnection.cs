using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLib1.Services
{
    public static class MyConnection
    {
        private const string dbName = "dw_db.db";
        private static string _connectionString = string.Empty;
        public static string GetConnection()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
               _connectionString = $"Data Source = {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName)}";
               //_connectionString = Path.Combine(FileSystem.AppDataDirectory, dbName);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                _connectionString = $"Data Source = {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", dbName)}";
            }
            else
            {
                _connectionString = $"Data Source = {Path.Combine(FileSystem.AppDataDirectory, dbName)}";
            }
            return _connectionString;
        }
    }
}
