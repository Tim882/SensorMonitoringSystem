FROM node:18-alpine AS development

WORKDIR /app

# Копируем файлы конфигурации для кэширования
COPY package.json ./
COPY vite.config.ts ./
COPY tsconfig.json ./
COPY tsconfig.node.json ./
COPY index.html ./

# Устанавливаем зависимости
RUN npm install

# Копируем исходный код
COPY src ./src
COPY public ./public

EXPOSE 3000

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

FROM node:18-alpine AS builder

WORKDIR /app

COPY package.json ./
COPY vite.config.ts ./
COPY tsconfig.json ./
COPY tsconfig.node.json ./
COPY index.html ./

RUN npm install

COPY src ./src
COPY public ./public

RUN npm run build

FROM nginx:alpine AS production

COPY --from=builder /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]