import socketserver
import os.path

from .StartServer import GameHandler

__isServer = os.path.exists(os.path.abspath(os.path.join(__file__, "..//..//IsServer")))

__HOST_NAME = ("localhost", "212.83.56.221")[__isServer]
__PORT = 8080

with socketserver.TCPServer((__HOST_NAME, __PORT), GameHandler) as tcp:
    tcp.serve_forever()
