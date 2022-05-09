# build
FROM node:lts-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm install -g cnpm --registry=https://registry.npm.taobao.org
RUN cnpm install
COPY . .
RUN npm run build

# deploy
FROM nginx:stable-alpine as deploy
COPY --from=build /app/dist/ /usr/nginx/wwwroot
COPY /nginx/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

