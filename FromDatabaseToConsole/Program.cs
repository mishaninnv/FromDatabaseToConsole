using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace FromDatabaseToConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputString = args[0];
            var connString = @"Server=localhost\SQLEXPRESS01;Database=DataBaseTest;Trusted_Connection=True;";
            var step = 5;

            var conn = new SqlConnection(connString);
            conn.Open();

            var cmdString = string.Format(@"SELECT * FROM Words WHERE word LIKE '{0}%'", inputString);

            using (var sqlCmd = new SqlCommand(cmdString, conn))
            {
                var reader = sqlCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    var result = new Dictionary<string, int>();

                    while (reader.Read())
                    {
                        result.Add(reader.GetValue(0).ToString(), reader.GetInt32(1));
                    }

                    var result2 = result.OrderByDescending(s => s.Value).ThenBy(d => d.Key).Take(step);

                    foreach (var word in result2)
                    {
                        Console.WriteLine($"Слово: {word.Key}, количество повторений: {word.Value}");
                    }
                }
                else 
                {
                    Console.WriteLine("Совпадений не найдено.");
                }
            }
        }
    }
}
