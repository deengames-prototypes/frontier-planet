extends Area2D

var _player_in_range = false
var _player = null

func _on_Lake_body_entered(body):
	if body.name == "MapPlayer":
		_player_in_range = true
		_player = body

func _on_Lake_body_exited(body):
	if body.name == "MapPlayer" and _player_in_range:
		_player_in_range = false
		_player = null

func _unhandled_key_input(event):
	if _player_in_range and event.is_pressed() and event.is_action_pressed("ui_accept"):
		# save player position and GO FISHIN~!
		Globals.player_position = _player.position
		get_tree().change_scene("res://Scenes/Fishing.tscn")