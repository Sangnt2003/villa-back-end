namespace DACN_VILLA.Helper
{
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }

        // Constructor
        public Error(string code, string description)
        {
            Code = code;
            Description = description;
        }

        // If needed, you could also provide a parameterless constructor
        public Error() { }
    }

}
