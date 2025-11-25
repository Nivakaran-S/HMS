#!/bin/bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

pushd "$ROOT_DIR" > /dev/null

echo "🛠️ Building backend services..."
for SERVICE in PatientService DoctorService AppointmentService BillingService ApiGateway; do
  echo " - Building $SERVICE"
  dotnet publish "backend/src/${SERVICE}/${SERVICE}.csproj" -c Release -o "artifacts/${SERVICE}"
done

echo "🐋 Building Docker images..."
docker build -t hospital/patient-service:latest -f backend/src/PatientService/Dockerfile backend/src
docker build -t hospital/doctor-service:latest -f backend/src/DoctorService/Dockerfile backend/src
docker build -t hospital/appointment-service:latest -f backend/src/AppointmentService/Dockerfile backend/src
docker build -t hospital/billing-service:latest -f backend/src/BillingService/Dockerfile backend/src
docker build -t hospital/api-gateway:latest -f backend/src/ApiGateway/Dockerfile backend/src
docker build -t hospital/hms-frontend:latest -f frontend/Dockerfile frontend

echo "✅ Build complete."

popd > /dev/null

