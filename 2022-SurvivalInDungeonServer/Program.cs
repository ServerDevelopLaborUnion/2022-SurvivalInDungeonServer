using Main.Util;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Server is running...");
            MainServer.Instance.StartUp().Wait();
        }
    }
}