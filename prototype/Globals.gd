extends Node

# Used when transitioning from map => fishing => map
var player # used for easy handling of stuff like freeze
var player_position:Vector2 = Vector2.ZERO
var player_inventory = [] # Array of strings for now