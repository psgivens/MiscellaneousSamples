module ToastmastersRecord.Domain.CommandHandler

(*** Infrastructure ***)
type CommandHandlerState<'a> = (int16 * 'a list)
type CommandHandlerFunction<'a> = (CommandHandlerState<'a> -> Async<'a * CommandHandlerState<'a>>)
//type CommandHandler<'a> = CommandHandlers<'a> -> 'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<'a>
type CommandHandlerBuilder<'a> (raise:int16 -> 'a -> unit) =    
    member this.Bind ((result:Async<'a>), (rest:unit -> CommandHandlerFunction<'a>)) =
        fun (version, history) -> 
            async {
                let! event = result
                raise version event
                let state = (version + 1s, event::history)
                return! (rest ()) state
            }
    member this.Return (result:Async<'a>) = 
        fun (version, history) -> 
            async { 
                let! event = result
                raise version event
                return event, (version + 1s, event::history)
            }
let raise event = async { return event }
//let commandHandler = CommandHandlerBuilder 
type CommandHandlers<'a> (raiseVersionedEvent:int16 -> 'a -> unit) =
    member this.block = CommandHandlerBuilder raiseVersionedEvent
    member this.event event = this.block { return event |> raise }

