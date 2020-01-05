extends Node

const FOREST_ITEMS_TO_SPAWN = 10
const COMMUNITY_XP_TO_LEVEL_UP = 10

var player # used for easy handling of stuff like freeze

# Used when transitioning from map => fishing => map
var player_position:Vector2 = Vector2.ZERO # saved to transition to/from fishing
var player_inventory = [] # Array of strings for now

# a hack of a hack of a hack; keep NPC[0]'s quest state
var quest_indicies = {
	"Farmer": 0
}

# Items spawned today; removed on consumption
var forest_items = []

var community_xp = 9
var community_level = 2

func sanitize_name(item_name):
	# Not sure why it *sometimes* is like @Sockeye@5
	if item_name[0] == "@":
		item_name = item_name.substr(1, len(item_name) - 1)
	if item_name.find('@') > -1:
		item_name = item_name.substr(0, item_name.find('@'))
	item_name = item_name.to_lower()
	return item_name

func gain_community_points(amount):
	community_xp += amount
	var levelled_up = false
	
	while community_xp >= COMMUNITY_XP_TO_LEVEL_UP:
		community_xp -= COMMUNITY_XP_TO_LEVEL_UP
		community_level += 1
		levelled_up = true
	
	return levelled_up