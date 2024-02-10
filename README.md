# SharpZipAES

[English](https://github.com/yutianqaq/SharpZipAES/blob/main/readme_en.md) 简体中文

使用 csharp (.net 4.5)开发的工具，用于压缩和加密文件，以缩短传输时间。支持多文件压缩和加密、单文件压缩、目录压缩。

```
 .\SharpZipAES.exe
Author: Yutian
Github: https://github.com/yutianqaq/SharpZipAES
[-] Usage:
  SharpZipAES.exe encrypt <encryption key> <path to compress>
  SharpZipAES.exe decrypt <encryption key> <path to encrypted ZIP>
```

## 多个文件压缩并加密
```powershell
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe encrypt Password1 .\SharpZipAES.exe .\SharpZipAES.exe.config .\SharpZipAES.pdb
Author: Yutian
[+] Wrote encrypted archive
[+] Packed compressed file
[+] Packed compressed file to swvHxCvZ.zip succeeded
[+] Wrote encrypted archive swvHxCvZ.aes.zip to disk!
[+] Removed encryption key from memory
[+] Deleted unecrypted archive
[+] Ready for exfil
[+] Program run time is: 00:00:00.3377748
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe decrypt Password1 .\swvHxCvZ.aes.zip
Author: Yutian
[+] Wrote encrypted archive
[+] Decrypting .\swvHxCvZ.aes.zip
[+] Decrypted swvHxCvZ.zip successfully!
[+] Program run time is: 00:00:00.3165076
```

## 单个文件压缩并加密
```powershell
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe encrypt Password1 .\SharpZipAES.exe
Author: Yutian
[+] Wrote encrypted archive
[+] Packed compressed file
[+] Packed compressed file to MGvmLoY2.zip succeeded
[+] Wrote encrypted archive MGvmLoY2.aes.zip to disk!
[+] Removed encryption key from memory
[+] Deleted unecrypted archive
[+] Ready for exfil
[+] Program run time is: 00:00:00.3319586
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe decrypt Password1 .\MGvmLoY2.aes.zip
Author: Yutian
[+] Wrote encrypted archive
[+] Decrypting .\MGvmLoY2.aes.zip
[+] Decrypted MGvmLoY2.zip successfully!
[+] Program run time is: 00:00:00.3204786
```

## 目录压缩并加密

```powershell
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe encrypt Password1 .\Test\
Author: Yutian
[+] Wrote encrypted archive
[+] Packed compressed directory to tvdOAOZP.zip succeeded
[+] Wrote encrypted archive tvdOAOZP.aes.zip to disk!
[+] Removed encryption key from memory
[+] Deleted unecrypted archive
[+] Ready for exfil
[+] Program run time is: 00:00:31.9781834
PS C:\Users\havoc\Desktop\SharpZipAES\SharpZipAES\bin\Release> .\SharpZipAES.exe decrypt Password1 .\tvdOAOZP.aes.zip
Author: Yutian
[+] Wrote encrypted archive
[+] Decrypting .\tvdOAOZP.aes.zip
[+] Decrypted tvdOAOZP.zip successfully!
[+] Program run time is: 00:00:11.5933884
```



# References

https://github.com/uknowsec/SharpZip

https://github.com/matterpreter/OffensiveCSharp
