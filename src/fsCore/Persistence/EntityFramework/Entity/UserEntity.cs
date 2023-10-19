using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("user", Schema = DbConstants.MainSchema)]
    internal class UserEntity : BaseEntity<User>
    {
        [Key]
        [Required]
        [Column(TypeName = "TEXT")]
        public string Email { get; set; }
        [Column(TypeName = "TEXT")]
        public string? Name { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool EmailVerified { get; set; }
        public override User ToRuntime()
        {
            return new User(Email, EmailVerified, Name);
        }
        public static UserEntity RuntimeToEntity(User user)
        {
            return new UserEntity
            {
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                Name = user.Name
            };
        }
    }
}