extends Node2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

const START_SCENE = "res://Scenes/Home.tscn"

func _ready():
	call_deferred("load_em_up")
	
func _load_em_up():
	SceneManagement.change_scene_to(get_tree(), load(START_SCENE).instance())
	add_child(load(START_SCENE).instance())
	get_tree().get_root().add_child(load("res://Scenes/Entities/Wall.tscn").instance())
	