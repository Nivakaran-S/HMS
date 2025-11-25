#!/bin/bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
K8S_DIR="$ROOT_DIR/infrastructure/kubernetes"

echo "🔧 Creating namespace (idempotent)..."
kubectl apply -f "$K8S_DIR/namespace.yaml"

echo "📦 Applying ConfigMaps and Secrets..."
kubectl apply -f "$K8S_DIR/configmaps/"
kubectl apply -f "$K8S_DIR/secrets/"

echo "💾 Applying persistent volume claims..."
kubectl apply -f "$K8S_DIR/persistentvolumes/"

echo "🚀 Deploying stateful services..."
kubectl apply -f "$K8S_DIR/deployments/postgres-deployments.yaml"
kubectl apply -f "$K8S_DIR/deployments/kafka-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/zookeeper-deployment.yaml"

echo "🚀 Deploying platform components..."
kubectl apply -f "$K8S_DIR/deployments/keycloak-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/eureka-deployment.yaml"

echo "🚀 Deploying application services..."
kubectl apply -f "$K8S_DIR/deployments/apigateway-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/frontend-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/patient-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/doctor-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/appointment-deployment.yaml"
kubectl apply -f "$K8S_DIR/deployments/billing-deployment.yaml"

echo "🔌 Applying services..."
kubectl apply -f "$K8S_DIR/services/"

echo "🌐 Applying ingress..."
kubectl apply -f "$K8S_DIR/ingress/ingress.yaml"

echo "✅ Kubernetes resources applied. Use 'kubectl get all -n hospital' to verify."

