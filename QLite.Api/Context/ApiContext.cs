namespace QLiteDataApi.Context
{
    public class ApiContext
    {
        public static IConfiguration Config { get; set; }
        public static string Env { get; set; } = "Production";
    }
}
