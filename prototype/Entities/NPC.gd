extends Area2D

export var dialog:String = ""

func _on_Door_body_entered(body):
	if body.name == "MapPlayer":
		print(dialog)