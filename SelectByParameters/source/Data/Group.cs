namespace SelectByParameters.Data
{
    using System.Collections.Generic;

    public class Group
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
