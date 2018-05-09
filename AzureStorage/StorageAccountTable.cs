using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;

namespace AzureStorage
{
    public class StorageAccountTable
    {
        private CloudStorageAccount _storageAccount;
        private CloudTableClient _tableClient;

        public StorageAccountTable()
        {
            // Sample run
            CreateAccount();
            CloudTable table = GetTable($"customers{DateTime.Now.ToLongTimeString().Replace(":", string.Empty)}");
            CreateCustomer(table, new CustomerUK("David", "davidrbourke@gmail.com"));    
            CreateCustomer(table, new CustomerUK("Davidco", "davidrbourke@gmail.co.uk"));

            PrintAllCustomers(table);

            CustomerUK customerToUpdate = GetCustomer(table, "UK", "davidrbourke@gmail.com");
            customerToUpdate.Name = "Dave";
            UpdateCustomer(table, customerToUpdate);

            PrintAllCustomers(table);

            DeleteCustomer(table, customerToUpdate);

            PrintAllCustomers(table);

            BatchInsert(table);

            PrintAllCustomers(table);

            table.Delete();
        }

        private void DeleteCustomer(CloudTable cloudTable, CustomerUK customerToUpdate)
        {
            TableOperation update = TableOperation.Delete(customerToUpdate);
            cloudTable.Execute(update);
        }

        private void UpdateCustomer(CloudTable cloudTable, CustomerUK customerToUpdate)
        {
            TableOperation update = TableOperation.Replace(customerToUpdate);
            cloudTable.Execute(update);
        }

        public void CreateAccount()
        {
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnection"));

            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        public CloudTable GetTable(string tableName)
        {
            CloudTable cloudTable = _tableClient.GetTableReference(tableName);
            cloudTable.CreateIfNotExists();

            return cloudTable;
        }

        public void CreateCustomer(CloudTable cloudTable, CustomerUK customer)
        {
            TableOperation insert = TableOperation.Insert(customer);

            cloudTable.Execute(insert);
        }

        public CustomerUK GetCustomer(CloudTable cloudTable, string partitionKey, string rowKey)
        {
            TableOperation retrieve = TableOperation.Retrieve<CustomerUK>(partitionKey, rowKey);
            var result = cloudTable.Execute(retrieve);
            return (CustomerUK)result.Result;
        }

        public void PrintAllCustomers(CloudTable cloudTable)
        {
            TableQuery<CustomerUK> query = new TableQuery<CustomerUK>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UK"));

            Console.WriteLine("**********");
            foreach(CustomerUK customer in cloudTable.ExecuteQuery(query))
            {
                Console.WriteLine($"{customer.Email} {customer.Name}");
            }
        }

        public void BatchInsert(CloudTable table)
        {
            TableBatchOperation tableBatchOperation = new TableBatchOperation();

            // CUSTOMERS
            tableBatchOperation.Insert(new CustomerUK("Adam", "a@gmail.com"));
            tableBatchOperation.Insert(new CustomerUK("Barry", "b@gmail.com"));
            tableBatchOperation.Insert(new CustomerUK("Chris", "c@bourke@gmail.com"));

            table.ExecuteBatch(tableBatchOperation);
        }
    }

    public class CustomerUK : TableEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }

        // Parameterless constructor is required for populating from Table Storage
        public CustomerUK()
        {
        }

        public CustomerUK(string name, string email)
        {
            this.Name = name;
            this.Email = email;

            // Required!!
            this.PartitionKey = "UK";
            this.RowKey = email;
        }
    }
}
