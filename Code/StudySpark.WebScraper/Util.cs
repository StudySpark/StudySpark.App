using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.WebScraper {
    public class Util {

        public static string ExtractBaseDomain(string domain) {
            // Split the domain by dots
            string[] parts = domain.Split('.');

            // If the domain has at least two parts, take the last two parts
            if (parts.Length >= 2) {
                return $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}";
            }

            // If the domain has only one part, return that part
            if (parts.Length == 1) {
                return parts[0];
            }

            // Invalid domain format, return empty string or throw an exception based on your requirements
            return string.Empty;
        }
    }
}
