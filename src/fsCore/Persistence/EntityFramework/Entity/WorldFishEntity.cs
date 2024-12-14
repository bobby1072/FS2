using fsCore.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Persistence.EntityFramework.Entity
{
    [Table("world_fish", Schema = DbConstants.PublicSchema)]
    internal record WorldFishEntity : BaseEntity<WorldFish>
    {
        [Key]
        public string Taxocode { get; set; }
        public string? ScientificName { get; set; }
        public string? Isscaap { get; set; }
        public string? A3_code { get; set; }
        public string? EnglishName { get; set; }
        public string? Nickname { get; set; }
        public static WorldFishEntity RuntimeToEntity(WorldFish originalObj)
        {
            var tempObj = new WorldFishEntity()
            {
                A3_code = originalObj.A3Code,
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
            return new WorldFish(Taxocode, Isscaap, A3_code, ScientificName, EnglishName, Nickname);
        }
    }
}
