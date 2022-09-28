using MySql.Data.MySqlClient;
using System.Text;

namespace Main.Util
{
    public class Database
    {
        private MySqlConnection _connection;

        public bool IsConnected => _connection.State == System.Data.ConnectionState.Open;

        public Database()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Server={Config.Database.Host};");
            stringBuilder.Append($"Database={Config.Database.DatabaseName};");
            stringBuilder.Append($"Uid={Config.Database.User};");
            stringBuilder.Append($"Pwd={Config.Database.Password};");
            _connection = new(stringBuilder.ToString());
        }

        public async Task Connect()
        {
            Logger.Info("Connecting to database...");
            await _connection.OpenAsync();
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                Logger.Warn("Not Connected to database.");
                return;
            }
            Logger.Info("Connected to database!");
            return;
        }

        public async Task Disconnect()
        {
            await _connection.CloseAsync();
        }
    }
}
