rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\data"
mkdir "ZipPackage\x64"
mkdir "ZipPackage\x86"

set "CONFIGURATION=bin\Release\net45"

rem Copy output files
xcopy "src\Wanderers.Game\%CONFIGURATION%\*.*" "ZipPackage\*.*" /s /e
copy "src\Wanderers.MapEditor\%CONFIGURATION%\Wanderers.MapEditor.exe" "ZipPackage" /Y
xcopy "data\*.*" "ZipPackage\data\*.*" /s
