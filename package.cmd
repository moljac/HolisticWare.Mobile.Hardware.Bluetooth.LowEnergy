@echo off

set NUGET=.nuget\nuget

%NUGET% ^
	pack ^
	"src\Company.Product.nuspec" ^
	-Symbols ^
	-OutputDirectory artifacts ^
	-Build ^
	-IncludeReferencedProjects ^
	-NonInteractive

@IF %ERRORLEVEL% NEQ 0 PAUSE	

