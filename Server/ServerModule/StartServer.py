import http.server
import hashlib
import sys
import random

from http import HTTPStatus
from http.server import HTTPServer, BaseHTTPRequestHandler

from .DataClasses import Manager


room_key = ""  # to join this room
manager = Manager()  # hold information about session
gamestate = 0
identifier = 0      # running int to identify users
participants = {}   # holds info of identifier (key) with value [name, ready]
picture_range = range(0, 0)
places = []         # array with identifiers, position in array determines picture the user gets


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

        command = self.path.split("/")[1]
        if hasattr(self, command):
            getattr(self, command)()

        self.send_header("Manager", "ID")
        self.end_headers()

    def do_POST(self):
        print(self.path)
        self.send_response_only(HTTPStatus.OK, "passt")

        command = self.path.split("/")[1]
        if hasattr(self, command):
            getattr(self, command)()
        else:
            self.send_header("success", str(False))
        self.end_headers()

    def KILLME(self):
        sys.exit(0)

    def CREATEROOM(self):
        global gamestate
        success = True
        if gamestate != 0:
            print("creating a room at this state of the game is not possible")
            success = False
        else:
            global manager
            global room_key
            global identifier
            room_key = self.headers["room_id"]
            manager.set(room_key, self.headers["picture"], identifier)
            identifier += 1
            gamestate += 1

        self.send_header("success", str(success))
        self.send_header("pers_id", str(manager.identifier))

    def JOINROOM(self):
        global gamestate
        global participants
        global identifier
        try:
            if self.headers["room_id"] != room_key or gamestate != 1:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong room or wrong gamestate")
                return
            else:
                print(self.headers["name"])
                identifier += 1
                participants[identifier] = [self.headers["name"], False]
                self.send_header("success", str(True))
                self.send_header("pers_id", str(identifier))

        except KeyError:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong format")
            return

    def READY(self):
        global gamestate
        global participants
        if gamestate != 1:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.keys() or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong format")
                    return
                else:
                    participants[int(self.headers["pers_id"])][1] = bool(self.headers["ready"])
                    # todo: ready loop if  everyone is ready, what
                    self.send_header("success", str(True))
                    self.send_header("no_user", str(len(participants)))
                    ready = 0
                    for key in participants.values():
                        if key[1]:
                            ready += 1
                    self.send_header("no_ready", str(ready))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def RESET(self):
        global gamestate
        global room_key
        global identifier
        try:
            gamestate = 0
            global participants
            participants.clear()
            room_key = ""
            identifier = 0
            self.send_header("success", str(True))

        except KeyError:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong format")
            return

    def UPDATE(self):
        global gamestate
        self.send_header("gamestate", str(gamestate))
        if gamestate == 0:
            return
        elif gamestate == 1:
            global participants
            self.send_header("participants", str(participants))
        elif gamestate == 2:
            ...
        elif gamestate == 3:
            ...
        else:
            self.send_header("success", str(False))

    def START(self):
        global gamestate
        global participants
        global manager
        if gamestate != 1:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) != manager.identifier or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    return
                else:
                    for user in participants.keys():
                        if participants[user][1] is False:
                            self.send_header("success", str(False))
                            self.send_header("reason", "not everyone is ready")
                    gamestate += 1
                    global manager
                    if manager.picture == 1:
                        select_picture_parts(5, len(participants))
                    elif manager.picture == 2:
                        select_picture_parts(10, len(participants))
                    else:
                        self.send_header("success", str(False))
                        self.send_header("reason", "picture does not exist")
                        return
                    global places
                    for user in participants.keys():
                        places.append(user)
                    random.shuffle(places)
                    self.send_header("success", str(True))
                    self.send_header("places", str(places))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def GETSTARTINFO(self):
        global gamestate
        global participants
        global manager
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.keys() or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    return
                else:
                    global places
                    self.send_header("places", str(places))
                    self.send_header("picture", str(manager.picture))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return


def select_picture_parts(image_length, users):
    global picture_range
    start = random.randint(0, image_length - (users - 1))
    picture_range = range(start, start + (users - 1))
