cd %~dp0
installutil TPXQohSync.exe
sc config XsScTPXQohSync start=auto
sc failure XsScTPXQohSync reset= 432000  actions= restart/30000/restart/60000/restart/90000