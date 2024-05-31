//using Data.DataContexts;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace UserProvider.Functions
//{
//    public class GetUsers(ILogger<GetUsers> logger, DataContext context)
//    {
//        private readonly ILogger<GetUsers> _logger = logger;
//        private readonly DataContext _context = context;

//        [Function("GetUsers")]
//        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
//        {
//            var users = await _context.Users.ToListAsync();
//            return new OkObjectResult(users);
//        }
//    }
//}

using Data.DataContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UserProvider.Functions
{
    public class GetUsers
    {
        private readonly ILogger<GetUsers> _logger;
        private readonly DataContext _context;

        public GetUsers(ILogger<GetUsers> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUsers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Retrieving users from database.");

            try
            {
                var users = await _context.Users.ToListAsync();

                _logger.LogInformation("Users retrieved successfully.");
                return new OkObjectResult(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

