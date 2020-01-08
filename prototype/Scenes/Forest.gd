extends Node2D

const Mushroom = preload("res://Entities/World/Items/Mushroom.tscn")
const Morel = preload("res://Entities/World/Items/Morel.tscn")
const Herb = preload("res://Entities/World/Items/Herb.tscn")

const monsters = [
	preload("res://Entities/World/Monsters/Slime.tscn")#,
	#preload("res://Entities/World/Monsters/Wolf.tscn")
]

func _ready():
	
	_spawn_items()
	_spawn_monsters()
	
func _spawn_monsters():
	for spot in $Monsters.get_children():
		var index = randi() % len(monsters)
		var which = monsters[index]
		var monster = which.instance()
		monster.position = spot.position
		add_child(monster)

func _spawn_items():
	var spot_num = 1
	for spawn_what in Globals.forest_items:
		# SPAWN MORE OVERLORDS!
		# TODO: could be more dynamic
		if spawn_what == "Morel":
			_spawn(Morel, spot_num, "Morel")
		elif spawn_what == "Mushroom":
			_spawn(Mushroom, spot_num, "Mushroom")
		else: # Herb
			_spawn(Herb, spot_num, "Herb")
		spot_num += 1

func _spawn(what, spot_num, item):
	var where = $ResourceSpot.get_node("Spot" + str(spot_num)).position
	var instance = what.instance()
	instance.position = where
	add_child(instance)
	# Hack: remove the first matching item
	instance.connect("on_picked_up", self, "_remove_forest_item", [item])

# Hack: remove the first matching item
func _remove_forest_item(item):
	Globals.forest_items.remove(Globals.forest_items.find(item))