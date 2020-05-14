dotnet --version
dotnet build TroublesOfJord.sln /p:Configuration=Release --no-incremental
call copy_zip_package_files.bat
rename "ZipPackage" "TroublesOfJord.%APPVEYOR_BUILD_VERSION%"
7z a TroublesOfJord.%APPVEYOR_BUILD_VERSION%.zip TroublesOfJord.%APPVEYOR_BUILD_VERSION%
