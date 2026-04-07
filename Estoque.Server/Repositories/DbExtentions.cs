using System.Data;

namespace Estoque.Server.Repositories;

public static class DbExtentions
{
    public static int GetInt32(this IDataReader reader, string column) =>
        reader.GetInt32(reader.GetOrdinal(column));

    public static decimal GetDecimal(this IDataReader reader, string column) =>
        reader.GetDecimal(reader.GetOrdinal(column));

    public static string GetString(this IDataReader reader, string column) =>
        reader.GetString(reader.GetOrdinal(column));

    public static bool GetBoolean(this IDataReader reader, string column) =>
        reader.GetBoolean(reader.GetOrdinal(column));

    public static Guid GetGuid(this IDataReader reader, string column) =>
        reader.GetGuid(reader.GetOrdinal(column));

    public static DateTime GetDateTime(this IDataReader reader, string column) =>
        reader.GetDateTime(reader.GetOrdinal(column));

    public static Guid? GetGuidNullable(this IDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetGuid(ordinal);
    }

    public static string GetStringSafe(this IDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }
}