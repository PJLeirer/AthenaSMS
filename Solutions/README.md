This is the Windows Service, Console Server and Core System for SMS Server. Also, the Windows Client Administrative Console and client jobs for scheduling

Installation:
  1. Create an MSSQL database called 'athenasms'
  1. Open Windows Explorer and create a folder called 'Athena' in the root of C. ('C:\Athena')
  2. Double-click the folder, once inside Athena folder Create sub-folders 'conf', 'jobs', 'bin' and 'log'.
  3. Copy the Executable CLI and Service files as well as the DLL file from project bin folder to 'C:\Athena\bin\';
     it will also run from VS
  4. Run the CLI exe first, this will create a config file, enter 'exit', and close console window.
  5. Go to 'C:\Athena\conf\' and open 'athenasms.conf' in notepad and update values.
  6. Run CLI exe again. If your database and credentials are all setup correctly, tables will be created. If database errors happen, the        console output should help you identify errors, fix probelms & repeat this step.
  7. IF all goes well, you should see a comand prompt ('Athena >'), enter 'help'.
  8. Once Everything is working with the CLI version. you can then install the service with 'installutil'.
  9. Optionally, check your event logs for output from Athena.
  10. Run AthenaConsole.exe for Administrative Console
  11. Enjoy!
