using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    internal class UserEntity : BaseEntity<User>
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public bool EmailVerified { get; set; }
        public string Username { get; set; }
        public override User ToRuntime()
        {
            return new User(Email, EmailVerified, Name, Username, Id);
        }
        public static UserEntity RuntimeToEntity(User user)
        {
            return new UserEntity
            {
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                Name = user.Name,
                Username = user.Username,
                Id = user.Id ?? Guid.NewGuid()
            };
        }
    }
}