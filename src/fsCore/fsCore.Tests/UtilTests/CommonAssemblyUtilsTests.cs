using Common.Models;
using Common.Utils;
using FluentAssertions;
using System.Text.Json;

namespace fsCore.Tests.UtilTests
{
    public class CommonAssemblyUtilsTests
    {
        private class ParseToChildOf_When_Called_With_Vehicle_object_Returns_Correct_Vehicle_Instance_Class_Data : TheoryData<object, Type>
        {
            public ParseToChildOf_When_Called_With_Vehicle_object_Returns_Correct_Vehicle_Instance_Class_Data()
            {
                var car = new TestBaseModelCar { DriveSystem = "4x4", Manufacturer = "volvo", Year = 2021 };
                Add(car, typeof(TestBaseModelCar));
                var boat = new TestBaseModelTruck { CargoType = "wood", Manufacturer = "volvo", Year = 2021 };
                Add(boat, typeof(TestBaseModelTruck));
            }
        }
        [Theory]
        [ClassData(typeof(ParseToChildOf_When_Called_With_Vehicle_object_Returns_Correct_Vehicle_Instance_Class_Data))]
        public void ParseToChildOf_When_Called_With_Vehicle_object_Returns_Correct_Vehicle_Instance(object obj, Type expectedType)
        {
            // Act
            var result = CommonAssemblyUtils.ParseToChildOf<TestBaseModelVehicle>(obj);

            // Assert
            result.Should().BeOfType(expectedType);
        }
        private class ParseToChildOf_When_Called_With_Deserialised_Json_Vehicle_Returns_Correct_Vehicle_Instance_Class_Data : TheoryData<object, Type>
        {
            public ParseToChildOf_When_Called_With_Deserialised_Json_Vehicle_Returns_Correct_Vehicle_Instance_Class_Data()
            {
                var car = new TestBaseModelCar { DriveSystem = "4x4", Manufacturer = "volvo", Year = 2021 };
                Add(JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(car)), typeof(TestBaseModelCar));
                var truck = new TestBaseModelTruck { CargoType = "wood", Manufacturer = "volvo", Year = 2021 };
                Add(JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(truck)), typeof(TestBaseModelTruck));
            }
        }
        [Theory]
        [ClassData(typeof(ParseToChildOf_When_Called_With_Deserialised_Json_Vehicle_Returns_Correct_Vehicle_Instance_Class_Data))]
        public void ParseToChildOf_When_Called_With_Deserialised_Json_Vehicle_Returns_Correct_Vehicle_Instance(object obj, Type expectedType)
        {
            // Act
            var result = CommonAssemblyUtils.ParseToChildOf<TestBaseModelVehicle>(obj);

            // Assert
            result.Should().BeOfType(expectedType);
        }
    }
}