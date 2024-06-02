using Data.DataContexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace UserProvider.Functions
{
    public class UpdateUser
    {
        private readonly ILogger<UpdateUser> _logger;
        private readonly DataContext _context;

        public UpdateUser(ILogger<UpdateUser> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("UpdateUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateUser/{userId}")] HttpRequestData req,
            string userId)
        {
            try
            {
                // Read the request body to get the updated user information
                var requestBody = await req.ReadAsStringAsync();
                var updatedUser = JsonConvert.DeserializeObject<ApplicationUser>(requestBody);

                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogError($"User with ID '{userId}' not found.");
                    return new NotFoundResult();
                }

                // Update user properties
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                // Add more properties as needed

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID '{userId}' updated successfully.");
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user with ID '{userId}': {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
