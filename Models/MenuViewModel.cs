using WebApplication2.Entities;

namespace WebApplication2.Models
{
    public class MenuViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
