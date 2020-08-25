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

    def do_OPTIONS(self):
        self.send_response(200, "ok")
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Credentials", "true")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Request-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, room_id, picture, success, pers_id, name, ready, no_user, no_ready, gamestate, participants, places, selected, range, reason")
        self.send_header("Access-Control-Request-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, room_id, picture, success, pers_id, name, ready, no_user, no_ready, gamestate, participants, places, selected, range, reason")
        self.send_header("Access-Control-Expose-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, room_id, picture, success, pers_id, name, ready, no_user, no_ready, gamestate, participants, places, selected, range, reason")
        self.end_headers()

    def do_GET(self):
        print("GET")
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

        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Credentials", "true")
        self.send_header("Manager", "ID")
        self.end_headers()

    def do_POST(self):
        print("POST")
        print(self.path)
        self.send_response_only(HTTPStatus.OK, "passt")
        print(self.headers)
        command = self.path.split("/")[1]
        self.send_header("Content-type", "text/plain")
        if hasattr(self, command):
            getattr(self, command)()
        else:
            self.send_header("success", str(False))
            self.end_headers()

        # self.send_header("Access-Control-Allow-Origin", "*")
        # self.send_header("Access-Control-Allow-Credentials", "true")

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
                self.end_headers()
                self.wfile.write(b'"success": False\n')
                return

        self.send_header("success", str(success))
        self.send_header("pers_id", str(manager.identifier))
        self.end_headers()
        self.wfile.write(b'"success": True\n')
        answer = f'"pers_id": {str(manager.identifier)}\n'
        self.wfile.write(bytes(answer, "utf-8"))

    def JOINROOM(self):
        global gamestate
        global participants
        global identifier
        global room_key
        try:
            if self.headers["room_id"] != room_key or gamestate != 1:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong room or wrong gamestate")
                self.end_headers()
                self.wfile.write(b'"success": False\n')
                return
            else:
                print(self.headers["name"])
                identifier += 1
                participants.items[identifier] = Participant()
                participants.items[identifier].set(identifier, self.headers["name"], False)
                self.send_header("success", str(True))
                self.send_header("pers_id", str(identifier))
                self.end_headers()
                self.wfile.write(b'"success": True\n')
                answer = f'"pers_id": {str(identifier)}\n'
                self.wfile.write(bytes(answer, "utf-8"))

        except KeyError:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong format")
            self.end_headers()
            self.wfile.write(b'"success": False\n')
            return

    def READY(self):
        global gamestate
        global participants
        global room_key
        if gamestate != 1:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            self.end_headers()
            self.wfile.write(b'"success": False\n')
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.items or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong format")
                    self.end_headers()
                    self.wfile.write(b'"success": False\n')
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
                    for _, user in participants.items.items():
                        if user.ready:
                            ready += 1
                    self.send_header("no_ready", str(ready))
                    self.end_headers()
                    self.wfile.write(b'"success": True\n')
                    answer = f'"no_user": {str(len(participants.items))}\n'
                    self.wfile.write(bytes(answer, "utf-8"))
                    answer = f'"no_ready": {str(ready)}\n'
                    self.wfile.write(bytes(answer, "utf-8"))

            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                self.end_headers()
                self.wfile.write(b'"success": False\n')
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
            self.end_headers()
            self.wfile.write(b'"success": True\n')

        except KeyError:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong format")
            self.end_headers()
            self.wfile.write(b'"success": False\n')
            return

    def UPDATE(self):
        global gamestate
        self.send_header("gamestate", str(gamestate))
        if gamestate == 0:
            self.end_headers()
            answer = f'"gamestate": {str(gamestate)}'
            self.wfile.write(bytes(answer, "utf-8"))
            return
        elif gamestate == 1:
            global participants
            self.send_header("participants", ParticipantEncoder().encode(participants))
            self.end_headers()
            answer = f'"gamestate": {str(gamestate)}'
            self.wfile.write(bytes(answer, "utf-8"))
            answer = f'"participants": {ParticipantEncoder().encode(participants)}'
            self.wfile.write(bytes(answer, "utf-8"))

        elif gamestate == 2:
            self.send_header("places", json.dumps({"items" : places}))
            self.send_header("selected", json.dumps({"items" : selected}))
            self.end_headers()
            answer = f'"gamestate": {str(gamestate)}'
            self.wfile.write(bytes(answer, "utf-8"))
            answer = json.dumps({"items":places})
            self.wfile.write(bytes(answer, "utf-8"))
            answer = json.dumps({"items":selected})
            self.wfile.write(bytes(answer, "utf-8"))
        elif gamestate == 3:
            self.send_header("places", json.dumps({"items" : places}))
            self.send_header("selected", json.dumps({"items" : selected}))
            self.end_headers()
            answer = f'"gamestate": {str(gamestate)}'
            self.wfile.write(bytes(answer, "utf-8"))
            answer = json.dumps({"items":places})
            self.wfile.write(bytes(answer, "utf-8"))
            answer = json.dumps({"items":selected})
            self.wfile.write(bytes(answer, "utf-8"))
        else:
            self.send_header("success", str(False))
            self.end_headers()
            answer = f'"success": False'
            self.wfile.write(bytes(answer, "utf-8"))

    def START(self):
        global gamestate
        global participants
        global manager
        global room_key
        if gamestate != 1:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            self.end_headers()
            return
        else:
            try:
                if int(self.headers["pers_id"]) != manager.identifier or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    self.end_headers()
                    return
                else:
                    for _,user in participants.items.items():
                        if not user.ready:
                            self.send_header("success", str(False))
                            self.send_header("reason", "not everyone is ready")
                            self.end_headers()
                            return
                    gamestate += 1
                    if manager.picture == 1:
                        select_picture_parts(4, len(participants.items.items()))
                    elif manager.picture == 2:
                        select_picture_parts(9, len(participants.items.items()))
                    else:
                        self.send_header("success", str(False))
                        self.send_header("reason", "picture does not exist")
                        self.end_headers()
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
                    self.end_headers()
                    answer = json.dumps({"items" : places})
                    self.wfile.write(bytes(answer,"utf-8"))
                    answer = json.dumps({"items" : list(picture_range)})
                    self.wfile.write(bytes(answer,"utf-8"))


            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                self.end_headers()
                return

    def GETSTARTINFO(self):
        global gamestate
        global participants
        global manager
        global room_key
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            self.end_headers()
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in participants.items or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    self.end_headers()
                    return
                else:
                    global places
                    global picture_range
                    self.send_header("places", json.dumps({"items" : places}))
                    self.send_header("picture", str(manager.picture))
                    self.send_header("range", json.dumps({"items" : list(picture_range)}))
                    self.end_headers()
                    answer = json.dumps({"items" : places})
                    self.wfile.write(bytes(answer,"utf-8"))
                    answer = json.dumps({"items" : list(picture_range)})
                    self.wfile.write(bytes(answer,"utf-8"))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                self.end_headers()
                return

    def SUBMITPLACE(self):
        global gamestate
        global places
        global room_key
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            self.end_headers()
            return
        else:
            try:
                if int(self.headers["pers_id"]) not in places or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    self.end_headers()
                    return
                else:
                    global selected
                    if self.headers["place"] == "-1":
                        selected[selected.index(self.headers["pers_id"])] = -1
                        self.send_header("success", str(True))
                        self.end_headers()
                    elif selected[int(self.headers["place"])] is -1:
                        if self.headers["pers_id"] in selected:
                            selected[selected.index(self.headers["pers_id"])] = -1
                        selected[int(self.headers["place"])] = int(self.headers["pers_id"])
                        self.send_header("success", str(True))
                        self.end_headers()
                    elif self.headers["pers_id"] == selected[int(self.headers["place"])]:
                        self.send_header("success", str(True))
                        self.end_headers()
                    else:
                        self.send_header("success", str(False))
                        self.send_header("reason", "already occupied")
                        self.end_headers()
                        self.wfile.write(b'False')
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                self.end_headers()
                return

    def END(self):
        global gamestate
        global manager
        global room_key
        global places
        if gamestate != 2:
            self.send_header("success", str(False))
            self.send_header("reason", "wrong gamestate")
            self.end_headers()
            return
        else:
            try:
                if int(self.headers["pers_id"]) != manager.identifier or self.headers["room_id"] != room_key:
                    self.send_header("success", str(False))
                    self.send_header("reason", "wrong room or not manager")
                    self.end_headers()
                    return
                else:
                    global selected
                    for user in selected:
                        if user is None:
                            self.send_header("success", str(False))
                            self.send_header("reason", "Not everyone has selected a place")
                            self.end_headers()
                            self.wfile.write(b'False')
                            return
                    gamestate += 1

                    self.send_header("success", str(True))
                    self.send_header("places", json.dumps({"items" : places}))
                    self.send_header("selected", json.dumps({"items" : selected}))
                    self.end_headers()
                    answer = json.dumps({"items" : places})
                    self.wfile.write(bytes(answer, "utf-8"))
                    answer = json.dumps({"items" : selected})
                    self.wfile.write(bytes(answer, "utf-8"))
            except KeyError:
                self.send_header("success", str(False))
                self.send_header("reason", "wrong format")
                self.end_headers()
                return

    def GETRESULT(self):
        ...


def select_picture_parts(image_length, users):
    global picture_range
    start = random.randint(0, image_length - users)
    picture_range = range(start, start + users)
