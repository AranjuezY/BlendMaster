using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public enum BillStatusType
{
    Unpaid,
    Pending,
    Paid,
    Refunded,
    Disputed
}

public enum PaymentMethodType
{
    CreditCard,
    DebitCard,
    PayPal,
    ApplePay,
    GooglePay
}

namespace WebApplication2.Entities
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BillId { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        public CustomerOrder? CustomerOrder { get; set; }

        [Required]
        public int TableId { get; set; }
        public Table? Table { get; set; }

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal Tax { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal AmountPaid { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        [Required]
        public BillStatusType Status { get; set; }
    }
}
