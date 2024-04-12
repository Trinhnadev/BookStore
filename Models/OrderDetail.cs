using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FPTBook.Models;
namespace FPTBook.Models{
    public partial class OrderDetail
{
    [Key]
    public int Id { get; set; }

    public int BookId { get; set; }
    public int OrderId { get; set; }
    public decimal Price { get; set; } 

    [ForeignKey("BookId")]
    [InverseProperty("OrderDetails")]
    public virtual Book? IdBookOrderNavigation { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order? IdOrderNavigation { get; set; } = null!;
}
}
