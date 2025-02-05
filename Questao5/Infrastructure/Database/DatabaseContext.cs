using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using System.Data.SqlClient;

namespace Questao5.Infrastructure.Database
{
    public class DatabaseContext
    {
        private readonly DatabaseConfig _databaseConfig;

        public DatabaseContext(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        public IDbConnection CreateConnection() => new SqliteConnection(_databaseConfig.Name);
    }
}
