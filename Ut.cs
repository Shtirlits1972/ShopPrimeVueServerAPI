namespace ShopPrimeVueServerAPI
{
    public class Ut
    {
        private static readonly ConfigurationBuilder builder = (ConfigurationBuilder)new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        private static IConfiguration Configuration = builder.Build();

        public static string GetConnetString()
        {
            return Configuration.GetConnectionString("SqlConnString")!;
        }
    }
}
