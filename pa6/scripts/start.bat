@echo off

cd ..

docker-compose build --no-cache && docker-compose up -d