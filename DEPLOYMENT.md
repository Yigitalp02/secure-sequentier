# Secure Sequentier - Linux Server Deployment Guide

This guide will help you deploy Secure Sequentier on your Ubuntu Server 24.04 LTS home server and expose it via Cloudflare Tunnel.

## Prerequisites

- Ubuntu Server 24.04 LTS
- Docker and Docker Compose installed
- Cloudflare Tunnel configured
- Domain `ybilgin.com` with Cloudflare DNS

## Directory Structure

Based on your server setup, we'll use SSD storage (`/opt/stack/`) for all application data:
- **Config**: `/opt/stack/config/secure-sequentier/` (SSD - Fast storage)
- **Data**: `/opt/stack/data/secure-sequentier/` (SSD - Fast storage)

## Step 1: Clone the Repository

```bash
cd /opt/stack
git clone <your-repo-url> secure-sequentier
cd secure-sequentier
```

**Note**: If you're integrating into an existing `/opt/stack/docker-compose.yml`, see [INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md) for step-by-step instructions.

## Step 2: Create Directory Structure

```bash
# Create config directory on SSD
sudo mkdir -p /opt/stack/config/secure-sequentier

# Create data directories on SSD (fast storage)
sudo mkdir -p /opt/stack/data/secure-sequentier/{watch,queue,processed,logs,subsystems}

# Set permissions
sudo chown -R $USER:$USER /opt/stack/config/secure-sequentier
sudo chown -R $USER:$USER /mnt/data/secure-sequentier
```

## Step 3: Configure the Application

### 3.1 Copy Configuration Template

```bash
cp SecureSolution2/DefaultConfig.linux.json.example /opt/stack/config/secure-sequentier/DefaultConfig.json
```

### 3.2 Edit Configuration

```bash
nano /opt/stack/config/secure-sequentier/DefaultConfig.json
```

Update the paths if needed (defaults should work with Docker volumes):
```json
{
  "WatchDirectory": "/app/data/watch",
  "QueueDirectory": "/app/data/queue/{USER}",
  "TimeoutSeconds": 30,
  "DefaultRetryCount": 3,
  "Mapping": {
    "signer": {
      "ExecutablePath": "/app/subsystems/SignerApp/SignerApp",
      "OutputDirectory": "/app/data/processed/{USER}/signatures"
    },
    "advanced-signer": {
      "ExecutablePath": "/app/subsystems/AdvancedSignerApp/AdvancedSignerApp",
      "OutputDirectory": "/app/data/processed/{USER}/advanced-signatures"
    },
    "sub-system1": {
      "ExecutablePath": "/app/subsystems/SuccessApp/DummySuccessApp",
      "OutputDirectory": "/app/data/processed/{USER}/sub-system1"
    }
  }
}
```

## Step 4: Prepare Signature Applications for Linux

The signature applications (`SignerApp` and `AdvancedSignerApp`) need to be rebuilt for Linux. You have two options:

### Option A: Rebuild on Linux Server

1. Copy the source code to your server:
   ```bash
   # On your Windows machine, copy the SignerApp and AdvancedSignerApp directories
   # to /opt/stack/data/secure-sequentier/subsystems/
   ```

2. Build for Linux:
   ```bash
   cd /opt/stack/data/secure-sequentier/subsystems/SignerApp
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
   
   cd ../AdvancedSignerApp
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
   ```

3. Copy executables:
   ```bash
   mkdir -p /opt/stack/data/secure-sequentier/subsystems/SignerApp
   mkdir -p /opt/stack/data/secure-sequentier/subsystems/AdvancedSignerApp
   
   cp SignerApp/bin/Release/net8.0/linux-x64/publish/SignerApp /opt/stack/data/secure-sequentier/subsystems/SignerApp/
   cp AdvancedSignerApp/bin/Release/net8.0/linux-x64/publish/AdvancedSignerApp /opt/stack/data/secure-sequentier/subsystems/AdvancedSignerApp/
   
   chmod +x /opt/stack/data/secure-sequentier/subsystems/SignerApp/SignerApp
   chmod +x /opt/stack/data/secure-sequentier/subsystems/AdvancedSignerApp/AdvancedSignerApp
   ```

### Option B: Use Docker Multi-Stage Build (Recommended)

Create a Dockerfile for building the signature apps:

```dockerfile
# subsystems/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy and build SignerApp
COPY SignerApp/ ./SignerApp/
RUN cd SignerApp && dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# Copy and build AdvancedSignerApp
COPY AdvancedSignerApp/ ./AdvancedSignerApp/
RUN cd AdvancedSignerApp && dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

FROM scratch AS export
COPY --from=build /src/SignerApp/bin/Release/net8.0/linux-x64/publish/SignerApp /SignerApp
COPY --from=build /src/AdvancedSignerApp/bin/Release/net8.0/linux-x64/publish/AdvancedSignerApp /AdvancedSignerApp
```

## Step 5: Update Docker Compose

Create a production `docker-compose.yml` in your deployment directory:

```yaml
version: '3.8'

services:
  backend:
    build:
      context: /opt/stack/secure-sequentier
      dockerfile: SecureSolution2/Dockerfile
    container_name: secure-sequentier-backend
    restart: unless-stopped
    ports:
      - "127.0.0.1:9999:9999"  # Only bind to localhost
    volumes:
      - /opt/stack/data/secure-sequentier/watch:/app/data/watch
      - /opt/stack/data/secure-sequentier/queue:/app/data/queue
      - /opt/stack/data/secure-sequentier/processed:/app/data/processed
      - /opt/stack/data/secure-sequentier/logs:/app/data/logs
      - /opt/stack/config/secure-sequentier/DefaultConfig.json:/app/DefaultConfig.json:ro
      - /opt/stack/data/secure-sequentier/subsystems:/app/subsystems:ro
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:9999
    networks:
      - secure-sequentier-network

  frontend:
    build:
      context: /mnt/data/secure-sequentier
      dockerfile: Web/Dockerfile
    container_name: secure-sequentier-frontend
    restart: unless-stopped
    ports:
      - "127.0.0.1:5188:5188"  # Only bind to localhost
    depends_on:
      - backend
    volumes:
      - /opt/stack/data/secure-sequentier/queue:/app/data/queue:ro
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5188
      - BACKEND_API_URL=http://backend:9999
    networks:
      - secure-sequentier-network

networks:
  secure-sequentier-network:
    driver: bridge
```

## Step 6: Build and Start Containers

```bash
cd /opt/stack/secure-sequentier
sudo docker compose -f docker-compose.production.yml build
sudo docker compose -f docker-compose.production.yml up -d
```

Check logs:
```bash
sudo docker compose -f docker-compose.production.yml logs -f
```

## Step 7: Configure Cloudflare Tunnel

### 7.1 Add to Cloudflare Tunnel Config

Edit your Cloudflare Tunnel configuration (usually in `/opt/stack/config/cloudflared/config.yml`):

```yaml
tunnel: <your-tunnel-id>
credentials-file: /opt/stack/config/cloudflared/credentials.json

ingress:
  # ... existing rules ...
  
  - hostname: secure.ybilgin.com
    service: http://localhost:5188
  
  # Catch-all rule (must be last)
  - service: http_status:404
```

### 7.2 Restart Cloudflare Tunnel

```bash
sudo systemctl restart cloudflared
# or if using docker:
docker restart cloudflared
```

## Step 8: Configure Cloudflare Access (Optional but Recommended)

Since you're using Cloudflare Access for other services:

1. Go to Cloudflare Zero Trust Dashboard
2. Add a new Access Application
3. Configure:
   - Application name: Secure Sequentier
   - Application domain: `secure.ybilgin.com`
   - Policy: Email OTP (or your preferred auth method)

## Step 9: Verify Deployment

1. **Check containers are running:**
   ```bash
   docker ps | grep secure-sequentier
   ```

2. **Test locally:**
   ```bash
   curl http://localhost:9999  # Backend
   curl http://localhost:5188   # Frontend
   ```

3. **Access via domain:**
   - Open `https://secure.ybilgin.com` in your browser
   - You should see the Secure Sequentier interface

## Step 10: Add to Homepage Dashboard

Add to your Homepage services configuration (`/opt/stack/config/homepage/services.yaml`):

```yaml
- Secure Sequentier:
    - href: https://secure.ybilgin.com
      description: Secure File Processing System
      icon: https://secure.ybilgin.com/favicon.png
```

## Maintenance

### View Logs
```bash
sudo docker compose -f docker-compose.production.yml logs -f backend
sudo docker compose -f docker-compose.production.yml logs -f frontend
```

### Restart Services
```bash
sudo docker compose -f docker-compose.production.yml restart
```

### Update Application
```bash
cd /opt/stack/secure-sequentier
git pull
sudo docker compose -f docker-compose.production.yml build
sudo docker compose -f docker-compose.production.yml up -d
```

### Backup Data
```bash
# Backup configuration
tar -czf secure-sequentier-config-backup-$(date +%Y%m%d).tar.gz /opt/stack/config/secure-sequentier/

# Backup data (to HDD if you want long-term storage)
tar -czf /mnt/data/backups/secure-sequentier-data-backup-$(date +%Y%m%d).tar.gz /opt/stack/data/secure-sequentier/
```

## Troubleshooting

### Containers won't start
- Check logs: `sudo docker compose -f docker-compose.production.yml logs`
- Verify paths exist and have correct permissions
- Ensure ports 9999 and 5188 are not in use: `sudo netstat -tulpn | grep -E '9999|5188'`

### Can't access via domain
- Verify Cloudflare Tunnel is running: `sudo systemctl status cloudflared`
- Check tunnel config has correct ingress rule
- Verify DNS: `dig secure.ybilgin.com`

### Signature apps not working
- Ensure executables have execute permission: `chmod +x /opt/stack/data/secure-sequentier/subsystems/*/SignerApp`
- Check logs for execution errors
- Verify paths in DefaultConfig.json match actual locations

## Security Notes

- Ports are bound to `127.0.0.1` only (not exposed externally)
- All external access goes through Cloudflare Tunnel
- Use Cloudflare Access for authentication
- Regular backups recommended
- Keep Docker and containers updated

