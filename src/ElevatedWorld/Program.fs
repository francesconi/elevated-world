open Npgsql.FSharp

let [<Literal>] InsertPrize = "
    INSERT INTO prizes (year, category, firstname, surname)
    VALUES (@year, @category, @firstname, @surname)
"

let [<Literal>] DeletePrizes = "
    DELETE FROM prizes
"

let [<Literal>] SelectPrizes = "
    SELECT * FROM prizes
    ORDER BY year DESC
"

let setupData = SqlAction (fun ctx ->
    ctx.Connection
    |> Sql.existingConnection
    |> Sql.executeTransactionAsync [
        DeletePrizes, []
        InsertPrize, [
            [ "@year", Sql.int 2016
              "@category", Sql.text "physics"
              "@firstname", Sql.text "David J."
              "@surname", Sql.text "Thouless" ]
            [ "@year", Sql.int 2015
              "@category", Sql.text "medicine"
              "@firstname", Sql.text "William C."
              "@surname", Sql.text "Campbell" ]
            [ "@year", Sql.int 2014
              "@category", Sql.text "chemistry"
              "@firstname", Sql.text "Eric"
              "@surname", Sql.text "Betzig" ]
        ]
    ]
)

let queryData =
    SqlAction (fun ctx ->
        ctx.Connection
        |> Sql.existingConnection
        |> Sql.query SelectPrizes
        |> Sql.executeAsync (fun read ->
            {|
                Year = read.int "year"
                Category = read.text "category"
                Firstname = read.text "firstname"
                Surname = read.text "surname"
            |})
    )

let queryAndPrintData = sql {
    let! prizes = queryData

    // for prize in prizes do
    //     printfn "%A" prize

    prizes
    |> List.iter (printfn "%A")

    // do! queryData
    //     |> SqlAction.map (printfn "%A")

    return ()
}

[<EntryPoint>]
let main argv =
    let connectionString = "Host=localhost;Port=9092;Database=nobel;Username=postgres;Password=postgres"
    let execute action =
        SqlAction.execute connectionString action
        |> Async.RunSynchronously

    sql {
        printfn "Seeding data..."
        let! _ = setupData
        printfn "Querying data..."
        do! queryAndPrintData
        return ()
    }
    |> execute
    |> ignore

    0