module ToastmastersRecord.Domain.CommandHandler

open ToastmastersRecord.Domain.Infrastructure

(*** Infrastructure ***)
//type CommandHandlerState<'a, 'b> = (Version * 'a list * 'b option)
//type CommandHandlerFunction<'a, 'b> = (CommandHandlerState<'a, 'b> -> Async<CommandHandlerState<'a, 'b>>)
type CommandHandlerFunction<'b> = ('b -> Async<'b>)

type CommandHandlerBuilder<'a, 'b> (raise:'b -> 'a -> 'b) =
    member this.Bind ((result:Async<'a>), (rest:unit -> CommandHandlerFunction<'b>)) =
        fun version -> 
            async {
                let! event = result                
                return! (rest ()) (raise version event)
            }
    member this.Return (result:Async<'a>) = 
        fun version -> 
            async { 
                let! event = result
                return raise version event
            }
let raise event = async { return event }

type CommandHandlers<'a,'b> (raiseVersionedEvent:'b -> 'a -> 'b) =
    member this.block = CommandHandlerBuilder raiseVersionedEvent
    member this.event event = this.block { return event |> raise }

