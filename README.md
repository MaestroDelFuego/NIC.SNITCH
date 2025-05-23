# NIC.SNITCH

**NICSnitch** is a lightweight C# console application that monitors your Ethernet cable connection status and detects tampering events such as unplugging, reconnecting, and IP address changes.

---

## Features

- **Automatic IP learning** on startup
- Real-time detection of Ethernet cable **disconnects** and **reconnects**
- Alerts when the IP address changes after reconnection
- Color-coded console output for quick status recognition:
  - **Green**: Cable connected / IP match
  - **Orange**: Cable reconnected
  - **Red**: Cable disconnected / IP mismatch
- UTF-8 and emoji support for clear, user-friendly console messages

---

## Requirements

- .NET 6.0 or later
- Windows (best experience with Windows Terminal or another UTF-8 compatible console)
