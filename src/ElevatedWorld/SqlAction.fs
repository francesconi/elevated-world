namespace Npgsql.FSharp

module SqlAction =
    open Npgsql

    let run ctx (SqlAction action) = action ctx

    let retn x = SqlAction(fun _ -> async.Return x)

    let ok x = SqlAction(fun _ -> async.Return(Ok x))

    let bind f xActionResult =
        let newAction ctx =
            async {
                let! xResult = run ctx xActionResult

                let yAction =
                    match xResult with
                    | Ok x -> f x
                    | Error err -> (Error err) |> retn

                return! run ctx yAction
            }

        SqlAction newAction

    let defaultCtx conn = { Connection = conn }

    let execute connectionString action =
        async {
            use conn = new NpgsqlConnection(connectionString)
            do! conn.OpenAsync() |> Async.AwaitTask
            printfn "[SQL] Opening"
            let ctx = defaultCtx conn
            let! result = run ctx action
            do! conn.CloseAsync() |> Async.AwaitTask
            printfn "[SQL] Closing"
            return result
        }
