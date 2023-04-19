using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace augroberto
{
    public class augroberto
    {
        [FunctionName("augroberto")]
        public void Run([QueueTrigger("postcarddownloadlogs", Connection = "QueueConn")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            persistmysql(myQueueItem);
        }

        public void persistmysql(string queuemessage)
        {
            string connectionString = "Server=augroberto.mysql.database.azure.com;UserID =augroberto;Password=P@ssw0rd;Database=downloadlogs;";

            // Best practice is to scope the MySqlConnection to a "using" block
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {

                Message? message = JsonConvert.DeserializeObject<Message>(queuemessage);

                // Connect to the database
                conn.Open();

                // Read rows
                MySqlCommand selectCommand = new MySqlCommand("INSERT INTO logs (host, arquivo, url) values ('"+message.host+"','"+message.arquivo+"','"+message.url+"')"  , conn);
                MySqlDataReader results = selectCommand.ExecuteReader();

                // Enumerate over the rows
                while (results.Read())
                {
                    Console.WriteLine("Column 0: {0} Column 1: {1}", results[0], results[1]);
                }
            }
        }
    }

    public class Message
    {
        public string host { get; set; }
        public string arquivo { get; set; }
        public string url { get; set; }
    }
}

