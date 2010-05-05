-- Default Member (password = Akshar2010)
SET IDENTITY_INSERT [akshcore].[dbo].[Members] ON
GO
INSERT INTO [akshcore].[dbo].[Members]([UserId],[UserName],[Password],[Email],[FullName],[CreatedON],[FromIP],[IsDeleted]) VALUES(5,N'Varun',N'9B19477787F9FD16D847427CFEC2B5B5',N'varunkhosla@outlook.com',N'Varun Khosla',CAST(0x9D6602E4 AS SmallDateTime),N'59.180.83.107',0)
GO
SET IDENTITY_INSERT [akshcore].[dbo].[Members] OFF
GO

-- Languages
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'AR-IN', N'Arabic', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'AS', N'Assamese', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'BH', N'Bihari', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'BN-IN', N'Bengali', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'DE', N'German', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'EN', N'English', 5)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'FR', N'French', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'GU', N'Gujarati', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'HI', N'Hindi', 2)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'KN', N'Kannada', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'KOK', N'Konkani', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'KS', N'Kashmiri', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'ML', N'Malayalam', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'MR', N'Marathi', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'NE', N'Nepali', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'OR', N'Oriya', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'PA', N'Panjabi', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'PI', N'Pali', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'SA', N'Sanskrit', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'SD-IN', N'Sindhi', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'SI', N'Sinhala', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'TA-IN', N'Tamil', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'TE', N'Telugu', 1)
INSERT [dbo].[Languages] ([LangId], [Language], [Status]) VALUES (N'UR-IN', N'Urdu', 1)
insert into languages
values('RU', 'Russian', 1)

-- InputScripts
SET IDENTITY_INSERT [dbo].[InputScripts] ON
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (1, N'Devanagari', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (2, N'Arabic', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (3, N'Bengali', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (4, N'Gujarati', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (5, N'Gurmukhi', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (6, N'Kannada', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (7, N'Malayalam', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (8, N'Oriya', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (9, N'Sinhala', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (10, N'Tamil', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (11, N'Telugu', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (12, N'Latin', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (13, N'Cyrillic', NULL)
INSERT [dbo].[InputScripts] ([ScriptId], [Script], [LocalName]) VALUES (14, N'Basic Latin', NULL)
SET IDENTITY_INSERT [dbo].[InputScripts] OFF


-- IScriptLangs
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'BH')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'HI ')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'KOK')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'KS')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'MR')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'NE')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'PA')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'PI')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (1, N'SA')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (2, N'AR-IN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (2, N'PA')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (2, N'SD-IN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (2, N'UR-IN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (3, N'AS')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (3, N'BN-IN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (4, N'GU')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (5, N'PA')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (6, N'KN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (7, N'ML')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (8, N'OR')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (9, N'SI')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (10, N'TA-IN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (11, N'TE')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (13, N'RU')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (14, N'EN')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (14, N'FR')
INSERT [dbo].[IScriptLangs] ([ScriptId], [LangId]) VALUES (14, N'DE')

SET IDENTITY_INSERT [akshcore].[dbo].[VKLs] ON
GO

INSERT INTO [akshcore].[dbo].[VKLs]
([VKLId],[LangCode],[Name],[Type],[userId],[Visibility])
VALUES(1,N'HI ',N'Hindi',1,1,1),
(2,N'AR-IN',N'Arabic-Arabic',1,1,1),
(3,N'RU',N'Russian',1,1,1),
(4,N'FR ',N'French',1,1,1),
(5,N'BN-IN',N'Bengali',1,1,1),
(6,N'TA-IN',N'Tamil',1,1,1),
(7,N'KN',N'Kannada',1,1,1),
(8,N'BH',N'Bihari',1,1,1),
(9,N'AS',N'Assamese',1,1,1),
--Incomplete:
(10,N'TE',N'Telugu',1,1,0),
(11,N'EN ',N'English',1,1,0);
GO

SET IDENTITY_INSERT [akshcore].[dbo].[VKLs] OFF
GO

