### jsonbase

An API based JSON storage implementation with dotnet core and Docker (ref. https://jsonbase.com).

## Where can I find the docker image?

Visit https://github.com/izaxon?tab=packages.

## How do I use jsonbase?

1. Run jsonbase (either from the source code or the docker image).

2. Interact with the API using HTTP GET and PUT requests (see [test.http](./test.http)).

```bash
curl -XPUT 'http://localhost:80/demo_bucket/hello' \
-H 'content-type: application/json' \
-d '{"hello": "world"}'
{"hello":"world"}
```

```bash
curl 'http://localhost:80/demo_bucket/hello'

{"hello":"world"}
```

## How to host in Kubernetes?

See files [skaffold.yaml](./skaffold.yaml) and files in [kubernetes](./kubernetes)

## References

- https://jsonbase.com



