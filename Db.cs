using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ReconPanel;

public static class Db
{
    static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recon.db");

    public static SqliteConnection Open()
    {
        var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        Init(conn);
        return conn;
    }

    static void Init(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS wordlists (
                name TEXT PRIMARY KEY,
                size INTEGER,
                charset TEXT,
                content TEXT,
                created TEXT
            )";
        cmd.ExecuteNonQuery();
    }
}
