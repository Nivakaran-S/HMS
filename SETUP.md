# Hospital Management System - Setup Guide

## 📋 Complete Setup Instructions

### Step 1: Prerequisites Installation

#### Install Docker Desktop
1. Download from https://www.docker.com/products/docker-desktop
2. Install and start Docker Desktop
3. Verify installation:
```bash
docker --version
docker-compose --version
```

#### Install .NET 9 SDK (Optional - for local development)
1. Download from https://dotnet.microsoft.com/download/dotnet/9.0
2. Verify:
```bash
dotnet --version
```

#### Install Node.js 20+ (Optional - for local frontend development)
1. Download from https://nodejs.org
2. Verify:
```bash
node --version
npm --version
```

### Step 2: Clone and Setup Project

```bash
# Clone repository
git clone <your-repo-url>
cd hospital-management-system

# Make scripts executable
chmod +x scripts/*.sh
```

### Step 3: Configure Environment Variables

The system uses default configuration suitable for Docker Compose. If you need to modify:

#### Backend Services
Edit `backend/src/<ServiceName>/appsettings.json`

#### Frontend
Edit `frontend/.env.local`

### Step 4: Start the System

```bash
./scripts/start-local.sh
```

This command will:
1. Build all Docker images
2. Start PostgreSQL databases
3. Start Kafka & Zookeeper
4. Start Keycloak
5. Start Eureka Server
6. Start all microservices
7. Start API Gateway
8. Start Frontend

**Initial startup may take 5-10 minutes** as Docker pulls images and builds services.

### Step 5: Configure Keycloak

#### Access Keycloak Admin Console
1. Navigate to http://localhost:8080
2. Click "Administration Console"
3. Login with:
   - Username: `admin`
   - Password: `admin`

#### Import the Predefined Realm
We ship a production-ready realm export in `infrastructure/keycloak/realm-export.json` with roles (`admin`, `doctor`, `billing`, `reception`) and the required clients.

1. In the Admin Console, hover over "Master" and choose "Add realm"
2. Click `Select file` and pick `infrastructure/keycloak/realm-export.json`
3. Click `Create`
4. The `hospital` realm will be imported automatically

#### Create Client
1. Go to "Clients" in left sidebar
2. Click "Create client"
3. Client ID: `hospital-frontend`
4. Client type: `OpenID Connect`
5. Click "Next"
6. Enable these options:
   - ✅ Client authentication: OFF
   - ✅ Authorization: OFF
   - ✅ Standard flow: ON
   - ✅ Direct access grants: ON
7. Click "Next"
8. Valid redirect URIs: `http://localhost:3000/*`
9. Web origins: `http://localhost:3000`
10. Click "Save"

#### Create Client for Backend API
1. Create another client
2. Client ID: `hospital-api`
3. Client authentication: ON
4. Save and note the client secret

#### Create Users
1. Go to "Users" in left sidebar
2. Click "Add user"
3. Fill in details:
   - Username: `doctor1`
   - Email: `doctor1@hospital.com`
   - First name: `John`
   - Last name: `Doe`
4. Click "Create"
5. Go to "Credentials" tab
6. Click "Set password"
7. Enter password (e.g., `password`)
8. Turn OFF "Temporary"
9. Click "Save"

Repeat for more users (admin, receptionist, etc.)

### Step 6: Access the Application

#### Open Frontend
1. Navigate to http://localhost:3000
2. You'll be redirected to Keycloak login
3. Enter credentials (e.g., doctor1 / password)
4. You should see the dashboard

#### Verify Services
- **Eureka Dashboard**: http://localhost:8761
  - All services should appear in the registry
- **API Gateway Swagger**: http://localhost:5000/swagger (if enabled)
- **Individual Service Swagger**:
  - Patient: http://localhost:5001/swagger
  - Doctor: http://localhost:5002/swagger
  - Appointment: http://localhost:5003/swagger
  - Billing: http://localhost:5004/swagger

### Step 7: Test the Application

#### Create a Doctor
1. Navigate to "Manage Doctors"
2. Click "Add Doctor"
3. Fill in form and submit

#### Create a Patient
1. Navigate to "Manage Patients"
2. Click "Add Patient"
3. Fill in form and submit

#### Book an Appointment
1. Navigate to "Manage Appointments"
2. Click "Book Appointment"
3. Select patient and doctor
4. Choose date/time
5. Submit

#### Complete Appointment & Generate Bill
1. Find the appointment
2. Click "Complete"
3. Check "View Billing" - a bill should be auto-generated via Kafka

## 🔧 Troubleshooting

### Issue: Services not starting

**Solution:**
```bash
# Check Docker logs
docker-compose logs <service-name>

# Common service names:
# - eureka-server
# - patient-service
# - doctor-service
# - appointment-service
# - billing-service
# - api-gateway
# - frontend
```

### Issue: Port already in use

**Solution:**
```bash
# Find process using port (example: 5000)
lsof -i :5000

# Kill process
kill -9 <PID>

# Or change port in docker-compose.yml
```

### Issue: Database connection failed

**Solution:**
```bash
# Wait for databases to be fully ready
docker-compose logs postgres-patient

# Restart specific service
docker-compose restart patient-service
```

### Issue: Kafka connection errors

**Solution:**
```bash
# Check Kafka is running
docker-compose logs kafka

# Restart Kafka and dependent services
docker-compose restart zookeeper kafka
docker-compose restart appointment-service billing-service
```

### Issue: Keycloak redirect errors

**Solution:**
1. Verify client configuration in Keycloak
2. Check redirect URIs match `http://localhost:3000/*`
3. Ensure realm is `hospital`
4. Clear browser cache and cookies

### Issue: 401 Unauthorized errors

**Solution:**
1. Check token is valid
2. Verify Keycloak Authority URL in appsettings.json
3. Ensure user has proper realm role
4. Check token expiration

## 🧪 Testing Individual Services

### Test Patient Service
```bash
# Get JWT token first from Keycloak
TOKEN="your-jwt-token"

# Create patient
curl -X POST http://localhost:5000/api/patients \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@email.com",
    "phone": "1234567890",
    "dateOfBirth": "1990-01-01",
    "gender": "Male",
    "bloodType": "O+"
  }'

# Get all patients
curl http://localhost:5000/api/patients \
  -H "Authorization: Bearer $TOKEN"
```

### Test Kafka Message Flow
1. Create an appointment via frontend
2. Complete the appointment
3. Check logs:
```bash
docker-compose logs appointment-service | grep "published"
docker-compose logs billing-service | grep "received"
```
4. Verify bill was created in Billing page

## ☸️ Deploying to Kubernetes

The repository now contains production-ready manifests under `infrastructure/kubernetes`. To deploy:

```bash
./scripts/start-k8s.sh
```

The script will:

1. Create/update the `hospital` namespace
2. Apply ConfigMaps, Secrets, and PVCs
3. Deploy PostgreSQL, Kafka/Zookeeper, Keycloak, Eureka
4. Deploy all microservices, API gateway, and the frontend
5. Apply cluster Services and the `hospital.local` ingress

> Tip: Add `127.0.0.1 hospital.local` to your `/etc/hosts` to access the ingress locally.

## 📊 Monitoring

### View Service Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f patient-service

# Last 100 lines
docker-compose logs --tail=100 patient-service
```

### Check Service Health
```bash
curl http://localhost:5000/api/patients/health
curl http://localhost:5000/api/doctors/health
curl http://localhost:5000/api/appointments/health
curl http://localhost:5000/api/billing/health
```

### Database Access
```bash
# Connect to patient database
docker exec -it postgres-patient psql -U postgres -d patientdb

# List tables
\dt

# Query data
SELECT * FROM "Patients";
```

## 🔄 Updates and Maintenance

### Rebuild Single Service
```bash
docker-compose build patient-service
docker-compose up -d patient-service
```

### Rebuild All Services
```bash
docker-compose down
docker-compose build
docker-compose up -d
```

### Clear All Data (Fresh Start)
```bash
docker-compose down -v
docker-compose up -d
```

## 🎯 Next Steps

1. **Add More Features**:
   - Medical records management
   - Prescription management
   - Lab test results
   - Staff management

2. **Enhance Security**:
   - Role-based access control
   - Audit logging
   - Data encryption

3. **Scale Services**:
   - Add Redis for caching
   - Implement circuit breakers
   - Add API rate limiting

4. **Deploy to Production**:
   - Use Kubernetes manifests in `infrastructure/kubernetes/`
   - Configure production databases
   - Set up monitoring (Prometheus, Grafana)
   - Configure SSL/TLS

## 📞 Support

For issues and questions:
1. Check logs first
2. Review this guide
3. Check Docker container status
4. Verify network connectivity

Happy coding! 🎉