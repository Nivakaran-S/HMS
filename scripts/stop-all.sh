#!/bin/bash

echo "🛑 Stopping Hospital Management System..."
echo "========================================"

cd "$(dirname "$0")/.."

echo "Stopping all containers..."
docker-compose down

echo ""
echo "✅ All services stopped"
echo ""
echo "💡 To remove volumes (delete all data):"
echo "   docker-compose down -v"
echo ""