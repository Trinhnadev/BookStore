using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FPTBook.Models;

public partial class Cart
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }

    public int BookId { get; set; }

    public decimal Price { get; set; } 

    [ForeignKey("BookId")]
    [InverseProperty("Carts")]
    public virtual Book? IdBookNavigation { get; set; } = null!;
}