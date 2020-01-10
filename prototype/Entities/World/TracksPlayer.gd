extends Area2D

var _player_in_range = false
var _player = null

signal player_arrived
signal player_interacted
signal player_departed

func _on_body_entered(body):
	if body.name == "MapPlayer":
		_player_in_range = true
		_player = body
		self.emit_signal("player_arrived")

func _on_body_exited(body):
	if body.name == "MapPlayer" and _player_in_range:
		_player_in_range = false
		_player = null
		self.emit_signal("player_departed")

func _unhandled_key_input(event):
	if _player_in_range and event.is_pressed() and event.is_action_pressed("ui_accept"):
		self.emit_signal("player_interacted")