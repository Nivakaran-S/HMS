# Hospital Management System

A complete microservices-based hospital management system built with ASP.NET Core 9, Next.js, Kafka, and Keycloak.

## 🏗️ Architecture

### Backend Services (ASP.NET Core 9)
- **Service Discovery** - Eureka Server for service registration
- **API Gateway** - Ocelot for routing and load balancing
- **Patient Service** - Manage patient records
- **Doctor Service** - Manage doctor profiles
- **Appointment Service** - Schedule and manage appointments
- **Billing Service** - Handle billing and payments (with Kafka consumer)

### Frontend (Next.js 14)
- TypeScript-based React application
- Keycloak authentication
- Tailwind CSS styling
- Real-time updates

### Infrastructure
- **Kafka** - Message broker for inter-service communication
- **PostgreSQL** - Separate database per microservice
- **Keycloak** - Authentication and authorization
- **Docker & Docker Compose** - Containerization
- **Kubernetes** - Optional orchestration (manifests included)

## 📋 Prerequisites

- Docker Desktop (latest version)
- .NET 9 SDK (for local development)
- Node.js 20+ (for local frontend development)
- 8GB+ RAM recommended

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd hospital-management-system
```

### 2. Start All Services
```bash
# Make scripts executable
chmod +x scripts/*.sh

# Start everything with Docker Compose
./scripts/start-local.sh
```

This will start:
- 5 PostgreSQL databases
- Kafka & Zookeeper
- Keycloak
- Eureka Server
- 4 Microservices
- API Gateway
- Frontend

### 3. Access the Application

- **Frontend**: http://localhost:3000
- **API Gateway**: http://localhost:5000
- **Eureka Dashboard**: http://localhost:8761
- **Keycloak Admin**: http://localhost:8080 (admin/admin)

### 4. Default Login Credentials

After Keycloak initializes, create a user:
1. Go to http://localhost:8080
2. Login with admin/admin
3. Create a user in the 'hospital' realm
4. Use those credentials to login to the frontend

## 📁 Project Structure

```
hospital-management-system/
├── backend/src/
│   ├── Common/              # Shared library
│   ├── ServiceDiscovery/    # Eureka server
│   ├── ApiGateway/          # Ocelot gateway
│   ├── PatientService/      # Patient microservice
│   ├── DoctorService/       # Doctor microservice
│   ├── AppointmentService/  # Appointment microservice
│   └── BillingService/      # Billing microservice
├── frontend/                # Next.js application
├── infrastructure/          # Docker & K8s configs
└── scripts/                 # Utility scripts
```

## 🔧 Development

### Backend Development
```bash
cd backend/src/PatientService
dotnet run
```

### Frontend Development
```bash
cd frontend
npm install
npm run dev
```

## 📊 Service Ports

| Service | Port |
|---------|------|
| Frontend | 3000 |
| API Gateway | 5000 |
| Patient Service | 5001 |
| Doctor Service | 5002 |
| Appointment Service | 5003 |
| Billing Service | 5004 |
| Eureka Server | 8761 |
| Keycloak | 8080 |
| Kafka | 9092 |
| PostgreSQL (Patient) | 5432 |
| PostgreSQL (Doctor) | 5433 |
| PostgreSQL (Appointment) | 5434 |
| PostgreSQL (Billing) | 5435 |
| PostgreSQL (Keycloak) | 5436 |

## 🧪 Testing

### Test Endpoints
```bash
# Health checks
curl http://localhost:5000/api/patients/health
curl http://localhost:5000/api/doctors/health
curl http://localhost:5000/api/appointments/health
curl http://localhost:5000/api/billing/health
```

## 🛑 Stopping Services

```bash
./scripts/stop-all.sh
```

## 📝 Features

- ✅ Patient management (CRUD)
- ✅ Doctor management (CRUD)
- ✅ Appointment scheduling
- ✅ Automated billing via Kafka
- ✅ Service discovery with Eureka
- ✅ API Gateway with Ocelot
- ✅ JWT authentication via Keycloak
- ✅ Event-driven architecture
- ✅ Microservices pattern
- ✅ Docker containerization
- ✅ Responsive UI with Tailwind CSS

## 🔐 Security

- JWT-based authentication
- Keycloak for identity management
- CORS configured for frontend
- API Gateway authentication

## 📚 Documentation

See [SETUP.md](SETUP.md) for detailed setup instructions and troubleshooting.

## 🐛 Troubleshooting

### Services not starting?
- Ensure Docker Desktop is running
- Check port availability
- Run `docker-compose logs <service-name>` for errors

### Database connection errors?
- Wait for PostgreSQL containers to be healthy
- Check connection strings in appsettings.json

### Keycloak login issues?
- Ensure realm 'hospital' is created
- Create a user in Keycloak admin console
- Check client configuration

## 📄 License

MIT License

## 👥 Contributors

Nivakaran S. 