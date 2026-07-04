# Deluge test daemon

A throwaway [linuxserver/deluge](https://hub.docker.com/r/linuxserver/deluge) container for running
`Ziusudra.Client.IntegrationTests` against a real daemon — no VPN, no touching a real server, and easy
to point at different Deluge versions.

The init script (`custom-cont-init.d/99-ziusudra-remote`) runs before the daemon starts and:

- enables remote RPC (`allow_remote`), so the published `58846` port is reachable from the host;
- seeds a disposable admin account **`deluge` / `deluge`**.

## Run

```powershell
# from test/deluge
docker compose up -d

# point the tests at the container (or copy .env.example to .env in the test project)
$env:ZIUSUDRA_DELUGE_HOST = "127.0.0.1"
$env:ZIUSUDRA_DELUGE_USERNAME = "deluge"
$env:ZIUSUDRA_DELUGE_PASSWORD = "deluge"
dotnet test Ziusudra.Client.IntegrationTests

docker compose down
```

Environment variables take precedence over a local `.env`, so the line above overrides one that
points at another server.

## A different Deluge version

```powershell
$env:DELUGE_IMAGE_TAG = "2.1.1"   # any linuxserver/deluge tag
docker compose up -d
```
