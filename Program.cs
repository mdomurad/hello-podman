using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;

static DockerClient CreateClient()
{
    return new DockerClientConfiguration(new Uri(GetClientUri())).CreateClient();

    static string GetClientUri()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "npipe://./pipe/docker_engine";
        }
        else
        {
            string podmanPath = $"/run/user/{geteuid()}/podman/podman.sock";
            if (File.Exists(podmanPath))
            {
                return $"unix:{podmanPath}";
            }

            return "unix:/var/run/docker.sock";
        }
    }

    [DllImport("libc")]
    static extern uint geteuid();
}

var image = "registry.access.redhat.com/ubi8/dotnet-80:latest";
var command = new[] { "dotnet", "--version" };

var client = CreateClient();

Console.WriteLine("Pull the image.");
await client.Images.CreateImageAsync(
    new() { FromImage = image },
    authConfig: null,
    progress: new Progress<JSONMessage>()
);

Console.WriteLine("Create a container for running the command.");
var container = await client.Containers.CreateContainerAsync(
    new() { Image = image, Cmd = command }
);

try
{
    Console.WriteLine("Start the container.");
    await client.Containers.StartContainerAsync(container.ID, new());

    Console.WriteLine("Wait till the container terminates.");
    var waitResponse = await client.Containers.WaitContainerAsync(container.ID);
    int exitCode = (int)waitResponse.StatusCode;

    Console.WriteLine("Read the logs.");
    var logStream = await client.Containers.GetContainerLogsAsync(
        container.ID,
        tty: false,
        new() { ShowStdout = true, ShowStderr = true }
    );
    (string stdout, string stderr) = await logStream.ReadOutputToEndAsync(default);

    Console.WriteLine("Output:");
    string output = (exitCode == 0 ? stdout : stderr).Trim();
    Console.WriteLine(output);

    return exitCode;
}
finally
{
    await client.Containers.RemoveContainerAsync(container.ID, new());
}

/* var builder = WebApplication.CreateBuilder(args); */
/* var app = builder.Build(); */
/**/
/* app.MapGet("/", () => "Hello World!"); */
/**/
/* app.Run(); */
