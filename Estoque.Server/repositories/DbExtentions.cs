using System.Data;

namespace Estoque.Repositories;

public static class DbExtentions
{
    public static int GetInt32(this IDataReader reader, string column) =>
        reader.GetInt32(reader.GetOrdinal(column));

    public static string GetString(this IDataReader reader, string column) =>
        reader.GetString(reader.GetOrdinal(column));

    public static bool GetBoolean(this IDataReader reader, string column) =>
        reader.GetBoolean(reader.GetOrdinal(column));
}