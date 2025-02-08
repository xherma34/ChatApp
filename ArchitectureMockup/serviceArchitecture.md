## List of all calls that can be made
#### Terminology:
- IsAdmin $\rightarrow$ User requesting service method is of admin role
- IsSameId $\rightarrow$ User requesting service method has the same id as requested service of id
  - requestor_id = 1 - CALLS GetUserById(1) $\rightarrow$ IsSameId
- IsInChat $\rightarrow$ User requesting service method is in the chat of the requested chat information
  - requestor is in chatroom with id 3 and wants to see all messages of user with id 2 $\rightarrow$ he must be in the chatroom to see this information
- IsUser -> registered user authority
- 

### Table
|Request|Authority needed|Service|
|-|-|-|
|Get user by id|admin, IsSameId|User|
|Get all users|admin|User|
|Get all users in a chat|admin, IsInChat|User|
|Get all chat msgs of user|IsAdmin, IsSameId|Message|
|Get all chat msgs of user within a chat|IsAdmin, IsInChat|Chat OR Message|
|Get all chat rooms|IsUser|Chat|
|Get all chat rooms of user|IsSameId, IsAdmin|Chat|
|Get sender of message|IsAdmin, IsInChat|User|
