using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PrisonApp.Models;

namespace PrisonApp.Data
{
    public class PrisonerRepository : IPrisonerRepository
    {
        private readonly string _connectionString;

        public PrisonerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddPrisoner(Prisoner prisoner)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Prisoners
(PrisonerNumber, NationalId, FullName, Gender, BirthDate, Nationality,
 LegalStatus, EntryDate, ReleaseDate, CrimeDescription,
 CurrentBlock, CurrentRoom, CreatedBy, Notes, AttachmentsPath, IsArchived)
VALUES
(@PrisonerNumber, @NationalId, @FullName, @Gender, @BirthDate, @Nationality,
 @LegalStatus, @EntryDate, @ReleaseDate, @CrimeDescription,
 @CurrentBlock, @CurrentRoom, @CreatedBy, @Notes, @AttachmentsPath, 0);

SELECT SCOPE_IDENTITY();";

                cmd.Parameters.AddWithValue("@PrisonerNumber", (object)prisoner.PrisonerNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NationalId", prisoner.NationalId);
                cmd.Parameters.AddWithValue("@FullName", prisoner.FullName);
                cmd.Parameters.AddWithValue("@Gender", prisoner.Gender);
                cmd.Parameters.AddWithValue("@BirthDate", prisoner.BirthDate);
                cmd.Parameters.AddWithValue("@Nationality", prisoner.Nationality);
                cmd.Parameters.AddWithValue("@LegalStatus", prisoner.LegalStatus);
                cmd.Parameters.AddWithValue("@EntryDate", prisoner.EntryDate);
                cmd.Parameters.AddWithValue("@ReleaseDate", (object?)prisoner.ReleaseDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CrimeDescription", (object)prisoner.CrimeDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrentBlock", (object)prisoner.CurrentBlock ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrentRoom", (object)prisoner.CurrentRoom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object)prisoner.CreatedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)prisoner.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttachmentsPath", (object)prisoner.AttachmentsPath ?? DBNull.Value);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public void UpdatePrisoner(Prisoner prisoner)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Prisoners SET
    PrisonerNumber = @PrisonerNumber,
    NationalId = @NationalId,
    FullName = @FullName,
    Gender = @Gender,
    BirthDate = @BirthDate,
    Nationality = @Nationality,
    LegalStatus = @LegalStatus,
    EntryDate = @EntryDate,
    ReleaseDate = @ReleaseDate,
    CrimeDescription = @CrimeDescription,
    CurrentBlock = @CurrentBlock,
    CurrentRoom = @CurrentRoom,
    CreatedBy = @CreatedBy,
    Notes = @Notes,
    AttachmentsPath = @AttachmentsPath
WHERE PrisonerId = @PrisonerId AND IsArchived = 0;";

                cmd.Parameters.AddWithValue("@PrisonerId", prisoner.PrisonerId);
                cmd.Parameters.AddWithValue("@PrisonerNumber", (object)prisoner.PrisonerNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NationalId", prisoner.NationalId);
                cmd.Parameters.AddWithValue("@FullName", prisoner.FullName);
                cmd.Parameters.AddWithValue("@Gender", prisoner.Gender);
                cmd.Parameters.AddWithValue("@BirthDate", prisoner.BirthDate);
                cmd.Parameters.AddWithValue("@Nationality", prisoner.Nationality);
                cmd.Parameters.AddWithValue("@LegalStatus", prisoner.LegalStatus);
                cmd.Parameters.AddWithValue("@EntryDate", prisoner.EntryDate);
                cmd.Parameters.AddWithValue("@ReleaseDate", (object?)prisoner.ReleaseDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CrimeDescription", (object)prisoner.CrimeDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrentBlock", (object)prisoner.CurrentBlock ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrentRoom", (object)prisoner.CurrentRoom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object)prisoner.CreatedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)prisoner.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttachmentsPath", (object)prisoner.AttachmentsPath ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ArchivePrisoner(int prisonerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Prisoners
SET IsArchived = 1
WHERE PrisonerId = @PrisonerId;";

                cmd.Parameters.AddWithValue("@PrisonerId", prisonerId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Prisoner GetById(int prisonerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT * FROM Prisoners
WHERE PrisonerId = @PrisonerId AND IsArchived = 0;";

                cmd.Parameters.AddWithValue("@PrisonerId", prisonerId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return MapPrisoner(reader);
                }
            }
        }

        public IEnumerable<Prisoner> GetAll()
        {
            var list = new List<Prisoner>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Prisoners WHERE IsArchived = 0;";
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapPrisoner(reader));
                    }
                }
            }

            return list;
        }

        public IEnumerable<Prisoner> Search(
            string fullName = null,
            string nationalId = null,
            string prisonerNumber = null,
            string legalStatus = null,
            string block = null,
            string nationality = null,
            DateTime? entryDateFrom = null,
            DateTime? entryDateTo = null)
        {
            var list = new List<Prisoner>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                var conditions = new List<string>();
                conditions.Add("IsArchived = 0");

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    conditions.Add("FullName LIKE @FullName");
                    cmd.Parameters.AddWithValue("@FullName", "%" + fullName + "%");
                }
                if (!string.IsNullOrWhiteSpace(nationalId))
                {
                    conditions.Add("NationalId = @NationalId");
                    cmd.Parameters.AddWithValue("@NationalId", nationalId);
                }
                if (!string.IsNullOrWhiteSpace(prisonerNumber))
                {
                    conditions.Add("PrisonerNumber = @PrisonerNumber");
                    cmd.Parameters.AddWithValue("@PrisonerNumber", prisonerNumber);
                }
                if (!string.IsNullOrWhiteSpace(legalStatus))
                {
                    conditions.Add("LegalStatus = @LegalStatus");
                    cmd.Parameters.AddWithValue("@LegalStatus", legalStatus);
                }
                if (!string.IsNullOrWhiteSpace(block))
                {
                    conditions.Add("CurrentBlock = @Block");
                    cmd.Parameters.AddWithValue("@Block", block);
                }
                if (!string.IsNullOrWhiteSpace(nationality))
                {
                    conditions.Add("Nationality = @Nationality");
                    cmd.Parameters.AddWithValue("@Nationality", nationality);
                }
                if (entryDateFrom.HasValue)
                {
                    conditions.Add("EntryDate >= @EntryDateFrom");
                    cmd.Parameters.AddWithValue("@EntryDateFrom", entryDateFrom.Value.Date);
                }
                if (entryDateTo.HasValue)
                {
                    conditions.Add("EntryDate <= @EntryDateTo");
                    cmd.Parameters.AddWithValue("@EntryDateTo", entryDateTo.Value.Date);
                }

                cmd.CommandText = "SELECT * FROM Prisoners WHERE " + string.Join(" AND ", conditions);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapPrisoner(reader));
                    }
                }
            }

            return list;
        }

        public IEnumerable<Prisoner> GetByLegalStatus(string legalStatus)
        {
            return Search(legalStatus: legalStatus);
        }

        public IEnumerable<Prisoner> GetReleasedBetween(DateTime from, DateTime to)
        {
            var list = new List<Prisoner>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT * FROM Prisoners
WHERE IsArchived = 0
  AND ReleaseDate IS NOT NULL
  AND ReleaseDate BETWEEN @From AND @To;";

                cmd.Parameters.AddWithValue("@From", from.Date);
                cmd.Parameters.AddWithValue("@To", to.Date);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapPrisoner(reader));
                    }
                }
            }

            return list;
        }

        private Prisoner MapPrisoner(IDataRecord reader)
        {
            return new Prisoner
            {
                PrisonerId = (int)reader["PrisonerId"],
                PrisonerNumber = reader["PrisonerNumber"] as string,
                NationalId = reader["NationalId"] as string,
                FullName = reader["FullName"] as string,
                Gender = reader["Gender"] as string,
                BirthDate = (DateTime)reader["BirthDate"],
                Nationality = reader["Nationality"] as string,
                LegalStatus = reader["LegalStatus"] as string,
                EntryDate = (DateTime)reader["EntryDate"],
                ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? null : (DateTime?)reader["ReleaseDate"],
                CrimeDescription = reader["CrimeDescription"] as string,
                CurrentBlock = reader["CurrentBlock"] as string,
                CurrentRoom = reader["CurrentRoom"] as string,
                CreatedBy = reader["CreatedBy"] as string,
                Notes = reader["Notes"] as string,
                AttachmentsPath = reader["AttachmentsPath"] as string
            };
        }
    }
}
