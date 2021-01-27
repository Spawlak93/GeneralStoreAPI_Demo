using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI_Demo.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey (nameof(Customer))]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey (nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int ItemCount { get; set; }

        public DateTime GetDateOfTransaction() => DateTime.Now.Date;
    }
}