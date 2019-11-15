cd %4
Rem default access permission list
dcomperm -da set %1\IUSR_%1 permit
dcomperm -da set %1\IWAM_%1 permit
dcomperm -da set Interactive permit
dcomperm -da set %1\%2 permit

Rem default launch permission list
dcomperm -dl set %1\IUSR_%1 permit
dcomperm -dl set %1\IWAM_%1 permit
dcomperm -dl set Interactive permit
dcomperm -dl set %1\%2 permit

Rem word.exe
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set Interactive permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set Interactive permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -runas {00020906-0000-0000-C000-000000000046} %2 %3

Rem excel.exe
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set Interactive permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set Interactive permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -runas {00020812-0000-0000-C000-000000000046} %2 %3

Rem powerpoint.exe
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IUSR_%1 permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IWAM_%1 permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set Interactive permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\%2 permit

dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IUSR_%1 permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IWAM_%1 permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set Interactive permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\%2 permit

dcomperm -runas {64818D10-4F9B-11CF-86EA-00AA00B929E8} %2 %3

Rem DO IT AGAIN

Rem default access permission list
dcomperm -da set %1\IUSR_%1 permit
dcomperm -da set %1\IWAM_%1 permit
dcomperm -da set Interactive permit
dcomperm -da set %1\%2 permit

Rem default launch permission list
dcomperm -dl set %1\IUSR_%1 permit
dcomperm -dl set %1\IWAM_%1 permit
dcomperm -dl set Interactive permit
dcomperm -dl set %1\%2 permit

Rem word.exe
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set Interactive permit
dcomperm -aa {00020906-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set Interactive permit
dcomperm -al {00020906-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -runas {00020906-0000-0000-C000-000000000046} %2 %3

Rem excel.exe
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set Interactive permit
dcomperm -aa {00020812-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\IUSR_%1 permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\IWAM_%1 permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set Interactive permit
dcomperm -al {00020812-0000-0000-C000-000000000046} set %1\%2 permit

dcomperm -runas {00020812-0000-0000-C000-000000000046} %2 %3

Rem powerpoint.exe
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IUSR_%1 permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IWAM_%1 permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set Interactive permit
dcomperm -aa {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\%2 permit

dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IUSR_%1 permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\IWAM_%1 permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set Interactive permit
dcomperm -al {64818D10-4F9B-11CF-86EA-00AA00B929E8} set %1\%2 permit

dcomperm -runas {64818D10-4F9B-11CF-86EA-00AA00B929E8} %2 %3