﻿using System;
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
    public class UsersController : ControllerBase
    {
        private readonly HealthCareContext _context;

        UserValidation uv = new UserValidation();

        public UsersController(HealthCareContext context)
        {
            _context = context;
        }

             
        [HttpGet]
        [Route("SignIn")]

        public async Task<ActionResult<UserValidation>> SignIn(string Email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email.Equals(Email)&&x.Password.Equals(password));


            if (user == null)
            {
                return this.BadRequest(new { error = "invalid_grant", error_description = "Invalid Credentials" });
            }
            else
            {
                uv.UserId = user.Id;
                uv.IsValidUser = true;
                uv.IsAdmin = user.IsAdmin;
                
            }
            return uv;
        }

        // PUT: api/Users/5
        [HttpPut]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
