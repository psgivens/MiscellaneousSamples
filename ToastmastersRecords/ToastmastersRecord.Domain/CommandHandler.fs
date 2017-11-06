module ToastmastersRecord.Domain.CommandHandler

(*** Infrastructure ***)
type CommandHandlerState<'a, 'b> = (int16 * 'a list * 'b option)
type CommandHandlerFunction<'a, 'b> = (CommandHandlerState<'a, 'b> -> Async<'a * CommandHandlerState<'a, 'b>>)
//type CommandHandler<'a> = CommandHandlers<'a> -> 'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<'a>
type CommandHandlerBuilder<'a, 'b> (raise:'b option -> int16 -> 'a -> 'b option) =    
    member this.Bind ((result:Async<'a>), (rest:unit -> CommandHandlerFunction<'a, 'b>)) =
        fun (version, history, state) -> 
            async {
                let! event = result
                let newState = raise state version event
                let value = (version + 1s, event::history, newState)
                return! (rest ()) value
            }
    member this.Return (result:Async<'a>) = 
        fun (version, history, state) -> 
            async { 
                let! event = result
                let newState = raise state version event
                return event, (version + 1s, event::history, newState)
            }
let raise event = async { return event }
//let commandHandler = CommandHandlerBuilder 
type CommandHandlers<'a, 'b> (raiseVersionedEvent:'b option -> int16 -> 'a -> 'b option) =
    member this.block = CommandHandlerBuilder raiseVersionedEvent
    member this.event event = this.block { return event |> raise }

