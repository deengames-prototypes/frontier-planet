extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	call_deferred("position_player")

func position_player():
	$MapPlayer.position = get_node(Globals.target_node).position
	