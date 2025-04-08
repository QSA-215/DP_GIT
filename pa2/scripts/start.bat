@echo off

cd ..\Valuator\bin\Debug\net8.0

start valuator.exe run --urls "http://0.0.0.0:5001"
start valuator.exe run --urls "http://0.0.0.0:5002"

cd ..\..\..\..\nginx
start nginx