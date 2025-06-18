# 🔧 Build Your Own Web Server in .NET

This is a simple, educational HTTP web server built from scratch in C# using `.NET`, fulfilling the **Build Your Own Web Server** challenge.

The goal is to understand how HTTP works under the hood — including sockets, request parsing, concurrency, path sanitization, and even dynamic content via CGI.

---

## 🚀 Features Implemented

| Step | Feature |
|------|---------|
| ✅ 0  | Project bootstrapped in .NET with async main loop |
| ✅ 1  | Basic socket-based HTTP server responding to `GET` |
| ✅ 2  | Serves static HTML from `www` folder with correct status codes |
| ✅ 3  | Handles multiple concurrent connections via async tasks |
| ✅ 4  | Validates and sanitizes file paths to prevent directory traversal |
| ✅ Bonus | Executes CGI-style scripts via `/cgi-bin/` path |

---

## 🧠 Challenge Summary

The original [challenge]([https://codecrafters.io/challenges/web-server](https://codingchallenges.fyi/challenges/challenge-webserver#step-4)) asks participants to:

1. **Build an HTTP/1.1-compatible server**
2. **Respond with simple plain text and HTML**
3. **Support static file serving**
4. **Add concurrency**
5. **Prevent directory traversal vulnerabilities**
6. **(Optional)**: Add CGI execution

---

## 🏗️ How It Works

The server:

- Uses `TcpListener` to bind to `localhost:8080`
- Accepts client connections using `AcceptTcpClientAsync()`
- Parses basic HTTP `GET` requests
- Maps requests to files in the `www` folder
- Prevents access outside the root using `Path.GetFullPath()` comparison
- For `/cgi-bin/` requests, executes the corresponding script and streams its output

---

## 🖥️ Example Usage

```bash
dotnet run
```

## 🖥️ Example Usage

Then in another terminal:

```bash
curl http://localhost:8080/
curl http://localhost:8080/index.html
curl http://localhost:8080/doesnotexist.html
```
## 🧪 Sample Output

```http
GET / HTTP/1.1

HTTP/1.1 200 OK
Content-Type: text/html; charset=UTF-8

<!DOCTYPE html>
<html>
  <body>
    <h1>Hello from your web server!</h1>
  </body>
</html>
```
Output will be either the HTML page or a 404 Not Found.

## 🛡️ Security: Path Traversal Protection

To avoid path traversal attacks (e.g., GET /../../secret.txt), the code uses:
```c#
if (!requestedPath.StartsWith(rootDir))
{
    // 403 Forbidden
}
```
This ensures only files within the www/ directory are accessible.

## 🧰 Technologies Used
- .NET 8
- TCP Networking (TcpListener, NetworkStream)
- File I/O
- Async/Await
- Process execution (CGI support)

## 🗂️ Project Structure
```plaintext
web-server/
│
├── Program.cs           # Server logic
├── www/                 # HTML files served
│   └── index.html       
├── cgi-bin/             # Executable scripts (optional)
│   └── hello.py         
└── README.md
```

## ✍️ Author's Notes

- This challenge was both fun and instructive. I learned:
- How low-level HTTP actually works
- How to safely handle file access in a server
- How async in .NET helps with high concurrency
- The basics of CGI (a blast from the past!)

## 📎 License

MIT — feel free to use, improve, or contribute!

---

## 🤝 Contributions

Feel free to fork this repo and share your improvements or suggestions.

Inspired by the **Build Your Own Web Server** challenge.
