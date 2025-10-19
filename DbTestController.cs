using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnswerUA.Data;
using answer_ua.Data;

namespace AnswerUA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DbTestController : ControllerBase
    {
        private readonly ApplicationDbContext _appDb;
        private readonly ShopDbContext _shopDb;

        public DbTestController(ApplicationDbContext appDb, ShopDbContext shopDb)
        {
            _appDb = appDb;
            _shopDb = shopDb;
        }

        [HttpGet("check")]
        public IActionResult CheckDatabases()
        {
            var result = new Dictionary<string, string>();

            // Перевірка ApplicationDbContext (Identity)
            try
            {
                if (_appDb.Database.CanConnect())
                    result["ApplicationDb"] = "✅ Connected successfully";
                else
                    result["ApplicationDb"] = "❌ Cannot connect";
            }
            catch (Exception ex)
            {
                result["ApplicationDb"] = $"❌ Error: {ex.Message}";
            }

            // Перевірка ShopDbContext (Answer)
            try
            {
                if (_shopDb.Database.CanConnect())
                    result["ShopDb"] = "✅ Connected successfully";
                else
                    result["ShopDb"] = "❌ Cannot connect";
            }
            catch (Exception ex)
            {
                result["ShopDb"] = $"❌ Error: {ex.Message}";
            }

            return Ok(result);
        }
    }
}
