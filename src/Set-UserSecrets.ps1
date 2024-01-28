$currentPath = (Get-Location).Path

if ($currentPath -notcontains "src/ChristopherBriddock.Service.Identity")
{
    Set-Location "ChristopherBriddock.Service.Identity"
}

dotnet user-secrets set "ConnectionStrings:Default" "Host=hostHere;Port=5432;Username=pguser;Password=pwHere;Database=dbHere;"
dotnet user-secrets set "ConnectionStrings:Redis" ""
dotnet user-secrets set "ConnectionStrings:RedisInstanceName" ""
dotnet user-secrets set "Jwt:Audience" "https://localhost:7081"
dotnet user-secrets set "Jwt:Secret" "B`RNvn^c[8zy4/d>{*UTDx"
dotnet user-secrets set "Jwt:Expires" "3600"
dotnet user-secrets set "Jwt:Issuer" "ChristopherBriddock.Service.Identity"
dotnet user-secrets set "ApplicationInsights:InstrumentationKey" ""
dotnet user-secrets set "Serilog:WriteTo:1:Args:serverUrl" "http://localhost:5431"
dotnet user-secrets set "Seq:Endpoint" "http://localhost:5431"
dotnet user-secrets set "Seq:ApiKey" ""

cd ..

if ($currentPath -notcontains "ChristopherBriddock.Service.Email") 
{
    Set-Location "ChristopherBriddock.Service.Email"

    dotnet user-secrets set "Email:Server" "smtp-mail.outlook.com"
    dotnet user-secrets set "Email:Port" "587"
    dotnet user-secrets set "Email:Credentials:EmailAddress" "cbthehero666@gmail.com"
    dotnet user-secrets set "Email:Credentials:Password" ""

}