Param(
    $generateReports = $true,
    $runTests = $true
)

Function Start-Tests() 
{
    if ($runTests) 
    {
        Set-Location "ChristopherBriddock.Service.Identity.Tests\"
        dotnet test --collect:"XPlat Code Coverage" 
    }
}

Function Start-Reports()
{
    Set-Location "TestResults"
    
    if ($generateReports) 
    {
        reportgenerator -reports:"*/coverage.cobertura.xml" -targetdir:"../coveragereport" -reporttypes:Html
    }
}

Start-Tests
Start-Reports