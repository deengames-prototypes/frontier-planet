extends Area2D

func _on_Egg_body_entered(body):
	if body.name == "MapPlayer":
		Globals.player_inventory.append("egg")
		self.get_parent().remove_child(self)
		self.queue_free()
