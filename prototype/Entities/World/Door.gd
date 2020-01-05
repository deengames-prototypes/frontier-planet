extends Area2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

export var destination:String = "WorldMap"
export var location:String = ""

func _on_Door_body_entered(body):
	if body.name == "MapPlayer":
		var full_path = "res://Scenes/" + destination + ".tscn"
		var map = load(full_path).instance()
		call_deferred("_change_map", map)

func _change_map(map):
	SceneManagement.change_map_to(get_tree(), map, location)