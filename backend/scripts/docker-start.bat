<<<<<<< HEAD
@echo off

if not exist .env (
    echo [WARNING] .env file not found!
    echo [INFO] Creating .env from .env.example...
    copy .env.example .env
    echo.
    echo [CONFIG] Please edit .env file and add your API keys:
    echo    - AVIATION_STACK_API_KEY
    echo    - JWT_SECRET_KEY
    echo.
    pause
)

echo [INFO] Starting FlightWatch...
docker-compose up --build

echo.
echo [INFO] FlightWatch stopped
pause

=======
@echo off

if not exist .env (
    echo [WARNING] .env file not found!
    echo [INFO] Creating .env from .env.example...
    copy .env.example .env
    echo.
    echo [CONFIG] Please edit .env file and add your API keys:
    echo    - AVIATION_STACK_API_KEY
    echo    - JWT_SECRET_KEY
    echo.
    pause
)

echo [INFO] Starting FlightWatch...
docker-compose up --build

echo.
echo [INFO] FlightWatch stopped
pause

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
