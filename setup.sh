#!/bin/bash

set -e

# 1. Install .NET 10 if not present
if ! command -v dotnet &> /dev/null || [[ "$(dotnet --version)" != 10.0* ]]; then
    echo "Installing .NET 10..."
    curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh -o dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 10.0
    export PATH=$HOME/.dotnet:$PATH
    # Add to shell profile if needed, but for now just current session
    echo "Please add \$HOME/.dotnet to your PATH"
else
    echo ".NET $(dotnet --version) is already installed."
fi

# 2. Compile native library
echo "Compiling native library..."
make shared_generic

# 3. Build C# projects
echo "Building C# projects..."
dotnet build Flux.NET
dotnet build Flux.Example

echo "Setup complete!"
echo ""
echo "To run the example:"
echo "export LD_LIBRARY_PATH=\$PWD:\$LD_LIBRARY_PATH"
echo "dotnet run --project Flux.Example"
