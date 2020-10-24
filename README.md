# jsonbase

An API based JSON storage implementation with dotnet core and Docker (ref. https://jsonbase.com).

![Publish Docker image](https://github.com/izaxon/jsonbase/workflows/Publish%20Docker%20image/badge.svg)

## Where can I find the docker image?

Visit https://github.com/izaxon?tab=packages.

## How do I run from the Windows command prompt?

```cmd
set ROOT_PATH=.
dotnet run
```

## How do I use jsonbase?

1. Run jsonbase (either from the source code or the docker image).

2. Interact with the API using HTTP GET and PUT requests (see [test.http](./test.http)).

```bash
curl -X PUT 'http://localhost:5000/demo_bucket/hello' \
-H 'content-type: application/json' \
-d '{"hello": "world"}'
{"hello":"world"}
```

```bash
curl 'http://localhost:5000/demo_bucket/hello'

{"hello":"world"}
```

## How to host in Kubernetes?

See files [skaffold.yaml](./skaffold.yaml) and files in [kubernetes](./kubernetes)

## How do I connect from the client (javascript)?

```html
<html>
<body>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/3.1.7/signalr.min.js"></script>
    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:5001/apihub?path=/demo_bucket/hello")
            .configureLogging(signalR.LogLevel.Debug)
            .build();
        
        async function start() {
            try {
                await connection.start();
                console.log("SignalR Connected.");
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        connection.onclose(start);

        // Start the connection.
        start();

        connection.on("SendUpdated", (path) => {
            const li = document.createElement("li");
            li.textContent = `${path}`;
            document.getElementById("messageList").appendChild(li);
        });
    </script>
    <div id="messageList"></div>
</body>
Hello!
</html>
```

## References

- https://jsonbase.com



