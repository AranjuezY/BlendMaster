using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public class Table
    {
        [Key]
        public int TableId { get; set; }

        [StringLength(50, ErrorMessage = "Table name cannot exceed 50 characters.")]
        public string? TableName { get; set; }
    }
}
