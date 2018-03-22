using System.Linq;
using System.Net.Http.Headers;

namespace DontMergeMeYet.Extensions
{
    static class HttpHeadersExtensions
    {
        public static string GetValueOrDefault(this HttpHeaders headers, string name)
        {
            if (headers.TryGetValues(name, out var values))
                return values.FirstOrDefault();
            return null;
        }
    }
}
