#!/bin/bash
# Build script for signature applications on Linux
# Run this on your Linux server to build the signature apps

set -e

#!/bin/bash
# Build script for signature applications on Linux
# Outputs to /opt/stack/data/secure-sequentier/subsystems/

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SUBSYSTEMS_DIR="/opt/stack/data/secure-sequentier/subsystems"

echo "Building signature applications for Linux..."
echo "Output directory: $SUBSYSTEMS_DIR"

# Create output directory
mkdir -p "$SUBSYSTEMS_DIR"

# Build SignerApp
if [ -d "$SCRIPT_DIR/../Subsystems/SignerApp" ]; then
    echo "Building SignerApp..."
    cd "$SCRIPT_DIR/../Subsystems/SignerApp"
    dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false
    
    mkdir -p "$SUBSYSTEMS_DIR/SignerApp"
    cp bin/Release/net8.0/linux-x64/publish/SignerApp "$SUBSYSTEMS_DIR/SignerApp/"
    chmod +x "$SUBSYSTEMS_DIR/SignerApp/SignerApp"
    echo "✓ SignerApp built successfully"
else
    echo "⚠ SignerApp source not found. Skipping..."
fi

# Build AdvancedSignerApp
if [ -d "$SCRIPT_DIR/../Subsystems/AdvancedSignerApp" ]; then
    echo "Building AdvancedSignerApp..."
    cd "$SCRIPT_DIR/../Subsystems/AdvancedSignerApp"
    dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false
    
    mkdir -p "$SUBSYSTEMS_DIR/AdvancedSignerApp"
    cp bin/Release/net8.0/linux-x64/publish/AdvancedSignerApp "$SUBSYSTEMS_DIR/AdvancedSignerApp/"
    chmod +x "$SUBSYSTEMS_DIR/AdvancedSignerApp/AdvancedSignerApp"
    echo "✓ AdvancedSignerApp built successfully"
else
    echo "⚠ AdvancedSignerApp source not found. Skipping..."
fi

echo ""
echo "Build complete! Executables are in: $SUBSYSTEMS_DIR"
echo "Make sure DefaultConfig.json points to: /app/subsystems/..."

