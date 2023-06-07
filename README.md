# Level Generation Prototype

###[Web Demo](https://jongamedev.itch.io/level-generation-prototype?secret=L9M5bJ1y4MZm5PhPhqLEPzJ9CE)

### LevelBuilder.cs
- Places an Origin object (Transform + Collider) at the Builder location, the Builder is moved afterwards
- Builder location is moved Up, Left, Down, or Right after placing an Origin
  - A raycast checks if an existing origin is in a selected direction before moving
  - If no directions are open, the Builder object is moved back to the first location before checking again. It moves to the next location in the array until an opening is found

### WallGenerator.cs
- Builds a Wall if no bordering origins are found
- Builds a Door if a bordering origin is found

### RoomGenerator.cs
- Builds a StartingRoom at index [0], and a Boss Room at the last index
- Available location indexes are stored in a List, this list is updated as rooms are built
- Places a number of random Shop and Trial rooms (if there are variants) at random available locations
- Normal rooms are placed throughout the remaining locations
