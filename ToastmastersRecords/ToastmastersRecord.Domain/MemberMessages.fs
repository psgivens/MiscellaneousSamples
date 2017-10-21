module ToastmastersRecord.Domain.MemberMessages

open ToastmastersRecord.Domain.DomainTypes

type MemberMessageCommand =
    | Create of MemberId * MemberMessage
