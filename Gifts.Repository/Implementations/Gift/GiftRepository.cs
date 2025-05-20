using Gifts.Models;
using Dapper;
using Gifts.Repository.Base;
using Gifts.Repository.Helpers;
using Gifts.Repository.Interfaces.Gift;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Gifts.Repository.Implementations.Gift
{
    public class GiftRepository : BaseRepository<Models.Gift>, IGiftRepository
    {
        private const string IdDbFieldEnumeratorName = "GiftId";

        protected override string GetTableName()
        {
            return "Gifts";
        }
        protected override string[] GetColumns() => new[]
        {
            IdDbFieldEnumeratorName,
            "Name",
            "Description",
            "Price"
        };

        protected override Models.Gift MapEntity(SqlDataReader reader)
        {
            return new Models.Gift
            {
                GiftId = Convert.ToInt32(reader[IdDbFieldEnumeratorName]),
                Name = Convert.ToString(reader["Name"]),
                Description = !reader.IsDBNull(reader.GetOrdinal("Description"))
                ? Convert.ToString(reader["Description"])
                : null,
                Price = Convert.ToDecimal(reader["Price"])
            };
        }

        public Task<int> CreateAsync(Models.Gift entity)
        {
            return base.CreateAsync(entity, IdDbFieldEnumeratorName);
        }

        public Task<Models.Gift> RetrieveAsync(int objectId)
        {
            return base.RetrieveAsync(IdDbFieldEnumeratorName, objectId);
        }

        public IAsyncEnumerable<Models.Gift> RetrieveCollectionAsync(GiftFilter filter)
        {
            Filter commandFilter = new Filter();

            if (filter.Name is not null)
            {
                commandFilter.AddCondition("Name", filter.Name);
            }

            return base.RetrieveCollectionAsync(commandFilter);
        }

        public async Task<bool> UpdateAsync(int objectId, GiftUpdate update)
        {
            using SqlConnection connection = await ConnectionFactory.Connect();

            UpdateCommand updateCommand = new UpdateCommand(
                connection,
                "Gifts",
                IdDbFieldEnumeratorName, objectId);

            updateCommand.AddSetClause("Name", update.Name);
            updateCommand.AddSetClause("Desription", update.Desription);
            updateCommand.AddSetClause("Price", update.Price);

            return await updateCommand.ExecuteNonQueryAsync() > 0;
        }

        public Task<bool> DeleteAsync(int objectId)
        {
            return base.DeleteAsync(IdDbFieldEnumeratorName, objectId);
        }
    }
} 