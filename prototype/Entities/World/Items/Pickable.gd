extends Area2D

signal on_picked_up

func _on_body_entered(body):
	if body.name == "MapPlayer":
		Globals.player_inventory.append(self.name.to_lower())
		emit_signal("on_picked_up")
		call_deferred("_remove_me")

func _remove_me():
	self.get_parent().remove_child(self)
	self.queue_free()