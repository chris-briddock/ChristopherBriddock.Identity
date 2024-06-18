using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

    protected override void ConfigureClient(HttpClient client)
    {
        try
        {
             // Get the current directory and navigate to the desired directory
            string currentDirectory = Directory.GetCurrentDirectory();
            string desiredDirectory = Path.Combine(
            currentDirectory, 
            "..", "..", "..", "..", 
            "ChristopherBriddock.Service.Identity"
        );

            // Ensure the path is properly formatted
            desiredDirectory = Path.GetFullPath(desiredDirectory);
            // Create a new process info
            ProcessStartInfo psi = new()
            {
                FileName = "dotnet",
                Arguments = "user-jwts create --output json --audience http://localhost",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true, 
                WorkingDirectory = desiredDirectory
            };

            // Create and start the process
            using Process process = new() { StartInfo = psi };
            process.Start();

            // Read the standard output
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            JObject jwtJson = JObject.Parse(output);
            string token = jwtJson["Token"]!.ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Optionally, read and handle standard error
            string error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }
        catch (Exception)
        {
            throw;
        }

        base.ConfigureClient(client);
    }

}