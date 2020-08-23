import json

class Manager:
    def __init__(self):
        self.key = 0
        self.picture = 0
        self.identifier = 0

    def set(self, key, picture, identifier):
        self.key = key
        self.picture = picture
        self.identifier = identifier

class ParticipantEncoder(json.JSONEncoder):

    def default(self, obj):
        try:
            it = iter(obj)
        except TypeError:
            pass
        else:
            return list(it)
        return super().default(obj)
