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
            var storageAccountManagement = new StorageAccountManagement();

            Console.ReadKey();
        }
    }
}
