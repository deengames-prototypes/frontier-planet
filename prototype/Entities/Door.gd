extends Area2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

export var destination:String

func _on_Door_body_entered(body):
	# assumes body = player
	var full_path = "res://Scenes/" + destination + ".tscn"
	SceneManagement.change_scene_to(get_tree(), load(full_path).instance())
