#!/bin/sh
set -e

# Runtime config: replace ${API_URL} / ${IDENTITY_URL} / ${MAPBOX_TOKEN}
# placeholders (baked at build time from environment.prod.ts) with the values
# provided as environment variables. Lets one image run against any host without
# rebuilding — change env + restart instead of recompiling the Angular app.
DIR="${WWW_DIR:-/usr/share/nginx/html}"

find "$DIR" -type f -name "*.js" 2>/dev/null | while IFS= read -r f; do
  sed -i \
    -e "s|\${API_URL}|${API_URL}|g" \
    -e "s|\${IDENTITY_URL}|${IDENTITY_URL}|g" \
    -e "s|\${MAPBOX_TOKEN}|${MAPBOX_TOKEN}|g" \
    "$f"
done

exec "$@"
