-- This file contains strings for data annotations.

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageRequiredEmail')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageRequiredEmail',
		N'Email is required.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageRequiredEmail')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageRequiredEmail',
		N'Емэйл обязателен.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationValidationMessageEmailAddress')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationValidationMessageEmailAddress',
		N'Email address format.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationValidationMessageEmailAddress')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationValidationMessageEmailAddress',
		N'Формат Емэйл.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationValidationMessagePhoneNumber')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationValidationMessagePhoneNumber',
		N'Phone number format.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationValidationMessagePhoneNumber')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationValidationMessagePhoneNumber',
		N'Формат Телефон.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameEmail')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameEmail',
		N'Email', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameEmail')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameEmail',
		N'Емэйл', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageRequiredPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageRequiredPassword',
		N'Password is required.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageRequiredPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageRequiredPassword',
		N'Пароль обязателен.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X6Password')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X6Password',
		N'Password must be 6 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X6Password')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X6Password',
		N'Пароль должен быть между 6 и 100 символов.', getDate());
GO

delete ResourceStrings where [Key] = 'AnnotationDisplayNamePassword'
GO

delete ResourceStrings where [Key] = 'AnnotationDisplayNameConfirmPassword'
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationPassword',
		N'Password', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationPassword',
		N'Пароль', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationConfirmPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationConfirmPassword',
		N'Confirm Password', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationConfirmPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationConfirmPassword',
		N'Повторный Пароль', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationOldPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationOldPassword',
		N'Old Password', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationOldPassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationOldPassword',
		N'Старый Пароль', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageComparePassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageComparePassword',
		N'Password and Confirm Password do not match.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageComparePassword')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageComparePassword',
		N'Пароль и Повторный Пароль не совпадают.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageRequiredUserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageRequiredUserName',
		N'User Name is required.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageRequiredUserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageRequiredUserName',
		N'Укажите Имя Пользователя', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X6UserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X6UserName',
		N'User Name must be 6 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X6UserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X6UserName',
		N'Имя Пользователя должено быть между 6 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X4UserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X4UserName',
		N'User Name must be 4 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X4UserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X4UserName',
		N'Имя Пользователя должено быть между 4 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameUserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameUserName',
		N'User Name', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameUserName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameUserName',
		N'Имя Пользователя', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X6DisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X6DisplayName',
		N'Display Name must be 6 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X6DisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X6DisplayName',
		N'Псевдоним должен быть между 6 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X3DisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X3DisplayName',
		N'Display Name must be 3 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X3DisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X3DisplayName',
		N'Псевдоним должен быть между 3 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameDisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameDisplayName',
		N'Display Name', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameDisplayName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameDisplayName',
		N'Псевдоним', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageRequiredReason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageRequiredReason',
		N'Please specify the reason.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageRequiredReason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageRequiredReason',
		N'Пожалуйста, укажите причину.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameReason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameReason',
		N'Reason', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameReason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameReason',
		N'Причина', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength1000X3Reason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength1000X3Reason',
		N'Reason must be 6 to 1000 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength1000X3Reason')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength1000X3Reason',
		N'Причина должна быть от 6 до 1000 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameNumberOfAnswers')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameNumberOfAnswers',
		N'Number of Opinions', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameNumberOfAnswers')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameNumberOfAnswers',
		N'Всего Мнений', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameNumberOfDescriptions')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameNumberOfDescriptions',
		N'Number of Descriptions', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameNumberOfDescriptions')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameNumberOfDescriptions',
		N'Всего Описаний', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameNumberOfVotes')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameNumberOfVotes',
		N'Number of Votes', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameNumberOfVotes')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameNumberOfVotes',
		N'Всего Голосов', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameNumberOfFlags')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameNumberOfFlags',
		N'Number of Flags', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameNumberOfFlags')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameNumberOfFlags',
		N'Всего Флагов', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameNumberOfComments')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameNumberOfComments',
		N'Number of Comments', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameNumberOfComments')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameNumberOfComments',
		N'Всего Комментариев', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameJoinDate')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameJoinDate',
		N'Join Date', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameJoinDate')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameJoinDate',
		N'Дата Регистрации', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNamePhoneNumber')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNamePhoneNumber',
		N'Phone Number', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNamePhoneNumber')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNamePhoneNumber',
		N'Телефон', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameCompanyName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameCompanyName',
		N'Company Name', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameCompanyName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameCompanyName',
		N'Имя Компании', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameWebSite')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameWebSite',
		N'Website', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameWebSite')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameWebSite',
		N'Сайт', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationDisplayNameUserDescription')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationDisplayNameUserDescription',
		N'Additional Info', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationDisplayNameUserDescription')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationDisplayNameUserDescription',
		N'Дополнение', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X4CompanyName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X4CompanyName',
		N'Company Name must be 4 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X4CompanyName')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X4CompanyName',
		N'Имя Компании должено быть между 4 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength100X4WebSite')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength100X4WebSite',
		N'WebSite must be 4 to 100 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength100X4WebSite')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength100X4WebSite',
		N'Имя Сайта должено быть между 4 и 100 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationErrorMessageStringLength1000X20Description')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationErrorMessageStringLength1000X20Description',
		N'20 to 1000 characters.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationErrorMessageStringLength1000X20Description')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationErrorMessageStringLength1000X20Description',
		N'от 20 до 1000 символов.', getDate());
GO

if not exists(select * from ResourceStrings where CultureName = 'en-US' and [Key] = 'AnnotationValidationMessageUrl')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('en-US', 'AnnotationValidationMessageUrl',
		N'Phone URL.', getDate());
if not exists(select * from ResourceStrings where CultureName = 'ru-RU' and [Key] = 'AnnotationValidationMessageUrl')
	insert ResourceStrings(CultureName, [Key], Value, DateAdded) values('ru-RU', 'AnnotationValidationMessageUrl',
		N'Формат URL.', getDate());
GO
