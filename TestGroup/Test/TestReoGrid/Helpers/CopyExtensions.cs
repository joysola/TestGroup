using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Helpers
{
    public static class CopyExtensions
    {
        private static readonly Mapper _mapper = new(/*new Mapster.TypeAdapterConfig()*/);

        public static T DeepMap<T>(this T obj) where T : class
        {
            if (obj is not null)
            {
                return _mapper.Map<T>(obj);
            }
            return null;
        }
    }
}
