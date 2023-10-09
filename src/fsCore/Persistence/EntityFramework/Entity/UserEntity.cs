using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    internal class UserEntity : BaseEntity<User>
    {
        [Key]
        [Required]
        [Column(TypeName = "TEXT")]
        public string Email { get; set; }
        [Column(TypeName = "TEXT")]
        public string? Name { get; set; }
        public override User ToRuntime()
        {
            return new User(Email, Name);
        }
        public static UserEntity RuntimeToEntity(User user)
        {
            return new UserEntity
            {
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}