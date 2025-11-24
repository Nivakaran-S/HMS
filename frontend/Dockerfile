# Use Node.js LTS version
FROM node:20-alpine

# Set working directory inside the container
WORKDIR /app

# Copy package files first to use Docker caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the Next.js application
RUN npm run build

# Expose the port the app runs on
EXPOSE 3000

# Start the application
CMD ["npm", "start"]