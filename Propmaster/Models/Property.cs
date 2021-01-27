using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class Repository
    {
        private readonly CloudTable propertyTable = null;
        public Repository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=propmasterstorage;AccountKey=JGpbuqhdRVZ+xIS6562o1HgsC6VJPP8hap1wvEc8ZBtkI/qQBbWiUTMcA3S2e9T62TjB0qaSGhe+il0AgrsNNw==;EndpointSuffix=core.windows.net");
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            propertyTable = cloudTableClient.GetTableReference("PropertyTable");
            propertyTable.CreateIfNotExists();
        }

        public IEnumerable<Property> GetAll()
        {
            TableQuery<Property> query = new TableQuery<Property>();
            IEnumerable<Property> entties = propertyTable.ExecuteQuery(query);
            return entties;
        }
        public void CreateOrUpdate(Property property)
        {
            TableOperation operation = TableOperation.InsertOrReplace(property);
            propertyTable.Execute(operation);
        }
        public void Delete(Property property)
        {
            TableOperation operation = TableOperation.Delete(property);
            propertyTable.Execute(operation);
        }
        public Property Get(string PartitionKey, string RowKey)
        {
            TableOperation operation = TableOperation.Retrieve<Property>(PartitionKey, RowKey);
            TableResult result = propertyTable.Execute(operation);
            return result.Result as Property;
        }
    }

    public class Property : TableEntity
    {
        public string PropertyId { get; set; }

        public string CreatedBy { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int PropertySize { get; set; }

        [Required]
        public string PropertyLocation { get; set; }

        [Required]
        public string PropertyType { get; set; }

        [Required]
        public string Price { get; set; }

        [Required]
        public string Furnished { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Bedroom { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Bathroom { get; set; }

        [Column(TypeName = "int")]
        public int Carpark { get; set; }

        [Required]
        public string PicUrl { get; set; }

        [Required]
        public string PropertyStatus { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}
