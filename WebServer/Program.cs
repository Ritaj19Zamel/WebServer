using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    private const int Port = 8080;
    private static readonly string WebRoot = Path.GetFullPath("www");
    private static readonly string CgiRoot = Path.GetFullPath("cgi-bin");

    static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Loopback, Port);
        listener.Start();
        Console.WriteLine($"Server is running on http://localhost:{Port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        string? requestLine = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(requestLine) || !requestLine.StartsWith("GET"))
        {
            await WriteResponse(writer, "400 Bad Request", "Bad Request");
            return;
        }

        Console.WriteLine($"Received: {requestLine}");

        var parts = requestLine.Split(' ');
        if (parts.Length < 2)
        {
            await WriteResponse(writer, "400 Bad Request", "Bad Request");
            return;
        }

        string path = parts[1];
        if (path.StartsWith("/cgi-bin/"))
        {
            await HandleCgiRequest(writer, path);
        }
        else
        {
            await ServeStaticFile(writer, path);
        }
    }

    private static async Task ServeStaticFile(StreamWriter writer, string path)
    {
        string relativePath = path == "/" ? "index.html" : path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string requestedPath = Path.GetFullPath(Path.Combine(WebRoot, relativePath));

        if (!requestedPath.StartsWith(WebRoot))
        {
            await WriteResponse(writer, "403 Forbidden", "Forbidden");
            return;
        }

        if (File.Exists(requestedPath))
        {
            string content = await File.ReadAllTextAsync(requestedPath);
            await WriteResponse(writer, "200 OK", content, "text/html");
        }
        else
        {
            await WriteResponse(writer, "404 Not Found", "Not Found");
        }
    }

    private static async Task HandleCgiRequest(StreamWriter writer, string path)
    {
        string relativePath = path.Replace("/cgi-bin/", "").Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.GetFullPath(Path.Combine(CgiRoot, relativePath));

        if (!fullPath.StartsWith(CgiRoot) || !File.Exists(fullPath))
        {
            await WriteResponse(writer, "404 Not Found", "Script Not Found");
            return;
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fullPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            await writer.WriteAsync(output); 
        }
        catch (Exception ex)
        {
            await WriteResponse(writer, "500 Internal Server Error", ex.Message);
        }
    }

    private static async Task WriteResponse(StreamWriter writer, string status, string body, string contentType = "text/plain")
    {
        writer.WriteLine($"HTTP/1.1 {status}");
        writer.WriteLine($"Content-Type: {contentType}; charset=UTF-8");
        writer.WriteLine($"Content-Length: {body.Length}");
        writer.WriteLine();
        await writer.WriteAsync(body);
    }
}
