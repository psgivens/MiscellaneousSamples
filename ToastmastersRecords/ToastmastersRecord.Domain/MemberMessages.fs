module ToastmastersRecord.Domain.MemberMessages

open System
open ToastmastersRecord.Domain.DomainTypes

type MemberMessageCommand =
    | Create of MemberId * DateTime * MemberMessage
