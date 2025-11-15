#!/bin/bash

echo "🏥 Checking FlightWatch Health..."
echo ""

HEALTH_URL="http://localhost:8080/health"

response=$(curl -s -o /dev/null -w "%{http_code}" $HEALTH_URL)

if [ $response -eq 200 ]; then
    echo "✅ FlightWatch is HEALTHY"
    echo ""
    echo "📊 Full health response:"
    curl -s $HEALTH_URL | jq .
else
    echo "❌ FlightWatch is UNHEALTHY (HTTP $response)"
    echo ""
    echo "📋 Check logs:"
    echo "   docker-compose logs -f flightwatch-api"
fi

