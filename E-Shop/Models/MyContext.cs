using E_Shop.Models;
using Microsoft.EntityFrameworkCore;


public class MyContext : DbContext
{
  

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseMySQL("server=mysqlstudenti.litv.sssvt.cz;database=4b1_sunkovskyadam_db1;user=sunkovskyadam;password=123456");
       
    }
    public DbSet<Product> Product { get; set; }
    public DbSet<Login> Login { get; set; }
    public DbSet<ProductDetail> ProductDetail { get; set; }
    public DbSet<Image> Image { get; set; }
    public DbSet<WebsiteLog> WebSiteLog { get; set; }
    public DbSet<Warehouse> Warehouse { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Order> Order { get; set; }
}
