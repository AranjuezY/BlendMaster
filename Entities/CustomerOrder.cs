using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Entities
{
    public enum OrderStatusType
    {
        Preparing,
        Completed,
        Cancelled
    }

    public class CustomerOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderId { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Total must be a non-negative value.")]
        public decimal Total { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public OrderStatusType OrderStatus { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }

        [Required]
        public int TableId { get; set; }
        public Table? Table { get; set; }
        
        public string? MemberId { get; set; }
        public Member? Member { get; set; }
    }
}
