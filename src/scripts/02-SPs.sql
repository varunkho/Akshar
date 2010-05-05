
create procedure [dbo].[Brief_UserInfo]
@userName varchar(15)
as
select UserId, UserName, Password
from Members
where UserName = @userName;


GO



create procedure [dbo].[New_VKL]
@langCode varchar(6),
@name varchar(25),
@userId int,
@type tinyint,
@visibility tinyint
as
insert into VKLs
(LangCode,Name,userId,Type,Visibility)
values(@langCode,@name,@userId,@type,@visibility)

select SCOPE_IDENTITY()


GO




create procedure [dbo].[sel_inscripts]
@LangId varchar(6)
as
if (@LangId is null)
select ScriptId, Script from InputScripts
else
select ins.ScriptId, Script from IScriptLangs isl, InputScripts ins
where isl.ScriptId = ins.ScriptId and isl.LangId = @LangId
GO



create procedure [dbo].[Sel_UserVKLs]
@userId int
as
select VKLId, Name, Type, Visibility from VKLs
where userId = @userId

GO





create procedure [dbo].[Sel_VKL]
@Id int = null,
@Name varchar(25) = null,
@type tinyint = null
as
select * from VKLs
where (@Id is not null and VKLId = @Id) 
or (@Name is not null and Name = @Name and Type = @type)



GO




create procedure [dbo].[Sel_VKLs](
@userId int = null,
@admin bit = 0
)
as
select  VKLId as id, rtrim(langCode) as lcode, name, type, Visibility as status
from VKLs
-- Admin will see all the records.
where Visibility in (1,2) or @admin = 1 or (@userId is not null and userId = @userId)
order by LangCode asc, status desc, Name asc

GO



create procedure [dbo].[UserExists]
@userName varchar(15)
as
declare @uid int;
set @uid = -1;
select @uid = UserId
from Members
where UserName = @userName;

select @uid
GO

create procedure Upd_VKLVisibility
(
@id int,
@newVisibility int,
@userId int = null
)
as
update VKLs
set Visibility = @newVisibility
where VKLId = @id
and (@userId is null or userId = @userId)

GO

