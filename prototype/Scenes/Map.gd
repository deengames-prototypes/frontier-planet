extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	call_deferred("position_player")

func move_player_to(node_name):
	if node_name != null:
		$MapPlayer.position = get_node(node_name).position
	elif Globals.player_position != Vector2.ZERO:
		$MapPlayer.position = Globals.player_position
		Globals.player_position = Vector2.ZERO
	