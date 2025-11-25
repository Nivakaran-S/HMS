#!/bin/bash

echo "🏥 Starting Hospital Management System..."
echo "========================================"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Navigate to project root
cd "$(dirname "$0")/.."

echo "🔨 Building Docker images..."
docker-compose build

echo ""
echo "🚀 Starting all services..."
docker-compose up -d

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 10

echo ""
echo "📊 Service Status:"
docker-compose ps

echo ""
echo "✅ All services started!"
echo ""
echo "🌐 Access URLs:"
echo "   Frontend:        http://localhost:3000"
echo "   API Gateway:     http://localhost:5000"
echo "   Eureka Dashboard: http://localhost:8761"
echo "   Keycloak Admin:  http://localhost:8080 (admin/admin)"
echo ""
echo "⚠️  Important: Configure Keycloak realm and users before using the app"
echo "   1. Go to http://localhost:8080"
echo "   2. Login with admin/admin"
echo "   3. Create 'hospital' realm (if not exists)"
echo "   4. Create a client 'hospital-frontend'"
echo "   5. Create users in the realm"
echo ""
echo "📝 View logs:"
echo "   docker-compose logs -f <service-name>"
echo ""
echo "🛑 Stop all services:"
echo "   ./scripts/stop-all.sh"
echo ""