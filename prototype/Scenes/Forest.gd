extends Node2D

const Mushroom = preload("res://Entities/World/Items/Mushroom.tscn")
const Morel = preload("res://Entities/World/Items/Morel.tscn")
const Herb = preload("res://Entities/World/Items/Herb.tscn")

func _ready():
	
	var spot_num = 1
	for spawn_what in Globals.forest_items:
		# SPAWN MORE OVERLORDS!
		# TODO: could be more dynamic
		if spawn_what == "Morel":
			_spawn(Morel, spot_num)
		elif spawn_what == "Mushroom":
			_spawn(Mushroom, spot_num)
		else: # Herb
			_spawn(Herb, spot_num)
		spot_num += 1
				
func _spawn(what, spot_num):
	var where = $ResourceSpot.get_node("Spot" + str(spot_num)).position
	var instance = what.instance()
	instance.position = where
	add_child(instance)