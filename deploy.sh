#!/bin/bash
# Deployment script for Secure Sequentier on Linux Server
# This script helps set up the application on your home server

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_DIR="/opt/stack/config/secure-sequentier"
DATA_DIR="/opt/stack/data/secure-sequentier"

echo "🚀 Secure Sequentier Deployment Script"
echo "========================================"
echo ""

# Check if running as root for directory creation
if [ "$EUID" -eq 0 ]; then
    echo "⚠️  Please run this script as a regular user (not root)"
    echo "   You'll be prompted for sudo when needed."
    exit 1
fi

# Create directory structure
echo "📁 Creating directory structure..."
sudo mkdir -p "$CONFIG_DIR"
sudo mkdir -p "$DATA_DIR"/{watch,queue,processed,logs,subsystems}

# Set permissions
echo "🔐 Setting permissions..."
sudo chown -R $USER:$USER "$CONFIG_DIR"
sudo chown -R $USER:$USER "$DATA_DIR"

# Copy configuration
if [ ! -f "$CONFIG_DIR/DefaultConfig.json" ]; then
    echo "📋 Copying configuration template..."
    cp "$SCRIPT_DIR/SecureSolution2/DefaultConfig.linux.json.example" "$CONFIG_DIR/DefaultConfig.json"
    echo "✓ Configuration copied to $CONFIG_DIR/DefaultConfig.json"
    echo "   Please edit this file and update paths if needed."
else
    echo "✓ Configuration file already exists at $CONFIG_DIR/DefaultConfig.json"
fi

# Build signature applications
echo ""
read -p "Do you want to build signature applications now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    if [ -f "$SCRIPT_DIR/build-subsystems-linux.sh" ]; then
        bash "$SCRIPT_DIR/build-subsystems-linux.sh"
    else
        echo "⚠️  build-subsystems-linux.sh not found. Skipping signature app build."
        echo "   You can build them manually later."
    fi
fi

# Build Docker images
echo ""
read -p "Do you want to build Docker images now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🐳 Building Docker images..."
    cd "$SCRIPT_DIR"
    sudo docker compose -f docker-compose.production.yml build
    echo "✓ Docker images built successfully"
fi

# Start containers
echo ""
read -p "Do you want to start the containers now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🚀 Starting containers..."
    cd "$SCRIPT_DIR"
    sudo docker compose -f docker-compose.production.yml up -d
    echo "✓ Containers started"
    echo ""
    echo "View logs with: sudo docker compose -f docker-compose.production.yml logs -f"
fi

echo ""
echo "✅ Deployment setup complete!"
echo ""
echo "📁 Directory Structure:"
echo "   Config: $CONFIG_DIR"
echo "   Data:   $DATA_DIR (SSD - Fast storage)"
echo ""
echo "Next steps:"
echo "1. Edit configuration: nano $CONFIG_DIR/DefaultConfig.json"
echo "2. Configure Cloudflare Tunnel (see DEPLOYMENT.md)"
echo "3. Access at: https://secure.ybilgin.com"
echo ""
echo "Useful commands:"
echo "  View logs:    sudo docker compose -f docker-compose.production.yml logs -f"
echo "  Restart:      sudo docker compose -f docker-compose.production.yml restart"
echo "  Stop:         sudo docker compose -f docker-compose.production.yml down"
echo "  Update:       git pull && sudo docker compose -f docker-compose.production.yml up -d --build"
echo ""
echo "💾 Note: All data is stored on SSD (/opt/stack/) for fast performance."
echo "   For backups, consider copying to HDD: cp -r $DATA_DIR /mnt/data/backups/"

