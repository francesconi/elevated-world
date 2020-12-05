namespace Npgsql.FSharp

open Npgsql

type SqlContext = { Connection: NpgsqlConnection }

type SqlAction<'a> = SqlAction of (SqlContext -> Async<Result<'a, exn>>)
