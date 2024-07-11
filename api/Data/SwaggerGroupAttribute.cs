namespace api.Data
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerGroupAttribute : Attribute
    {
        public string[] GroupNames { get; }

        public SwaggerGroupAttribute(params string[] groupNames)
        {
            GroupNames = groupNames;
        }
    }
}
