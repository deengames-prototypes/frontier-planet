extends Node2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

const Home = preload("res://Scenes/Home.tscn")

func _ready():
	randomize()
	call_deferred("_change_scene")

func _change_scene():
	var home = Home.instance()
	home.start_new_day()
	SceneManagement.change_map_to(get_tree(), home, "Start")