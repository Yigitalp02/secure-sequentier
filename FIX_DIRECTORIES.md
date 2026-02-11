# Fix: Directory Creation Issue

The error occurs because the directories don't exist yet. Run these commands **in order**:

## Step-by-Step Fix

```bash
# 1. First, make sure you're in the right place
cd /opt/stack

# 2. Create the parent config directory if it doesn't exist
sudo mkdir -p /opt/stack/config

# 3. Create the secure-sequentier config directory
sudo mkdir -p /opt/stack/config/secure-sequentier

# 4. Create the parent data directory if it doesn't exist
sudo mkdir -p /opt/stack/data

# 5. Create all secure-sequentier data subdirectories
sudo mkdir -p /opt/stack/data/secure-sequentier/watch
sudo mkdir -p /opt/stack/data/secure-sequentier/queue
sudo mkdir -p /opt/stack/data/secure-sequentier/processed
sudo mkdir -p /opt/stack/data/secure-sequentier/logs
sudo mkdir -p /opt/stack/data/secure-sequentier/subsystems

# 6. Now set permissions (this will work now that directories exist)
sudo chown -R bilgin:bilgin /opt/stack/config/secure-sequentier
sudo chown -R bilgin:bilgin /opt/stack/data/secure-sequentier

# 7. Verify directories were created
ls -la /opt/stack/config/
ls -la /opt/stack/data/secure-sequentier/
```

## Alternative: One-Liner Approach

If you prefer, you can create everything at once:

```bash
# Create all directories
sudo mkdir -p /opt/stack/config/secure-sequentier
sudo mkdir -p /opt/stack/data/secure-sequentier/{watch,queue,processed,logs,subsystems}

# Set permissions
sudo chown -R bilgin:bilgin /opt/stack/config/secure-sequentier
sudo chown -R bilgin:bilgin /opt/stack/data/secure-sequentier
```

## Verify It Worked

```bash
# Check if directories exist
ls -ld /opt/stack/config/secure-sequentier
ls -ld /opt/stack/data/secure-sequentier

# Check permissions
ls -la /opt/stack/config/ | grep secure-sequentier
ls -la /opt/stack/data/ | grep secure-sequentier
```

You should see `bilgin` as the owner.

