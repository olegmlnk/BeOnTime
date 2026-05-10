#!/bin/bash
# init-letsencrypt.sh
# First-time SSL certificate setup for Let's Encrypt
#
# Usage:
#   chmod +x init-letsencrypt.sh
#   ./init-letsencrypt.sh
#
# Prerequisites:
#   - Docker and Docker Compose installed
#   - Domain pointing to this server's IP
#   - Ports 80 and 443 open

set -e

# ============================================
# CONFIGURATION — Edit these values
# ============================================
DOMAIN="example.com"          # <-- Replace with your domain
EMAIL="admin@example.com"     # <-- Replace with your email
STAGING=0                     # Set to 1 for testing (to avoid rate limits)

# ============================================
# DO NOT EDIT BELOW THIS LINE
# ============================================

CERT_DIR="./certbot/conf/live"
RSA_KEY_SIZE=4096

echo "### Creating directories..."
mkdir -p ./certbot/conf/live
mkdir -p ./certbot/www

echo "### Downloading recommended TLS parameters..."
if [ ! -e "./certbot/conf/options-ssl-nginx.conf" ]; then
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf > "./certbot/conf/options-ssl-nginx.conf"
fi
if [ ! -e "./certbot/conf/ssl-dhparams.pem" ]; then
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem > "./certbot/conf/ssl-dhparams.pem"
fi

echo "### Creating dummy certificate for $DOMAIN..."
mkdir -p "$CERT_DIR"
openssl req -x509 -nodes -newkey rsa:$RSA_KEY_SIZE -days 1 \
  -keyout "$CERT_DIR/privkey.pem" \
  -out "$CERT_DIR/fullchain.pem" \
  -subj "/CN=localhost" 2>/dev/null

echo "### Starting nginx..."
docker compose up -d frontend

echo "### Removing dummy certificate..."
rm -rf "$CERT_DIR"

echo "### Requesting Let's Encrypt certificate for $DOMAIN..."

# Select staging or production server
if [ $STAGING != "0" ]; then
  STAGING_ARG="--staging"
else
  STAGING_ARG=""
fi

docker compose run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  $STAGING_ARG \
  --email $EMAIL \
  --agree-tos \
  --no-eff-email \
  -d $DOMAIN \
  -d www.$DOMAIN

echo "### Reloading nginx..."
docker compose exec frontend nginx -s reload

echo ""
echo "============================================"
echo " SSL certificate obtained successfully!"
echo " Domain: $DOMAIN"
echo "============================================"
echo ""
echo "Now start all services with:"
echo "  docker compose up -d"
