import http.server
import hashlib
import sys
import random
import json

from http import HTTPStatus
from http.server import HTTPServer, BaseHTTPRequestHandler

from .DataClasses import Manager, ParticipantEncoder, Participant, Participants


room_key = ""  # to join this room
manager = Manager()  # hold information about session
gamestate = 0
identifier = 0      # running int to identify users
participants = Participants()   # holds info of identifier (key) with value [name, ready]
picture_range = range(0, 0)
places = []         # array with identifiers, position in array determines picture the user gets
selected = []


class GameHandler(BaseHTTPRequestHandler):

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
        print(self.headers)
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
            try:
                room_key = self.headers["room_id"]
                manager.set(room_key, int(self.headers["picture"]), identifier)
                identifier += 1
                gamestate += 1
            except (KeyError, TypeError):
                self.send_header("success", str(False))
                self.send_header("reason", "worng headers")
                return

        self.send_header("success", str(success))
        self.send_header("pers_id", str(manager.identifier))

    def JOINROOM(self):
        global gamestate
        global participants
        global identifier
        global room_key
        try:
            if self.headers["room_id"] != room_key or gamestate != 1:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong room or wrong gamestate")
                return
            else:
                print(self.headers["name"])
                identifier += 1
                participants.items[identifier] = Participant()
                participants.items[identifier].set(identifier, self.headers["name"], False)
                self.send_header("success", str(True))
                self.send_header("pers_id", str(identifier))

        except KeyError:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong format")
            return

    def READY(self):
        global gamestate
        global participants
        global room_key
        if gamestate != 1:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.items or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong format")
                    return
                else:
                    if self.headers["ready"] == "True":
                        participants.items[int(self.headers["pers_id"])].ready = True
                    else:
                        participants.items[int(self.headers["pers_id"])].ready = False
                        # todo: ready loop if  everyone is ready, what
                    self.send_header("success", str(True))
                    self.send_header("no_user", str(len(participants.items)))
                    ready = 0
                    for _,user in participants.items.items():
                        if user.ready:
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
        global places
        global selected
        try:
            gamestate = 0
            global participants
            participants = Participants()
            places.clear()
            selected.clear()
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
            self.send_header("participants", ParticipantEncoder().encode(participants))
        elif gamestate == 2:
            self.send_header("places", json.dumps({"items" : places}))
            self.send_header("selected", json.dumps({"items" : selected}))
        elif gamestate == 3:
            self.send_header("places", json.dumps({"items" : places}))
            self.send_header("selected", json.dumps({"items" : selected}))
        else:
            self.send_header("success", str(False))

    def START(self):
        global gamestate
        global participants
        global manager
        global room_key
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
                    for _,user in participants.items.items():
                        if not user.ready:
                            self.send_header("success", str(False))
                            self.send_header("reason", "not everyone is ready")
                            return
                    gamestate += 1
                    if manager.picture == 1:
                        select_picture_parts(4, len(participants.items.items()))
                    elif manager.picture == 2:
                        select_picture_parts(9, len(participants.items.items()))
                    else:
                        self.send_header("success", str(False))
                        self.send_header("reason", "picture does not exist")
                        return
                    global places
                    for _,user in participants.items.items():
                        places.append(user.identifier)
                    random.shuffle(places)
                    global selected
                    global picture_range
                    selected = [-1] * len(participants.items.items())
                    self.send_header("success", str(True))
                    self.send_header("places", json.dumps({"items" : places}))
                    self.send_header("range", json.dumps({"items" : list(picture_range)}))

            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def GETSTARTINFO(self):
        global gamestate
        global participants
        global manager
        global room_key
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.items or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    return
                else:
                    global places
                    global picture_range
                    self.send_header("places", json.dumps({"items" : places}))
                    self.send_header("picture", str(manager.picture))
                    self.send_header("range", json.dumps({"items" : list(picture_range)}))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def SUBMITPLACE(self):
        global gamestate
        global places
        global room_key
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in places or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    return
                else:
                    global selected
                    if self.headers["place"] == "-1":
                        selected[selected.index(self.headers["pers_id"])] = -1
                        self.send_header("success", str(True))
                    elif selected[int(self.headers["place"])] is -1:
                        if self.headers["pers_id"] in selected:
                            selected[selected.index(self.headers["pers_id"])] = -1
                        selected[int(self.headers["place"])] = int(self.headers["pers_id"])
                        self.send_header("success", str(True))
                    elif self.headers["pers_id"] == selected[int(self.headers["place"])]:
                        self.send_header("success", str(True))
                    else:
                        self.send_header("success", str(False))
                        self.send_header("reason", "already occupied")
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def END(self):
        global gamestate
        global manager
        global room_key
        global places
        if gamestate != 2:
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
                    global selected
                    for user in selected:
                        if user is None:
                            self.send_header("success", str(False))
                            self.send_header("reason", "Not everyone has selected a place")
                            return
                    gamestate += 1

                    self.send_header("success", str(True))
                    self.send_header("places", json.dumps({"items" : places}))
                    self.send_header("selected", json.dumps({"items" : selected}))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                return

    def GETRESULT(self):
        ...


def select_picture_parts(image_length, users):
    global picture_range
    start = random.randint(0, image_length - users)
    picture_range = range(start, start + users)
