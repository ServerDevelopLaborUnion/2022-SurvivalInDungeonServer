namespace Main
{
    public static class Config
    {
        public static class Server
        {
            public const int Port = 3000;
            public const int BufferSize = 1024;
            public const uint MaxPacketSize = 1024 * 1024;
        }

        public static class Database
        {
            public const string Host = "localhost";
            public const string DatabaseName = "SID3D";
            public const string User = "root";
            public const string Password = "qmfdnjs613";
        }
    }
}
