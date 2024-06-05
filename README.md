# Runtime Node System for Unity

> ## Demo
### <a href="https://ysj0828.github.io/NodeSystem_Demo/" target="_blank">Click Here!</a>

<br>
<br>

> ### [Update log](./UpdateLog.md)

> ### Overview

Node System is a UI Framework to create UI objects that can be visually linked.

There are 6 main modules: Manager, Entity, Node, Connection, Line Renderer and Pointer.

<br>
<br>
<br>

> Manager

Manager is a component attached to a Canvas that handles the addition/removal of entities, connections between entities, and holds data for global settings of connections.
 
> Entity

Entity is an object that contains nodes as its children. Its border is highlighted once selected and can be moved by dragging.
 
> Node

Node is where all the connections are connected to and from. There are mainly two types of nodes: Input and output. The node class has a list of its own connections, keeping the track of which entity is connected to this node.
 
> Connection

Connection component visually connects nodes with lines. It holds a reference to the Line that is rendered by Line Renderer component along with its width, colour and shape.
 
> Line Renderer

Line renderer component is a UI Graphic component that draws all the lines. It stores all the line references and their points.


> Pointer

Pointer component is responsible for handling click and drag actions of the cursor.
