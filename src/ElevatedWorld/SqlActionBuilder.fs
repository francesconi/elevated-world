namespace Npgsql.FSharp

type SqlActionBuilder() =
    member __.Zero(): SqlAction<unit> = SqlAction.ok ()

    member __.Delay(generator: unit -> SqlAction<'T>): SqlAction<'T> =
        SqlAction(fun ctx -> async.Delay(fun () -> SqlAction.run ctx (generator ())))

    member __.Return(value: 'T): SqlAction<'T> = SqlAction.ok value
    
    member __.ReturnFrom(computation: SqlAction<'T>): SqlAction<'T> = computation

    member __.Bind(computation: SqlAction<'T>, binder: 'T -> SqlAction<'U>): SqlAction<'U> =
        SqlAction.bind binder computation

    member __.TryFinally(SqlAction asyncResult, compensation: unit -> unit): SqlAction<'a> =
        SqlAction(fun ctx -> async.TryFinally(asyncResult ctx, compensation))

    member self.Using(resource: 'T, binder: 'T -> SqlAction<'a>): SqlAction<'a> when 'T :> System.IDisposable =
        self.TryFinally(binder resource, (fun _ -> resource.Dispose()))

    member self.While(guard: unit -> bool, computation: SqlAction<unit>): SqlAction<unit> =
        if guard () then
            let mutable whileSql = Unchecked.defaultof<_>
            whileSql <- self.Bind(computation, (fun () -> if guard () then whileSql else self.Zero()))
            whileSql
        else
            self.Zero()

    member self.For(sequence: seq<'T>, body: 'T -> SqlAction<unit>): SqlAction<unit> =
        self.Using
            (sequence.GetEnumerator(),
             (fun ie -> self.While((fun () -> ie.MoveNext()), self.Delay(fun () -> body ie.Current))))

    member __.Combine(computation1: SqlAction<'T>, computation2: SqlAction<'U>): SqlAction<'U> =
        computation1
        |> SqlAction.bind (fun _ -> computation2 |> SqlAction.map id)

// member inline __.TryWith(SqlAction asyncResult, catchHandler: exn -> SqlAction<'a>): SqlAction<'a> =
//     SqlAction(fun ctx -> async.TryWith(asyncResult ctx, (fun exn -> SqlAction.run ctx (catchHandler exn))))

[<AutoOpen>]
module SqlActionBuilderImpl =
    let sql = SqlActionBuilder()
