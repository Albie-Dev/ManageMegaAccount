@echo off
:menu
cls
echo ================================
echo Select an option to run the .NET project:
echo ================================
echo 1. dotnet run --no-build
echo 2. dotnet run
echo 3. dotnet watch run
echo 4. Migrations
echo 5. Exit
echo ================================
set /p choice=Enter your choice (1-5): 

if %choice%==1 goto run_no_build
if %choice%==2 goto run
if %choice%==3 goto watch_run
if %choice%==4 goto migrations
if %choice%==5 goto exit

:run_no_build
start cmd /k "cd /d %~dp0src\Microservices\MMA.API && dotnet run --no-build"
start cmd /k "cd /d %~dp0src\Web.UI\MMA.BlazorWasm && dotnet run --no-build"
pause
goto menu

:run
start cmd /k "cd /d %~dp0src\Microservices\MMA.API && dotnet run"
start cmd /k "cd /d %~dp0src\Web.UI\MMA.BlazorWasm && dotnet run"
pause
goto menu

:watch_run
start cmd /k "cd /d %~dp0src\Microservices\MMA.API && dotnet watch run"
start cmd /k "cd /d %~dp0src\Web.UI\MMA.BlazorWasm && dotnet watch run"
pause
goto menu

:migrations
cls
echo ================================
echo Migrations Options:
echo ================================
echo 1. List Migrations
echo 2. Add Migration
echo 3. Remove Migration
echo 4. Update Database
echo 5. Back to Main Menu
echo ================================
set /p migration_choice=Enter your choice (1-5): 

if %migration_choice%==1 goto list_migrations
if %migration_choice%==2 goto add_migration
if %migration_choice%==3 goto remove_migration
if %migration_choice%==4 goto update_database
if %migration_choice%==5 goto menu

:list_migrations
start cmd /k "cd /d %~dp0 && dotnet ef migrations list --project .\src\MicroServices\MMA.Service\MMA.Service.csproj --startup-project .\src\Microservices\MMA.API\MMA.API.csproj"
pause
goto migrations

:add_migration
set /p migration_name=Enter the migration name: 
if not defined migration_name (
    echo Migration name cannot be empty. Please try again.
    pause
    goto migrations
)
start cmd /k "cd /d %~dp0 && dotnet ef migrations add %migration_name% --project .\src\MicroServices\MMA.Service\MMA.Service.csproj --startup-project .\src\Microservices\MMA.API\MMA.API.csproj"
pause
goto migrations

:remove_migration
set /p migration_name=Enter the migration name to remove: 
if not defined migration_name (
    echo Migration name cannot be empty. Please try again.
    pause
    goto migrations
)
start cmd /k "cd /d %~dp0 && dotnet ef migrations remove --project .\src\MicroServices\MMA.Service\MMA.Service.csproj --startup-project .\src\Microservices\MMA.API\MMA.API.csproj"
pause
goto migrations

:update_database
set /p migration_name=Enter the migration name to update: 
if not defined migration_name (
    echo Migration name cannot be empty. Please try again.
    pause
    goto migrations
)
start cmd /k "cd /d %~dp0 && dotnet ef database update %migration_name% --project .\src\MicroServices\MMA.Service\MMA.Service.csproj --startup-project .\src\Microservices\MMA.API\MMA.API.csproj"
pause
goto migrations

:exit
exit
