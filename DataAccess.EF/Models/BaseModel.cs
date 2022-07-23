using System.ComponentModel.DataAnnotations;

namespace DataAccess.EF.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}
