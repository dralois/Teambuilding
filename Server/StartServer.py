import http.server
from http import HTTPStatus
from Server.DataClasses import Manager
import socketserver
import hashlib
from http.server import HTTPServer, BaseHTTPRequestHandler
import sys
# useless comment
HOST_NAME = 'holymacaroni.de'
PORT = 8080
room_key = ""
manager = Manager()
gamestate = 0
identifier = 0
participants = {}


class GameHandler(BaseHTTPRequestHandler):
    def do_HEAD(self):
        ...

    def do_GET(self):
        print(self.path)
        print(self.client_address)
        # print(self.headers)
        global room_key
        global identifier
        global gamestate
        name = "Manager"
        # todo: parse message from header
        room_key = "fill here"
        ident = 1809229009489699977
        if gamestate == 0:
            print(hash(name))
            manager.set(room_key, name, identifier)
            gamestate += 1
            identifier += 1
        elif gamestate == 1:
            if ident == manager.identifier:
                print("good input")
        # self.send_response_only(HTTPStatus.OK, f"path is: {self.path}, host, port: {self.client_address}")
        self.parse_message()
        self.send_header("Manager", "ID")
        self.end_headers()

    def do_POST(self):
        print(self.path)
        self.send_response_only(HTTPStatus.OK, "passt")
        self.parse_message()
        self.end_headers()

    def parse_message(self):
        nachricht = self.path.split("/")
        if nachricht[1] == "createRoom":
            self.create_room()
        elif nachricht[1] == "openRoom":
            self.open_room()
        elif nachricht[1] == "joinRoom":
            self.join_room()
        elif nachricht[1] == "readyPlayer":
            self.ready_player()
        elif nachricht[1] == "KILLME":
            sys.exit()
        else:
            print("bad input")

    def create_room(self):
        global gamestate
        success = True
        if gamestate != 0:
            print("creating a room at this state of the game is not possible")
            success = False
        else:
            global manager
            global room_key
            global identifier
            room_key = self.headers["roomKey"]
            manager.set(room_key, self.headers["name"], identifier)
            identifier += 1
            gamestate += 1

        self.send_header("success", str(success))
        self.send_header("ident", str(manager.identifier))



    def open_room(self):
        ...

    def join_room(self):
        ...

    def ready_player(self):
        ...

def start():
    PORT = 8080
    Handler = GameHandler


    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print("serving at port", PORT)
        httpd.serve_forever()


if __name__ == "__main__":
    start()
