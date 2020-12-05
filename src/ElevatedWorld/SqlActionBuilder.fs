namespace Npgsql.FSharp

type SqlActionBuilder() =
    member inline __.Return(value: 'T): SqlAction<'T> = SqlAction.ok value

    member inline __.ReturnFrom(computation: SqlAction<'T>): SqlAction<'T> = computation

    member inline __.Bind(computation: SqlAction<'T>, binder: 'T -> SqlAction<'U>): SqlAction<'U> =
        SqlAction.bind binder computation

[<AutoOpen>]
module SqlActionBuilderImpl =
    let sql = SqlActionBuilder()
