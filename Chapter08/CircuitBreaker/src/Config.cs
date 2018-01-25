namespace CircuitBreakerSample 
{
    public static class Config 
    {
        public static string DbName => "CBDatabase";
        public static string DbUrl => "127.0.0.1";
        public static int CircuitOpenTimeout => 4000;
        public static int CircuitClosedErrorLimit = 6;
    }
}