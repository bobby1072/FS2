using Common.Models;

namespace DataImporter.MockModelBuilders
{
    public static class MockWorldFishBuilder
    {
        public static WorldFish Build()
        {
            return new WorldFish(Faker.Lorem.GetFirstWord(), Faker.Lorem.GetFirstWord(), Faker.Lorem.GetFirstWord(), Faker.Name.First(), Faker.Name.First(), Faker.Name.First());
        }
    }
}