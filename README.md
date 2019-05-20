# trident
A tool to upload/backup/sync local or network folder data to Amazon AWS S3 (Simple Storage Service) bucket(s).
Supported on Windows environment only.  
Trident can be used to backup personal media files and folders so that it can be useful during failures of main external hard drives or network attach storage or computer disks.  Data uploaded by trident is one-way sync meanning it remains in s3 bucket until user deletes the data manually in s3,  that way if a file is deleted in computer or external hard drives, the data remains unchanged in s3.
### Prerequisite
1. Windows 7 or older 
2. Visual Studio 2017 Community edition [download link](https://visualstudio.microsoft.com/vs/community/) (free!)
3. .NET Framework 4.6 or older.  .NET Core is not supported.
4. AWS account [(link)](https://aws.amazon.com/)

> **Important**: You are responsilble for any cost associated with using AWS services. 

> **Note**: You are responsible to configure security as per the best practices on your AWS account to protect your data in AWS.
### Compile
- Install Visual Studio 2017 (VS) community edition.
- Install .NET Framework 4.6 or latest. 
- Clone or download the souce code zip from this github project [(download link)](https://github.com/jabbar-gabbar/trident/archive/master.zip) and compile it using VS. The complier will generate app binaries.

### AWS setup
Trident uses aws s3 API to upload objects(files) to aws. 
#### Create aws credential file
#### Store cred file
Run the following commands to create aws credential file in 'C:\users\{myuser}\.aws' folder.
Open cmd window

    cd %HOMEPATH%

    mkdir .aws

    cd .aws

    copy NUL credentials

enter credentials in the file in following format. 

### Build the code
### Deploy the build
### Configure settings
### Schedule the auto sync
Useful command:

dir \\myserver\iphone /o:-s/b/s