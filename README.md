# 🔒 Secure Sequentier

A web-based file signing and verification platform powered by real cryptographic algorithms. Upload files for batch digital signing, verify file integrity, and calculate hashes — all through a modern web interface.

**Live:** [secure.ybilgin.com](https://secure.ybilgin.com)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| **Digital Signing** | Upload files and generate cryptographic signatures using SHA-256, SHA-512, and MD5 |
| **File Verification** | Verify file integrity by comparing against a known hash — runs entirely in-browser |
| **Hash Calculator** | Instantly calculate SHA-256, SHA-512, and MD5 hashes for any file — client-side |
| **Batch Processing** | Upload multiple files at once with real-time progress tracking |
| **Download Results** | Download all signed outputs as a ZIP archive |
| **Signature Certificate** | Generate a printable certificate for each signed batch |
| **Run History** | Browse, sort, and filter all past signing runs by date |
| **Dark Mode** | Toggle between light and dark themes |
| **Drag & Drop** | Modern drag-and-drop upload zones on all file inputs |
| **Auto-Cleanup** | Configurable retention period for processed files |

## 🏗️ Architecture

```
┌─────────────────┐     HTTP      ┌─────────────────┐
│   Frontend      │ ──────────►   │    Backend       │
│   (ASP.NET MVC) │   REST API    │   (ASP.NET Core) │
│   Port 5188     │ ◄──────────   │   Port 9999      │
└─────────────────┘               └────────┬─────────┘
                                           │
                                  ┌────────▼─────────┐
                                  │  Signer Apps      │
                                  │  • SignerApp      │
                                  │  • AdvancedSigner │
                                  └──────────────────┘
```

- **Frontend** — ASP.NET Core MVC with Bootstrap 5, Bootstrap Icons, CryptoJS
- **Backend** — Minimal API with SignalR, Serilog, background services
- **Subsystems** — Console apps that perform the actual cryptographic signing
- **Orchestrator** — Background service that dequeues jobs, spawns signer processes, handles retries/timeouts
- **Cleanup Service** — Background service that removes old files based on retention settings

## 🔐 Algorithms

| Algorithm | Output | Use Case |
|-----------|--------|----------|
| **SHA-256** | 64-char hex | Digital signatures, TLS, blockchain |
| **SHA-512** | 128-char hex | Password hashing, high-security verification |
| **MD5** | 32-char hex | Quick checksums, file integrity |

## 🚀 Deployment (Docker)

### Prerequisites

- Docker & Docker Compose
- Git

### Quick Start

```bash
# Clone
git clone https://github.com/Yigitalp02/secure-sequentier.git
cd secure-sequentier

# Create config
cp DefaultConfig.example.json SecureSolution2/DefaultConfig.json

# Build & run
docker compose up -d --build
```

The app will be available at:
- **Frontend:** http://localhost:5188
- **Backend API:** http://localhost:9999

### Production Deployment

For production on a Linux server with an existing Docker stack:

```bash
# Clone to your server
git clone https://github.com/Yigitalp02/secure-sequentier.git /opt/stack/secure-sequentier

# Create directories
sudo mkdir -p /opt/stack/data/secure-sequentier/{watch,queue,processed,logs}
sudo mkdir -p /opt/stack/config/secure-sequentier

# Copy and edit config
sudo cp /opt/stack/secure-sequentier/DefaultConfig.example.json \
        /opt/stack/config/secure-sequentier/DefaultConfig.json

# Add services to your docker-compose.yml (see docker-compose.stack.yml for reference)
# Build and start
sudo docker compose build secure-sequentier-backend secure-sequentier-frontend
sudo docker compose up -d secure-sequentier-backend secure-sequentier-frontend
```

### Configuration

Edit `DefaultConfig.json` to customize:

```json
{
  "WatchDirectory": "/app/data/watch",
  "QueueDirectory": "/app/data/queue/{USER}",
  "TimeoutSeconds": 30,
  "DefaultRetryCount": 3,
  "FileRetentionHours": 1,
  "Mapping": {
    "signer": {
      "ExecutablePath": "/app/subsystems/SignerApp/SignerApp",
      "OutputDirectory": "/app/data/processed/{USER}/signatures"
    },
    "advanced-signer": {
      "ExecutablePath": "/app/subsystems/AdvancedSignerApp/AdvancedSignerApp",
      "OutputDirectory": "/app/data/processed/{USER}/advanced-signatures"
    }
  }
}
```

| Setting | Description |
|---------|-------------|
| `TimeoutSeconds` | Max time per file before retry (hot-reloadable) |
| `DefaultRetryCount` | Number of retry attempts per file |
| `FileRetentionHours` | Hours before processed files are auto-deleted (0 = disabled) |
| `{USER}` | Placeholder replaced with the user's session ID at runtime |

### Cloudflare Tunnel (HTTPS)

To expose via Cloudflare Zero Trust Tunnel, add to your tunnel config:

```yaml
- hostname: secure.yourdomain.com
  service: http://secure-sequentier-frontend:5188
```

## 🛠️ Local Development

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run Locally

```bash
# Terminal 1 — Backend
cd SecureSolution2
dotnet run

# Terminal 2 — Frontend
cd Web
dotnet run
```

## 📁 Project Structure

```
secure-sequentier/
├── SecureSolution2/          # Backend API
│   ├── Program.cs            # Entry point, API endpoints
│   ├── Services/             # Orchestrator, Queue, Config, Cleanup
│   ├── Models/               # Job, JobFile, UserConfig
│   ├── Hubs/                 # SignalR hub
│   └── Dockerfile
├── Web/                      # Frontend MVC
│   ├── Controllers/          # FileQueueController (upload, verify, hash, etc.)
│   ├── Views/FileQueue/      # Index, Queue, History, Verify, HashCalculator, About, Certificate
│   ├── Services/             # API client
│   └── Dockerfile
├── Subsystems/               # Signer applications
│   ├── SignerApp/             # SHA-256 signer
│   └── AdvancedSignerApp/    # Multi-algorithm signer (SHA-256 + SHA-512 + MD5)
├── docker-compose.yml        # Local Docker setup
├── docker-compose.stack.yml  # Production stack reference
└── DefaultConfig.example.json
```

## 🧰 Tech Stack

- **Backend:** .NET 8, ASP.NET Core Minimal API, SignalR, Serilog
- **Frontend:** ASP.NET Core MVC, Bootstrap 5, Bootstrap Icons, CryptoJS
- **Infrastructure:** Docker, Docker Compose, Cloudflare Tunnel
- **Server:** Ubuntu Server 24.04 LTS

## 👤 Author

**Yiğit Alp Bilgin**

Developed as an internship project. Deployed on a home server via Docker and Cloudflare Zero Trust Tunnel.
