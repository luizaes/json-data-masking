namespace JsonDataMasking.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class SensitiveDataAttribute : Attribute
    {
        public bool PreserveLength;

        public int ShowFirst;

        public int ShowLast;

        public string? SubstituteText;

        public string Mask;

        public SensitiveDataAttribute(bool preserveLength = true, int showFirst = 0, int showLast = 0,
            string? substituteText = null, string? mask = null)
        {
            PreserveLength = preserveLength;
            ShowFirst = showFirst;
            ShowLast = showLast;
            SubstituteText = substituteText;
            Mask = mask ?? "*";
        }
    }
}
