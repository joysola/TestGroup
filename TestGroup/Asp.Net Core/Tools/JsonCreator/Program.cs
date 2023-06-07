using Newtonsoft.Json;
using Shared.DataTransferObjects;

namespace JsonCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cfc1 = new CompanyForCreationDto("Fox", "Street ABC", "US");
            var cfc2 = new CompanyForCreationDto("Fox", "Street ABC", "US");
            var cfc3 = new CompanyForCreationDto("Fox", "Street ABC", "US");
            var list = new List<CompanyForCreationDto> { cfc1, cfc2, cfc3 };


            //var efc1 = new EmployeeForCreationDto("Jack", 33, "ABVsdhja");

            //var efu1 = new EmployeeForUpdateDto("joysola", 25, "Software Engineer");
            //var cfu1 = new CompanyForUpdateDto("Santiago Anymore", "Lasagna Bluetooth", "Italy",
            //    new[] { new EmployeeForCreationDto("Jackson", 99, "Marketing Design") });
            //var json = JsonConvert.SerializeObject(cfu1);
            Console.WriteLine("Hello, World!");
        }
    }
}