extends Node2D

const Egg = preload("res://Entities/World/Items/Egg.tscn")

func on_end_day():
	var egg = Egg.instance()
	egg.position = self.position + Vector2(50, 50) # right/down
	get_parent().add_child(egg)