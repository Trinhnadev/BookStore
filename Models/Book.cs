using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models;

[Table("Book")]
public partial class Book
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Author { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? Image { get; set; }

    [Column("CatID")]
    public int CatId { get; set; }

    [ForeignKey("CatId")]
    [InverseProperty("Books")]
    public virtual Category? Cat { get; set; } = null!;
    [NotMapped]
    public IFormFile? ImageFile {get; set;}

    [InverseProperty("IdBookNavigation")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("IdBookOrderNavigation")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    //  [NotMapped]
    // public IFormFile NewImageFile { get; set; }
}
