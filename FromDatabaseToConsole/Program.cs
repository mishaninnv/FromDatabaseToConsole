using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace FromDatabaseToConsole
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1) return;
            var inputString = args[0];

            var connString = ConfigurationManager.AppSettings.Get("connectionString");
            var conn = new SqlConnection(connString);

            var tableName = ConfigurationManager.AppSettings.Get("tableName");
            var cmdString = $@"SELECT * FROM {tableName} WHERE word LIKE '{inputString}%'";

            var data = SelectDataFromDb(conn, cmdString);

            if (data == null) return;
            ConsoleOutput(data);
        }

        private static IEnumerable<KeyValuePair<string, int>> SelectDataFromDb(SqlConnection conn, string cmdString)
        {
            using var sqlCmd = new SqlCommand(cmdString, conn);
            conn.Open();
            var reader = sqlCmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("Совпадений не найдено.");
                return null;
            }

            var data = GetReadData(reader);
            var sortedData = SortData(data);

            return sortedData;
        }

        private static Dictionary<string, int> GetReadData(SqlDataReader reader)
        {
            var result = new Dictionary<string, int>();

            while (reader.Read())
            {
                result.Add(reader.GetValue(0).ToString(), reader.GetInt32(1));
            }

            return result;
        }

        private static IEnumerable<KeyValuePair<string, int>> SortData(Dictionary<string, int> data)
        {
            var countElements = Convert.ToInt32(ConfigurationManager.AppSettings.Get("countElements"));
            return data.OrderByDescending(s => s.Value).ThenBy(d => d.Key).Take(countElements);
        }

        private static void ConsoleOutput(IEnumerable<KeyValuePair<string,int>> keyValuePairs)
        {
            foreach (var element in keyValuePairs)
            {
                Console.WriteLine($"Слово: {element.Key}, количество повторений: {element.Value}");
            }
        }
    }
}
