using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Gifts.Repository.Helpers
{
    public class UpdateCommand : IDisposable
    {
        private List<string> setClauses = new List<string>();
        private SqlCommand sqlCommand;
        private readonly string idDbFieldName;
        private readonly int idDbFieldValue;

        public UpdateCommand(
            SqlConnection sqlConnection,
            string tableName,
            string idDbFieldName, int idDbFieldValue)
        {
            sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = $"UPDATE {tableName} SET ";

            this.idDbFieldName = idDbFieldName;
            this.idDbFieldValue = idDbFieldValue;
        }

        public void AddSetClause(string dbFieldName, INullable? dbFieldValue)
        {
            if (dbFieldValue is not null)
            {
                setClauses.Add($"{dbFieldName} = @{dbFieldName}");
                sqlCommand.Parameters.AddWithValue($"@{dbFieldName}", dbFieldValue);
            }
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            if (setClauses.Count == 0)
            {
                throw new Exception("No fields to update! You should pass at least one!");
            }

            sqlCommand.CommandText += string.Join(", ", setClauses);
            sqlCommand.CommandText += $" WHERE {idDbFieldName} = @{idDbFieldName}";

            sqlCommand.Parameters.AddWithValue($"@{idDbFieldName}", idDbFieldValue);

            SqlTransaction transaction = sqlCommand.Connection.BeginTransaction();
            sqlCommand.Transaction = transaction;

            try
            {
                int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                if (rowsAffected != 1)
                {
                    transaction.Rollback();
                    throw new Exception($"Just one row should be updated! Command aborted, because {rowsAffected} could have been updated!");
                }

                transaction.Commit();
                return rowsAffected;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            sqlCommand.Dispose();
        }
    }
}