# communication protocol
This is the design for the communication protocol between Revit and Unity. 
Note that this is not meant to be a production ready application and thus everything is hardcoded and intended to only work between 1 server and 1 client. 

Data that should be communicated:
- Geometry (triangle meshes)
- Materials

In Unity, we have multiple storages, each stored with a hash map:
- Geometry. identifier: GeometryId (long)
- Materials. identifier: MaterialId (long)
- Elements. identifier: ElementId (long)

## Conflict resolution
Whenever a change is made by the server, and the change has also been made by the client, the server takes precedent. We could implement a locking mechanism where only one client (assuming the Revit UI also as a client) can modify data, and when they have acquired a lock, all other clients are notified that they can't edit that data until the client has finished the editing operation. 

## Steps:

Notes: how do we store each element, geometry and material so that we can calculate the hash and see whether it requires updating?

### Setup
Client connects with server.
Server returns which document is opened: DocumentId (or if no document is opened)
Client communicates what it has cached. 
Server sends only data that is not cached.

### During
DocumentChanged events

