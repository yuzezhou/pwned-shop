# pwned-shop

Pwned Shop is an online shopping website which sells Steam games. The website is currently hosted on Azure App Service and can be accessed here<a href='https://pwned-shop.azurewebsites.net/'>here</a>

The website might take around 5s to load since the instance is running on free plan.

# MacOS SQL server configuration

If you're using MacOS, please change ConnectionString "DbConn" under appsettings.json
according to your current configuration.

ConnectionString to a SQL server running on Docker should look something like this

"DbConn": "Server=localhost,[port]; Database=PwnedShopDb; User Id=<your-user-id>; Password=<your-password>;"

# User account
Please use the following user account to test website features. You can also register using any email account
Email: dprice@msn.com
Password: HelloBY90

# App database
The app is configured to delete and generate a new db every time the app is run,
to ensure consistency across test runs.

If you want to disable this, proceed to comment out this line in Data/DbInitializer.cs
16             db.Database.EnsureDeleted();

# Emailing features
Currently, we're using a developer account on MailGun to send out receipt emails.
Developer account is meant for testing so only emails registered with MailGun can
receive receipt emails.

Please use these credentials, which has been registered with MailGun to test email
sending feature

Pwned Shop User account:
Email: pwnedshop_tester@outlook.sg
Password: Kim6969$$$

Outlook email account:
Email: pwnedshop_tester@outlook.sg
Password: Kim6969$$$

If you would like to test this feature with your own email, please send your email
to truongkimson@u.nus.edu to register your email with MailGun.
