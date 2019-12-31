extends Node2D

const SceneManagement = preload("res://Scripts/SceneManagement.gd")

const START_SCENE = "res://Scenes/Home.tscn"

func _ready():
	Globals.target_node = "Start"
	get_tree().change_scene(START_SCENE)
	