# Secure Sequentier - Secure File Processing System

A secure file processing system with a web interface that allows users to upload files and process them through various subsystems (signers, processors, etc.). The system provides real-time job monitoring, configurable processing pipelines, and comprehensive audit trails.

## Project Structure

- **SecureSolution2**: The main API service that handles file processing, job queuing, and orchestration
- **Web**: The web interface (MVC) that provides a user-friendly interface for file uploads and job monitoring

## Features

- **File Upload**: Upload multiple files through a web interface
- **Job Queue**: Process files in batches with configurable retry logic
- **Real-time Status**: Monitor job progress with SignalR updates
- **User Profiles**: Per-user configuration and file storage
- **History Tracking**: View past job runs and their status
- **Configurable Subsystems**: Support for multiple processing applications (signers, processors, etc.)

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Configuration

1. **Copy Configuration Template**: 
   ```bash
   cp SecureSolution2/DefaultConfig.json.example SecureSolution2/DefaultConfig.json
   ```

2. **Update Paths**: Edit `SecureSolution2/DefaultConfig.json` and replace `{USER}` with your Windows username, or customize paths as needed:
   - Update executable paths for your subsystem applications
   - Update output directories
   - Configure timeout and retry settings

3. **Create Directories**: Create the following directory structure (replace `{USER}` with your username):
   ```
   C:\Users\{USER}\SecureWatch\
   C:\Users\{USER}\SecureSequentialProfiles\{USER}\
   C:\Users\{USER}\Processed\{USER}\
   C:\Users\{USER}\Subsystems\
   ```

4. **Subsystem Applications**: Place your processing applications in the Subsystems directory:
   - SignerApp.exe
   - DummySuccessApp.exe
   - etc.

### Running the Application

#### **Easy Method (Recommended):**
1. **Run the startup script**:
   ```powershell
   .\start_secure_solution.ps1
   ```
   - This will automatically start both backend and frontend
   - Opens the web interface in your browser
   - Press any key in the console to stop both applications

#### **Manual Method:**
1. **Start the API Service**:
   ```bash
   cd SecureSolution2
   dotnet run
   ```
   The API will run on `http://localhost:9999`

2. **Start the Web Interface**:
   ```bash
   cd Web
   dotnet run
   ```
   The web interface will run on `http://localhost:5188`

### Usage

1. Open the web interface in your browser
2. Select a target application (signer, sub-system1, etc.)
3. Upload one or more files
4. Monitor the processing status in real-time
5. View job history and details

## Configuration Details

### DefaultConfig.json
- `WatchDirectory`: Directory where uploaded files are placed
- `QueueDirectory`: Directory for job queue files and logs
- `TimeoutSeconds`: Timeout for each file processing
- `DefaultRetryCount`: Number of retries for failed files
- `Mapping`: Configuration for each subsystem (executable path, output directory)

### User Configuration
The system automatically creates per-user configurations by replacing `{USER}` placeholders with the current user's name.

## Architecture

- **QueueStore**: Manages job queues and persistence
- **OrchestratorBackgroundService**: Processes jobs in the background
- **QueueHub**: SignalR hub for real-time updates
- **FileQueueController**: Web interface for file uploads and status
- **SecureSequentialApi**: HTTP client for communication between projects

## Development Notes

- The system uses Serilog for structured logging
- Jobs are persisted as JSON files in daily queue files
- Each user has their own directory structure
- The web interface polls for status updates
- SignalR provides real-time updates for active jobs

## Troubleshooting

1. **Build Issues**: Ensure all NuGet packages are restored
2. **Path Issues**: Verify all paths in configuration files exist
3. **Permission Issues**: Ensure the application has write access to configured directories
4. **Port Conflicts**: Check if ports 5087 and the web port are available

## License

This project was developed as part of an internship project.

## Docker Deployment

This application can be deployed on Linux servers using Docker. See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed instructions on deploying to a Linux server with Cloudflare Tunnel.

### Quick Docker Start (Development)

```bash
docker-compose up --build
```

### Production Deployment

See `DEPLOYMENT.md` for production deployment instructions including:
- Linux server setup
- Cloudflare Tunnel configuration
- Production docker-compose setup
- Signature application builds for Linux

## Author

Yiğit Alp Bilgin - Internship Project

---

**Note**: Make sure to copy `DefaultConfig.json.example` to `DefaultConfig.json` and update the paths before running the application.

