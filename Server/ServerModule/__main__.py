import socketserver

from .StartServer import GameHandler

#HOST_NAME = "212.83.56.221"
HOST_NAME = "localhost"
PORT = 8080

with socketserver.TCPServer((HOST_NAME, PORT), GameHandler) as tcp:
    tcp.serve_forever()
