# Chat app - Architecture
## Goal of the app
- Join or create chats
- Send messages in real-time (SignalR, Websockets)
- Recieve notifications
- Store and Retrieve Chat history (PostgreSQL + Redis Cache?)
- Handle security (Rate-limiting, authentication, authorization)
- Scale and Deploy (API gateway, microservices, Docker?)

## Features to implement?
- Sending friend requests and having friend lists
  - Services update -> you can get information on friendly users
  - Models update -> friends list object
  - Relationship definition for DbContext
- Blocking of other users
  - User property BlockedUsers ->
  - Update service logic -> blocked user's messages are not seen by the blockator and vice versa
- Message reactions: create a table for this MessageReaction:
  - id, messageid, userid, emoji (string)
- Messages nested threading
  - Add a ParentMessageId property to Message model and implement services for add (as threaded) etc.
- Banning users from chat rooms or whole accounts

## Functionality
- users can be part of a chat
- Users can create and be part of groupchats
- ?Users can get notifications
- Store and retrieve chat history
- Security (OAuth, jwt, etc.)
- Scale and deploy efficiently

## Entities and their relationship
#### Definition of entities usage
|	**Entity**	|	**Purpose**	|
|---------------|---------------|
|User|Stores user data|
|Chat|Represents conversation 1-to-1|
|Message|Stores message between users|
|UserChat|Joint table for chat and user|
|Notification|friend request, new msg, GC invite|

##### Entities

|User|Chat|UserChat|Message|Notification|
|-|-|-|-|-|
|<<P.K>> ID|<<P.K>> ID|<<F.K>> UserID|<<P.K>> ID|Notification|
|Nickname|Name|<<F.K>> ChatID|<<F.K>> SenderID|<<P.K>> ID|
|Password hash|Type|Role|<<F.K>> RecipientID|<<F.K>> UserID|
|Mail address|||<<F.K>> ChatID|Message|
|Date of join|||Content|Type|
||||Timestamp||

### Model use case
- **User**
  - Gets his chats
  - Participates in chats
  - Writes messages
  - Gets notifications
  - Creates chats
  - Manages people/chats based on role

### ER-diagram
|Entity|Relationship|Entity|
|-|-|-|
|User (sends)|1-N|Message|
|User (participates in)|N-N|Chat|
|User (has)|1-0..N|Notification|
|User (Links to chat)|1-N|UserChat|
|Chat (Links to user)|1-N|UserChat|
|Chat (consists of)| 1-N|Message|

### TODO: ER-diagram scheme

## Modules and what they do
#### 1. Authentication module
- User registration (JWT)
- OAuth login (Google/GitHub login)
- Role-Based authorization (User/Admin)
- JWT Token refresh system

##### Submodules:
- AuthService.cs $\rightarrow$ login, registration, token management
- UserController.cs $\rightarrow$ API endpoints
- IdentifyServer4 $\rightarrow$ for OAuth
- JWT Token generator $\rightarrow$ Issue secure tokens

#### 2. User management module
- View user profile
- Update user info
- Search users by username
- Manage user status

#### 3. Chat module
- Create 1-on-1 chats or groupchats
- Join/Leave chat
- Assign chat admins

#### 4. Messaging module (Real-Time messaging)
- Send and recieve Messages in Real-time (SignalR websockets)
- Store chats via PostgreSQL
- Retrieve messages (Pagination?)

#### 5. Notificaiton module?
TBD

#### 6. Users search module?
- Search users by username
- Search messages in chat

#### 7. API and security
- API gateway (Ocelot?)
- Rate-limiting
- CORS policies (prevents unathorized access)

#### 8. DB and Caching module
- PostgreSQL for primary storage
- Redis caching for chat history
- DB indexing and optimization




