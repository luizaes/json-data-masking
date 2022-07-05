using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDataMasking
{
    [AttributeUsage(AttributeTargets.All)]
    public class SensitiveDataAttribute : Attribute
    {
        public bool PreserveLength;

        public int ShowFirst;

        public int ShowLast;

        public string? Text;

        public string Mask;

        public SensitiveDataAttribute(bool preserveLength = true, int showFirst = 0, int showLast = 0,
            string? text = null, string? mask = null)
        {
            PreserveLength = preserveLength;
            ShowFirst = showFirst;
            ShowLast = showLast;
            Text = text;
            Mask = mask ?? "*";
        }
    }
}
