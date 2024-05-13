# notes

Connecting via localhost doesn't work between macOS and Windows running on Parallels Desktop, so we need to fix this. 

https://forum.parallels.com/threads/access-local-network-of-mac-from-windows.359527/

https://forum.parallels.com/threads/how-to-use-localhost-on-my-mac-from-inside-parallels.363066/

Getting the hostname on macOS:

```
ipconfig getifaddr en0
```

I set up the Networking mode in Parallels Desktop to `Bridged > Adapter`. 

https://apple.stackexchange.com/questions/261767/how-to-access-macos-localhost-port-3000-from-parallels-ie

List all network devices on Windows:

```
arp -a
```

https://en.wikipedia.org/wiki/Port_forwarding