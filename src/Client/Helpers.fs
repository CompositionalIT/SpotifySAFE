namespace global

/// Represents a piece of state that is created asynchronously.
type Deferred<'T> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 'T
    member this.IsLoading =
        this
        |> function
        | InProgress -> true
        | _ -> false

/// Represents an asynchronous operation that returns some 'T
type AsyncOp<'T> =
    | Started
    | Completed of 'T

[<AutoOpen>]
module Extensions =
    type Fable.FontAwesome.Fa.IconOption with
        member this.Value =
            match this with
            | Fable.FontAwesome.Fa.IconOption.Icon s -> s
            | _ -> ""