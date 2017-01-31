select * from HelpItems

-- delete HelpItems
-- truncate table HelpItems

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_wisapinioner')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_wisapinioner', 'Who is Apinioner?',
		'Apinioner comes from the word "opinion". It is a person who likes sharing his opinions with others and helps people make better choices.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_wisapinioner')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_wisapinioner', 'Who is Apinioner?',
		'A');
go

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_bapinioner')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_bapinioner', 'How to become Apinioner?',
		'<a href="/en-US/Account/Register">Create</a> free account, <a href="/en-US/Home/Contribute">contribute</a> by adding new opinions or discussing existing opinions or voting for the best ones.
		You will gain levels and achivements and be able to compete for the top.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_bapinioner')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_bapinioner', 'How to become Apinioner?',
		'A');
go

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_dopay')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_dopay', 'Do I need to pay?',
		'No. No payment of any sort is required for using the site.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_dopay')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_dopay', 'Do I need to pay?',
		'A');
go

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_doregister')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_doregister', 'Do I have to register?',
		'No. You can post opinions and descriptions without registration.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_doregister')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_doregister', 'Do I have to register?',
		'A');
go

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_why_regis')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_why_regis', 'Why would I register?',
		'Registration allows creation of public profile. This is a great way to advertise your writing skills and services.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_why_regis')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_why_regis', 'Why would I register?',
		'A');
go

if not exists(select * from HelpItems where CultureName = 'en-US' and [Key] = 'help_dosell')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('en-US', getdate(), 'help_dosell', 'Do you sell products?',
		'No. We find products that match opinions from other sites. Clicking product link will navigate to the seller''s site.');
if not exists(select * from HelpItems where CultureName = 'ru-RU' and [Key] = 'help_dosell')
	insert HelpItems(CultureName, DateAdded, [Key], HelpTitle, Value) values('ru-RU', getdate(), 'help_dosell', 'Do you sell products?',
		'A');
go

