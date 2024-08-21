using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Entities
{
    public class TestDbContext : IdentityDbContext<IdentityUser>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<CustomerOrder> CustomerOrder { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Table> Table { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Member> Member { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CustomerOrder>()
                .HasMany(co => co.OrderDetails)
                .WithOne(od => od.CustomerOrder)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var classicCategoryId = Guid.NewGuid();
            var tropicalCategoryId = Guid.NewGuid();
            var modernCategoryId = Guid.NewGuid();

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = classicCategoryId, CategoryName = "Classic" },
                new Category { CategoryId = tropicalCategoryId, CategoryName = "Tropical" },
                new Category { CategoryId = modernCategoryId, CategoryName = "Modern" }
            );

            modelBuilder.Entity<Table>().HasData(
                new Table { TableId = 1, TableName = "Table1" },
                new Table { TableId = 2, TableName = "Table2" },
                new Table { TableId = 3, TableName = "Table3" },
                new Table { TableId = 4, TableName = "Table4" },
                new Table { TableId = 5, TableName = "Table5" }
            );

            var martiniId = Guid.NewGuid();
            var maitaiId = Guid.NewGuid();
            var espressoId = Guid.NewGuid();
            
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Martini",
                    Price = 11.99m,
                    ProductDescription = "A sophisticated cocktail made with gin and vermouth, garnished with an olive.",
                    CategoryId = classicCategoryId,
                    RecipeId = martiniId
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Mai Tai",
                    Price = 10.99m,
                    ProductDescription = "A tropical mix of rum, lime juice, and orgeat syrup.",
                    CategoryId = tropicalCategoryId,
                    RecipeId = maitaiId
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Espresso Martini",
                    Price = 13.49m,
                    ProductDescription = "A sophisticated mix of vodka, coffee liqueur, and espresso.",
                    CategoryId = modernCategoryId,
                    RecipeId = espressoId
                }
            );

            modelBuilder.Entity<Recipe>().HasData(
                new Recipe
                {
                    RecipeId = martiniId,
                    Name = "Classic Martini",
                    Description = "A classic Martini made with gin and vermouth, garnished with an olive or lemon twist.",
                    Ingredients = new List<string>
                    {
                        "2 oz Gin",
                        "1/2 oz Dry Vermouth",
                        "Ice",
                        "Olive or Lemon Twist (for garnish)"
                    },
                    Instructions = new List<string>
                    {
                        "Fill a mixing glass with ice.",
                        "Add gin and dry vermouth to the glass.",
                        "Stir well until the mixture is chilled.",
                        "Strain into a chilled Martini glass.",
                        "Garnish with an olive or a lemon twist."
                    },
                    Tags = new List<string>
                    {
                        "Cocktail",
                        "Classic",
                        "Martini",
                        "Gin"
                    },
                    Status = RecipeStatusType.RolledOut
                },
                new Recipe
                {
                    RecipeId = maitaiId,
                    Name = "Mai Tai",
                    Description = "A tropical cocktail with a blend of rum, lime, and fruit juices, garnished with a mint sprig and cherry.",
                    Ingredients = new List<string>
                    {
                        "1 1/2 oz White Rum",
                        "1/2 oz Dark Rum",
                        "1/2 oz Orange Curacao",
                        "1/2 oz Lime Juice",
                        "1/2 oz Orgeat Syrup",
                        "1/4 oz Simple Syrup",
                        "Crushed Ice",
                        "Mint Sprig (for garnish)",
                        "Maraschino Cherry (for garnish)"
                    },
                    Instructions = new List<string>
                    {
                        "Fill a shaker with ice.",
                        "Add white rum, orange curacao, lime juice, orgeat syrup, and simple syrup to the shaker.",
                        "Shake well and strain into a glass filled with crushed ice.",
                        "Float dark rum on top by pouring it over the back of a spoon.",
                        "Garnish with a mint sprig and a maraschino cherry."
                    },
                    Tags = new List<string>
                    {
                        "Cocktail",
                        "Tropical",
                        "Fruity",
                        "Rum"
                    },
                    Status = RecipeStatusType.RolledOut
                },
                new Recipe
                {
                    RecipeId = espressoId,
                    Name = "Espresso Martini",
                    Description = "A coffee-flavored cocktail combining vodka, coffee liqueur, and fresh espresso, perfect for a night out.",
                    Ingredients = new List<string>
                    {
                        "1 1/2 oz Vodka",
                        "1 oz Coffee Liqueur",
                        "1 oz Fresh Espresso",
                        "1/2 oz Simple Syrup",
                        "Ice",
                        "Coffee Beans (for garnish)"
                    },
                    Instructions = new List<string>
                    {
                        "Fill a shaker with ice.",
                        "Add vodka, coffee liqueur, fresh espresso, and simple syrup to the shaker.",
                        "Shake vigorously to combine and chill the ingredients.",
                        "Strain into a chilled martini glass.",
                        "Garnish with a few coffee beans on top."
                    },
                    Tags = new List<string>
                    {
                        "Cocktail",
                        "Espresso",
                        "After-Dinner",
                        "Vodka"
                    },
                    Status = RecipeStatusType.RolledOut
                }
            );
        }
    }
}
