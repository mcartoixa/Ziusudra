<#
.SYNOPSIS
    Waits until the test Deluge daemon is accepting remote RPC.

.DESCRIPTION
    A host-side TCP check is unreliable here: docker's port proxy accepts connections before the
    daemon is ready, so it would report success too early. Instead this polls the container's own
    listening sockets for one bound to 0.0.0.0:<port>, which only appears once deluged has started
    with allow_remote enabled. The socket query runs inside the (Linux) container; the script itself
    is pure PowerShell and needs no shell on the host.

.PARAMETER Container
    The container name (matches docker-compose.yml).

.PARAMETER Port
    The daemon RPC port.

.PARAMETER TimeoutSeconds
    How long to wait before giving up.

.PARAMETER IntervalSeconds
    How long to wait between polls.

.EXAMPLE
    ./wait-for-deluged.ps1
#>
[CmdletBinding()]
param(
    [string] $Container = 'ziusudra-deluge',
    [int]    $Port = 58846,
    [int]    $TimeoutSeconds = 60,
    [int]    $IntervalSeconds = 2
)

$state = docker inspect -f '{{.State.Running}}' $Container 2>$null
if ($state -ne 'true') {
    Write-Error "Container '$Container' is not running. Start it with: docker compose -f test\deluge\docker-compose.yml up -d"
    exit 1
}

$needle = "0.0.0.0:$Port"
$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
Write-Host "Waiting for '$Container' to accept remote RPC on $needle (timeout ${TimeoutSeconds}s)..."

while ((Get-Date) -lt $deadline) {
    # Prefer ss; fall back to netstat if the image lacks it.
    $listeners = docker exec $Container ss -ltn 2>$null
    if ($LASTEXITCODE -ne 0) {
        $listeners = docker exec $Container netstat -ltn 2>$null
    }

    if ($listeners | Select-String -SimpleMatch $needle -Quiet) {
        Write-Host "Ready: deluged is listening on $needle."
        exit 0
    }
    Start-Sleep -Seconds $IntervalSeconds
}

Write-Error "'$Container' was not ready within $TimeoutSeconds seconds."
exit 1
