#!/usr/bin/env bash
set -euo pipefail

curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh

export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$DOTNET_ROOT:$PATH"

bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$DOTNET_ROOT"

dotnet publish src/ConsuCare.Client/ConsuCare.Client.csproj \
  --configuration Release \
  --output src/ConsuCare.Client/bin/Release/net8.0/publish