# uart

## Feature
1. for visual studio study
## Use
Visual Studio
## 常用屬性

|Method |功能|
|-----|--------|
|Uart()|初始化 Uart       |
|Uart(string PortName, int BaudRate)  |初始化並自訂連接埠的 PortName 與 BaudRate      |
|void OpenSerial() |打開連接埠|
|void CloseSerial() |關閉連接埠|
|string ReadLine() |若讀取到 ‘\n’ 就回傳當前的字串，否則回傳 “”|
|void Send(string s) |透過 uart 傳入字串|
|void ClearBuffer() |清空 buffer|

## Tips
```sh
TBD