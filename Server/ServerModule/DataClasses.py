import json
from typing import Dict

class Manager:
    def __init__(self):
        self.key = 0
        self.picture = 0
        self.identifier = 0

    def set(self, key, picture, identifier):
        self.key = key
        self.picture = picture
        self.identifier = identifier

class Participant:
    def __init__(self):
        self.identifier = -1
        self.name =""
        self.ready = False

    def set(self, identifier, name, ready):
        self.identifier = identifier
        self.name = name
        self.ready = ready

class Participants:
    def __init__(self):
        self.items : Dict[int, Participant]
        self.items = {}

    def set(self, items):
        self.items = items

class ParticipantEncoder(json.JSONEncoder):

    def default(self, obj):
        if isinstance(obj, Participant):
            return {"identifier":obj.identifier,
                    "name":obj.name,
                    "ready":obj.ready}
        elif isinstance(obj, Participants):
            return {"items":list(obj.items.values())}
        else:
            return super().default(obj)
