[![CI Build](https://github.com/aszego/winhello-secure/actions/workflows/ci-build.yml/badge.svg)](https://github.com/aszego/winhello-secure/actions/workflows/ci-build.yml)
# winhello-secure
Ever wanted to RDP into your machines without entering your credentials and *just use* Windows Hello's biometrics?

*winhello-secure* is Windows Hello-protected RDP credential storage and launch tool. Encrypts RDP passwords with Windows Hello (PIN or biometrics) and launches RDP sessions using stored credentials.

## Usage

### RDP

**Import credentials:**
```
winhello-secure.exe /importRdp <path> /username <username>
```
Prompts for the RDP password, encrypts it with Windows Hello, and stores it in JSON under `%LOCALAPPDATA%`.

**Launch RDP:**
```
winhello-secure.exe <rdpPath> [/multiMon]
```
Looks up stored credentials, decrypts via Windows Hello, creates a temporary RDP file with the password (still encrypted with DPAPI), and runs `mstsc`. Deletes the temporarz file in a few seconds.

Use `/multiMon` to patch the RDP file for multi-monitor layout.

### General (scripts)

```
winhello-secure.exe [encrypt|decrypt] <base64data>
```
Returns base64-encoded encrypted or decrypted data. Uses Windows Hello for cryptography. Useful for integrating with other tools or scripts.

## Requirements

- Windows with Windows Hello (PIN or biometrics)
- .NET 10.0

## License

winhello-secure is licensed under the [GNU General Public License v3.0](LICENSE).

## Credits

The `AuthProviders/` code is derived from [KeePassWinHello](https://github.com/sirAndros/KeePassWinHello) by sirAndros and contributors (MIT License). See [NOTICE](NOTICE) for details.
