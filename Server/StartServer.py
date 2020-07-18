import http.server
from http import HTTPStatus
import socketserver
from http.server import HTTPServer, BaseHTTPRequestHandler

HOST_NAME = 'holymacaroni.de'
PORT = 8080


class GameHandler(BaseHTTPRequestHandler):
    def do_HEAD(self):
        ...

    def do_GET(self):
        print(self.path)
        print(self.client_address)
        self.send_response_only(HTTPStatus.OK, f"path is: {self.path}, host, port: {self.client_address}")
        self.end_headers()

def start():
    PORT = 8080
    Handler = GameHandler

    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print("serving at port", PORT)
        httpd.serve_forever()


if __name__ == "__main__":
    start()
