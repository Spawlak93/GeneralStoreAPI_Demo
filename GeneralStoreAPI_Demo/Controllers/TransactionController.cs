using GeneralStoreAPI_Demo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI_Demo.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Post
        //Tracking inventory
        [HttpPost]
        public IHttpActionResult CreateTransaction([FromBody]Transaction transaction)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(transaction is null)
            {
                return BadRequest("Request Body Cannot Be Empty");
            }

            Customer customer = _context.Customers.Find(transaction.CustomerId);
            Product product = _context.Products.Find(transaction.ProductId);
            if(customer == null)
            {
                return BadRequest("Customer Not Found");
            }
            if(product == null)
            {
                return BadRequest("Product Not Found");
            }
            
            if(product.NumberInInventory < transaction.ItemCount)
            {
                return BadRequest("Not Enough in inventory");
            }

            _context.Transactions.Add(transaction);
            transaction.Product.NumberInInventory -= transaction.ItemCount;

            _context.SaveChanges();

            return Ok("Transaction Added");
        }

        //Get
        [HttpGet]
        public IHttpActionResult  GetAllTransactions()
        {
            return Ok(_context.Transactions.ToList());
        }

        //Get By Transaction ID
        [HttpGet]
        public IHttpActionResult GetByTransactionId(int id)
        {
            Transaction transaction = _context.Transactions.Find(id);
            if(transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }
        //Get By CustomerID
        [HttpGet]
        [Route("api/Transaction/GetByCustomerId/{id}")]
        public IHttpActionResult GetByCustomerId(int id)
        {
            List<Transaction> transactions =  _context.Transactions.Where(t => t.CustomerId == id).ToList();
            if (transactions.Count > 0)
                return Ok(transactions);

            return BadRequest("Customer has no transactions");
        }

        //Put(update)
        [HttpPut]
        public IHttpActionResult UpdateTransaction([FromUri]int id, [FromBody]Transaction updatedTransaction)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(updatedTransaction == null)
            {
                return BadRequest("Body Cannot Be Empty");
            }
            Transaction oldTransaction = _context.Transactions.Find(id);
            Customer newCustomer = _context.Customers.Find(updatedTransaction.CustomerId);
            Product newProduct = _context.Products.Find(updatedTransaction.ProductId);
            if(newCustomer is null || newProduct is null || oldTransaction is null)
            {
                return NotFound();
            }

            oldTransaction.Product.NumberInInventory += oldTransaction.ItemCount;

            oldTransaction.CustomerId = updatedTransaction.CustomerId;
            oldTransaction.ProductId = updatedTransaction.ProductId;
            oldTransaction.ItemCount = updatedTransaction.ItemCount;

            newProduct.NumberInInventory -= oldTransaction.ItemCount;

            int numberOfChanges = _context.SaveChanges();

            if (numberOfChanges > 0)
            {
                return Ok("Updated the Transaction");
            }

            return InternalServerError();
        }

        //Delete
        [HttpDelete]
        public IHttpActionResult DeleteTransaction(int id)
        {
            Transaction transaction = _context.Transactions.Find(id);

            if(transaction == null)
            {
                return NotFound();
            }
            transaction.Product.NumberInInventory += transaction.ItemCount;

            _context.Transactions.Remove(transaction);


            if (_context.SaveChanges() == 2)
            {
                return Ok("Transaction Deleted");
            }

            return InternalServerError();
        }
    }
}
