using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Sample Blob Storage Account
            // var storageAccountManagement = new StorageAccountBlob();

            // 2. Sample Table Storage Account
            // var storageAccountTable = new StorageAccountTable();

            // 3. Sample Queue Storage Account
            var storageAccountQueue = new StorageAccountQueue();

            Console.ReadKey();
        }
    }
}
