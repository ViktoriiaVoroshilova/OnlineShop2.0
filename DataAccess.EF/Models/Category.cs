using System.ComponentModel.DataAnnotations;

namespace DataAccess.EF.Models;

public class Category : BaseModel
{
    [Required]
    public string Name { get; set; } = null!;
}