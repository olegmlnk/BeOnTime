#!/bin/bash
# init-letsencrypt.sh
# First-time SSL certificate setup for Let's Encrypt

set -e

DOMAIN="be-on-time.online"
EMAIL="admin@be-on-time.online"
STAGING=0  # Set to 1 for testing (to avoid rate limits)

CERT_DIR="./certbot/conf/live/$DOMAIN"
RSA_KEY_SIZE=4096

echo "### Creating directories..."
mkdir -p "./certbot/conf/live/$DOMAIN"
mkdir -p "./certbot/www"

echo "### Creating dummy certificate for $DOMAIN..."
openssl req -x509 -nodes -newkey rsa:$RSA_KEY_SIZE -days 1 \
  -keyout "$CERT_DIR/privkey.pem" \
  -out "$CERT_DIR/fullchain.pem" \
  -subj "/CN=localhost" 2>/dev/null

echo "### Starting nginx..."
docker compose up -d frontend

echo "### Waiting for nginx to start..."
sleep 5

echo "### Removing dummy certificate..."
rm -rf "$CERT_DIR"

echo "### Requesting Let's Encrypt certificate for $DOMAIN..."

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
