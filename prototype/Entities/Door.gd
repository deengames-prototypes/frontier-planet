extends Area2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

export var destination:String = "WorldMap"
export var location:String = ""

func _on_Door_body_entered(body):
	if body.name == "MapPlayer":
		Globals.target_node = self.location
		var full_path = "res://Scenes/" + destination + ".tscn"
		self.get_tree().change_scene(full_path)
	
