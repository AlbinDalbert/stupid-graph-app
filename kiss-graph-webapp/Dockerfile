# Stage 1: Build the React app
# Use a suitable Node version
FROM node:18-alpine AS build
WORKDIR /app
COPY package.json ./
COPY package-lock.json ./
RUN npm install
COPY . ./
RUN npm run build

# Stage 2: Serve the static files using Nginx
FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
# You might need a custom nginx.conf if you're using React Router
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]