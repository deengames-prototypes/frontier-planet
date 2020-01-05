extends Node

const FOREST_ITEMS_TO_SPAWN = 3

# Used when transitioning from map => fishing => map
var player # used for easy handling of stuff like freeze
var player_position:Vector2 = Vector2.ZERO
var player_inventory = [] # Array of strings for now
# a hack of a hack of a hack; keep NPC[0]'s quest state
var quest_indicies = {
	"Farmer": 0
}

# Items spawned today; removed on consumption
var forest_items = []
