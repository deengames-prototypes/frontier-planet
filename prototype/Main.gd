extends Node2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

const Home = preload("res://Scenes/Home.tscn")

func _ready():
	call_deferred("_change_scene")

func _change_scene():
	SceneManagement.change_scene_to(get_tree(), Home.instance(), "Start")