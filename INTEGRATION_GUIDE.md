# Integration Guide - Adding Secure Sequentier to Your Stack

This guide shows how to integrate Secure Sequentier into your existing `/opt/stack/docker-compose.yml` setup.

## Step 1: Clone Repository

```bash
cd /opt/stack
git clone <your-repo-url> secure-sequentier
cd secure-sequentier
```

## Step 2: Create Directory Structure

```bash
# Create config directory (if it doesn't exist)
sudo mkdir -p /opt/stack/config/secure-sequentier

# Create data directories (if they don't exist)
sudo mkdir -p /opt/stack/data/secure-sequentier/{watch,queue,processed,logs,subsystems}

# Set permissions (only works if directories exist)
sudo chown -R bilgin:bilgin /opt/stack/config/secure-sequentier
sudo chown -R bilgin:bilgin /opt/stack/data/secure-sequentier
```

**Note**: If you get "No such file or directory" error, make sure you ran the `mkdir -p` commands first. The `-p` flag creates parent directories if they don't exist.

## Step 3: Configure Application

```bash
# Copy configuration template
cp SecureSolution2/DefaultConfig.linux.json.example /opt/stack/config/secure-sequentier/DefaultConfig.json

# Edit configuration (paths should already be correct)
nano /opt/stack/config/secure-sequentier/DefaultConfig.json
```

## Step 4: Build Signature Applications (Optional)

If you have the signature app source code:

```bash
cd /opt/stack/secure-sequentier
bash build-subsystems-linux.sh
```

Or build manually and place executables in `/opt/stack/data/secure-sequentier/subsystems/`

## Step 5: Add to docker-compose.yml

Edit your main docker-compose file:

```bash
sudo nano /opt/stack/docker-compose.yml
```

Add the Secure Sequentier services. You can copy the content from `docker-compose.stack.yml` or add this section:

```yaml
  # -------------------------------------------------------
  # SECURE SEQUENTIER - File Processing System
  # -------------------------------------------------------
  secure-sequentier-backend:
    build:
      context: /opt/stack/secure-sequentier
      dockerfile: SecureSolution2/Dockerfile
    container_name: secure-sequentier-backend
    restart: unless-stopped
    ports:
      - "9999:9999"
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
      - proxy-net
      - data-net
    healthcheck:
      test: ["CMD-SHELL", "curl -f http://localhost:9999/api/health || exit 1"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M

  secure-sequentier-frontend:
    build:
      context: /opt/stack/secure-sequentier
      dockerfile: Web/Dockerfile
    container_name: secure-sequentier-frontend
    restart: unless-stopped
    ports:
      - "5188:5188"
    depends_on:
      secure-sequentier-backend:
        condition: service_healthy
    volumes:
      - /opt/stack/data/secure-sequentier/queue:/app/data/queue:ro
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5188
      - BACKEND_API_URL=http://secure-sequentier-backend:9999
    networks:
      - proxy-net
      - data-net
    healthcheck:
      test: ["CMD-SHELL", "curl -f http://localhost:5188 || exit 1"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 256M
```

## Step 6: Build and Start

```bash
cd /opt/stack
sudo docker compose build secure-sequentier-backend secure-sequentier-frontend
sudo docker compose up -d secure-sequentier-backend secure-sequentier-frontend
```

Or rebuild everything:

```bash
sudo docker compose up -d --build
```

## Step 7: Configure Cloudflare Tunnel

Add to your Cloudflare Tunnel config (usually in `/home/bilgin/homepage/config/cloudflared/config.yml` or wherever your cloudflared config is):

```yaml
ingress:
  # ... existing rules ...
  
  - hostname: secure.ybilgin.com
    service: http://192.168.50.100:5188
  
  # Catch-all (must be last)
  - service: http_status:404
```

Restart cloudflared:

```bash
# If running in docker-compose
cd /home/bilgin/homepage
docker-compose restart cloudflared

# Or if running as systemd service
sudo systemctl restart cloudflared
```

## Step 8: Test Access

### Local Access:
```bash
# Backend API
curl http://192.168.50.100:9999/api/health

# Frontend
curl http://192.168.50.100:5188
```

### Via Domain:
Open `https://secure.ybilgin.com` in your browser.

## Step 9: Add to Homepage Dashboard

Edit `/home/bilgin/homepage/config/homepage/services.yaml`:

```yaml
- Secure Sequentier:
    - href: https://secure.ybilgin.com
      description: Secure File Processing System
      icon: https://secure.ybilgin.com/favicon.png
```

## Management Commands

```bash
# View logs
sudo docker compose logs -f secure-sequentier-backend secure-sequentier-frontend

# Restart services
sudo docker compose restart secure-sequentier-backend secure-sequentier-frontend

# Stop services
sudo docker compose stop secure-sequentier-backend secure-sequentier-frontend

# Update application
cd /opt/stack/secure-sequentier
git pull
cd /opt/stack
sudo docker compose build secure-sequentier-backend secure-sequentier-frontend
sudo docker compose up -d secure-sequentier-backend secure-sequentier-frontend
```

## Ports Used

- **9999**: Backend API (internal use, accessed via frontend)
- **5188**: Frontend Web Interface (exposed via Cloudflare Tunnel)

## Network Integration

Secure Sequentier is connected to:
- `proxy-net`: For potential future integration with Nginx Proxy Manager
- `data-net`: For data sharing with other services if needed

## Troubleshooting

**Containers won't start:**
```bash
# Check logs
sudo docker compose logs secure-sequentier-backend
sudo docker compose logs secure-sequentier-frontend

# Verify paths exist
ls -la /opt/stack/config/secure-sequentier/
ls -la /opt/stack/data/secure-sequentier/
```

**Can't access via domain:**
- Verify Cloudflare Tunnel is running: `sudo docker ps | grep cloudflared`
- Check tunnel config syntax
- Verify DNS: `dig secure.ybilgin.com`

**Build fails:**
- Ensure you're in `/opt/stack` directory
- Check Docker has enough space: `sudo docker system df`
- Verify .NET SDK is available in build context (it's in the Dockerfile)

## Notes

- The application runs entirely on SSD (`/opt/stack/`) for fast performance
- Backend and frontend communicate internally via Docker network
- Only port 5188 needs to be exposed through Cloudflare Tunnel
- Backend port 9999 is for internal communication only

