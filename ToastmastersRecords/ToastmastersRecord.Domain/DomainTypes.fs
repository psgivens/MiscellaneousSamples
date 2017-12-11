module ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.Infrastructure

type MemberMessage = string
type MessageId = FsGuidType
type RequestAbbreviation = string
type MemberId = FsGuidType
type TMMemberId = FsType<int>
type MeetingId = FsGuidType
type RoleRequestId = FsGuidType

// These values are mirrored in the database
type RoleTypeId = 
    | Toastmaster = 1
    | GeneralEvaluator = 2
    | TableTopicsMaster = 3
    | Evaluator = 4
    | Speaker = 5
    | OpeningThoughtAndBallotCounter = 6
    | ClosingThoughtAndGreeter = 7
    | JokeMaster = 8
    | ErAhCounter = 9
    | Grammarian = 10
    | Timer = 11
    | Videographer = 12

let category = 
    function
    | RoleTypeId.Toastmaster
    | RoleTypeId.GeneralEvaluator
    | RoleTypeId.TableTopicsMaster -> "Facilitator"
    | RoleTypeId.Evaluator
    | RoleTypeId.Speaker -> "Major"
    | RoleTypeId.OpeningThoughtAndBallotCounter 
    | RoleTypeId.ClosingThoughtAndGreeter 
    | RoleTypeId.JokeMaster -> "Minor"
    | RoleTypeId.ErAhCounter
    | RoleTypeId.Grammarian
    | RoleTypeId.Timer
    | RoleTypeId.Videographer -> "Functionary"
    | _ -> "other"


