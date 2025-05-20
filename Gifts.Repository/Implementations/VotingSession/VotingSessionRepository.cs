using Gifts.Models;
using Dapper;
using Gifts.Repository.Base;
using Gifts.Repository.Helpers;
using Gifts.Repository.Interfaces.VotingSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Gifts.Repository.Implementations.VotingSession
{
    public class VotingSessionRepository : BaseRepository<Models.VotingSession>, IVotingSessionRepository
    {
        private const string IdDbFieldEnumeratorName = "VotingSessionId";

        protected override string GetTableName()
        {
            return "VotingSessions";
        }

        protected override string[] GetColumns() => new[]
        {
            IdDbFieldEnumeratorName,
            "BirthdayPersonId",
            "CreatedById",
            "StartDate",
            "EndDate",
            "IsActive",
            "BirthYear"
        };

        protected override Models.VotingSession MapEntity(SqlDataReader reader)
        {
            return new Models.VotingSession
            {
                VotingSessionId = Convert.ToInt32(reader["VotingSessionId"]),
                BirthdayPersonId = Convert.ToInt32(reader["BirthdayPersonId"]),
                CreatedById = Convert.ToInt32(reader["CreatedById"]),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = !reader.IsDBNull(reader.GetOrdinal("EndDate"))
                    ? Convert.ToDateTime(reader["EndDate"])
                    : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                BirthYear = Convert.ToInt32(reader["BirthYear"])
            };
        }

        public Task<int> CreateAsync(Models.VotingSession entity)
        {
            return base.CreateAsync(entity, IdDbFieldEnumeratorName);
        }

        public Task<Models.VotingSession> RetrieveAsync(int objectId)
        {
            return base.RetrieveAsync(IdDbFieldEnumeratorName, objectId);
        }

        public IAsyncEnumerable<Models.VotingSession> RetrieveCollectionAsync(VotingSessionFilter filter)
        {
            Filter commandFilter = new Filter();

            if (filter.CreatedById is not null)
            {
                commandFilter.AddCondition("CreatedById", filter.CreatedById);
            }
            if (filter.BirthdayPersonId is not null)
            {
                commandFilter.AddCondition("BirthdayPersonId", filter.BirthdayPersonId);
            }
            if (filter.IsActive is not null)
            {
                commandFilter.AddCondition("IsActive", filter.IsActive);
            }

            return base.RetrieveCollectionAsync(commandFilter);
        }

        public async Task<bool> UpdateAsync(int objectId, VotingSessionUpdate update)
        {
            using SqlConnection connection = await ConnectionFactory.Connect();

            UpdateCommand updateCommand = new UpdateCommand(
                connection,
                "VotingSessions",
                IdDbFieldEnumeratorName, objectId);

            updateCommand.AddSetClause("StartDate", update.StartDate);
            updateCommand.AddSetClause("EndDate", update.EndDate);
            updateCommand.AddSetClause("IsActive", update.IsActive);

            return await updateCommand.ExecuteNonQueryAsync() > 0;
        }

        public Task<bool> DeleteAsync(int objectId)
        {
            throw new NotImplementedException();
        }
    }
} 