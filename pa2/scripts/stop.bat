@echo off

cd ../nginx
nginx -s stop

taskkill /F /IM valuator.exe