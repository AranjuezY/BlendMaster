using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string? ProductName { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        [Range(0.0, 20.0, ErrorMessage = "Price must be 0 to 20")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Product description cannot exceed 500 characters.")]
        public string? ProductDescription { get; set; }
        
        [Required]
        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string? ImageUrl { get; set; }
    }
}
