using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage
{
    public class StorageAccountQueue
    {
        private CloudStorageAccount _storageAccount;

        public StorageAccountQueue()
        {
            // Sample run
            CreateAccount();
        }

        public void CreateAccount()
        {
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnection"));
            
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = queueClient.GetQueueReference("tasks");
            cloudQueue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage("beep boop1");

            // Customise time to live from 7 days default to 24 hours
            var timeSpan = new TimeSpan(24, 0, 0);
            cloudQueue.AddMessage(message, timeToLive: timeSpan);

            // Dequeue a message - also need to call delete for some reason - but Azure appears to delete on GetMessage anyway

            // CloudQueueMessage retrievedMessage = cloudQueue.GetMessage();
            // Console.WriteLine($"Found message: {retrievedMessage.AsString}");
            // cloudQueue.DeleteMessage(retrievedMessage);

            // Use peek to read from queue without dequeuing
            CloudQueueMessage peekedMessage = cloudQueue.PeekMessage();
            Console.WriteLine($"Peeked message: {peekedMessage.AsString}");
            
            // Update a message
            CloudQueueMessage updateMessage = cloudQueue.GetMessage();
            Console.WriteLine($"Found message for update: {updateMessage.AsString}");
            updateMessage.SetMessageContent("beep boop2");
            cloudQueue.UpdateMessage(updateMessage, new TimeSpan(0, 0, 10), MessageUpdateFields.Content | MessageUpdateFields.Visibility);

        }
    }
}
