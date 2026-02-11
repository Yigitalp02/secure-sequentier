# Quick Start Guide - Linux Server Deployment

This is a condensed guide for deploying Secure Sequentier on your Ubuntu Server 24.04 LTS home server.

## Prerequisites Checklist

- [ ] Docker and Docker Compose installed
- [ ] Cloudflare Tunnel running
- [ ] Domain `ybilgin.com` configured in Cloudflare
- [ ] .NET 8.0 SDK installed (for building signature apps)

## Step 1: Clone and Setup

```bash
cd /opt/stack
git clone <your-repo-url> secure-sequentier
cd secure-sequentier

# Run deployment script
bash deploy.sh
```

The script will:
- Create directory structure
- Copy configuration template
- Optionally build signature apps
- Optionally build and start Docker containers

## Step 2: Configure

Edit the configuration file:
```bash
nano /opt/stack/config/secure-sequentier/DefaultConfig.json
```

Verify paths match your setup (defaults should work).

## Step 3: Build Signature Apps (If Needed)

If you have the signature app source code:
```bash
bash build-subsystems-linux.sh
```

Or build manually:
```bash
cd /path/to/SignerApp
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
cp bin/Release/net8.0/linux-x64/publish/SignerApp /opt/stack/data/secure-sequentier/subsystems/SignerApp/
chmod +x /opt/stack/data/secure-sequentier/subsystems/SignerApp/SignerApp
```

## Step 4: Start Services

```bash
cd /opt/stack/secure-sequentier
docker-compose -f docker-compose.production.yml up -d
```

## Step 5: Configure Cloudflare Tunnel

Add to your Cloudflare Tunnel config (`/opt/stack/config/cloudflared/config.yml`):

```yaml
ingress:
  # ... existing rules ...
  - hostname: secure.ybilgin.com
    service: http://localhost:5188
  - service: http_status:404  # Must be last
```

Restart tunnel:
```bash
sudo systemctl restart cloudflared
# or if using docker:
docker restart cloudflared
```

## Step 6: Configure Cloudflare Access (Optional)

1. Go to Cloudflare Zero Trust Dashboard
2. Add Application: `secure.ybilgin.com`
3. Set Policy: Email OTP (or your preference)

## Step 7: Verify

```bash
# Check containers
docker ps | grep secure-sequentier

# Check logs
docker-compose -f docker-compose.production.yml logs -f

# Test locally
curl http://localhost:5188
curl http://localhost:9999/api/health
```

Access via: `https://secure.ybilgin.com`

## Common Commands

```bash
# View logs
docker-compose -f docker-compose.production.yml logs -f

# Restart
docker-compose -f docker-compose.production.yml restart

# Stop
docker-compose -f docker-compose.production.yml down

# Update
cd /opt/stack/secure-sequentier
git pull
docker-compose -f docker-compose.production.yml up -d --build
```

## Troubleshooting

**Containers won't start:**
- Check logs: `docker-compose -f docker-compose.production.yml logs`
- Verify paths exist: `ls -la /opt/stack/config/secure-sequentier/`
- Check ports: `sudo netstat -tulpn | grep -E '9999|5188'`

**Can't access via domain:**
- Check tunnel: `sudo systemctl status cloudflared`
- Verify DNS: `dig secure.ybilgin.com`
- Check tunnel config syntax

**Signature apps not working:**
- Verify executables exist: `ls -la /opt/stack/data/secure-sequentier/subsystems/*/`
- Check permissions: `chmod +x /opt/stack/data/secure-sequentier/subsystems/*/*`
- Check logs for errors

For detailed information, see [DEPLOYMENT.md](DEPLOYMENT.md).

