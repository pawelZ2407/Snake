# Snake
Everything is based on a grid system. 
Grid system class contains an array of cell positions and coordinates in column, row. (Always column first)
Grid system contain a dictionary with a tuple as a key and string as a value. String is defining what is inside cell.
Map Controller is responsible for controlling the map (spawning food etc.)
PlayerController contain grid based snake movement, and reaction on the enviroment
SettingsSO contain configurable settings for game. 
I choosed this option to manage it because it is easy to balance the game and add new things like difficulty levels
