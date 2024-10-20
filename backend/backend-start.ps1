Start-Process "C:\Program Files\Redis\redis-server.exe"
Start-Sleep -Milliseconds 1000

Start-Process "dotnet" -ArgumentList "run" -WorkingDirectory "C:\Users\user\Desktop\VS_Code\Trading Application\Backend\UserMicroService"
Start-Sleep -Milliseconds 1000

Start-Process "dotnet" -ArgumentList "run" -WorkingDirectory "C:\Users\user\Desktop\VS_Code\Trading Application\Backend\MarketDataMicroService"
Start-Sleep -Milliseconds 1000

Start-Process "dotnet" -ArgumentList "run" -WorkingDirectory "C:\Users\user\Desktop\VS_Code\Trading Application\Backend\PortfolioMicroService"
Start-Sleep -Milliseconds 1000

Start-Process "dotnet" -ArgumentList "run" -WorkingDirectory "C:\Users\user\Desktop\VS_Code\Trading Application\Backend\OrderMicroService"
