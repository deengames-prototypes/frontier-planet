extends Area2D

export var damage_per_second = 10
var _player_in_range = false

func _on_body_entered(body):
	if body.name == "MapPlayer":
		_player_in_range = true

func _on_body_exited(body):
	if body.name == "MapPlayer" and _player_in_range:
		_player_in_range = false

func _process(delta):
	if _player_in_range:
		Globals.player_health -= (damage_per_second * delta)
		if Globals.player_health <= 0:
			Globals.player_health = 0
			Globals.player.get_parent().remove_child(Globals.player)
			Globals.player.queue_free()