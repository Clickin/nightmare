# Nightmare Web Server

## Description

Nightmare is a web server based on AspNetCore that supports the latest protocols and scripting languages. Currently, it supports PHP and CSX scripts.

## Quick start

```shell
docker run --name Website1 -d -p 80:8080 -p 443:8443/udp -p 443:8443 \
  -v <VHOST_DIR>:/app/vhost \
  nyakuz/nightmare:latest
```

An example of a vhost path.
```
<VHOST_DIR>/
- anyhost
- localhost
-- index.php
-- test.csx
-- robots.txt
-- favicon.ico
- example.com/
-- ..
- localhost.txt (test.localhost\n\napi.localhost)
- example.com.txt (www.example.com\n\napi.example.com)
```
