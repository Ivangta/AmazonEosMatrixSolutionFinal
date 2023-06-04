using System.Text.RegularExpressions;

namespace AmazonEosMatrixSolution.Helpers
{
    public class Helpers
    {
        public string RemoveWhiteSpacesAddDecimalPointUsingRegex(string source)
        {
            var newSourceFormat = source.Insert(source.Length - 2, ".");
            var finalSourceFormat = Regex.Replace(newSourceFormat, @"\s", string.Empty);
            return finalSourceFormat;
        }

        public string RemoveNonNumericValuesUsingRegex(string source)
        {
            return Regex.Replace(source, "[^\\d.]", string.Empty);
        }
    }
}
