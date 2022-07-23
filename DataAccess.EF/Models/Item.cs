using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.EF.Models;

public class Item : BaseModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public double Price { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
}