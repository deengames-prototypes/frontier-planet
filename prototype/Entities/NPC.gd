extends Area2D

const DialogPanel = preload("res://UI/DialogPanel.tscn")

export var dialog:String = ""
var _panel#:DialogPanel

func _on_Door_body_entered(body):
	if body.name == "MapPlayer":
		_panel = DialogPanel.instance()
		_panel.set_text(dialog)
		_panel.position.y += 100
		add_child(_panel)

func _on_NPC_body_exited(body):
	if body.name == "MapPlayer":
		remove_child(_panel)
		_panel.free()
