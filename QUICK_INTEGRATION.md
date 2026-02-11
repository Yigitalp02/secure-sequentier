# Quick Integration - Add to Existing Stack

This is a quick reference for adding Secure Sequentier to your `/opt/stack/docker-compose.yml`.

## Prerequisites

- SSH access: `ssh bilgin@192.168.50.100`
- Existing `/opt/stack/docker-compose.yml` setup
- Cloudflare Tunnel configured

## Quick Setup (5 minutes)

```bash
# 1. Clone repository
cd /opt/stack
git clone <your-repo-url> secure-sequentier

# 2. Create directories (create them first!)
sudo mkdir -p /opt/stack/config/secure-sequentier
sudo mkdir -p /opt/stack/data/secure-sequentier/watch
sudo mkdir -p /opt/stack/data/secure-sequentier/queue
sudo mkdir -p /opt/stack/data/secure-sequentier/processed
sudo mkdir -p /opt/stack/data/secure-sequentier/logs
sudo mkdir -p /opt/stack/data/secure-sequentier/subsystems

# 3. Set permissions (after directories are created)
sudo chown -R bilgin:bilgin /opt/stack/config/secure-sequentier
sudo chown -R bilgin:bilgin /opt/stack/data/secure-sequentier

# 3. Copy config
cp secure-sequentier/SecureSolution2/DefaultConfig.linux.json.example \
   /opt/stack/config/secure-sequentier/DefaultConfig.json

# 4. Edit docker-compose.yml
sudo nano /opt/stack/docker-compose.yml
```

Add these services to your `docker-compose.yml` (copy from `docker-compose.stack.yml`):

```yaml
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

```bash
# 5. Build and start
cd /opt/stack
sudo docker compose build secure-sequentier-backend secure-sequentier-frontend
sudo docker compose up -d secure-sequentier-backend secure-sequentier-frontend

# 6. Configure Cloudflare Tunnel
# Option A: If you use the Cloudflare Zero Trust Dashboard (web UI):
#   Go to: https://one.dash.cloudflare.com → Networks → Tunnels → Your Tunnel → Public Hostname
#   Add: secure.ybilgin.com → http://192.168.50.100:5188
#
# Option B: If you have a local cloudflared config file:
#   Find it with: sudo find / -name "config.yml" -path "*cloudflared*" 2>/dev/null
#   Then add to the ingress section:
#     - hostname: secure.ybilgin.com
#       service: http://192.168.50.100:5188
#   And restart cloudflared:
#   sudo docker compose restart cloudflared
```

## Verify

```bash
# Check containers
sudo docker ps | grep secure-sequentier

# Test locally
curl http://192.168.50.100:5188
curl http://192.168.50.100:9999/api/health

# Access via domain
# Open: https://secure.ybilgin.com
```

## Update Later

```bash
cd /opt/stack/secure-sequentier
git pull
cd /opt/stack
sudo docker compose build secure-sequentier-backend secure-sequentier-frontend
sudo docker compose up -d secure-sequentier-backend secure-sequentier-frontend
```

Done! 🎉

