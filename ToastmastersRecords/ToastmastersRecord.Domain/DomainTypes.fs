module ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.Infrastructure

type MemberMessage = string
type MessageId = FsGuidType
type RequestAbbreviation = string
type RoleTypeId = FsType<int>
type MemberId = FsGuidType
type TMMemberId = FsType<int>
type MeetingId = FsGuidType
type RoleRequestId = FsGuidType

