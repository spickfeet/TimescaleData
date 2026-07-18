namespace WebAPI.Const
{
    public class ServiceRoutes
    {
        private const string Api = "/api";
        public static class Timescale
        {
            public const string Base = Api + "/timescale";
            public const string Upload = Base + "/upload";
            public const string Results = Base + "/results";
            public const string ValuesByFileName = Base + "/values/{fileName}";
        }
    }
}
