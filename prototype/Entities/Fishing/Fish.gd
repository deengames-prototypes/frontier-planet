extends Area2D

func on_fish_body_entered(body):
	if body.name == "Bobber":
		print("BOB'S YER UNCLE")

func on_fish_body_exited(body):
	if body.name == "Bobber":
		print("BYE BOB")