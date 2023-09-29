using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;
namespace Persistence.EntityFramework.Entity
{
    [Table("world_fish", Schema = DbConstants.MainSchema)]
    internal class WorldFishEntity : BaseEntity
    {
        [Key]
        [Column(TypeName = "TEXT")]
        [Required]
        public string Taxocode { get; set; }
        [Column(TypeName = "TEXT")]
        public string? ScientificName { get; set; }
        [Column(TypeName = "INTEGER")]
        public int? Isscaap { get; set; }
        [Column(TypeName = "TEXT")]
        public string? A3Code { get; set; }
        [Column(TypeName = "TEXT")]
        public string? EnglishName { get; set; }
        [Column(TypeName = "TEXT")]
        public string? Nickname { get; set; }
        public static WorldFishEntity RuntimeToEntity(WorldFish originalObj)
        {
            var tempObj = new WorldFishEntity()
            {
                A3Code = originalObj.A3Code,
                EnglishName = originalObj.EnglishName,
                Isscaap = originalObj.Isscaap,
                Nickname = originalObj.Nickname,
                ScientificName = originalObj.ScientificName,
                Taxocode = originalObj.Taxocode
            };
            return tempObj;
        }
        public override WorldFish ToRuntime()
        {
            return new WorldFish(Taxocode, Isscaap, A3Code, ScientificName, EnglishName, Nickname);
        }
    }
}
