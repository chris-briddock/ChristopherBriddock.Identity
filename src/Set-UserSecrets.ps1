$currentPath = (Get-Location).Path

if ($currentPath -notcontains "src/ChristopherBriddock.Service.IdentityServer")
{
    Set-Location "src/ChristopherBriddock.Service.IdentityServer"
}

dotnet user-secrets set "ConnectionStrings:Default" "Host=172.16.1.8;Port=5432;Username=pguser;Password=drRhxp397NP4__;Database=ChristopherBriddock.Service.Identity;"
dotnet user-secrets set "ConnectionStrings:Redis" ""
dotnet user-secrets set "ConnectionStrings:RedisInstanceName" ""
dotnet user-secrets set "Jwt:Audience" "https://localhost"
dotnet user-secrets set "Jwt:Secret" "B`RNvn^c[8zy4/d>{*UTDx"
dotnet user-secrets set "Jwt:Expires" "3600"
dotnet user-secrets set "ApplicationInsights:InstrumentationKey" ""
dotnet user-secrets set "Serilog:WriteTo:0:Args:serverUrl" "http://localhost:5431"