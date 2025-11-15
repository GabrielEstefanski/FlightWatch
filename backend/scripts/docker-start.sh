#!/bin/bash

if [ ! -f .env ]; then
    echo "⚠️  .env file not found!"
    echo "📋 Creating .env from .env.example..."
    cp .env.example .env
    echo ""
    echo "⚙️  Please edit .env file and add your API keys:"
    echo "   - AVIATION_STACK_API_KEY"
    echo "   - JWT_SECRET_KEY"
    echo ""
    read -p "Press Enter after configuring .env to continue..."
fi

echo "🐳 Starting FlightWatch..."
docker-compose up --build

echo ""
echo "🛑 FlightWatch stopped"

