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

func sanitize_name(item_name):
	# Not sure why it *sometimes* is like @Sockeye@5
	if item_name[0] == "@":
		item_name = item_name.substr(1, len(item_name) - 1)
	if item_name.find('@') > -1:
		item_name = item_name.substr(0, item_name.find('@'))
	item_name = item_name.to_lower()
	return item_name