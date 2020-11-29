dotnet --version
dotnet build Jord.sln /p:Configuration=Release --no-incremental
call copy_zip_package_files.bat
rename "ZipPackage" "Jord.%APPVEYOR_BUILD_VERSION%"
7z a Jord.%APPVEYOR_BUILD_VERSION%.zip Jord.%APPVEYOR_BUILD_VERSION%
