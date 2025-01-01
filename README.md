# Docker .NET Sample Application

This is a simple .NET application that demonstrates how to use the Docker.DotNet NuGet package to interact with Docker. The application pulls a Docker image, creates a container, runs a command inside the container, and retrieves the output.

## Prerequisites

- .NET SDK installed on your machine
- Docker or Podman installed and running on your machine

## Getting Started

1. Clone the repository:

2. Restore the dependencies:

   ```sh
   dotnet restore
   ```

3. Run the application:

   ```sh
   dotnet run
   ```

## How It Works

The application performs the following steps:

1. Creates a Docker client using the `Docker.DotNet` package.
2. Pulls the `registry.access.redhat.com/ubi8/dotnet-80:latest` image.
3. Creates a container from the pulled image and specifies the command to run (`dotnet --version`).
4. Starts the container and waits for it to terminate.
5. Retrieves and prints the logs from the container.
6. Removes the container after execution.

## License

This project is licensed under the MIT License.
