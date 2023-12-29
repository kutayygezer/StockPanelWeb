using Microsoft.EntityFrameworkCore;
using StockPanelWeb.Models;

namespace StockPanelWeb.Data
{
    public class StockPanelDBContext : DbContext
    {
        public StockPanelDBContext(DbContextOptions<StockPanelDBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LoginViewModel> loginViewModels { get; set; }
    }
}
