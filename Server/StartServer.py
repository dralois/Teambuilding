import http.server
from http import HTTPStatus
import socketserver
from http.server import HTTPServer, BaseHTTPRequestHandler

HOST_NAME = 'holymacaroni.de'
PORT = 8080


class Gamehandler(BaseHTTPRequestHandler):
    def do_HEAD(self):
        ...

    def do_GET(self):
        print(self.path)
        print(self.client_address)
        self.send_response_only(HTTPStatus.OK, f"path is: {self.path}, host, port: {self.client_address}")


def run(server_class=HTTPServer, handler_class=BaseHTTPRequestHandler):
    server_address = ('', 8000)
    httpd = server_class(server_address, handler_class)
    httpd.serve_forever()


def start():
    PORT = 8080
    Handler = Gamehandler

    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print("serving at port", PORT)
        httpd.serve_forever()


if __name__ == "__main__":
    start()
