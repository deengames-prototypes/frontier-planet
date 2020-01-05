extends Node2D

export var resource_probability = 50

const Mushroom = preload("res://Entities/World/Items/Mushroom.tscn")
const Morel = preload("res://Entities/World/Items/Morel.tscn")
const Herb = preload("res://Entities/World/Items/Herb.tscn")

# TODO: could probably be preload(...) => int
const SPAWN_PROBABILITIES = {
	"Morel": 20,
	"Mushroom": 50,
	"Herb": 30
}

func _ready():
	for child in $ResourceSpot.get_children():
		if randi() % 100 <= resource_probability:
			# SPAWN MORE OVERLORDS!
			# TODO: could be more dynamic
			var spawn_what = randi() % 100
			if spawn_what <= SPAWN_PROBABILITIES["Morel"]:
				_spawn(Morel, child.position)
			elif spawn_what <= SPAWN_PROBABILITIES["Morel"] + SPAWN_PROBABILITIES["Mushroom"]:
				_spawn(Mushroom, child.position)
			else:
				_spawn(Herb, child.position)
				
func _spawn(what, where):
	var instance = what.instance()
	instance.position = where
	add_child(instance)
	print(str(what))