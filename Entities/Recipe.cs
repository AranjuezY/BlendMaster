using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public enum RecipeStatusType
    {
        Testing, 
        RolledOut,
        Rejected,
        Archived
    }

    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RecipeId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Ingredients list cannot be empty.")]
        public List<string>? Ingredients { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Instructions list cannot be empty.")]
        public List<string>? Instructions { get; set; }

        [Required]
        public List<string>? Tags { get; set; }

        [Required]
        public RecipeStatusType Status { get; set; }
    }
}
