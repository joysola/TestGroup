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
            var json = JsonConvert.SerializeObject(list);
            Console.WriteLine("Hello, World!");
        }
    }
}