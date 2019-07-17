FROM node:10.16.0-alpine AS base-image
LABEL author="Devil"

# Building Angular app
WORKDIR /app
COPY package*.json /app/
RUN npm ci
COPY . /app
RUN npm run build

FROM nginx:1.16.0-alpine
RUN rm -rf /usr/share/nginx/html/*
COPY --from=base-image /app/dist /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
# CMD ["nginx", "-g", "daemon off;"]



# EXPOSE 4200
# RUN npm run build -- --prod
# CMD npm run start -- --host 0.0.0.0
 
# This image will be used for creating container
# FROM node:8.9-alpine
# WORKDIR /app
# Copying dist folder from node-angular-cli image
# COPY --from=test/pokemon-img /app/dist/browser .
# EXPOSE 80
# ENV PORT 80
# RUN npm install http-server -g
# CMD [ "npm start" ]
