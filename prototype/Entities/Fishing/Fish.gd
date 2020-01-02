extends Area2D

signal hooked

func on_fish_body_entered(body):
	if body.name == "Bobber":
		self.emit_signal("hooked")

func on_fish_body_exited(body):
	pass