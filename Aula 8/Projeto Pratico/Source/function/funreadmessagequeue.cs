using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace function
{
    public class funreadmessagequeue
    {
        [FunctionName("funreadmessagequeue")]
        public void Run([QueueTrigger("postcarddownloadlogs", Connection = "StorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            persistemysql(myQueueItem);
        }

        public void persistemysql(string messagequeue)
        {
            string connectionString = "Server=postcard.mysql.database.azure.com;UserID=augroberto;Password=P@ssw0rd;Database=downloadlogs;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Message? message = JsonConvert.DeserializeObject<Message>(messagequeue);

                conn.Open();

                MySqlCommand insertcommand = new MySqlCommand("INSERT INTO logs (host, arquivo, url) values ('"+message.host+"','"+message.arquivo+"','"+message.url+"')"  , conn);
                MySqlDataReader result = insertcommand.ExecuteReader();

                while (result.Read())
                {
                    Console.WriteLine("Column 0: {0} Column 1: {1}", result[0], result[1]);
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
}
