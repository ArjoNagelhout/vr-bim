# notes

Connecting via localhost doesn't work between macOS and Windows running on Parallels Desktop, so we need to fix this. 

https://forum.parallels.com/threads/access-local-network-of-mac-from-windows.359527/

https://forum.parallels.com/threads/how-to-use-localhost-on-my-mac-from-inside-parallels.363066/

Getting the host ip address on macOS:

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

https://superuser.com/questions/153624/access-localhost-on-mac-os-x-from-parallels-machine

ifconfig vnic0

List all network devices on macOS:

```
sudo nmap -sP {host ip address}/24
```

sP is skip port scan

Router ip address
192.168.0.1

What we need to do right now is make sure that the client from a totally different device (the Meta Quest Pro) can talk to the WebSocket server on Windows. 

So parallels desktop actually sets up a virtual subnet / functions as a virtual router

virtual subnet

Bridged means it uses a virtual network interface card (NIC) and does not create a virtual subnet. 

This sounds like the one we need. Then we also need to set the IP address. 

192.168.0.100 is the static IP address now assigned to the server. 