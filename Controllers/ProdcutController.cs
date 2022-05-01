using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeAPI_Project.Models;

namespace PracticeAPI_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HealthCareContext _context;

        public ProductsController(HealthCareContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        [Route("GetAllMedicine")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllMedicine(bool IsAdminUser)
        {
            if (IsAdminUser)
            {
                return await _context.Products.ToListAsync();
            }
            return this.BadRequest(new { error = "invalid_grant", error_description = "Invalid Credentials" });
        }

        // GET: api/Products/5
        [HttpGet]
        [Route("GetMedicineById")]
        public async Task<ActionResult<Product>> GetMedicineById(int id, bool IsAdminUser)
        {
            if (IsAdminUser)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                return product;
            }
            return this.BadRequest(new { error = "invalid_grant", error_description = "Invalid Credentials" });
        }

        // PUT: api/Products/5        
        [HttpPut]
        [Route("UpdateMedicine")]
        public async Task<IActionResult> UpdateMedicine(int id, Product product, bool IsAdminUser)
        {
            
            if ((id != product.Id)||(IsAdminUser == false))
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products      
        [HttpPost]
        [Route("AddMedicine")]
        public async Task<ActionResult<Product>> AddMedicine(Product product, bool IsAdminUser)
        {
            if (IsAdminUser)
            {
                _context.Products.Add(product);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ProductExists(product.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            return this.BadRequest(new { error = "invalid_grant", error_description = "Invalid Credentials" });
        }
       
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }



    }
}
