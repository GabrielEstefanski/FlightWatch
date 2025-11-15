#!/bin/bash

echo "🐳 Building FlightWatch Docker Image..."

docker-compose build --no-cache

if [ $? -eq 0 ]; then
    echo "✅ Build completed successfully!"
    echo ""
    echo "🚀 To run the application:"
    echo "   docker-compose up"
else
    echo "❌ Build failed!"
    exit 1
fi

