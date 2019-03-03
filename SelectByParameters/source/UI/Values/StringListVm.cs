namespace SelectByParameters.UI.Values
{
    using System.Collections.Generic;

    public class StringListVm
    {
        public StringListVm(List<string> values)
        {
            Values = values;
        }

        public List<string> Values { get; set; }
        public string Value { get; set; }
    }
}
