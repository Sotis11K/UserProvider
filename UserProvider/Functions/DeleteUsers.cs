using Data.DataContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UserProvider.Functions
{
    public class DeleteUser
    {
        private readonly ILogger<DeleteUser> _logger;
        private readonly DataContext _context;

        public DeleteUser(ILogger<DeleteUser> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteUser/{userId}")] HttpRequestData req,
            string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogError($"User with ID '{userId}' not found.");
                    return new NotFoundResult();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID '{userId}' deleted successfully.");
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user with ID '{userId}': {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
